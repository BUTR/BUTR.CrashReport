namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents the module or loader plugin update information.
/// </summary>
public sealed record UpdateInfoModuleOrLoaderPlugin
{
    /// <summary>
    /// The provider of the update. Can be 'NexusMods' or 'GitHub', for example.
    /// </summary>
    public required string Provider { get; set; }

    /// <summary>
    /// The value of the update. Can be the mod ID for NexusMods or the 'user/repo' for GitHub, for example.
    /// </summary>
    public required string Value { get; set; }

    /// <inheritdoc/>
    public override string ToString() => $"{Provider}:{Value}";

    /// <inheritdoc />
    public bool Equals(UpdateInfoModuleOrLoaderPlugin? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Provider == other.Provider && Value == other.Value;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            return (Provider.GetHashCode() * 397) ^ Value.GetHashCode();
        }
    }
}