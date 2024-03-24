using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents a log source. Could be a file or anything else.
/// </summary>
public record LogSource
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
}