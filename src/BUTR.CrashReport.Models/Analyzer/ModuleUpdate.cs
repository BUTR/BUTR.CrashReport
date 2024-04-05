namespace BUTR.CrashReport.Models.Analyzer;

/// <summary>
/// Represents an available module update.
/// </summary>
public sealed record ModuleUpdate
{
    /// <summary>
    /// <inheritdoc cref="ModuleModel.Id"/>
    /// </summary>
    /// <returns><inheritdoc cref="ModuleModel.Id"/></returns>
    public required string ModuleId { get; set; }

    /// <summary>
    /// The <see cref="ModuleModel.Version"/>.
    /// </summary>
    public required string ModuleVersion { get; set; }

    /// <summary>
    /// Whether the module was involced in the crash.
    /// </summary>
    public required bool IsModuleInvolved { get; set; }

    /// <inheritdoc />
    public bool Equals(ModuleUpdate? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return ModuleId == other.ModuleId && ModuleVersion == other.ModuleVersion && IsModuleInvolved == other.IsModuleInvolved;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = ModuleId.GetHashCode();
            hashCode = (hashCode * 397) ^ ModuleVersion.GetHashCode();
            hashCode = (hashCode * 397) ^ IsModuleInvolved.GetHashCode();
            return hashCode;
        }
    }
}