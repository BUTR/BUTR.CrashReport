using BUTR.CrashReport.Decompilers.Utils;
using BUTR.CrashReport.Interfaces;
using BUTR.CrashReport.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

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
            return new()
            {
                SourceAssemblyId = assembly?.Id,
                SourceModuleId = assembly?.ModuleId,
                SourceLoaderPluginId = assembly?.LoaderPluginId,
                Type = ex.GetType().FullName ?? string.Empty,
                Message = ex.Message,
                Source = ex.Source,
                HResult = ex.HResult,
                CallStack = ex.StackTrace ?? string.Empty,
                InnerException = ex.InnerException is not null ? GetRecursiveExceptionInternal(assemblies, ex.InnerException) : null,
                AdditionalMetadata = [],
            };
        }

        return GetRecursiveExceptionInternal(assemblies, crashReport.Exception);
    }

    /// <summary>
    /// Returns the <see cref="List{EnhancedStacktraceFrameModel}"/>
    /// </summary>
    public static List<EnhancedStacktraceFrameModel> GetEnhancedStacktrace(CrashReportInfo crashReport, IReadOnlyCollection<AssemblyModel> assemblies, IAssemblyUtilities assemblyUtilities, IHttpUtilities httpUtilities)
    {
        var enhancedStacktraceFrameModels = new List<EnhancedStacktraceFrameModel>();
        foreach (var stacktrace in crashReport.Stacktrace.GroupBy(x => x.StackFrameDescription))
        {
            foreach (var entry in stacktrace)
            {
                var patches = entry.PatchMethods.Select(patchMethod =>
                {
                    var patchDecompiled = MethodDecompiler.DecompileMethod(patchMethod.Patch.Patch, null, false, assemblyUtilities.GetAssemblyStream, assemblyUtilities.GetPdbStream, httpUtilities.GetStringFromUrl);
                    return new MethodRuntimePatchModel
                    {
                        Provider = patchMethod.Patch.PatchProvider,
                        Type = patchMethod.Patch.PatchType,
                        AssemblyId = patchMethod.AssemblyId,
                        ModuleId = patchMethod.ModuleInfo?.Id,
                        LoaderPluginId = patchMethod.LoaderPluginInfo?.Id,
                        MethodDeclaredTypeName = patchMethod.Method.DeclaringType?.FullName,
                        MethodName = patchMethod.Method.Name,
                        MethodFullDescription = patchMethod.Method.FullDescription().ToString(),
                        MethodTypeParameters = patchMethod.Method is MethodInfo { IsGenericMethodDefinition: true } mi
                            ? mi.GetGenericMethodDefinition().GetGenericArguments().Select(x => x.FullName ?? x.Name).ToArray()
                            : [],
                        MethodTypeArguments = patchMethod.Method is MethodInfo { IsGenericMethodDefinition: false }
                            ? patchMethod.Method.GetGenericArguments().Select(x => x.FullDescription().ToString()).ToArray()
                            : [],
                        MethodParameters = patchMethod.Method.GetParameters().Select(x => x.ParameterType.FullDescription().ToString()).ToArray(),
                        MethodParameterNames = patchMethod.Method.GetParameters().Select(x => x.Name).ToArray(),
                        ILInstructions = new()
                        {
                            Instructions = patchDecompiled.IL.Code,
                            Highlight = null,
                        },
                        ILMixedInstructions = new()
                        {
                            Instructions = patchDecompiled.ILMixed.Code,
                            Highlight = null,
                        },
                        CSharpInstructions = new()
                        {
                            Instructions = patchDecompiled.CSharp.Code,
                            Highlight = null,
                        },
                        AdditionalMetadata = [],
                    };
                }).ToList();

                var executingAssemblyName = entry.Method.Module.Assembly.GetName();
                var executingAssembly = assemblies.FirstOrDefault(x => x.Id.Equals(executingAssemblyName));

                var originalAssemblyName = entry.OriginalMethod?.Method.Module.Assembly.GetName();
                var originalAssembly = originalAssemblyName is not null ? assemblies.FirstOrDefault(x => x.Id.Equals(originalAssemblyName)) : null;

                // Do not reverse engineer copyrighted or flagged original assemblies
                static bool IsProtected(AssemblyModel? assembly)
                {
                    if (assembly is null) return false;

                    if ((assembly.Type & AssemblyType.ProtectedFromDisassembly) != 0) return true;

                    if ((assembly.Type & AssemblyType.AllowedDisassembly) != 0) return false;

                    return (assembly.Type & AssemblyType.GameCore) != 0 || (assembly.Type & AssemblyType.GameModule) != 0;
                }

                var skipDisassemblyForOriginal = IsProtected(originalAssembly);
                var skipDisassemblyForExecuting = IsProtected(originalAssembly) || IsProtected(executingAssembly);

                var nativeInstructions = MethodDecompiler.DecompileNativeCode(entry.NativeCodePtr, entry.NativeOffset);
                var originalDecompiled = entry.OriginalMethod != null ? MethodDecompiler.DecompileMethod(entry.OriginalMethod.Method, null, skipDisassemblyForOriginal, assemblyUtilities.GetAssemblyStream, assemblyUtilities.GetPdbStream, httpUtilities.GetStringFromUrl) : null;
                var executingDecompiled = MethodDecompiler.DecompileMethod(entry.Method, entry.ILOffset, skipDisassemblyForExecuting, assemblyUtilities.GetAssemblyStream, assemblyUtilities.GetPdbStream, httpUtilities.GetStringFromUrl);
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
                        MethodFullDescription = entry.Method.FullDescription().ToString(),
                        MethodTypeParameters = entry.Method is MethodInfo { IsGenericMethodDefinition: true } mi1
                            ? mi1.GetGenericMethodDefinition().GetGenericArguments().Select(x => x.FullName ?? x.Name).ToArray()
                            : [],
                        MethodTypeArguments = entry.Method is MethodInfo { IsGenericMethodDefinition: false }
                            ? entry.Method.GetGenericArguments().Select(x => x.FullDescription().ToString()).ToArray()
                            : [],
                        MethodParameters = entry.Method.GetParameters().Select(x => x.ParameterType.FullDescription().ToString()).ToArray(),
                        MethodParameterNames = entry.Method.GetParameters().Select(x => x.Name).ToArray(),
                        NativeInstructions = new()
                        {
                            Instructions = nativeInstructions.Code,
                            Highlight = nativeInstructions.Highlight is null ? null : new()
                            {
                                StartLine = nativeInstructions.Highlight.StartLine,
                                StartColumn = nativeInstructions.Highlight.StartColumn,
                                EndLine = nativeInstructions.Highlight.EndLine,
                                EndColumn = nativeInstructions.Highlight.EndColumn,
                            }
                        },
                        ILInstructions = new()
                        {
                            Instructions = executingDecompiled.IL.Code,
                            Highlight = executingDecompiled.IL.Highlight is null ? null : new()
                            {
                                StartLine = executingDecompiled.IL.Highlight.StartLine,
                                StartColumn = executingDecompiled.IL.Highlight.StartColumn,
                                EndLine = executingDecompiled.IL.Highlight.EndLine,
                                EndColumn = executingDecompiled.IL.Highlight.EndColumn,
                            },
                        },
                        ILMixedInstructions = new()
                        {
                            Instructions = executingDecompiled.ILMixed.Code,
                            Highlight = executingDecompiled.ILMixed.Highlight is null ? null : new()
                            {
                                StartLine = executingDecompiled.ILMixed.Highlight.StartLine,
                                StartColumn = executingDecompiled.ILMixed.Highlight.StartColumn,
                                EndLine = executingDecompiled.ILMixed.Highlight.EndLine,
                                EndColumn = executingDecompiled.ILMixed.Highlight.EndColumn,
                            },
                        },
                        CSharpInstructions = new()
                        {
                            Instructions = executingDecompiled.CSharp.Code,
                            Highlight = executingDecompiled.CSharp.Highlight is null ? null : new()
                            {
                                StartLine = executingDecompiled.CSharp.Highlight.StartLine,
                                StartColumn = executingDecompiled.CSharp.Highlight.StartColumn,
                                EndLine = executingDecompiled.CSharp.Highlight.EndLine,
                                EndColumn = executingDecompiled.CSharp.Highlight.EndColumn,
                            },
                        },
                        AdditionalMetadata = [],
                    },
                    OriginalMethod = entry.OriginalMethod is not null ? new()
                    {
                        AssemblyId = originalAssembly?.Id,
                        ModuleId = entry.OriginalMethod.ModuleInfo?.Id,
                        LoaderPluginId = entry.OriginalMethod.LoaderPluginInfo?.Id,
                        MethodDeclaredTypeName = entry.OriginalMethod.Method.DeclaringType?.FullName,
                        MethodName = entry.OriginalMethod.Method.Name,
                        MethodFullDescription = entry.OriginalMethod.Method.FullDescription().ToString(),
                        MethodTypeParameters = entry.OriginalMethod.Method is MethodInfo { IsGenericMethodDefinition: true } mi2
                            ? mi2.GetGenericMethodDefinition().GetGenericArguments().Select(x => x.FullName ?? x.Name).ToArray()
                            : [],
                        MethodTypeArguments = entry.OriginalMethod.Method is MethodInfo { IsGenericMethodDefinition: false }
                            ? entry.OriginalMethod.Method.GetGenericArguments().Select(x => x.FullDescription().ToString()).ToArray()
                            : [],
                        MethodParameters = entry.OriginalMethod.Method.GetParameters().Select(x => x.ParameterType.FullDescription().ToString()).ToArray(),
                        MethodParameterNames = entry.OriginalMethod.Method.GetParameters().Select(x => x.Name).ToArray(),
                        ILInstructions = originalDecompiled is null ? null : new()
                        {
                            Instructions = originalDecompiled.IL.Code,
                            Highlight = null,
                        },
                        ILMixedInstructions = originalDecompiled is null ? null : new()
                        {
                            Instructions = originalDecompiled.ILMixed.Code,
                            Highlight = null,
                        },
                        CSharpInstructions = originalDecompiled is null ? null : new()
                        {
                            Instructions = originalDecompiled.CSharp.Code,
                            Highlight = null,
                        },
                        AdditionalMetadata = [],
                    } : null,
                    PatchMethods = patches,
                    ILOffset = entry.ILOffset,
                    NativeOffset = entry.NativeOffset,
                    AdditionalMetadata = [],
                });
            }
        }
        return enhancedStacktraceFrameModels;
    }

    /// <summary>
    /// Returns the <see cref="List{InvolvedModuleOrPluginModel}"/>
    /// </summary>
    public static List<InvolvedModuleOrPluginModel> GetInvolvedModules(List<EnhancedStacktraceFrameModel> enhancedStacktrace)
    {
        var involvedModels = new List<InvolvedModuleOrPluginModel>();
        foreach (var stacktraces in enhancedStacktrace.Where(x => !string.IsNullOrEmpty(x.ExecutingMethod.ModuleId)).GroupBy(m => m.ExecutingMethod.ModuleId!))
        {
            foreach (var stacktrace in stacktraces)
            {
                involvedModels.Add(new()
                {
                    ModuleOrLoaderPluginId = stacktraces.Key,
                    EnhancedStacktraceFrameName = stacktrace.FrameDescription,
                    Type = InvolvedModuleOrPluginType.Direct,
                    AdditionalMetadata = [],
                });
            }
        }
        foreach (var stacktrace in enhancedStacktrace)
        {
            foreach (var patch in stacktrace.PatchMethods.Where(x => !string.IsNullOrEmpty(x.ModuleId)))
            {
                if (involvedModels.Any(x => x.EnhancedStacktraceFrameName == stacktrace.FrameDescription)) continue;

                involvedModels.Add(new()
                {
                    ModuleOrLoaderPluginId = patch.ModuleId!,
                    EnhancedStacktraceFrameName = stacktrace.FrameDescription,
                    Type = InvolvedModuleOrPluginType.Patch,
                    AdditionalMetadata = [],
                });
            }
        }
        return involvedModels;
    }

    /// <summary>
    /// Returns the <see cref="List{InvolvedModuleOrPluginModel}"/>
    /// </summary>
    public static List<InvolvedModuleOrPluginModel> GetInvolvedPlugins(List<EnhancedStacktraceFrameModel> enhancedStacktrace)
    {
        var involvedPluginModels = new List<InvolvedModuleOrPluginModel>();
        foreach (var stacktraces in enhancedStacktrace.GroupBy(m => m.ExecutingMethod.LoaderPluginId))
        {
            if (stacktraces.Key is { } loaderPluginId && !string.IsNullOrEmpty(loaderPluginId))
            {
                involvedPluginModels.Add(new()
                {
                    ModuleOrLoaderPluginId = loaderPluginId,
                    EnhancedStacktraceFrameName = stacktraces.Last().FrameDescription,
                    Type = InvolvedModuleOrPluginType.Direct,
                    AdditionalMetadata = [],
                });
            }
        }
        foreach (var stacktrace in enhancedStacktrace)
        {
            foreach (var patch in stacktrace.PatchMethods)
            {
                if (string.IsNullOrEmpty(patch.LoaderPluginId)) continue;
                if (involvedPluginModels.Any(x => x.EnhancedStacktraceFrameName == stacktrace.FrameDescription)) continue;

                involvedPluginModels.Add(new()
                {
                    ModuleOrLoaderPluginId = patch.LoaderPluginId!,
                    EnhancedStacktraceFrameName = stacktrace.FrameDescription,
                    Type = InvolvedModuleOrPluginType.Patch,
                    AdditionalMetadata = [],
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
        foreach (var (assemblyName, assembly) in crashReport.AvailableAssemblies)
        {
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
                AdditionalMetadata = [],
            });
        }

        return assemblyModels;
    }

    /// <summary>
    /// Returns the <see cref="List{RuntimePatchesModel}"/>
    /// </summary>
    public static List<RuntimePatchesModel> GetRuntimePatches(CrashReportInfo crashReport, IModuleProvider moduleProvider, ILoaderPluginProvider loaderPluginProvider)
    {
        var builder = new List<RuntimePatchesModel>(crashReport.LoadedManagedRuntimePatches.Count);

        foreach (var (originalMethod, patches) in crashReport.LoadedManagedRuntimePatches)
        {
            var patchesBuilder = new List<RuntimePatchModel>(patches.Count);
            foreach (var patch in patches)
            {
                var method = patch.Patch;
                if (method.DeclaringType is not { } declaringType)
                    continue;

                var assemblyId = AssemblyIdModel.FromAssembly(patch.Patch.Module.Assembly.GetName());
                var moduleInfo = moduleProvider.GetModuleByType(declaringType);
                var loaderPluginInfo = loaderPluginProvider.GetLoaderPluginByType(declaringType);

                //if (moduleInfo is not null || loaderPluginInfo is not null)
                patchesBuilder.Add(new()
                {
                    ModuleId = moduleInfo?.Id,
                    LoaderPluginId = loaderPluginInfo?.Id,
                    AssemblyId = assemblyId,
                    Provider = patch.PatchProvider,
                    Type = patch.PatchType,
                    FullName = $"{method.DeclaringType.FullName}.{method.Name}",
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
                    AdditionalMetadata = [],
                });
            }
        }

        foreach (var (originalMethodPtr, patches) in crashReport.LoadedNativeRuntimePatches)
        {
            var patchesBuilder = new List<RuntimePatchModel>(patches.Count);
            foreach (var patch in patches)
            {
                var method = patch.Patch;
                if (method.DeclaringType is not { } declaringType)
                    continue;

                var assemblyId = AssemblyIdModel.FromAssembly(patch.Patch.Module.Assembly.GetName());
                var moduleInfo = moduleProvider.GetModuleByType(declaringType);
                var loaderPluginInfo = loaderPluginProvider.GetLoaderPluginByType(declaringType);

                //if (moduleInfo is not null || loaderPluginInfo is not null)
                patchesBuilder.Add(new()
                {
                    ModuleId = moduleInfo?.Id,
                    LoaderPluginId = loaderPluginInfo?.Id,
                    AssemblyId = assemblyId,
                    Provider = patch.PatchProvider,
                    Type = patch.PatchType,
                    FullName = $"{method.DeclaringType.FullName}.{method.Name}",
                    AdditionalMetadata = patch.AdditionalMetadata,
                });
            }

            if (patchesBuilder.Count > 0)
            {
                builder.Add(new()
                {
                    OriginalMethodDeclaredTypeName = null,
                    OriginalMethodName = originalMethodPtr.ToString(),
                    Patches = patchesBuilder,
                    AdditionalMetadata = [],
                });
            }
        }

        return builder;
    }
}