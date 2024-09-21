namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents a generic metadata extension for any model.
/// </summary>
public sealed record MetadataModel
{
    /// <summary>
    /// The key.
    /// </summary>
    public required string Key { get; set; }

    /// <summary>
    /// The value.
    /// </summary>
    public required string Value { get; set; }

    /// <inheritdoc />
    public bool Equals(MetadataModel? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Key == other.Key &&
               Value == other.Value;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            return (Key.GetHashCode() * 397) ^ Value.GetHashCode();
        }
    }
}