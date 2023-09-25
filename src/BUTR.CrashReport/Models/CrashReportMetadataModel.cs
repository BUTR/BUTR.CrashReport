using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

public record CrashReportMetadataModel
{
    public required string? LauncherType { get; set; }
    public required string? LauncherVersion { get; set; }

    public required string? Runtime { get; set; }

    public required IReadOnlyList<KeyValuePair<string, string>> AdditionalMetadata { get; set; }
}