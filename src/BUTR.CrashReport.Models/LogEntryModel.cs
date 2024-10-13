using System;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents a single log entry.
/// </summary>
public sealed record LogEntryModel
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

    /// <inheritdoc />
    public bool Equals(LogEntryModel? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Date.Equals(other.Date) &&
               Type == other.Type &&
               Level == other.Level &&
               Message == other.Message;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Date.GetHashCode();
            hashCode = (hashCode * 397) ^ Type.GetHashCode();
            hashCode = (hashCode * 397) ^ (int) Level;
            hashCode = (hashCode * 397) ^ Message.GetHashCode();
            return hashCode;
        }
    }
}