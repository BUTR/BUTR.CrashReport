﻿using BUTR.CrashReport.Interfaces;
using BUTR.CrashReport.Models;

using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using static BUTR.CrashReport.Decompilers.Utils.MethodDecompiler;
using static BUTR.CrashReport.Decompilers.Utils.MonoModUtils;

using HarmonyPatch = BUTR.CrashReport.Models.HarmonyPatch;

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
        public required MethodBase? Original { get; set; }
        
        /// <summary>
        /// <inheritdoc cref="EnhancedStacktraceFrameModel.ExecutingMethod"/>
        /// </summary>
        public required MethodInfo? Replacement { get; set; }
        
        /// <summary>
        /// <inheritdoc cref="EnhancedStacktraceFrameModel.PatchMethods"/>
        /// </summary>
        public required List<MethodEntry> Patches { get; set; }
        
        /// <summary>
        /// <inheritdoc cref="EnhancedStacktraceFrameModel.MethodFromStackframeIssue"/>
        /// </summary>
        public required bool Issues { get; set; }

        /// <summary>
        /// Deconstructs the object.
        /// </summary>
        public void Deconstruct(out MethodBase? original, out MethodInfo? replacement, out List<MethodEntry> patches, out bool issues)
        {
            original = Original;
            replacement = Replacement;
            patches = Patches;
            issues = Issues;
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

    /// <summary>
    /// Gets the module info if the method is from a mod.
    /// </summary>
    public static IModuleInfo? GetModuleInfoIfMod(MethodBase? method, IEnumerable<Assembly> assemblies, IModuleProvider moduleProvider)
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
                .SelectMany(AccessTools.GetTypesFromAssembly)
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
    public static ILoaderPluginInfo? GetLoaderPluginIfMod(MethodBase? method, IEnumerable<Assembly> assemblies, ILoaderPluginProvider loaderPluginProvider)
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
                .SelectMany(AccessTools.GetTypesFromAssembly)
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

    /// <summary>
    /// Gets the Harmony data from the stackframe.
    /// </summary>
    public static StackframePatchData GetHarmonyData(StackFrame frame, IHarmonyProvider harmonyProvider, IModuleProvider moduleProvider, ILoaderPluginProvider loaderPluginProvider)
    {
        MethodBase? method;
        var methodFromStackframeIssue = false;
        try
        {
            method = harmonyProvider.GetMethodFromStackframe(frame);
        }
        // NullReferenceException means the method was not found. Harmony doesn't handle this case gracefully
        catch (NullReferenceException e)
        {
            Trace.TraceError(e.ToString());
            method = frame.GetMethod()!;
        }
        // The given generic instantiation was invalid.
        // From what I understand, this will occur with generic methods
        // Also when static constructors throw errors, Harmony resolution will fail
        catch (Exception e)
        {
            Trace.TraceError(e.ToString());
            methodFromStackframeIssue = true;
            method = frame.GetMethod()!;
        }

        var methods = new List<MethodEntry>();
        var identifiableMethod = method is MethodInfo mi ? GetIdentifiable(mi) is MethodInfo v ? v : mi : null;
        var original = identifiableMethod is not null ? harmonyProvider.GetOriginalMethod(identifiableMethod) : null;
        var patches = original is not null ? harmonyProvider.GetPatchInfo(original) : null;

        foreach (var (patch, moduleInfo, loaderPluginInfo) in GetHarmonyPatchMethods(patches, moduleProvider, loaderPluginProvider))
        {
            methods.Add(new MethodEntryHarmony
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
            Original = original,
            Replacement = identifiableMethod,
            Patches = methods,
            Issues = methodFromStackframeIssue,
        };
    }

    /// <summary>
    /// Gets all involved modules in the exception stacktrace.
    /// </summary>
    public static IEnumerable<StacktraceEntry> GetAllInvolvedModules(Exception ex, ICollection<Assembly> assemblies, IModuleProvider moduleProvider, ILoaderPluginProvider loaderPluginProvider, IHarmonyProvider harmonyProvider)
    {
        var inner = ex.InnerException;
        if (inner is not null)
        {
            foreach (var modInfo in GetAllInvolvedModules(inner, assemblies, moduleProvider, loaderPluginProvider, harmonyProvider))
                yield return modInfo;
        }

        var trace = new EnhancedStackTrace(ex);
        foreach (var frame in trace.GetFrames())
        {
            if (!frame.HasMethod()) continue;

            var (original, identifiableMethod, patches, methodFromStackframeIssue) = GetHarmonyData(frame, harmonyProvider, moduleProvider, loaderPluginProvider);

            var ilOffset = frame.GetILOffset();
            var nativeILOffset = frame.GetNativeOffset();
            yield return new()
            {
                Method = identifiableMethod!,
                OriginalMethod = original is not null ? new()
                {
                    Method = original,
                    ModuleInfo = GetModuleInfoIfMod(original, assemblies, moduleProvider),
                    LoaderPluginInfo = GetLoaderPluginIfMod(original, assemblies, loaderPluginProvider),
                    ILInstructions = DecompileILCode(original),
                    CSharpILMixedInstructions = DecompileILWithCSharpCode(original),
                    CSharpInstructions = DecompileCSharpCode(original),
                } : null,
                MethodFromStackframeIssue = methodFromStackframeIssue,
                ModuleInfo = GetModuleInfoIfMod(identifiableMethod, assemblies, moduleProvider),
                LoaderPluginInfo = GetLoaderPluginIfMod(identifiableMethod, assemblies, loaderPluginProvider),
                ILOffset = ilOffset != StackFrame.OFFSET_UNKNOWN ? ilOffset : null,
                NativeOffset = nativeILOffset != StackFrame.OFFSET_UNKNOWN ? nativeILOffset : null,
                StackFrameDescription = frame.ToString(),
                NativeInstructions = DecompileNativeCode(identifiableMethod, nativeILOffset),
                ILInstructions = DecompileILCode(identifiableMethod),
                CSharpILMixedInstructions = DecompileILWithCSharpCode(identifiableMethod),
                CSharpInstructions = DecompileCSharpCode(identifiableMethod),
                PatchMethods = patches.ToArray(),
            };
        }
    }
}