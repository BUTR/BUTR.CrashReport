using System;

namespace BUTR.CrashReport.Models;

public record LogEntry
{
    public required DateTime Date { get; set; }
    public required string Message { get; set; }
}