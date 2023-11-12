namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents a generic metadata extension for any model.
/// </summary>
public record MetadataModel
{
    /// <summary>
    /// The key.
    /// </summary>
    public required string Key { get; set; }

    /// <summary>
    /// The value.
    /// </summary>
    public required string Value { get; set; }
}