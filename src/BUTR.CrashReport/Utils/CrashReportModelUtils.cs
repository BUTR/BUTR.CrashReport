using BUTR.CrashReport.Extensions;
using BUTR.CrashReport.Interfaces;
using BUTR.CrashReport.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;

namespace BUTR.CrashReport.Utils;

/// <summary>
/// Exposes the code we use to create the <see cref="CrashReportModel"/>
/// </summary>
public static class CrashReportModelUtils
{
    /// <summary>
    /// Returns the <see cref="ExceptionModel"/>
    /// </summary>
    public static ExceptionModel GetRecursiveException(CrashReportInfo crashReport, IReadOnlyCollection<AssemblyModel> assemblies)
    {
        static ExceptionModel GetRecursiveExceptionInternal(IReadOnlyCollection<AssemblyModel> assemblies, Exception ex)
        {
            // TODO: Check if there are collisions
            var assembly = assemblies.FirstOrDefault(y => y.Id.Name == ex.Source);
            return new ExceptionModel
            {
                SourceAssemblyId = assembly?.Id,
                SourceModuleId = assembly?.ModuleId,
                SourceLoaderPluginId = assembly?.LoaderPluginId,
                Type = ex.GetType().FullName ?? string.Empty,
                Message = ex.Message,
                CallStack = ex.StackTrace ?? string.Empty,
                InnerException = ex.InnerException is not null ? GetRecursiveExceptionInternal(assemblies, ex.InnerException) : null,
                AdditionalMetadata = Array.Empty<MetadataModel>(),
            };
        }

        return GetRecursiveExceptionInternal(assemblies, crashReport.Exception);
    }

    /// <summary>
    /// Returns the <see cref="List{EnhancedStacktraceFrameModel}"/>
    /// </summary>
    public static List<EnhancedStacktraceFrameModel> GetEnhancedStacktrace(CrashReportInfo crashReport, IReadOnlyCollection<AssemblyModel> assemblies)
    {
        var enhancedStacktraceFrameModels = new List<EnhancedStacktraceFrameModel>();
        foreach (var stacktrace in crashReport.Stacktrace.GroupBy(x => x.StackFrameDescription))
        {
            foreach (var entry in stacktrace)
            {
                var methods = new List<MethodSimpleModel>(entry.PatchMethods.Length);
                foreach (var patchMethod in entry.PatchMethods)
                {
                    var patchAssemblyName = entry.Method.DeclaringType?.Assembly.GetName();
                    var patchAssembly = patchAssemblyName is not null ? assemblies.FirstOrDefault(x => x.Id.Equals(patchAssemblyName)) : null;
                    var methodSimple = new MethodSimpleModel
                    {
                        AssemblyId = patchAssembly?.Id,
                        ModuleId = patchMethod.ModuleInfo?.Id,
                        LoaderPluginId = patchMethod.LoaderPluginInfo?.Id,
                        MethodDeclaredTypeName = patchMethod.Method.DeclaringType?.FullName,
                        MethodName = patchMethod.Method.Name,
                        MethodFullDescription = patchMethod.Method.FullDescription(),
                        MethodParameters = patchMethod.Method.GetParameters().Select(x => x.ParameterType.FullName ?? string.Empty).ToArray(),
                        ILInstructions = patchMethod.ILInstructions,
                        CSharpILMixedInstructions = patchMethod.CSharpILMixedInstructions,
                        CSharpInstructions = patchMethod.CSharpInstructions,
                        AdditionalMetadata = Array.Empty<MetadataModel>(),
                    };
                    methods.Add(patchMethod switch
                    {
                        /*
                        MethodEntryHarmony meh => methodSimple with
                        {
                            AdditionalMetadata = methodSimple.AdditionalMetadata.Append(new MetadataModel { Key = "HarmonyPatchType", Value = meh.Patch.Type.ToString() }).ToArray()
                        },
                        */
                        _ => methodSimple
                    });
                }

                var executingAssemblyName = entry.Method.DeclaringType?.Assembly.GetName();
                var executingAssembly = executingAssemblyName is not null ? assemblies.FirstOrDefault(x => x.Id.Equals(executingAssemblyName)) : null;

                var originalAssemblyName = entry.OriginalMethod?.Method.DeclaringType?.Assembly.GetName();
                var originalAssembly = originalAssemblyName is not null ? assemblies.FirstOrDefault(x => x.Id.Equals(originalAssemblyName)) : null;

                // Do not reverse engineer copyrighted or flagged original assemblies
                static bool IsProtected(AssemblyModel? assembly) => assembly is not null &&
                                                                    ((assembly.Type & AssemblyType.GameCore) != 0 ||
                                                                     (assembly.Type & AssemblyType.GameModule) != 0 ||
                                                                     (assembly.Type & AssemblyType.ProtectedFromDisassembly) != 0);

                var skipDisassemblyForOriginal = IsProtected(originalAssembly);
                var skipDisassemblyForExecuting = IsProtected(originalAssembly) || IsProtected(executingAssembly);

                enhancedStacktraceFrameModels.Add(new()
                {
                    FrameDescription = entry.StackFrameDescription,
                    ExecutingMethod = new()
                    {
                        AssemblyId = executingAssembly?.Id,
                        ModuleId = entry.ModuleInfo?.Id,
                        LoaderPluginId = entry.LoaderPluginInfo?.Id,
                        MethodDeclaredTypeName = entry.Method.DeclaringType?.FullName,
                        MethodName = entry.Method.Name,
                        MethodFullDescription = entry.Method.FullDescription(),
                        MethodParameters = entry.Method.GetParameters().Select(x => x.ParameterType.FullName ?? string.Empty).ToArray(),
                        NativeInstructions = entry.NativeInstructions,
                        ILInstructions = entry.ILInstructions,
                        CSharpILMixedInstructions = skipDisassemblyForExecuting ? [] : entry.CSharpILMixedInstructions,
                        CSharpInstructions = skipDisassemblyForExecuting ? [] : entry.CSharpInstructions,
                        AdditionalMetadata = Array.Empty<MetadataModel>(),
                    },
                    OriginalMethod = entry.OriginalMethod is not null ? new()
                    {
                        AssemblyId = originalAssembly?.Id,
                        ModuleId = entry.OriginalMethod.ModuleInfo?.Id,
                        LoaderPluginId = entry.OriginalMethod.LoaderPluginInfo?.Id,
                        MethodDeclaredTypeName = entry.OriginalMethod.Method.DeclaringType?.FullName,
                        MethodName = entry.OriginalMethod.Method.Name,
                        MethodFullDescription = entry.OriginalMethod.Method.FullDescription(),
                        MethodParameters = entry.OriginalMethod.Method.GetParameters().Select(x => x.ParameterType.FullName ?? string.Empty).ToArray(),
                        ILInstructions = entry.OriginalMethod.ILInstructions,
                        CSharpILMixedInstructions = skipDisassemblyForOriginal ? [] : entry.OriginalMethod.CSharpILMixedInstructions,
                        CSharpInstructions = skipDisassemblyForOriginal ? [] : entry.OriginalMethod.CSharpInstructions,
                        AdditionalMetadata = Array.Empty<MetadataModel>()
                    } : null,
                    PatchMethods = methods,
                    ILOffset = entry.ILOffset,
                    NativeOffset = entry.NativeOffset,
                    AdditionalMetadata = Array.Empty<MetadataModel>(),
                });
            }
        }
        return enhancedStacktraceFrameModels;
    }

    /// <summary>
    /// Returns the <see cref="List{InvolvedModuleOrPluginModel}"/>
    /// </summary>
    public static List<InvolvedModuleOrPluginModel> GetInvolvedModules(CrashReportInfo crashReport)
    {
        var involvedModels = new List<InvolvedModuleOrPluginModel>();
        foreach (var stacktraces in crashReport.FilteredStacktrace.GroupBy(m => m.ModuleInfo))
        {
            if (stacktraces.Key is { } module)
            {
                involvedModels.Add(new()
                {
                    ModuleOrLoaderPluginId = module.Id,
                    EnhancedStacktraceFrameName = stacktraces.Last().StackFrameDescription,
                    AdditionalMetadata = Array.Empty<MetadataModel>(),
                });
            }
        }
        foreach (var stacktrace in crashReport.FilteredStacktrace)
        {
            foreach (var patch in stacktrace.PatchMethods)
            {
                if (patch.ModuleInfo is null) continue;

                involvedModels.Add(new()
                {
                    ModuleOrLoaderPluginId = patch.ModuleInfo.Id,
                    EnhancedStacktraceFrameName = stacktrace.StackFrameDescription,
                    AdditionalMetadata = Array.Empty<MetadataModel>(),
                });
            }
        }
        return involvedModels;
    }

    /// <summary>
    /// Returns the <see cref="List{InvolvedModuleOrPluginModel}"/>
    /// </summary>
    public static List<InvolvedModuleOrPluginModel> GetInvolvedPlugins(CrashReportInfo crashReport)
    {
        var involvedPluginModels = new List<InvolvedModuleOrPluginModel>();
        foreach (var stacktraces in crashReport.FilteredStacktrace.GroupBy(m => m.LoaderPluginInfo))
        {
            if (stacktraces.Key is { } loaderPlugin)
            {
                involvedPluginModels.Add(new()
                {
                    ModuleOrLoaderPluginId = loaderPlugin.Id,
                    EnhancedStacktraceFrameName = stacktraces.Last().StackFrameDescription,
                    AdditionalMetadata = Array.Empty<MetadataModel>(),
                });
            }
        }
        foreach (var stacktrace in crashReport.FilteredStacktrace)
        {
            foreach (var patch in stacktrace.PatchMethods)
            {
                if (patch.LoaderPluginInfo is null) continue;

                involvedPluginModels.Add(new()
                {
                    ModuleOrLoaderPluginId = patch.LoaderPluginInfo.Id,
                    EnhancedStacktraceFrameName = stacktrace.StackFrameDescription,
                    AdditionalMetadata = Array.Empty<MetadataModel>(),
                });
            }
        }
        return involvedPluginModels;
    }

    /// <summary>
    /// Returns the <see cref="List{AssemblyModel}"/>
    /// </summary>
    public static List<AssemblyModel> GetAssemblies(CrashReportInfo crashReport, IAssemblyUtilities assemblyUtilities, IPathAnonymizer pathAnonymizer)
    {
        static bool IsGAC(Assembly assembly)
        {
            try
            {
                return assembly.GlobalAssemblyCache;
            }
            catch (Exception) { return false; }
        }
        static ProcessorArchitecture GetProcessorArchitecture(AssemblyName assemblyName)
        {
            try
            {
                return assemblyName.ProcessorArchitecture;
            }
            catch (Exception) { return ProcessorArchitecture.None; }
        }
        static bool IsProtectedFromDisassembly(Assembly assembly)
        {
            var attibutes = assembly.GetCustomAttributes<AssemblyMetadataAttribute>();
            return attibutes.Any(x => x.Key == "ProtectedFromDisassembly" && !string.Equals(x.Value, "false", StringComparison.OrdinalIgnoreCase));
        }

        var assemblyModels = new List<AssemblyModel>(crashReport.AvailableAssemblies.Count);

        var systemAssemblyDirectory = Path.GetDirectoryName(typeof(object).Assembly.Location);
        foreach (var kv in crashReport.AvailableAssemblies)
        {
            var assemblyName = kv.Key;
            var assembly = kv.Value;

            var type = AssemblyType.Unclassified;

            // TODO: On unity the system folder is the unity root folder.
            // With unity thre is not system folder, so everything will classified as Game
            // TODO: BepInEx detection
            var isSystem =
                (!assembly.IsDynamic && !string.IsNullOrWhiteSpace(assembly.Location) && Path.GetDirectoryName(assembly.Location)?.Equals(systemAssemblyDirectory, StringComparison.Ordinal) == true) ||
                (assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product == "Microsoft® .NET Framework") ||
                (assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product == "Microsoft® .NET");

            if (isSystem) type |= AssemblyType.System;
            if (assembly.IsDynamic) type |= AssemblyType.Dynamic;
            if (IsGAC(assembly)) type |= AssemblyType.GAC;
            if (IsProtectedFromDisassembly(assembly)) type |= AssemblyType.ProtectedFromDisassembly;

            var module = assemblyUtilities.GetAssemblyModule(crashReport, assembly);
            if (module is not null) type |= AssemblyType.Module;
            var loaderPlugin = assemblyUtilities.GetAssemblyPlugin(crashReport, assembly);
            if (loaderPlugin is not null) type |= AssemblyType.LoaderPlugin;

            type = assemblyUtilities.GetAssemblyType(type, crashReport, assembly);

            var path = !assembly.IsDynamic ? assembly.Location : string.Empty;
            var anonymizedPath = string.Empty;
            if (!assembly.IsDynamic && !pathAnonymizer.TryHandlePath(path, out anonymizedPath))
                anonymizedPath = Anonymizer.AnonymizePath(path);

            assemblyModels.Add(new()
            {
                Id = AssemblyIdModel.FromAssembly(assemblyName),
                ModuleId = module?.Id,
                LoaderPluginId = loaderPlugin?.Id,
                CultureName = assemblyName.CultureName,
                Architecture = (AssemblyArchitectureType) GetProcessorArchitecture(assemblyName),
                Hash = assembly.IsDynamic || string.IsNullOrWhiteSpace(assembly.Location) || !File.Exists(assembly.Location) ? "NONE" : CrashReportUtils.CalculateMD5(assembly.Location),
                AnonymizedPath = assembly.IsDynamic ? "DYNAMIC" : string.IsNullOrWhiteSpace(assembly.Location) ? "EMPTY" : !File.Exists(assembly.Location) ? "MISSING" : anonymizedPath,
                Type = type,
                ImportedTypeReferences = (type & AssemblyType.System) == 0
                    ? crashReport.ImportedTypeReferences.TryGetValue(assemblyName, out var values) ? values.Select(x => new AssemblyImportedTypeReferenceModel
                    {
                        Namespace = x.Namespace,
                        Name = x.Name,
                        FullName = x.FullName,
                    }).ToArray() : []
                    : [],
                ImportedAssemblyReferences = (type & AssemblyType.System) == 0
                    ? assembly.GetReferencedAssemblies().Select(AssemblyImportedReferenceModelExtensions.Create).ToArray()
                    : [],
                AdditionalMetadata = Array.Empty<MetadataModel>(),
            });
        }

        return assemblyModels;
    }
    
    /*
    /// <summary>
    /// Returns the <see cref="List{HarmonyPatchesModel}"/>
    /// </summary>
    public static List<HarmonyPatchesModel> GetHarmonyPatches(CrashReportInfo crashReport, IReadOnlyCollection<AssemblyModel> assemblies, IModuleProvider moduleProvider, ILoaderPluginProvider loaderPluginProvider)
    {
        var builder = new List<HarmonyPatchesModel>(crashReport.LoadedHarmonyPatches.Count);

        static void AppendPatches(ICollection<HarmonyPatchModel> builder, HarmonyPatchType type, IEnumerable<HarmonyPatch> patches, IReadOnlyCollection<AssemblyModel> assemblies, IModuleProvider moduleProvider, ILoaderPluginProvider loaderPluginProvider)
        {
            foreach (var patch in patches)
            {
                var assemblyId = patch.PatchMethod.DeclaringType?.Assembly.GetName() is { } asmName ? AssemblyIdModel.FromAssembly(asmName) : null;
                var module = moduleProvider.GetModuleByType(patch.PatchMethod.DeclaringType);
                var loaderPlugin = loaderPluginProvider.GetLoaderPluginByType(patch.PatchMethod.DeclaringType);

                builder.Add(new()
                {
                    Type = type,
                    AssemblyId = assemblyId,
                    ModuleId = module?.Id,
                    LoaderPluginId = loaderPlugin?.Id,
                    Owner = patch.Owner,
                    Namespace = $"{patch.PatchMethod.DeclaringType!.FullName}.{patch.PatchMethod.Name}",
                    Index = patch.Index,
                    Priority = patch.Priority,
                    Before = patch.Before,
                    After = patch.After,
                    AdditionalMetadata = Array.Empty<MetadataModel>(),
                });
            }
        }

        foreach (var kv in crashReport.LoadedHarmonyPatches)
        {
            var originalMethod = kv.Key;
            var patches = kv.Value;

            var patchBuilder = new List<HarmonyPatchModel>(patches.Prefixes.Count + patches.Postfixes.Count + patches.Finalizers.Count + patches.Transpilers.Count);

            AppendPatches(patchBuilder, HarmonyPatchType.Prefix, patches.Prefixes, assemblies, moduleProvider, loaderPluginProvider);
            AppendPatches(patchBuilder, HarmonyPatchType.Postfix, patches.Postfixes, assemblies, moduleProvider, loaderPluginProvider);
            AppendPatches(patchBuilder, HarmonyPatchType.Finalizer, patches.Finalizers, assemblies, moduleProvider, loaderPluginProvider);
            AppendPatches(patchBuilder, HarmonyPatchType.Transpiler, patches.Transpilers, assemblies, moduleProvider, loaderPluginProvider);

            if (patchBuilder.Count > 0)
            {
                builder.Add(new()
                {
                    OriginalMethodDeclaredTypeName = originalMethod.DeclaringType?.FullName,
                    OriginalMethodName = originalMethod.Name,
                    Patches = patchBuilder,
                    AdditionalMetadata = Array.Empty<MetadataModel>(),
                });
            }
        }

        return builder;
    }

    /// <summary>
    /// Returns the <see cref="List{HarmonyPatchesModel}"/>
    /// </summary>
    public static List<MonoModPatchesModel> GetMonoModDetours(CrashReportInfo crashReport, IReadOnlyCollection<AssemblyModel> assemblies, IModuleProvider moduleProvider, ILoaderPluginProvider loaderPluginProvider)
    {
        var builder = new List<MonoModPatchesModel>(crashReport.LoadedMonoModPatches.Count);

        static void AppendPatches(ICollection<MonoModPatchModel> builder, MonoModPatchModelType type, IEnumerable<MonoModPatch> patches, IReadOnlyCollection<AssemblyModel> assemblies, IModuleProvider moduleProvider, ILoaderPluginProvider loaderPluginProvider)
        {
            foreach (var patch in patches)
            {
                var assemblyId = patch.Method.DeclaringType?.Assembly.GetName() is { } asmName ? AssemblyIdModel.FromAssembly(asmName) : null;
                var module = moduleProvider.GetModuleByType(patch.Method.DeclaringType);
                var loaderPlugin = loaderPluginProvider.GetLoaderPluginByType(patch.Method.DeclaringType);

                builder.Add(new()
                {
                    Id = patch.Id,
                    Namespace = $"{patch.Method.DeclaringType!.FullName}.{patch.Method.Name}",
                    Type = type,
                    IsActive = false,
                    AssemblyId = assemblyId,
                    ModuleId = module?.Id,
                    LoaderPluginId = loaderPlugin?.Id,
                    Index = patch.Index,
                    MaxIndex = patch.MaxIndex,
                    GlobalIndex = patch.GlobalIndex,
                    Priority = patch.Priority,
                    SubPriority = patch.SubPriority,
                    Before = patch.Before,
                    After = patch.After,
                    AdditionalMetadata = Array.Empty<MetadataModel>(),
                });
            }
        }

        foreach (var kv in crashReport.LoadedMonoModPatches)
        {
            var originalMethod = kv.Key;
            var patches = kv.Value;

            var detoursBuilder = new List<MonoModPatchModel>(patches.Detours.Count);

            AppendPatches(detoursBuilder, MonoModPatchModelType.Detour, patches.Detours, assemblies, moduleProvider, loaderPluginProvider);
            AppendPatches(detoursBuilder, MonoModPatchModelType.ILHook, patches.ILHooks, assemblies, moduleProvider, loaderPluginProvider);

            if (detoursBuilder.Count > 0)
            {
                builder.Add(new()
                {
                    OriginalMethodDeclaredTypeName = originalMethod.DeclaringType?.FullName,
                    OriginalMethodName = originalMethod.Name,
                    Detours = detoursBuilder,
                    AdditionalMetadata = Array.Empty<MetadataModel>(),
                });
            }
        }

        return builder;
    }
    */
    
    public static List<RuntimePatchesModel> GetRuntimePatches(CrashReportInfo crashReport, IReadOnlyCollection<AssemblyModel> assemblies, IModuleProvider moduleProvider, ILoaderPluginProvider loaderPluginProvider)
    {
        var builder = new List<RuntimePatchesModel>(crashReport.LoadedRuntimePatches.Count);

        foreach (var kv in crashReport.LoadedRuntimePatches)
        {
            var originalMethod = kv.Key;
            var patches = kv.Value;

            var patchesBuilder = new List<RuntimePatchModel>(crashReport.LoadedRuntimePatches.Count);
            foreach (var patch in patches)
            {
                var assemblyId = patch.Patch.DeclaringType?.Assembly.GetName() is { } asmName ? AssemblyIdModel.FromAssembly(asmName) : null;
                var module = moduleProvider.GetModuleByType(patch.Patch.DeclaringType);
                var loaderPlugin = loaderPluginProvider.GetLoaderPluginByType(patch.Patch.DeclaringType);

                patchesBuilder.Add(new()
                {
                    AssemblyId = assemblyId,
                    ModuleId = module?.Id,
                    LoaderPluginId = loaderPlugin?.Id,
                    Provider = patch.PatchProvider,
                    Type = patch.PatchType,
                    FullName = patch.Patch.DeclaringType is not null ? $"{patch.Patch.DeclaringType.FullName}.{patch.Patch.Name}" : patch.Patch.Name,
                    AdditionalMetadata = patch.AdditionalMetadata,
                });
            }

            if (patchesBuilder.Count > 0)
            {
                builder.Add(new()
                {
                    OriginalMethodDeclaredTypeName = originalMethod.DeclaringType?.FullName,
                    OriginalMethodName = originalMethod.Name,
                    Patches = patchesBuilder,
                    AdditionalMetadata = Array.Empty<MetadataModel>(),
                });
            }
        }

        return builder;
    }
}