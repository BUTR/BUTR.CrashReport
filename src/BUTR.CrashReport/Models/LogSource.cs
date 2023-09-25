using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

public record LogSource
{
    public required string Name { get; set; }
    public required IReadOnlyList<LogEntry> Logs { get; set; }

}