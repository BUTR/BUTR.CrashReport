using BUTR.CrashReport.Interfaces;
using BUTR.CrashReport.Models;
using BUTR.CrashReport.Utils;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using static BUTR.CrashReport.Decompilers.Utils.ReferenceImporter;

namespace BUTR.CrashReport;

/// <summary>
/// The initial crash report info to be converted into the POCO
/// </summary>
public class CrashReportInfo
{
    /// <summary>
    /// Converts the CrashReportInfo into a CrashReportModel
    /// </summary>
    public static CrashReportModel ToModel(CrashReportInfo crashReport,
        ICrashReportMetadataProvider crashReportMetadataProvider,
        IModelConverter modelConverter,
        IModuleProvider moduleProvider,
        ILoaderPluginProvider loaderPluginProvider,
        IAssemblyUtilities assemblyUtilities,
        IPathAnonymizer pathAnonymizer)
    {
        var assemblies = CrashReportModelUtils.GetAssemblies(crashReport, assemblyUtilities, pathAnonymizer);
        var modules = modelConverter.ToModuleModels(crashReport.LoadedModules, assemblies);
        var plugins = modelConverter.ToLoaderPluginModels(crashReport.LoadedLoaderPlugins, assemblies);
        var metadata = crashReportMetadataProvider.GetCrashReportMetadataModel(crashReport);
        metadata.Runtime ??= System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
        var process = Process.GetCurrentProcess();
        var nativeModules = NativeModuleUtils.CollectModules(process, pathAnonymizer);
        var nativeAssemblies = nativeModules.Select(x => new NativeAssemblyModel
        {
            Id = new AssemblyIdModel
            {
                Name = x.ModuleName,
                Version = x.Version,
                PublicKeyToken = null,
            },
            Architecture = x.Architecture,
            Hash = x.Hash,
            AnonymizedPath = x.AnonymizedPath,
            AdditionalMetadata = Array.Empty<MetadataModel>(),
        }).ToArray();
        return new CrashReportModel
        {
            Id = crashReport.Id,
            Version = crashReport.Version,
            Exception = CrashReportModelUtils.GetRecursiveException(crashReport, assemblies),
            EnhancedStacktrace = CrashReportModelUtils.GetEnhancedStacktrace(crashReport, assemblies),
            InvolvedModules = CrashReportModelUtils.GetInvolvedModules(crashReport),
            Modules = modules,
            Assemblies = assemblies,
            NativeModules = nativeAssemblies,
            HarmonyPatches = CrashReportModelUtils.GetHarmonyPatches(crashReport, assemblies, moduleProvider, loaderPluginProvider),
            //MonoModDetours = Array.Empty<MonoModDetoursModel>(),
            LoaderPlugins = plugins,
            InvolvedLoaderPlugins = CrashReportModelUtils.GetInvolvedPlugins(crashReport),
            Metadata = metadata,
            AdditionalMetadata = Array.Empty<MetadataModel>(),
        };
    }

    /// <summary>
    /// Creates the CrashReportInfo based on initial crash report data.
    /// </summary>
    public static CrashReportInfo Create(Exception exception, Dictionary<string, string> additionalMetadata,
        IStacktraceFilter stacktraceFilter,
        IAssemblyUtilities assemblyUtilities,
        IModuleProvider moduleProvider,
        ILoaderPluginProvider loaderPluginProvider,
        IHarmonyProvider harmonyProvider) =>
        new(exception, additionalMetadata, stacktraceFilter, assemblyUtilities, moduleProvider, loaderPluginProvider, harmonyProvider);

    /// <summary>
    /// <inheritdoc cref="BUTR.CrashReport.Models.CrashReportModel.Version"/>
    /// </summary>
    /// <returns><inheritdoc cref="BUTR.CrashReport.Models.CrashReportModel.Version"/></returns>
    public readonly byte Version = 14;

    /// <summary>
    /// <inheritdoc cref="BUTR.CrashReport.Models.CrashReportModel.Id"/>
    /// </summary>
    /// <returns><inheritdoc cref="BUTR.CrashReport.Models.CrashReportModel.Id"/></returns>
    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>
    /// <inheritdoc cref="BUTR.CrashReport.Models.CrashReportModel.Exception"/>
    /// </summary>
    /// <returns><inheritdoc cref="BUTR.CrashReport.Models.CrashReportModel.Exception"/></returns>
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
    /// <inheritdoc cref="BUTR.CrashReport.Models.CrashReportModel.Modules"/>
    /// </summary>
    /// <returns><inheritdoc cref="BUTR.CrashReport.Models.CrashReportModel.Modules"/></returns>
    public ICollection<IModuleInfo> LoadedModules { get; }

    /// <summary>
    /// <inheritdoc cref="BUTR.CrashReport.Models.CrashReportModel.LoaderPlugins"/>
    /// </summary>
    /// <returns><inheritdoc cref="BUTR.CrashReport.Models.CrashReportModel.LoaderPlugins"/></returns>
    public ICollection<ILoaderPluginInfo> LoadedLoaderPlugins { get; }

    /// <summary>
    /// Lookup dictionary for available assemblies.
    /// </summary>
    public Dictionary<AssemblyName, Assembly> AvailableAssemblies { get; }

    /// <summary>
    /// Imported type references for assemblies.
    /// </summary>
    public Dictionary<AssemblyName, AssemblyTypeReference[]> ImportedTypeReferences { get; }

    /// <summary>
    /// <inheritdoc cref="BUTR.CrashReport.Models.CrashReportModel.HarmonyPatches"/>
    /// </summary>
    /// <returns><inheritdoc cref="BUTR.CrashReport.Models.CrashReportModel.HarmonyPatches"/></returns>
    public Dictionary<MethodBase, HarmonyPatches> LoadedHarmonyPatches { get; } = new();

    /// <summary>
    /// Additional metadata about the crash.
    /// </summary>
    public Dictionary<string, string> AdditionalMetadata { get; }

    /// <summary>
    /// Creates the CrashReportInfo based on initial crash report data.
    /// </summary>
    private CrashReportInfo(Exception exception, Dictionary<string, string> additionalMetadata,
        IStacktraceFilter stacktraceFilter,
        IAssemblyUtilities assemblyUtilities,
        IModuleProvider moduleProvider,
        ILoaderPluginProvider loaderPluginProvider,
        IHarmonyProvider harmonyProvider)
    {
        var assemblies = assemblyUtilities.Assemblies().ToArray();

        Exception = exception.Demystify();
        AdditionalMetadata = additionalMetadata;
        LoadedModules = moduleProvider.GetLoadedModules();
        LoadedLoaderPlugins = loaderPluginProvider.GetLoadedLoaderPlugins();

        AvailableAssemblies = assemblies.ToDictionary(x => x.GetName(), x => x);
        ImportedTypeReferences = GetImportedTypeReferences(AvailableAssemblies).ToDictionary(x => x.Key, x => x.Value.Select(y => new AssemblyTypeReference
        {
            Name = y.Name,
            Namespace = y.Namespace,
            FullName = y.FullName
        }).ToArray());

        Stacktrace = CrashReportUtils.GetAllInvolvedModules(Exception, assemblies, moduleProvider, loaderPluginProvider, harmonyProvider).ToArray();
        FilteredStacktrace = stacktraceFilter.Filter(Stacktrace).ToArray();

        foreach (var originalMethod in harmonyProvider.GetAllPatchedMethods())
        {
            var patches = harmonyProvider.GetPatchInfo(originalMethod);
            if (originalMethod is null || patches is null) continue;
            LoadedHarmonyPatches.Add(originalMethod, patches);
        }
    }
}