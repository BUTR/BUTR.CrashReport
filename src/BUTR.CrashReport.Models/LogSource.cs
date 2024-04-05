using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents a log source. Could be a file or anything else.
/// </summary>
public sealed record LogSource
{
    /// <summary>
    /// The name of the log source.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// The log entries associated with the log source.
    /// </summary>
    public required IList<LogEntry> Logs { get; set; } = new List<LogEntry>();

    /// <summary>
    /// <inheritdoc cref="CrashReportModel.AdditionalMetadata"/>
    /// </summary>
    /// <returns><inheritdoc cref="CrashReportModel.AdditionalMetadata"/></returns>
    public required IList<MetadataModel> AdditionalMetadata { get; set; } = new List<MetadataModel>();

    /// <inheritdoc />
    public bool Equals(LogSource? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name && Logs.Equals(other.Logs) && AdditionalMetadata.Equals(other.AdditionalMetadata);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Name.GetHashCode();
            hashCode = (hashCode * 397) ^ Logs.GetHashCode();
            hashCode = (hashCode * 397) ^ AdditionalMetadata.GetHashCode();
            return hashCode;
        }
    }
}