// <auto-generated>
//   This code file has automatically been added by the "BUTR.CrashReport.Bannerlord.Source" NuGet package (https://www.nuget.org/packages/BUTR.CrashReport.Bannerlord.Source).
//   Please see https://github.com/BUTR/BUTR.CrashReport for more information.
//
//   IMPORTANT:
//   DO NOT DELETE THIS FILE if you are using a "packages.config" file to manage your NuGet references.
//   Consider migrating to PackageReferences instead:
//   https://docs.microsoft.com/en-us/nuget/consume-packages/migrate-packages-config-to-package-reference
//   Migrating brings the following benefits:
//   * The "BUTR.CrashReport.Bannerlord.Source" folder and the "CrashReportCreator.cs" file don't appear in your project.
//   * The added file is immutable and can therefore not be modified by coincidence.
//   * Updating/Uninstalling the package will work flawlessly.
// </auto-generated>

#region License
// MIT License
//
// Copyright (c) Bannerlord's Unofficial Tools & Resources
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion

#if !BUTRCRASHREPORT_DISABLE
#nullable enable
#if !BUTRCRASHREPORT_ENABLEWARNINGS
#pragma warning disable
#endif

namespace BUTR.CrashReport.Bannerlord
{
    using global::Bannerlord.BUTR.Shared.Extensions;
    using global::Bannerlord.BUTR.Shared.Helpers;
    using global::Bannerlord.ModuleManager;

    using global::BUTR.CrashReport.Extensions;
    using global::BUTR.CrashReport.Models;
    using global::BUTR.CrashReport.Utils;

    using global::HarmonyLib;

    using global::System;
    using global::System.Collections.Generic;
    using global::System.Collections.Immutable;
    using global::System.Globalization;
    using global::System.IO;
    using global::System.Linq;
    using global::System.Reflection;
    using global::System.Security.Cryptography;
    using global::System.Threading.Tasks;

    internal class CrashReportCreator
    {
        public static CrashReportModel Create(CrashReportInfo crashReport)
        {
            var modules = GetModuleList(crashReport);
            var assemblies = GetAssemblyList(crashReport);
            return new CrashReportModel
            {
                Id = crashReport.Id,
                GameVersion = ApplicationVersionHelper.GameVersionStr(),
                Version = crashReport.Version,
                Exception = GetRecursiveException(crashReport, modules, assemblies),
                EnhancedStacktrace = GetEnhancedStacktrace(crashReport),
                InvolvedModules = GetInvolvedModuleList(crashReport),
                Modules = modules,
                Assemblies = GetAssemblyList(crashReport),
                HarmonyPatches = GetHarmonyPatchesListHtml(crashReport),
                MonoModDetours = ImmutableArray<MonoModDetoursModel>.Empty,
                Metadata = new()
                {
                    LauncherType = GetLauncherType(crashReport),
                    LauncherVersion = GetLauncherVersion(crashReport),

                    Runtime = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription,

                    AdditionalMetadata = ImmutableArray.Create<MetadataModel>(
                        new MetadataModel { Key = "BUTRLoaderVersion", Value = GetBUTRLoaderVersion(crashReport) },
                        new MetadataModel { Key = "BLSEVersion", Value = GetBLSEVersion(crashReport) },
                        new MetadataModel { Key = "LauncherExVersion", Value = GetLauncherExVersion(crashReport) }),
                },
                AdditionalMetadata = ImmutableArray<MetadataModel>.Empty,
            };
        }

        private static string GetBUTRLoaderVersion(CrashReportInfo crashReport)
        {
            if (crashReport.AvailableAssemblies.FirstOrDefault(x => x.Key.Name == "Bannerlord.BUTRLoader") is { Key: { } assemblyName } )
                return assemblyName.Version?.ToString() ?? string.Empty;
            return string.Empty;
        }
        private static string GetBLSEVersion(CrashReportInfo crashReport)
        {
            var blseMetadata = crashReport.AvailableAssemblies.FirstOrDefault(x => x.Key.Name == "Bannerlord.BLSE").Value?.GetCustomAttributes<AssemblyMetadataAttribute>();
            return blseMetadata?.FirstOrDefault(x => x.Key == "BLSEVersion")?.Value ?? string.Empty;
        }
        private static string GetLauncherExVersion(CrashReportInfo crashReport)
        {
            var launcherExMetadata = crashReport.AvailableAssemblies.FirstOrDefault(x => x.Key.Name == "Bannerlord.LauncherEx").Value?.GetCustomAttributes<AssemblyMetadataAttribute>();
            return launcherExMetadata?.FirstOrDefault(x => x.Key == "LauncherExVersion")?.Value ?? string.Empty;
        }

        private static string GetLauncherType(CrashReportInfo crashReport)
        {
            if (crashReport.AdditionalMetadata.TryGetValue("Parent_Process_Name", out var parentProcessName))
            {
                return parentProcessName switch
                {
                    "Vortex" => "vortex",
                    "BannerLordLauncher" => "bannerlordlauncher",
                    "steam" => "steam",
                    "GalaxyClient" => "gog",
                    "EpicGamesLauncher" => "epicgames",
                    "devenv" => "debuggervisualstudio",
                    "JetBrains.Debugger.Worker64c" => "debuggerjetbrains",
                    "explorer" => "explorer",
                    "NovusLauncher" => "novus",
                    "ModOrganizer" => "modorganizer",
                    _ => $"unknown launcher - {parentProcessName}"
                };
            }

            if (!string.IsNullOrEmpty(GetBUTRLoaderVersion(crashReport)))
                return "butrloader";

            return "vanilla";
        }

        private static string GetLauncherVersion(CrashReportInfo crashReport)
        {
            if (crashReport.AdditionalMetadata.TryGetValue("Parent_Process_File_Version", out var parentProcessFileVersion))
                return parentProcessFileVersion;

            if (GetBUTRLoaderVersion(crashReport) is { } bVersion && !string.IsNullOrEmpty(bVersion))
                return bVersion;

            return "0";
        }

        private static ExceptionModel GetRecursiveException(CrashReportInfo crashReport, ImmutableArray<ModuleModel> modules, ImmutableArray<AssemblyModel> assemblies)
        {
            static ExceptionModel GetRecursiveException(CrashReportInfo crashReport, ImmutableArray<ModuleModel> modules, ImmutableArray<AssemblyModel> assemblies, Exception ex) => new()
            {
                SourceModuleId = modules.FirstOrDefault(x => assemblies.Where(y => y.ModuleId == x.Id).Any(x => x.Name == ex.Source))?.Id,
                Type = ex.GetType().FullName ?? string.Empty,
                Message = ex.Message,
                CallStack = ex.StackTrace,
                InnerException = ex.InnerException is not null ? GetRecursiveException(crashReport, modules, assemblies, ex.InnerException) : null,
                AdditionalMetadata = ImmutableArray<MetadataModel>.Empty,
            };

            return GetRecursiveException(crashReport, modules, assemblies, crashReport.Exception);
        }

        private static ImmutableArray<EnhancedStacktraceFrameModel> GetEnhancedStacktrace(CrashReportInfo crashReport)
        {
            var builder = ImmutableArray.CreateBuilder<EnhancedStacktraceFrameModel>();
            foreach (var stacktrace in crashReport.Stacktrace.GroupBy(x => x.StackFrameDescription))
            {
                foreach (var entry in stacktrace)
                {
                    var methodsBuilder = ImmutableArray.CreateBuilder<MethodSimple>();
                    foreach (var patchMethod in entry.PatchMethods)
                    {
                        methodsBuilder.Add(new()
                        {
                            ModuleId = patchMethod.ModuleInfo?.Id,
                            MethodDeclaredTypeName = patchMethod.Method.DeclaringType?.FullName,
                            MethodName = patchMethod.Method.Name,
                            MethodFullDescription = patchMethod.Method.FullDescription(),
                            MethodParameters = patchMethod.Method.GetParameters().Select(x => x.ParameterType.FullName).ToImmutableArray(),
                            CilInstructions = patchMethod.CilInstructions.AsImmutableArray(),
                            CsharpWithCilInstructions = patchMethod.CsharpWithCilInstructions.AsImmutableArray(),
                            CsharpInstructions = patchMethod.CsharpInstructions.AsImmutableArray(),
                            AdditionalMetadata = ImmutableArray<MetadataModel>.Empty,
                        });
                    }

                    builder.Add(new()
                    {
                        FrameDescription = entry.StackFrameDescription,
                        ExecutingMethod = new()
                        {
                            ModuleId = entry.ModuleInfo?.Id,
                            MethodDeclaredTypeName = entry.Method.DeclaringType?.FullName,
                            MethodName = entry.Method.Name,
                            MethodFullDescription = entry.Method.FullDescription(),
                            MethodParameters = entry.Method.GetParameters().Select(x => x.ParameterType.FullName).ToImmutableArray(),
                            NativeInstructions = entry.NativeInstructions.AsImmutableArray(),
                            CilInstructions = entry.CilInstructions.AsImmutableArray(),
                            CsharpWithCilInstructions = entry.CsharpWithCilInstructions.AsImmutableArray(),
                            CsharpInstructions = entry.CsharpInstructions.AsImmutableArray(),
                            AdditionalMetadata = ImmutableArray<MetadataModel>.Empty,
                        },
                        OriginalMethod = entry.OriginalMethod is not null ? new()
                        {
                            ModuleId = entry.OriginalMethod.ModuleInfo?.Id,
                            MethodDeclaredTypeName = entry.OriginalMethod.Method.DeclaringType?.FullName,
                            MethodName = entry.OriginalMethod.Method.Name,
                            MethodFullDescription = entry.OriginalMethod.Method.FullDescription(),
                            MethodParameters = entry.OriginalMethod.Method.GetParameters().Select(x => x.ParameterType.FullName).ToImmutableArray(),
                            CilInstructions = entry.OriginalMethod.CilInstructions.AsImmutableArray(),
                            CsharpWithCilInstructions = entry.OriginalMethod.CsharpWithCilInstructions.AsImmutableArray(),
                            CsharpInstructions = entry.OriginalMethod.CsharpInstructions.AsImmutableArray(),
                            AdditionalMetadata = ImmutableArray<MetadataModel>.Empty
                        } : null,
                        PatchMethods = methodsBuilder.ToImmutable(),
                        ILOffset = entry.ILOffset,
                        NativeOffset = entry.NativeOffset,
                        MethodFromStackframeIssue = entry.MethodFromStackframeIssue,
                        AdditionalMetadata = ImmutableArray<MetadataModel>.Empty,
                    });
                }
            }
            return builder.ToImmutable();
        }

        private static ImmutableArray<InvolvedModuleModel> GetInvolvedModuleList(CrashReportInfo crashReport)
        {
            var builder = ImmutableArray.CreateBuilder<InvolvedModuleModel>();
            foreach (var stacktrace in crashReport.FilteredStacktrace.GroupBy(m => m.ModuleInfo))
            {
                var module = stacktrace.Key;
                if (module is null) continue;

                builder.Add(new()
                {
                    ModuleId = module.Id,
                    EnhancedStacktraceFrameName = stacktrace.Last().StackFrameDescription,
                    AdditionalMetadata = ImmutableArray<MetadataModel>.Empty,
                });
            }
            return builder.ToImmutable();
        }

        private static ImmutableArray<ModuleModel> GetModuleList(CrashReportInfo crashReport)
        {
            var builder = ImmutableArray.CreateBuilder<ModuleModel>();

            foreach (var module in crashReport.LoadedModules.OfType<ModuleInfo>().Select(x => x.InternalModuleInfo))
            {
                var isManagedByVortex = File.Exists(Path.Combine(module.Path, "__folder_managed_by_vortex"));

                builder.Add(new()
                {
                    Id = module.Id,
                    Name = module.Name,
                    Version = module.Version.ToString(),
                    IsExternal = module.IsExternal,
                    IsOfficial = module.IsOfficial,
                    IsSingleplayer = module.IsSingleplayerModule,
                    IsMultiplayer = module.IsMultiplayerModule,
                    Url = !string.IsNullOrEmpty(module.Url) ? module.Url : null,
                    UpdateInfo = !string.IsNullOrEmpty(module.UpdateInfo) ? module.UpdateInfo : null,
                    DependencyMetadatas = module.DependenciesAllDistinct().Select(x => new ModuleDependencyMetadataModel
                    {
                        ModuleId = x.Id,
                        Type = x.IsIncompatible ? ModuleDependencyMetadataModelType.Incompatible : (ModuleDependencyMetadataModelType) x.LoadType,
                        IsOptional = x.IsOptional,
                        Version = !x.Version.Equals(ApplicationVersion.Empty) ? x.Version.ToString() : null,
                        VersionRange = !x.VersionRange.Equals(ApplicationVersionRange.Empty) ? x.VersionRange.ToString() : null,
                        AdditionalMetadata = ImmutableArray<MetadataModel>.Empty,
                    }).ToImmutableArray(),
                    SubModules = module.SubModules.Where(ModuleInfoHelper.CheckIfSubModuleCanBeLoaded).Select(x => new ModuleSubModuleModel
                    {
                        Name = x.Name,
                        AssemblyName = x.DLLName,
                        Entrypoint = x.SubModuleClassType,
                        AdditionalMetadata = x.Assemblies.Select(y => new MetadataModel { Key = "METADATA:Assembly", Value = y })
                            .Concat(x.Tags.SelectMany(y => y.Value.Select(z => new MetadataModel { Key = y.Key, Value = z })))
                            .ToImmutableArray(),
                    }).ToImmutableArray(),
                    AdditionalMetadata = ImmutableArray.Create<MetadataModel>(new MetadataModel { Key = "METADATA:MANAGED_BY_VORTEX", Value = isManagedByVortex.ToString()}),
                });
            }

            return builder.ToImmutable();
        }

        private static ImmutableArray<AssemblyModel> GetAssemblyList(CrashReportInfo crashReport)
        {
            static string CalculateMD5(string filename)
            {
                using var md5 = MD5.Create();
                using var stream = File.OpenRead(filename);
                var hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }

            var builder = ImmutableArray.CreateBuilder<AssemblyModel>();

            foreach (var (assemblyName, assembly) in crashReport.AvailableAssemblies)
            {
                ModuleInfoExtendedWithMetadata? module = null;
                foreach (var loadedModule in crashReport.LoadedModules.OfType<ModuleInfo>().Select(x => x.InternalModuleInfo))
                {
                    if (ModuleInfoHelper.IsModuleAssembly(loadedModule, assembly))
                    {
                        module = loadedModule;
                        break;
                    }
                }

                var systemAssemblyDirectory = Path.GetDirectoryName(typeof(object).Assembly.Location);
                var isGAC = assembly.GlobalAssemblyCache;
                var isSystem = !assembly.IsDynamic && !string.IsNullOrWhiteSpace(assembly.Location) && Path.GetDirectoryName(assembly.Location).Equals(systemAssemblyDirectory, StringComparison.Ordinal);
                var isTWCore = !assembly.IsDynamic && assembly.Location.IndexOf(@"Mount & Blade II Bannerlord\bin\", StringComparison.InvariantCultureIgnoreCase) >= 0;

                var type = AssemblyModelType.Unclassified;
                if (assembly.IsDynamic) type |= AssemblyModelType.Dynamic;
                if (isGAC) type |= AssemblyModelType.GAC;
                if (isSystem) type |= AssemblyModelType.System;
                if (isTWCore) type |= AssemblyModelType.GameCore;
                if (module is not null) type |= AssemblyModelType.Module;
                if (module is not null && module.IsOfficial) type |= AssemblyModelType.GameModule;
                builder.Add(new()
                {
                    ModuleId = module?.Id,
                    Name = assemblyName.Name,
                    Culture = assemblyName.CultureName,
                    PublicKeyToken = string.Join(string.Empty, Array.ConvertAll(assemblyName.GetPublicKeyToken(), x => x.ToString("x2", CultureInfo.InvariantCulture))),
                    Version = assemblyName.Version.ToString(),
                    Architecture = assemblyName.ProcessorArchitecture.ToString(),
                    Hash = assembly.IsDynamic || string.IsNullOrWhiteSpace(assembly.Location) || !File.Exists(assembly.Location) ? string.Empty : CalculateMD5(assembly.Location),
                    AnonymizedPath = assembly.IsDynamic ? "DYNAMIC" : string.IsNullOrWhiteSpace(assembly.Location) ? "EMPTY" : !File.Exists(assembly.Location) ? "MISSING" : Anonymizer.AnonymizePath(assembly.Location),
                    Type = type,
                    ImportedTypeReferences = crashReport.ImportedTypeReferences.TryGetValue(assemblyName, out var values) ? values.Select(x => new AssemblyImportedTypeReferenceModel()
                    {
                        Namespace = x.Namespace,
                        Name = x.Name,
                        FullName = x.FullName,
                    }).ToImmutableArray() : ImmutableArray<AssemblyImportedTypeReferenceModel>.Empty,
                    ImportedAssemblyReferences = assembly.GetReferencedAssemblies().Select(x => AssemblyImportedReferenceModelExtensions.Create(x)).ToImmutableArray(),
                    AdditionalMetadata = ImmutableArray<MetadataModel>.Empty,
                });
            }

            return builder.ToImmutable();
        }

        private static ImmutableArray<HarmonyPatchesModel> GetHarmonyPatchesListHtml(CrashReportInfo crashReport)
        {
            var builder = ImmutableArray.CreateBuilder<HarmonyPatchesModel>();

            static void AppendPatches(ImmutableArray<HarmonyPatchModel>.Builder builder, HarmonyPatchModelType type, IEnumerable<Patch> patches)
            {
                foreach (var patch in patches)
                {
                    var moduleAssembly = patch.PatchMethod.DeclaringType?.Assembly;
                    builder.Add(new()
                    {
                        Type = type,
                        AssemblyName = patch.PatchMethod.DeclaringType?.Assembly.GetName().Name,
                        Owner = patch.owner,
                        Namespace = $"{patch.PatchMethod.DeclaringType!.FullName}.{patch.PatchMethod.Name}",
                        Index = patch.index,
                        Priority = patch.priority,
                        Before = patch.before.ToImmutableArray(),
                        After = patch.after.ToImmutableArray(),
                        AdditionalMetadata = ImmutableArray<MetadataModel>.Empty,
                    });
                }
            }

            foreach (var (originalMethod, patches) in crashReport.LoadedHarmonyPatches)
            {
                var patchBuilder = ImmutableArray.CreateBuilder<HarmonyPatchModel>();

                AppendPatches(patchBuilder, HarmonyPatchModelType.Prefix, patches.Prefixes);
                AppendPatches(patchBuilder, HarmonyPatchModelType.Postfix, patches.Postfixes);
                AppendPatches(patchBuilder, HarmonyPatchModelType.Finalizer, patches.Finalizers);
                AppendPatches(patchBuilder, HarmonyPatchModelType.Transpiler, patches.Transpilers);

                if (patchBuilder.Count > 0)
                {
                    builder.Add(new()
                    {
                        OriginalMethodDeclaredTypeName = originalMethod.DeclaringType?.FullName,
                        OriginalMethodName = originalMethod.Name,
                        Patches = patchBuilder.ToImmutable(),
                        AdditionalMetadata = ImmutableArray<MetadataModel>.Empty,
                    });
                }
            }

            return builder.ToImmutable();
        }
    }
}

#pragma warning restore
#nullable restore
#endif // BUTRCRASHREPORT_DISABLE