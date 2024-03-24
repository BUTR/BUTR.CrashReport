using System;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents a single log entry.
/// </summary>
public record LogEntry
{
    /// <summary>
    /// The date and time this log entry was created.
    /// </summary>
    public required DateTimeOffset Date { get; set; }

    /// <summary>
    /// The full name of the type of the log entry.
    /// </summary>
    public required string Type { get; set; }

    /// <summary>
    /// The level of the log entry.
    /// </summary>
    public required LogLevel Level { get; set; }

    /// <summary>
    /// The message of the log entry.
    /// </summary>
    public required string Message { get; set; }
}