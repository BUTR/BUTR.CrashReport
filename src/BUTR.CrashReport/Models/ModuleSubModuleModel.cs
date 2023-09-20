using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

public record ModuleSubModuleModel
{
    public required string Name { get; set; }
    public required string AssemblyName { get; set; }
    public required string Entrypoint { get; set; }
    public required ICollection<KeyValuePair<string, string>> AdditionalMetadata { get; set; }
}