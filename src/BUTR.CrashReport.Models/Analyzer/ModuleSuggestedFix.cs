namespace BUTR.CrashReport.Models.Analyzer;

/// <summary>
/// Represents a module specific crash report fix.
/// </summary>
public sealed record ModuleSuggestedFix
{
    /// <summary>
    /// <inheritdoc cref="ModuleModel.Id"/>
    /// </summary>
    /// <returns><inheritdoc cref="ModuleModel.Id"/></returns>
    public required string ModuleId { get; set; }

    /// <summary>
    /// The type of suggested fix.
    /// </summary>
    public required ModuleSuggestedFixType Type { get; set; }

    /// <inheritdoc />
    public bool Equals(ModuleSuggestedFix? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return ModuleId == other.ModuleId && Type == other.Type;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            return (ModuleId.GetHashCode() * 397) ^ (int) Type;
        }
    }
}