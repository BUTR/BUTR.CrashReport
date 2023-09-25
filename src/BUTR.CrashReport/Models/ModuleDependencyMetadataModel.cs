using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

public record ModuleDependencyMetadataModel
{
    public required string ModuleId { get; set; }
    public required ModuleDependencyMetadataModelType Type { get; set; }
    public required bool IsOptional { get; set; }
    public required bool IsIncompatible { get; set; }
    public required string Version { get; set; }
    public required string VersionRange { get; set; }
    public required IReadOnlyList<KeyValuePair<string, string>> AdditionalMetadata { get; set; }
}