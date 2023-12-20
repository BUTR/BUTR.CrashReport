using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using static BUTR.CrashReport.Utils.MethodDecompiler;
using static BUTR.CrashReport.Utils.MonoModUtils;
using static BUTR.CrashReport.Utils.ReferenceImporter;

namespace BUTR.CrashReport;

/// <summary>
/// Represents an imported type reference.
/// </summary>
public record AssemblyTypeReference
{
    /// <summary>
    /// <inheritdoc cref="AsmResolver.DotNet.TypeReference.Name"/>
    /// </summary>
    /// <returns><inheritdoc cref="AsmResolver.DotNet.TypeReference.Name"/></returns>
    public required string Name { get; set; }

    /// <summary>
    /// <inheritdoc cref="AsmResolver.DotNet.TypeReference.Namespace"/>
    /// </summary>
    /// <returns><inheritdoc cref="AsmResolver.DotNet.TypeReference.Namespace"/></returns>
    public required string Namespace { get; set; }

    /// <summary>
    /// <inheritdoc cref="AsmResolver.DotNet.TypeReference.FullName"/>
    /// </summary>
    /// <returns><inheritdoc cref="AsmResolver.DotNet.TypeReference.FullName"/></returns>
    public required string FullName { get; set; }
}

/// <summary>
/// Represents a Harmony patch.
/// </summary>
public record MethodEntry
{
    /// <summary>
    /// The Harmony patch method.
    /// </summary>
    public required MethodBase Method { get; set; }

    /// <summary>
    /// <inheritdoc cref="StacktraceEntry.ModuleInfo"/>
    /// </summary>
    /// <returns><inheritdoc cref="StacktraceEntry.ModuleInfo"/></returns>
    public required IModuleInfo? ModuleInfo { get; set; }

    /// <summary>
    /// <inheritdoc cref="StacktraceEntry.CilInstructions"/>
    /// </summary>
    /// <returns><inheritdoc cref="StacktraceEntry.CilInstructions"/></returns>
    public required string[] CilInstructions { get; set; }
    
    /// <summary>
    /// <inheritdoc cref="StacktraceEntry.CsharpWithCilInstructions"/>
    /// </summary>
    /// <returns><inheritdoc cref="StacktraceEntry.CsharpWithCilInstructions"/></returns>
    public required string[] CsharpWithCilInstructions { get; set; }
    
    /// <summary>
    /// <inheritdoc cref="StacktraceEntry.CsharpInstructions"/>
    /// </summary>
    /// <returns><inheritdoc cref="StacktraceEntry.CsharpInstructions"/></returns>
    public required string[] CsharpInstructions { get; set; }
}

/// <summary>
/// Represents a method from stack trace.
/// </summary>
public record StacktraceEntry
{
    /// <summary>
    /// The method from the stacktrace frame that is being executed.
    /// </summary>
    public required MethodBase Method { get; set; }

    /// <summary>
    /// The original method that might be patched.
    /// </summary>
    public required MethodEntry? OriginalMethod { get; set; }

    /// <summary>
    /// Whether there was an issue with getting the data from the stackframe.
    /// </summary>
    public required bool MethodFromStackframeIssue { get; set; }

    /// <summary>
    /// The module that holds the method. Can be null.
    /// </summary>
    public required IModuleInfo? ModuleInfo { get; set; }

    /// <summary>
    /// <inheritdoc cref="System.Diagnostics.StackFrame.GetILOffset"/>
    /// </summary>
    /// <returns><inheritdoc cref="System.Diagnostics.StackFrame.GetILOffset"/></returns>
    public required int? ILOffset { get; set; }

    /// <summary>
    /// <inheritdoc cref="System.Diagnostics.StackFrame.GetNativeOffset"/>
    /// </summary>
    /// <returns><inheritdoc cref="System.Diagnostics.StackFrame.GetNativeOffset"/></returns>
    public required int? NativeOffset { get; set; }

    /// <summary>
    /// <inheritdoc cref="System.Diagnostics.StackFrame.ToString"/>
    /// </summary>
    /// <returns><inheritdoc cref="System.Diagnostics.StackFrame.ToString"/></returns>
    public required string StackFrameDescription { get; set; }

    /// <summary>
    /// The native code of the method that was compiled by the JIT.
    /// </summary>
    public required string[] NativeInstructions { get; set; }

    /// <summary>
    /// The Common Intermediate Language (CIL) code of the method.
    /// </summary>
    public required string[] CilInstructions { get; set; }
    
    /// <summary>
    /// The C# code of the method.
    /// </summary>
    public required string[] CsharpWithCilInstructions { get; set; }
    
    /// <summary>
    /// The C# and Common Intermediate Language (CIL) code of the method.
    /// </summary>
    public required string[] CsharpInstructions { get; set; }

    /// <summary>
    /// The list of Harmony patch methods that are applied to the method.
    /// </summary>
    public required MethodEntry[] PatchMethods { get; set; }
}

/// <summary>
/// The initial crash report info to be converted into the POCO
/// </summary>
public class CrashReportInfo
{
    /// <summary>
    /// The version of the crash report.
    /// </summary>
    public readonly byte Version = 13;

    /// <summary>
    /// The id of the crash report.
    /// </summary>
    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>
    /// The exception that caused the crash.
    /// </summary>
    public Exception Exception { get; }

    /// <summary>
    /// Raw stacktrace.
    /// </summary>
    public ICollection<StacktraceEntry> Stacktrace { get; }

    /// <summary>
    /// Filtered stacktrace.
    /// </summary>
    public ICollection<StacktraceEntry> FilteredStacktrace { get; }

    /// <summary>
    /// The list of modules that are loaded in the process.
    /// </summary>
    public ICollection<IModuleInfo> LoadedModules { get; }

    /// <summary>
    /// Lookup dictionary for available assemblies.
    /// </summary>
    public Dictionary<AssemblyName, Assembly> AvailableAssemblies { get; }

    /// <summary>
    /// Imported type references for assemblies.
    /// </summary>
    public Dictionary<AssemblyName, AssemblyTypeReference[]> ImportedTypeReferences { get; }

    /// <summary>
    /// Loaded harmony patches for methods.
    /// </summary>
    public Dictionary<MethodBase, Patches> LoadedHarmonyPatches { get; } = new();

    /// <summary>
    /// Additional metadata about the crash.
    /// </summary>
    public Dictionary<string, string> AdditionalMetadata { get; }

    /// <summary>
    /// Creates the CrashReportInfo based on initial crash report data.
    /// </summary>
    /// <param name="exception">The exception that caused the crash.</param>
    /// <param name="crashReportHelper">The interface implementation of the needed basic functions.</param>
    /// <param name="additionalMetadata">Any additional metadata to be passed to the CrashReportInfo.</param>
    public CrashReportInfo(Exception exception, ICrashReportHelper crashReportHelper, Dictionary<string, string> additionalMetadata)
    {
        Exception = exception.Demystify();
        AdditionalMetadata = additionalMetadata;
        LoadedModules = crashReportHelper.GetLoadedModules().ToArray();

        AvailableAssemblies = crashReportHelper.Assemblies().ToDictionary(x => x.GetName(), x => x);
        ImportedTypeReferences = GetImportedTypeReferences(AvailableAssemblies).ToDictionary(x => x.Key, x => x.Value.Select(y => new AssemblyTypeReference
        {
            Name = y.Name,
            Namespace = y.Namespace,
            FullName = y.FullName
        }).ToArray());

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
        static IEnumerable<(MethodBase, IModuleInfo)> GetPatches(Patches? info, ICrashReportHelper moduleHelper)
        {
            if (info is null)
                yield break;

            var patchMethods = info.Prefixes.OrderBy(t => t.priority).Select(t => t.PatchMethod)
                .Concat(info.Postfixes.OrderBy(t => t.priority).Select(t => t.PatchMethod))
                .Concat(info.Transpilers.OrderBy(t => t.priority).Select(t => t.PatchMethod))
                .Concat(info.Finalizers.OrderBy(t => t.priority).Select(t => t.PatchMethod));

            foreach (var method in patchMethods)
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

            foreach (var (methodBase, extendedModuleInfo) in GetPatches(patches, crashReportHelper))
            {
                methods.Add(new()
                {
                    Method = methodBase,
                    ModuleInfo = extendedModuleInfo,
                    CilInstructions = DecompileILCode(methodBase),
                    CsharpWithCilInstructions = DecompileILWithCSharpCode(methodBase),
                    CsharpInstructions = DecompileCSharpCode(methodBase),
                });
            }

            var ilOffset = frame.GetILOffset();
            var nativeILOffset = frame.GetNativeOffset();
            yield return new()
            {
                Method = identifiableMethod!,
                OriginalMethod = original is not null ? new()
                {
                    Method = original,
                    ModuleInfo = GetModuleInfoIfMod(original, crashReportHelper),
                    CilInstructions = DecompileILCode(original),
                    CsharpWithCilInstructions = DecompileILWithCSharpCode(original),
                    CsharpInstructions = DecompileCSharpCode(original),
                } : null,
                MethodFromStackframeIssue = methodFromStackframeIssue,
                ModuleInfo = GetModuleInfoIfMod(identifiableMethod, crashReportHelper),
                ILOffset = ilOffset != StackFrame.OFFSET_UNKNOWN ? ilOffset : null,
                NativeOffset = nativeILOffset != StackFrame.OFFSET_UNKNOWN ? nativeILOffset : null,
                StackFrameDescription = frame.ToString(),
                NativeInstructions = DecompileNativeCode(identifiableMethod, nativeILOffset),
                CilInstructions = DecompileILCode(identifiableMethod),
                CsharpWithCilInstructions = DecompileILWithCSharpCode(identifiableMethod),
                CsharpInstructions = DecompileCSharpCode(identifiableMethod),
                PatchMethods = methods.ToArray(),
            };
        }
    }
}