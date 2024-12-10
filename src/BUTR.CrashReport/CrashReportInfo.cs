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
        IPathAnonymizer pathAnonymizer,
        IHttpUtilities httpUtilities)
    {
        var assemblies = CrashReportModelUtils.GetAssemblies(crashReport, assemblyUtilities, pathAnonymizer);
        var modules = modelConverter.ToModuleModels(crashReport, crashReport.LoadedModules);
        var plugins = modelConverter.ToLoaderPluginModels(crashReport, crashReport.LoadedLoaderPlugins);
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
            AdditionalMetadata = [],
        }).ToArray();
        var enhancedStacktrace = CrashReportModelUtils.GetEnhancedStacktrace(crashReport, assemblies, assemblyUtilities, httpUtilities);
        return new CrashReportModel
        {
            Id = crashReport.Id,
            Version = crashReport.Version,
            Exception = CrashReportModelUtils.GetRecursiveException(crashReport, assemblies),
            EnhancedStacktrace = enhancedStacktrace,
            InvolvedModules = CrashReportModelUtils.GetInvolvedModules(enhancedStacktrace),
            Modules = modules,
            Assemblies = assemblies,
            NativeModules = nativeAssemblies,
            RuntimePatches = CrashReportModelUtils.GetRuntimePatches(crashReport, moduleProvider, loaderPluginProvider),
            LoaderPlugins = plugins,
            InvolvedLoaderPlugins = CrashReportModelUtils.GetInvolvedPlugins(enhancedStacktrace),
            Metadata = metadata,
            AdditionalMetadata = [],
        };
    }

    /// <summary>
    /// Creates the CrashReportInfo based on initial crash report data.
    /// </summary>
    public static CrashReportInfo Create(Exception exception,
        IStacktraceFilter stacktraceFilter,
        IAssemblyUtilities assemblyUtilities,
        IModuleProvider moduleProvider,
        ILoaderPluginProvider loaderPluginProvider,
        IRuntimePatchProvider runtimePatchProvider) =>
        new(exception, stacktraceFilter, assemblyUtilities, moduleProvider, loaderPluginProvider, runtimePatchProvider);

    /// <summary>
    /// <inheritdoc cref="BUTR.CrashReport.Models.CrashReportModel.Version"/>
    /// </summary>
    /// <returns><inheritdoc cref="BUTR.CrashReport.Models.CrashReportModel.Version"/></returns>
    public readonly byte Version = 15;

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
    /// <inheritdoc cref="ManagedRuntimePatch"/>
    /// </summary>
    /// <returns><inheritdoc cref="ManagedRuntimePatch"/></returns>
    public Dictionary<MethodBase, IList<ManagedRuntimePatch>> LoadedManagedRuntimePatches { get; } = new();

    /// <summary>
    /// <inheritdoc cref="ManagedRuntimePatch"/>
    /// </summary>
    /// <returns><inheritdoc cref="ManagedRuntimePatch"/></returns>
    public Dictionary<IntPtr, IList<NativeRuntimePatch>> LoadedNativeRuntimePatches { get; } = new();

    /// <summary>
    /// Creates the CrashReportInfo based on initial crash report data.
    /// </summary>
    private CrashReportInfo(Exception exception,
        IStacktraceFilter stacktraceFilter,
        IAssemblyUtilities assemblyUtilities,
        IModuleProvider moduleProvider,
        ILoaderPluginProvider loaderPluginProvider,
        IRuntimePatchProvider runtimePatchProvider)
    {
        var assemblies = assemblyUtilities.Assemblies().ToArray();

        Exception = exception.Demystify();
        LoadedModules = moduleProvider.GetLoadedModules();
        LoadedLoaderPlugins = loaderPluginProvider.GetLoadedLoaderPlugins();

        AvailableAssemblies = assemblies.ToDictionary(x => x.GetName(), x => x);
        ImportedTypeReferences = AvailableAssemblies.ToDictionary(x => x.Key, x => GetImportedTypeReferences(x.Value, assemblyUtilities.GetAssemblyStream).Select(y => new AssemblyTypeReference
        {
            Name = y.Name,
            Namespace = y.Namespace,
            FullName = y.FullName,
        }).ToArray());

        Stacktrace = CrashReportUtils.GetEnhancedStacktrace(Exception, assemblies, assemblyUtilities, moduleProvider, loaderPluginProvider, runtimePatchProvider).ToArray();
        FilteredStacktrace = stacktraceFilter.Filter(Stacktrace).ToArray();

        var managedPatches = runtimePatchProvider.GetAllManagedPatches();
        for (var i = 0; i < managedPatches.Count; i++)
        {
            var runtimePatch = managedPatches[i];
            if (!LoadedManagedRuntimePatches.ContainsKey(runtimePatch.Original))
                LoadedManagedRuntimePatches[runtimePatch.Original] = new List<ManagedRuntimePatch>();
            LoadedManagedRuntimePatches[runtimePatch.Original].Add(runtimePatch);
        }

        var nativePatches = runtimePatchProvider.GetAllNativePatches();
        for (var i = 0; i < nativePatches.Count; i++)
        {
            var runtimePatch = nativePatches[i];
            if (!LoadedNativeRuntimePatches.ContainsKey(runtimePatch.Original))
                LoadedNativeRuntimePatches[runtimePatch.Original] = new List<NativeRuntimePatch>();
            LoadedNativeRuntimePatches[runtimePatch.Original].Add(runtimePatch);
        }
    }
}