using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

public record AssemblyModel
{
    public required string Name { get; set; }
    public required string FullName { get; set; }
    public required string Version { get; set; }
    public required string Architecture { get; set; }
    public required string Hash { get; set; }
    public required string Path { get; set; }
    public required AssemblyModelType Type { get; set; }
    public required string? ModuleId { get; set; }
    public required string? SubModuleId { get; set; }
    public required IReadOnlyList<KeyValuePair<string, string>> AdditionalMetadata { get; set; }
}