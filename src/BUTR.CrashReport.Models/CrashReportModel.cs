using System;
using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents the main model of a crash report.
/// </summary>
public record CrashReportModel
{
    /// <summary>
    /// The id of the crash report.
    /// </summary>
    public required Guid Id { get; set; }

    /// <summary>
    /// The version of the crash report.
    /// </summary>
    public required byte Version { get; set; }

    /// <summary>
    /// The exception that caused the crash.
    /// </summary>
    public required ExceptionModel Exception { get; set; }

    /// <summary>
    /// The metadata of the crash report.
    /// </summary>
    public required CrashReportMetadataModel Metadata { get; set; }

    /// <summary>
    /// The list of modules that are loaded in the process.
    /// </summary>
    public required IList<ModuleModel> Modules { get; set; } = new List<ModuleModel>();

    /// <summary>
    /// The list of involved modules in the crash.
    /// </summary>
    public required IList<InvolvedModuleOrPluginModel> InvolvedModules { get; set; } = new List<InvolvedModuleOrPluginModel>();

    /// <summary>
    /// The enhanced stack trace frames.
    /// </summary>
    public required IList<EnhancedStacktraceFrameModel> EnhancedStacktrace { get; set; } = new List<EnhancedStacktraceFrameModel>();

    /// <summary>
    /// The list of assemblies that are present.
    /// </summary>
    public required IList<AssemblyModel> Assemblies { get; set; } = new List<AssemblyModel>();


    /*
    /// <summary>
    /// The list of MonoMod detours that are present.
    /// MonoMod does not keep a list of detours. If you have a library that does, here it could be exposed.
    /// </summary>
    public required IList<MonoModDetoursModel> MonoModDetours { get; set; } = new List<MonoModDetoursModel>();
    */

    /// <summary>
    /// The list of Harmony patches that are present.
    /// </summary>
    public required IList<HarmonyPatchesModel> HarmonyPatches { get; set; } = new List<HarmonyPatchesModel>();

    /// <summary>
    /// The list of loader plugins that are present.
    /// </summary>
    public required IList<LoaderPluginModel> LoaderPlugins { get; set; } = new List<LoaderPluginModel>();

    /// <summary>
    /// The list of involved loader plugins in the crash.
    /// </summary>
    public required IList<InvolvedModuleOrPluginModel> InvolvedLoaderPlugins { get; set; } = new List<InvolvedModuleOrPluginModel>();

    /// <summary>
    /// Additional metadata associated with the model.
    /// </summary>
    /// <returns>A key:value list of metadatas.</returns>
    public required IList<MetadataModel> AdditionalMetadata { get; set; } = new List<MetadataModel>();
}