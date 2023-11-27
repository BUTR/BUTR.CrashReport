extern alias iced;

using AsmResolver.DotNet;
using AsmResolver.DotNet.Code.Cil;
using AsmResolver.DotNet.Dynamic;

using HarmonyLib;
using HarmonyLib.BUTR.Extensions;

using iced::Iced.Intel;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

using Decoder = iced::Iced.Intel.Decoder;

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
    /// The list of Harmony patch methods that are applied to the method.
    /// </summary>
    public required MethodEntry[] PatchMethods { get; set; }
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
        ImportedTypeReferences = AvailableAssemblies.ToDictionary(x => x.Key, x =>
        {
            foreach (var assemblyModule in x.Value.Modules)
            {
                try
                {
                    var module = ModuleDefinition.FromModule(assemblyModule);
                    return module.GetImportedTypeReferences().Select(y => new AssemblyTypeReference
                    {
                        Name = y.Name ?? string.Empty,
                        Namespace = y.Namespace ?? string.Empty,
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
                    CilInstructions = GetILInstructionLines(methodBase),
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
                    CilInstructions = GetILInstructionLines(original),
                } : null,
                MethodFromStackframeIssue = methodFromStackframeIssue,
                ModuleInfo = GetModuleInfoIfMod(identifiableMethod, crashReportHelper),
                ILOffset = ilOffset != StackFrame.OFFSET_UNKNOWN ? ilOffset : null,
                NativeOffset = nativeILOffset != StackFrame.OFFSET_UNKNOWN ? nativeILOffset : null,
                StackFrameDescription = frame.ToString(),
                NativeInstructions = GetInstructionLines(identifiableMethod, nativeILOffset),
                CilInstructions = GetILInstructionLines(identifiableMethod),
                PatchMethods = methods.ToArray(),
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