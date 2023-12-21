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
    /// The game version.
    /// </summary>
    public required string GameVersion { get; set; }

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
    public required IReadOnlyList<ModuleModel> Modules { get; set; } = new List<ModuleModel>();

    /// <summary>
    /// The list of involved modules in the crash.
    /// </summary>
    public required IReadOnlyList<InvolvedModuleModel> InvolvedModules { get; set; } = new List<InvolvedModuleModel>();

    /// <summary>
    /// The enhanced stack trace frames.
    /// </summary>
    public required IReadOnlyList<EnhancedStacktraceFrameModel> EnhancedStacktrace { get; set; } = new List<EnhancedStacktraceFrameModel>();

    /// <summary>
    /// The list of assemblies that are present.
    /// </summary>
    public required IReadOnlyList<AssemblyModel> Assemblies { get; set; } = new List<AssemblyModel>();

    /// <summary>
    /// The list of Harmony patches that are present.
    /// </summary>
    public required IReadOnlyList<HarmonyPatchesModel> HarmonyPatches { get; set; } = new List<HarmonyPatchesModel>();

    /// <summary>
    /// The list of MonoMod detours that are present.
    /// </summary>
    public required IReadOnlyList<MonoModDetoursModel> MonoModDetours { get; set; } = new List<MonoModDetoursModel>();

    /// <summary>
    /// Additional metadata associated with the model.
    /// </summary>
    /// <returns>A key:value list of metadatas.</returns>
    public required IReadOnlyList<MetadataModel> AdditionalMetadata { get; set; } = new List<MetadataModel>();
}