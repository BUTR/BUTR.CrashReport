namespace BUTR.CrashReport.ContextualAnalysis;

/// <summary>
/// Represents a pattern used to match a module or plugin ID.
/// </summary>
public sealed record CrashModuleIdOrPluginPattern
{
    /// <summary>
    /// The id of the module.
    /// </summary>
    public required string Id { get; set; }

    /// <inheritdoc />
    public bool Equals(CrashModuleIdOrPluginPattern? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = (Id != null ? Id.GetHashCode() : 0);
            return hashCode;
        }
    }
}