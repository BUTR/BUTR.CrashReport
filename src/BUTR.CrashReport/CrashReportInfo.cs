extern alias iced;

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
using System.Runtime.InteropServices;
using System.Text;

using iced::Iced.Intel;
using Decoder = iced::Iced.Intel.Decoder;

namespace BUTR.CrashReport;

public record AssemblyTypeReference
{
    public required string FullName { get; set; }
}

public record MethodEntry
{
    public required MethodBase Method { get; set; }
    public required IModuleInfo? ModuleInfo { get; set; }
    public required string[] NativeInstructions { get; set; }
    public required string[] CilInstructions { get; set; }
}

public record StacktraceEntry
{
    public required MethodBase Method { get; set; }
    public required bool MethodFromStackframeIssue { get; set; }
    public required IModuleInfo? ModuleInfo { get; set; }
    public required int? ILOffset { get; set; }
    public required int? NativeOffset { get; set; }
    public required string StackFrameDescription { get; set; }
    public required string[] NativeInstructions { get; set; }
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

    private delegate IntPtr GetNativeMethodBodyDelegate(object instance, MethodBase method);
    private static readonly GetNativeMethodBodyDelegate? GetNativeMethodBody =
        AccessTools2.GetDelegate<GetNativeMethodBodyDelegate>("MonoMod.Core.Platforms.PlatformTriple:GetNativeMethodBody");

    public readonly byte Version = 13;
    public Guid Id { get; } = Guid.NewGuid();
    public Exception Exception { get; }
    public ICollection<StacktraceEntry> Stacktrace { get; }
    public ICollection<StacktraceEntry> FilteredStacktrace { get; }
    public ICollection<IModuleInfo> LoadedModules { get; }
    public Dictionary<AssemblyName, Assembly> AvailableAssemblies { get; }
    public Dictionary<AssemblyName, AssemblyTypeReference[]> ImportedTypeReferences { get; }
    public Dictionary<MethodBase, Patches> LoadedHarmonyPatches { get; } = new();
    public Dictionary<string, string> AdditionalMetadata { get; }

    public CrashReportInfo(Exception exception, ICrashReportHelper crashReportHelper, Dictionary<string, string> additionalMetadata)
    {
        Exception = exception.Demystify();
        AdditionalMetadata = additionalMetadata;
        LoadedModules = crashReportHelper.GetLoadedModules().ToArray();

        AvailableAssemblies = crashReportHelper.Assemblies().ToDictionary(x => x.GetName(), x => x);
        ImportedTypeReferences = AvailableAssemblies.ToDictionary(x => x.Key, x =>
        {
            foreach (var assemblyModule in x.Value.Modules)
            {
                try
                {
                    var module = ModuleDefinition.FromModule(assemblyModule);
                    return module.GetImportedTypeReferences().Select(y => new AssemblyTypeReference()
                    {
                        FullName = y.FullName,
                    }).ToArray();
                }
                catch (Exception) { /* ignore */ }

            }
            return Array.Empty<AssemblyTypeReference>();
        });

        Stacktrace = GetAllInvolvedModules(Exception, crashReportHelper).ToArray();
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
                var patchPostfix = methodName.Split(new[] { "_Patch" }, StringSplitOptions.None);

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

        var trace = new EnhancedStackTrace(ex);
        foreach (var frame in trace.GetFrames())
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
                    NativeInstructions = Array.Empty<string>(),
                    CilInstructions = GetILInstructionLines(methodBase),
                });
            }

            foreach (var (methodBase, extendedModuleInfo) in GetPostfixes(patches, crashReportHelper))
            {
                methods.Add(new()
                {
                    Method = methodBase,
                    ModuleInfo = extendedModuleInfo,
                    NativeInstructions = Array.Empty<string>(),
                    CilInstructions = GetILInstructionLines(methodBase),
                });
            }

            foreach (var (methodBase, extendedModuleInfo) in GetPrefixes(patches, crashReportHelper))
            {
                methods.Add(new()
                {
                    Method = methodBase,
                    ModuleInfo = extendedModuleInfo,
                    NativeInstructions = Array.Empty<string>(),
                    CilInstructions = GetILInstructionLines(methodBase),
                });
            }

            foreach (var (methodBase, extendedModuleInfo) in GetTranspilers(patches, crashReportHelper))
            {
                methods.Add(new()
                {
                    Method = methodBase,
                    ModuleInfo = extendedModuleInfo,
                    NativeInstructions = Array.Empty<string>(),
                    CilInstructions = GetILInstructionLines(methodBase),
                });
            }

            var moduleInfo = GetModuleInfoIfMod(identifiableMethod, crashReportHelper);

            var ilOffset = frame.GetILOffset();
            var nativeILOffset = frame.GetNativeOffset();

            yield return new()
            {
                Method = identifiableMethod!,
                MethodFromStackframeIssue = methodFromStackframeIssue,
                ModuleInfo = moduleInfo,
                ILOffset = ilOffset != StackFrame.OFFSET_UNKNOWN ? ilOffset : null,
                NativeOffset = nativeILOffset != StackFrame.OFFSET_UNKNOWN ? nativeILOffset : null,
                StackFrameDescription = frame.ToString(),
                NativeInstructions = GetInstructionLines(identifiableMethod, nativeILOffset),
                CilInstructions = GetILInstructionLines(identifiableMethod),
                Methods = methods.ToArray(),
            };
        }
    }

    private static string[] GetInstructionLines(MethodBase? method, int nativeILOffset)
    {
        static IEnumerable<string> GetLines(MethodBase method, int nativeILOffset)
        {
            var nativeCodePtr = GetNativeMethodBody!(CurrentPlatformTriple!(), method);

            var length = (uint) nativeILOffset + 16;
            var bytecode = new byte[length];

            Marshal.Copy(nativeCodePtr, bytecode, 0, bytecode.Length);

            var codeReader = new ByteArrayCodeReader(bytecode);
            var decoder = Decoder.Create(IntPtr.Size == 4 ? 32 : 64, codeReader);

            var output = new StringOutput();
            var sb = new StringBuilder();

            var formatter = new NasmFormatter
            {
                Options =
                {
                    DigitSeparator = "`",
                    FirstOperandCharIndex = 10
                }
            };

            while (decoder.IP < length)
            {
                var instr = decoder.Decode();
                formatter.Format(instr, output); // Don't use instr.ToString(), it allocates more, uses masm syntax and default options
                sb.Append(instr.IP.ToString("X4")).Append(" ").Append(output.ToStringAndReset());
                yield return sb.ToString();
                sb.Clear();
            }
        }

        if (method is null) return Array.Empty<string>();
        if (nativeILOffset == StackFrame.OFFSET_UNKNOWN) return Array.Empty<string>();
        if (CurrentPlatformTriple is null || GetNativeMethodBody is null) return Array.Empty<string>();

        return GetLines(method, nativeILOffset).ToArray();
    }
    private static string[] GetILInstructionLines(MethodBase? method)
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