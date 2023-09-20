using AsmResolver.DotNet;
using AsmResolver.DotNet.Code.Cil;
using AsmResolver.DotNet.Dynamic;

using HarmonyLib;
using HarmonyLib.BUTR.Extensions;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace BUTR.CrashReport;

public record MethodEntry
{
    public required MethodBase Method { get; set; }
    public required IModuleInfo? ModuleInfo { get; set; }
    public required string[] CilInstructions { get; set; }
}

public record StacktraceEntry
{
    public required MethodBase Method { get; set; }
    public required bool MethodFromStackframeIssue { get; set; }
    public required IModuleInfo? ModuleInfo { get; set; }
    public required int? ILOffset { get; set; }
    public required string StackFrameDescription { get; set; }
    public required string[] CilInstructions { get; set; }
    public required MethodEntry[] Methods { get; set; }
}

/// <summary>
/// The initial crash report info to be converted into the POCO
/// </summary>
public class CrashReportInfo
{
    private delegate object GetCurrentPlatformTripleDelegate();
    private static readonly GetCurrentPlatformTripleDelegate? CurrentPlatformTriple =
        AccessTools2.GetPropertyGetterDelegate<GetCurrentPlatformTripleDelegate>("MonoMod.Core.Platforms.PlatformTriple:Current");

    private delegate MethodBase GetIdentifiableDelegate(object instance, MethodBase method);
    private static readonly GetIdentifiableDelegate? GetIdentifiable =
        AccessTools2.GetDelegate<GetIdentifiableDelegate>("MonoMod.Core.Platforms.PlatformTriple:GetIdentifiable");

    public readonly byte Version = 12;
    public Guid Id { get; } = Guid.NewGuid();
    public Exception Exception { get; }
    public ICollection<StacktraceEntry> Stacktrace { get; }
    public ICollection<StacktraceEntry> FilteredStacktrace { get; }
    public ICollection<IModuleInfo> LoadedModules { get; }
    public ICollection<Assembly> AvailableAssemblies { get; }
    public Dictionary<MethodBase, Patches> LoadedHarmonyPatches { get; } = new();
    public Dictionary<string, string> AdditionalMetadata { get; }

    public CrashReportInfo(Exception exception, ICrashReportHelper crashReportHelper, Dictionary<string, string> additionalMetadata)
    {
        Exception = exception;
        AdditionalMetadata = additionalMetadata;
        LoadedModules = crashReportHelper.GetLoadedModules().ToArray();

        AvailableAssemblies = crashReportHelper.Assemblies().ToArray();

        Stacktrace = GetAllInvolvedModules(exception, crashReportHelper).ToArray();
        FilteredStacktrace = crashReportHelper.Filter(Stacktrace).ToArray();

        foreach (var originalMethod in Harmony.GetAllPatchedMethods())
        {
            var patches = Harmony.GetPatchInfo(originalMethod);
            if (originalMethod is null || patches is null) continue;
            LoadedHarmonyPatches.Add(originalMethod, patches);
        }
    }

    private static IEnumerable<StacktraceEntry> GetAllInvolvedModules(Exception ex, ICrashReportHelper crashReportHelper)
    {
        static IEnumerable<(MethodBase, IModuleInfo)> GetPrefixes(Patches? info, ICrashReportHelper moduleHelper) => info is null
            ? Enumerable.Empty<(MethodBase, IModuleInfo)>()
            : AddMetadata(info.Prefixes.OrderBy(t => t.priority).Select(t => t.PatchMethod), moduleHelper);

        static IEnumerable<(MethodBase, IModuleInfo)> GetPostfixes(Patches? info, ICrashReportHelper moduleHelper) => info is null
            ? Enumerable.Empty<(MethodBase, IModuleInfo)>()
            : AddMetadata(info.Postfixes.OrderBy(t => t.priority).Select(t => t.PatchMethod), moduleHelper);

        static IEnumerable<(MethodBase, IModuleInfo)> GetTranspilers(Patches? info, ICrashReportHelper moduleHelper) => info is null
            ? Enumerable.Empty<(MethodBase, IModuleInfo)>()
            : AddMetadata(info.Transpilers.OrderBy(t => t.priority).Select(t => t.PatchMethod), moduleHelper);

        static IEnumerable<(MethodBase, IModuleInfo)> GetFinalizers(Patches? info, ICrashReportHelper moduleHelper) => info is null
            ? Enumerable.Empty<(MethodBase, IModuleInfo)>()
            : AddMetadata(info.Finalizers.OrderBy(t => t.priority).Select(t => t.PatchMethod), moduleHelper);

        static IEnumerable<(MethodBase, IModuleInfo)> AddMetadata(IEnumerable<MethodInfo> methods, ICrashReportHelper moduleHelper)
        {
            foreach (var method in methods)
            {
                if (method.DeclaringType is { } declaringType && moduleHelper.GetModuleByType(declaringType) is { } moduleInfo)
                    yield return (method, moduleInfo);
            }
        }

        static IModuleInfo? GetModuleInfoIfMod(MethodBase? method, ICrashReportHelper moduleHelper)
        {
            if (method is null)
                return null;

            if (method.DeclaringType is { Assembly.IsDynamic: false })
                return moduleHelper.GetModuleByType(method.DeclaringType);

            // The lambda methods don't have an owner, so we can't know who's the lambda creator
            if (method.DeclaringType is null && method.Name == "lambda_method")
                return null;

            // Patches contain as their name the full name of the method, including type and namespace
            // This is not possible
            if (method.DeclaringType is null && method.Name.Contains('.'))
            {
                var methodName = method.Name.Split('(')[0];
                var patchPostfix = methodName.Split(new[] {"_Patch"}, StringSplitOptions.None);

                if (!patchPostfix.Last().All(char.IsDigit))
                    return null;

                var fullMethodName = string.Join("", patchPostfix.Take(patchPostfix.Length - 1));
                var foundMethod = moduleHelper.Assemblies().Where(x => !x.IsDynamic)
                    .SelectMany(x => x.DefinedTypes)
                    .Where(x => !x.IsAbstract)
                    .SelectMany(x => x.DeclaredMethods)
                    .Where(x => x.DeclaringType is not null)
                    .FirstOrDefault(x => fullMethodName == $"{x.DeclaringType!.FullName}.{x.Name}");

                if (foundMethod is null)
                    return null;

                return moduleHelper.GetModuleByType(foundMethod.DeclaringType);
            }

            return null;
        }


        var inner = ex.InnerException;
        if (inner is not null)
        {
            foreach (var modInfo in GetAllInvolvedModules(inner, crashReportHelper))
                yield return modInfo;
        }

        var trace = crashReportHelper.FromException(ex);
        foreach (var frame in trace.GetFrames() ?? Array.Empty<StackFrame>())
        {
            if (!frame.HasMethod()) continue;

            MethodBase method;
            var methodFromStackframeIssue = false;
            try
            {
                method = Harmony.GetMethodFromStackframe(frame);
            }
            // NullReferenceException means the method was not found. Harmony doesn't handle this case gracefully
            catch (NullReferenceException)
            {
                method = frame.GetMethod()!;
            }
            // The given generic instantiation was invalid.
            // From what I understand, this will occur with generic methods
            // Also when static constructors throw errors, Harmony resolution will fail
            catch (Exception)
            {
                methodFromStackframeIssue = true;
                method = frame.GetMethod()!;
            }

            var methods = new List<MethodEntry>();
            var identifiableMethod = method is MethodInfo mi ? CurrentPlatformTriple?.Invoke() is { } cpt && GetIdentifiable?.Invoke(cpt, mi) is MethodInfo v ? v : mi : null;
            var original = identifiableMethod is not null ? Harmony.GetOriginalMethod(identifiableMethod) : null;
            var patches = original is not null ? Harmony.GetPatchInfo(original) : null;

            foreach (var (methodBase, extendedModuleInfo) in GetFinalizers(patches, crashReportHelper))
            {
                methods.Add(new()
                {
                    Method = methodBase,
                    ModuleInfo = extendedModuleInfo,
                    CilInstructions = GetInstructionLines(methodBase),
                });
            }

            foreach (var (methodBase, extendedModuleInfo) in GetPostfixes(patches, crashReportHelper))
            {
                methods.Add(new()
                {
                    Method = methodBase,
                    ModuleInfo = extendedModuleInfo,
                    CilInstructions = GetInstructionLines(methodBase),
                });
            }

            foreach (var (methodBase, extendedModuleInfo) in GetPrefixes(patches, crashReportHelper))
            {
                methods.Add(new()
                {
                    Method = methodBase,
                    ModuleInfo = extendedModuleInfo,
                    CilInstructions = GetInstructionLines(methodBase),
                });
            }

            foreach (var (methodBase, extendedModuleInfo) in GetTranspilers(patches, crashReportHelper))
            {
                methods.Add(new()
                {
                    Method = methodBase,
                    ModuleInfo = extendedModuleInfo,
                    CilInstructions = GetInstructionLines(methodBase),
                });
            }

            var moduleInfo = GetModuleInfoIfMod(identifiableMethod, crashReportHelper);

            var ilOffset = frame.GetILOffset();

            yield return new()
            {
                Method = identifiableMethod!,
                MethodFromStackframeIssue = methodFromStackframeIssue,
                ModuleInfo = moduleInfo,
                ILOffset = ilOffset != StackFrame.OFFSET_UNKNOWN ? ilOffset : null,
                StackFrameDescription = frame.ToString(),
                CilInstructions = GetInstructionLines(identifiableMethod),
                Methods = methods.ToArray(),
            };
        }
    }

    private static string[] GetInstructionLines(MethodBase? method)
    {
        static string[] ToLines(CilInstructionCollection? instructions) => instructions?.Select(x => x.ToString()).ToArray() ?? Array.Empty<string>();

        if (method is null) return Array.Empty<string>();

        try
        {
            try
            {
                var module = ModuleDefinition.FromModule(typeof(CrashReportInfo).Module);
                var dynamicMethodDefinition = new DynamicMethodDefinition(module, method);
                return ToLines(dynamicMethodDefinition.CilMethodBody?.Instructions);
            }
            catch (Exception) { /* ignore */ }

            try
            {
                var module = ModuleDefinition.FromModule(method.Module);
                var cilMethodBody = module.LookupMember(method.MetadataToken) is MethodDefinition methodDefinition ? methodDefinition.CilMethodBody : null;
                return ToLines(cilMethodBody?.Instructions);
            }
            catch (Exception) { /* ignore */ }
        }
        catch (Exception) { /* ignore */ }

        return Array.Empty<string>();
    }
}