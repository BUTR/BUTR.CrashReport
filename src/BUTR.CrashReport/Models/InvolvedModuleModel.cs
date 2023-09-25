using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

public record InvolvedModuleModel
{
    public required string Id { get; set; }
    public required string Stacktrace { get; set; }
    public required IReadOnlyList<KeyValuePair<string, string>> AdditionalMetadata { get; set; }
}