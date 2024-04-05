namespace BUTR.CrashReport.Models.Analyzer;

/// <summary>
/// Represents a generic crash report fix.
/// </summary>
public sealed record GenericSuggestedFix
{
    /// <summary>
    /// The type of suggested fix.
    /// </summary>
    public required GenericSuggestedFixType Type { get; set; }

    /// <inheritdoc />
    public bool Equals(GenericSuggestedFix? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Type == other.Type;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return (int) Type;
    }
}