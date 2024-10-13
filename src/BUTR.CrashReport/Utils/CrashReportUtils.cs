using BUTR.CrashReport.Interfaces;
using BUTR.CrashReport.Models;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;

using static BUTR.CrashReport.Decompilers.Utils.MethodDecompiler;

namespace BUTR.CrashReport.Utils;

/// <summary>
/// Exposes the code we use to create the <see cref="CrashReportInfo"/>
/// </summary>
public static class CrashReportUtils
{
    /// <summary>
    /// <inheritdoc cref="EnhancedStacktraceFrameModel"/>
    /// </summary>
    public record StackframePatchData
    {
        /// <summary>
        /// <inheritdoc cref="EnhancedStacktraceFrameModel.OriginalMethod"/>
        /// </summary>
        public required MethodBase? OriginalMethod { get; set; }

        /// <summary>
        /// <inheritdoc cref="EnhancedStacktraceFrameModel.ExecutingMethod"/>
        /// </summary>
        public required MethodInfo? ExecutingMethod { get; set; }

        /// <summary>
        /// <inheritdoc cref="EnhancedStacktraceFrameModel.PatchMethods"/>
        /// </summary>
        public required List<MethodEntry> Patches { get; set; }

        public required int ILOffset { get; set; }

        public required int NativeILOffset { get; set; }

        public required IntPtr NativeCodePtr { get; set; }
        
        /// <summary>
        /// Deconstructs the object.
        /// </summary>
        public void Deconstruct(out MethodBase? original, out MethodInfo? replacement, out List<MethodEntry> patches, out int ilOffset, out int nativeILOffset, out IntPtr nativeCodePtr)
        {
            original = OriginalMethod;
            replacement = ExecutingMethod;
            patches = Patches;
            ilOffset = ILOffset;
            nativeILOffset = NativeILOffset;
            nativeCodePtr = NativeCodePtr;
        }
    }

    /*
    /// <summary>
    /// Gets all involved modules in the exception stacktrace.
    /// </summary>
    public static IEnumerable<(MonoModPatch, IModuleInfo?, ILoaderPluginInfo?)> GetMonoModPatchMethods(MonoModPatches? patches, IModuleProvider moduleProvider, ILoaderPluginProvider loaderPluginProvider)
    {
        if (patches is null)
            yield break;

        var patchMethods = patches.Detours.OrderBy(t => t.Priority)
            .Concat(patches.ILHooks.OrderBy(t => t.Priority));

        foreach (var patch in patchMethods)
        {
            var method = patch.Method;
            if (method.DeclaringType is not { } declaringType)
                continue;

            var moduleInfo = moduleProvider.GetModuleByType(declaringType);
            var loaderPluginInfo = loaderPluginProvider.GetLoaderPluginByType(declaringType);

            if (moduleInfo is not null || loaderPluginInfo is not null)
                yield return (patch, moduleInfo, loaderPluginInfo);
        }
    }

    /// <summary>
    /// Gets all involved modules in the exception stacktrace.
    /// </summary>
    public static IEnumerable<(HarmonyPatch, IModuleInfo?, ILoaderPluginInfo?)> GetHarmonyPatchMethods(HarmonyPatches? patches, IModuleProvider moduleProvider, ILoaderPluginProvider loaderPluginProvider)
    {
        if (patches is null)
            yield break;

        var patchMethods = patches.Prefixes.OrderBy(t => t.Priority)
            .Concat(patches.Postfixes.OrderBy(t => t.Priority))
            .Concat(patches.Transpilers.OrderBy(t => t.Priority))
            .Concat(patches.Finalizers.OrderBy(t => t.Priority));

        foreach (var patch in patchMethods)
        {
            var method = patch.PatchMethod;
            if (method.DeclaringType is not { } declaringType)
                continue;

            var moduleInfo = moduleProvider.GetModuleByType(declaringType);
            var loaderPluginInfo = loaderPluginProvider.GetLoaderPluginByType(declaringType);

            if (moduleInfo is not null || loaderPluginInfo is not null)
                yield return (patch, moduleInfo, loaderPluginInfo);
        }
    }
    */

    /// <summary>
    /// Gets the module info if the method is from a mod.
    /// </summary>
    public static IModuleInfo? GetModuleInfoIfMod(MethodBase? method, IEnumerable<Assembly> assemblies, IAssemblyUtilities assemblyUtilities, IModuleProvider moduleProvider)
    {
        if (method is null)
            return null;

        if (method.DeclaringType is { Assembly.IsDynamic: false })
            return moduleProvider.GetModuleByType(method.DeclaringType);

        // The lambda methods don't have an owner, so we can't know who's the lambda creator
        if (method.DeclaringType is null && method.Name == "lambda_method")
            return null;

        // Patches contain as their name the full name of the method, including type and namespace
        // This is not possible
        if (method.DeclaringType is null && method.Name.Contains('.'))
        {
            var methodName = method.Name.Split('(')[0];
            var patchPostfix = methodName.Split(["_Patch"], StringSplitOptions.None);

            if (!patchPostfix.Last().All(char.IsDigit))
                return null;

            var fullMethodName = string.Join("", patchPostfix.Take(patchPostfix.Length - 1));
            var foundMethod = assemblies.Where(x => !x.IsDynamic)
                .SelectMany(assemblyUtilities.TypesFromAssembly)
                .Where(x => !x.IsAbstract)
                .Where(x => !string.IsNullOrEmpty(x.DeclaringType?.FullName) && fullMethodName.StartsWith(x.DeclaringType!.FullName))
                .SelectMany(x => x.GetMethods())
                .Where(x => x.DeclaringType is not null)
                .FirstOrDefault(x => fullMethodName == $"{x.DeclaringType!.FullName}.{x.Name}");

            if (foundMethod is null)
                return null;

            return moduleProvider.GetModuleByType(foundMethod.DeclaringType);
        }

        return null;
    }

    /// <summary>
    /// Gets the loader plugin if the method is from a mod.
    /// </summary>
    public static ILoaderPluginInfo? GetLoaderPluginIfMod(MethodBase? method, IEnumerable<Assembly> assemblies, IAssemblyUtilities assemblyUtilities, ILoaderPluginProvider loaderPluginProvider)
    {
        if (method is null)
            return null;

        if (method.DeclaringType is { Assembly.IsDynamic: false })
            return loaderPluginProvider.GetLoaderPluginByType(method.DeclaringType);

        // The lambda methods don't have an owner, so we can't know who's the lambda creator
        if (method.DeclaringType is null && method.Name == "lambda_method")
            return null;

        // Patches contain as their name the full name of the method, including type and namespace
        // This is not possible
        if (method.DeclaringType is null && method.Name.Contains('.'))
        {
            var methodName = method.Name.Split('(')[0];
            var patchPostfix = methodName.Split(["_Patch"], StringSplitOptions.None);

            if (!patchPostfix.Last().All(char.IsDigit))
                return null;

            var fullMethodName = string.Join("", patchPostfix.Take(patchPostfix.Length - 1));
            var foundMethod = assemblies.Where(x => !x.IsDynamic)
                .SelectMany(assemblyUtilities.TypesFromAssembly)
                .Where(x => !x.IsAbstract)
                .Where(x => !string.IsNullOrEmpty(x.DeclaringType?.FullName) && fullMethodName.StartsWith(x.DeclaringType!.FullName))
                .SelectMany(x => x.GetMethods())
                .FirstOrDefault(x => fullMethodName == $"{x.DeclaringType!.FullName}.{x.Name}");

            if (foundMethod is null)
                return null;

            return loaderPluginProvider.GetLoaderPluginByType(foundMethod.DeclaringType);
        }

        return null;
    }

    /*
    /// <summary>
    /// Gets the Harmony data from the stackframe.
    /// </summary>
    public static StackframePatchData GetPatchData(StackFrame frame, IPatchProvider patchProvider, IMonoModProvider monoModProvider, IHarmonyProvider harmonyProvider, IModuleProvider moduleProvider, ILoaderPluginProvider loaderPluginProvider)
    {
        var patches = new List<MethodEntry>();
        
        var executingMethodMonoMod = monoModProvider.GetExecutingMethod(frame);
        var originalMethodMonoMod = monoModProvider.GetOriginalMethod(frame);
        var ilOffsetMonoMod = frame.GetILOffset();
        var nativeILOffsetMonoMod = frame.GetNativeOffset();
        var nativeCodePtrMonoMod = executingMethodMonoMod is not null ? monoModProvider.GetNativeMethodBody(executingMethodMonoMod) : IntPtr.Zero;
        var monoModPatches = monoModProvider.GetPatchInfo(frame);
        foreach (var (patch, moduleInfo, loaderPluginInfo) in GetMonoModPatchMethods(monoModPatches, moduleProvider, loaderPluginProvider))
        {
            patches.Add(new MethodEntryMonoMod
            {
                Patch = patch,
                Method = patch.Method,
                ModuleInfo = moduleInfo,
                LoaderPluginInfo = loaderPluginInfo,
                ILInstructions = DecompileILCode(patch.Method),
                CSharpILMixedInstructions = DecompileILWithCSharpCode(patch.Method),
                CSharpInstructions = DecompileCSharpCode(patch.Method),
            });
        }
        
        var executingMethodHarmony = harmonyProvider.GetExecutingMethod(frame);
        var originalMethodHarmony = harmonyProvider.GetOriginalMethod(frame);
        var ilOffsetHarmony = frame.GetILOffset();
        var nativeILOffsetHarmony = frame.GetNativeOffset();
        var nativeCodePtrHarmony = executingMethodHarmony is not null ? harmonyProvider.GetNativeMethodBody(executingMethodHarmony) : IntPtr.Zero;
        var harmonyPatches = harmonyProvider.GetPatchInfo(frame);
        foreach (var (patch, moduleInfo, loaderPluginInfo) in GetHarmonyPatchMethods(harmonyPatches, moduleProvider, loaderPluginProvider))
        {
            patches.Add(new MethodEntryHarmony
            {
                Patch = patch,
                Method = patch.PatchMethod,
                ModuleInfo = moduleInfo,
                LoaderPluginInfo = loaderPluginInfo,
                ILInstructions = DecompileILCode(patch.PatchMethod),
                CSharpILMixedInstructions = DecompileILWithCSharpCode(patch.PatchMethod),
                CSharpInstructions = DecompileCSharpCode(patch.PatchMethod),
            });
        }

        return new()
        {
            OriginalMethod = originalMethodMonoMod ?? originalMethodHarmony,
            ExecutingMethod = executingMethodMonoMod ?? executingMethodHarmony,
            Patches = patches,
        };
    }
    */
    
    public static IEnumerable<(RuntimePatch, IModuleInfo?, ILoaderPluginInfo?)> GetPatchMethods(IList<RuntimePatch> patches, IModuleProvider moduleProvider, ILoaderPluginProvider loaderPluginProvider)
    {
        if (patches is null)
            yield break;

        foreach (var patch in patches)
        {
            var method = patch.Patch;
            if (method.DeclaringType is not { } declaringType)
                continue;

            var moduleInfo = moduleProvider.GetModuleByType(declaringType);
            var loaderPluginInfo = loaderPluginProvider.GetLoaderPluginByType(declaringType);

            if (moduleInfo is not null || loaderPluginInfo is not null)
                yield return (patch, moduleInfo, loaderPluginInfo);
        }
    }

    /// <summary>
    /// Gets all involved modules in the exception stacktrace.
    /// </summary>
    public static IEnumerable<StacktraceEntry> GetAllInvolvedModules(Exception ex, ICollection<Assembly> assemblies, IAssemblyUtilities assemblyUtilities, IModuleProvider moduleProvider, ILoaderPluginProvider loaderPluginProvider, IPatchProvider patchProvider/*, IMonoModProvider monoModProvider, IHarmonyProvider harmonyProvider*/)
    {
        var inner = ex.InnerException;
        if (inner is not null)
        {
            foreach (var modInfo in GetAllInvolvedModules(inner, assemblies, assemblyUtilities, moduleProvider, loaderPluginProvider, patchProvider/*, monoModProvider, harmonyProvider*/))
                yield return modInfo;
        }

        var trace = new EnhancedStackTrace(ex);
        foreach (var frame in trace.GetFrames())
        {
            if (!frame.HasMethod()) continue;

            //var (originalMethod, executingMethod, patches, ilOffset, nativeILOffset, nativeCodePtr) = GetPatchData(frame, patchProvider/*, monoModProvider, harmonyProvider*/, moduleProvider, loaderPluginProvider);
            if (patchProvider.GetPatches(frame) is not { } data) continue;
            
            var (originalMethod, executingMethod, patches, ilOffset, nativeILOffset, nativeCodePtr) = data;
            
            yield return new()
            {
                Method = executingMethod!,
                OriginalMethod = originalMethod is not null && originalMethod != executingMethod ? new()
                {
                    Method = originalMethod,
                    ModuleInfo = GetModuleInfoIfMod(originalMethod, assemblies, assemblyUtilities, moduleProvider),
                    LoaderPluginInfo = GetLoaderPluginIfMod(originalMethod, assemblies, assemblyUtilities, loaderPluginProvider),
                    ILInstructions = DecompileILCode(originalMethod),
                    CSharpILMixedInstructions = DecompileILWithCSharpCode(originalMethod),
                    CSharpInstructions = DecompileCSharpCode(originalMethod),
                } : null,
                ModuleInfo = GetModuleInfoIfMod(executingMethod, assemblies, assemblyUtilities, moduleProvider),
                LoaderPluginInfo = GetLoaderPluginIfMod(executingMethod, assemblies, assemblyUtilities, loaderPluginProvider),
                ILOffset = ilOffset != StackFrame.OFFSET_UNKNOWN ? ilOffset : null,
                NativeOffset = nativeILOffset != StackFrame.OFFSET_UNKNOWN ? nativeILOffset : null,
                StackFrameDescription = frame.ToString(),
                NativeInstructions = DecompileNativeCode(nativeCodePtr, nativeILOffset),
                ILInstructions = DecompileILCode(executingMethod),
                CSharpILMixedInstructions = DecompileILWithCSharpCode(executingMethod),
                CSharpInstructions = DecompileCSharpCode(executingMethod),
                PatchMethods = GetPatchMethods(patches, moduleProvider, loaderPluginProvider).Select(x =>
                {
                    var (patch, moduleInfo, loaderPluginInfo) = x;
                    return new MethodEntryRuntimePatch
                    {
                        Patch = patch,
                        Method = patch.Original,
                        ModuleInfo = moduleInfo,
                        LoaderPluginInfo = loaderPluginInfo,
                        ILInstructions = DecompileILCode(patch.Patch),
                        CSharpILMixedInstructions = DecompileILWithCSharpCode(patch.Patch),
                        CSharpInstructions = DecompileCSharpCode(patch.Patch),
                    };
                }).ToArray<MethodEntry>(),
            };
        }
    }

    public static string CalculateMD5(string filename)
    {
        using var stream = File.OpenRead(filename);
        return CalculateMD5(stream);
    }

    public static string CalculateMD5(Stream stream)
    {
        using var md5 = MD5.Create();
        var hash = md5.ComputeHash(stream);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
}