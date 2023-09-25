using System;

namespace BUTR.CrashReport.Models;

public record LogEntry
{
    public required DateTimeOffset Date { get; set; }
    public required string Type { get; set; }
    public required string Level { get; set; }
    public required string Message { get; set; }
}