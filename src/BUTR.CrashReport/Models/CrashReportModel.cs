using System;
using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

public record CrashReportModel
{
    public required Guid Id { get; set; }
    public required byte Version { get; set; }
    public required string GameVersion { get; set; }
    public required ExceptionModel Exception { get; set; }
    public required CrashReportMetadataModel Metadata { get; set; }
    public required IReadOnlyList<ModuleModel> Modules { get; set; } = new List<ModuleModel>();
    public required IReadOnlyList<InvolvedModuleModel> InvolvedModules { get; set; } = new List<InvolvedModuleModel>();
    public required IReadOnlyList<EnhancedStacktraceFrameModel> EnhancedStacktrace { get; set; } = new List<EnhancedStacktraceFrameModel>();
    public required IReadOnlyList<AssemblyModel> Assemblies { get; set; } = new List<AssemblyModel>();
    public required IReadOnlyList<HarmonyPatchesModel> HarmonyPatches { get; set; } = new List<HarmonyPatchesModel>();
    public required IReadOnlyList<MetadataModel> AdditionalMetadata { get; set; } = new List<MetadataModel>();
}