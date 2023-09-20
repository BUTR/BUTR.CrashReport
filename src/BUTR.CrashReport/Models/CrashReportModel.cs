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
    public required ICollection<ModuleModel> Modules { get; set; }
    public required ICollection<InvolvedModuleModel> InvolvedModules { get; set; }
    public required ICollection<EnhancedStacktraceFrameModel> EnhancedStacktrace { get; set; }
    public required ICollection<AssemblyModel> Assemblies { get; set; }
    public required ICollection<HarmonyPatchesModel> HarmonyPatches { get; set; }
    public required ICollection<KeyValuePair<string, string>> AdditionalMetadata { get; set; }
}