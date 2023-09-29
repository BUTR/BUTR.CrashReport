using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

public sealed record ModuleModel
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Version { get; set; }
    public required bool IsExternal { get; set; }
    public required bool IsOfficial { get; set; }
    public required bool IsSingleplayer { get; set; }
    public required bool IsMultiplayer { get; set; }
    public required string? Url { get; set; }
    public required IReadOnlyList<ModuleDependencyMetadataModel> DependencyMetadatas { get; set; } = new List<ModuleDependencyMetadataModel>();
    public required IReadOnlyList<ModuleSubModuleModel> SubModules { get; set; } = new List<ModuleSubModuleModel>();
    public required IReadOnlyList<MetadataModel> AdditionalMetadata { get; set; } = new List<MetadataModel>();
}