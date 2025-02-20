namespace BUTR.CrashReport.AutomatedRemediation;

/// <summary>
/// Represents a generic crash report fix.
/// </summary>
public sealed record CrashGenericFix
{
    /// <summary>
    /// The type of suggested fix.
    /// </summary>
    public required CrashGenericFixType Type { get; set; }

    /// <inheritdoc />
    public bool Equals(CrashGenericFix? other)
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