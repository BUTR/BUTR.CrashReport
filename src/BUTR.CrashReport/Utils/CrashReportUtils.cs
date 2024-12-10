using BUTR.CrashReport.Interfaces;
using BUTR.CrashReport.Models;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;

namespace BUTR.CrashReport.Utils;

/// <summary>
/// Exposes the code we use to create the <see cref="CrashReportInfo"/>
/// </summary>
public static class CrashReportUtils
{
    private static readonly char[] ParenthesisSeparator = ['('];
    private static readonly string[] PathSeparator = ["_Patch"];

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
            var methodName = method.Name.Split(ParenthesisSeparator)[0];
            var patchPostfix = methodName.Split(PathSeparator, StringSplitOptions.None);

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
            var methodName = method.Name.Split(ParenthesisSeparator)[0];
            var patchPostfix = methodName.Split(PathSeparator, StringSplitOptions.None);

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

    /// <summary>
    /// Gets all involved modules in the exception stacktrace.
    /// </summary>
    public static IEnumerable<StacktraceEntry> GetEnhancedStacktrace(Exception ex, ICollection<Assembly> assemblies, IAssemblyUtilities assemblyUtilities, IModuleProvider moduleProvider, ILoaderPluginProvider loaderPluginProvider, IRuntimePatchProvider runtimePatchProvider)
    {
        var inner = ex.InnerException;
        if (inner is not null)
        {
            foreach (var modInfo in GetEnhancedStacktrace(inner, assemblies, assemblyUtilities, moduleProvider, loaderPluginProvider, runtimePatchProvider))
                yield return modInfo;
        }

        var trace = new EnhancedStackTrace(ex);
        foreach (var frame in trace.GetFrames())
        {
            if (!frame.HasMethod()) continue;

            var method = frame.GetMethod();
            var ilOffset = frame.GetILOffset();
            var nativeOffset = frame.GetNativeOffset();

            if (runtimePatchProvider.GetPatchInfo(method) is not { } data) continue;
            var (originalMethod, executingMethod, patches, nativeCodePtr) = data;

            yield return new()
            {
                Method = executingMethod!,
                OriginalMethod = originalMethod is not null && originalMethod != executingMethod ? new()
                {
                    Method = originalMethod,
                    AssemblyId = AssemblyIdModel.FromAssembly(originalMethod.Module.Assembly.GetName()),
                    ModuleInfo = GetModuleInfoIfMod(originalMethod, assemblies, assemblyUtilities, moduleProvider),
                    LoaderPluginInfo = GetLoaderPluginIfMod(originalMethod, assemblies, assemblyUtilities, loaderPluginProvider),
                } : null,
                ModuleInfo = GetModuleInfoIfMod(executingMethod, assemblies, assemblyUtilities, moduleProvider),
                LoaderPluginInfo = GetLoaderPluginIfMod(executingMethod, assemblies, assemblyUtilities, loaderPluginProvider),
                ILOffset = ilOffset,
                NativeOffset = nativeOffset,
                NativeCodePtr = nativeCodePtr,
                StackFrameDescription = frame.ToString(),
                PatchMethods = patches.Select(patch =>
                {
                    var assemblyId = AssemblyIdModel.FromAssembly(patch.Patch.Module.Assembly.GetName());
                    var moduleInfo = moduleProvider.GetModuleByType(patch.Patch.DeclaringType);
                    var loaderPluginInfo = loaderPluginProvider.GetLoaderPluginByType(patch.Patch.DeclaringType);
                    return new MethodRuntimePatchEntry
                    {
                        Patch = patch,
                        Method = patch.Patch,
                        AssemblyId = assemblyId,
                        ModuleInfo = moduleInfo,
                        LoaderPluginInfo = loaderPluginInfo,
                    };
                }).ToArray(),
            };
        }
    }

    /// <summary>
    /// Get the MD5 hash of a file.
    /// </summary>
    public static string CalculateMD5(string filename)
    {
        using var stream = File.OpenRead(filename);
        return CalculateMD5(stream);
    }

    /// <summary>
    /// Get the MD5 hash of a stream.
    /// </summary>
    public static string CalculateMD5(Stream stream)
    {
        using var md5 = MD5.Create();
        var hash = md5.ComputeHash(stream);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
}