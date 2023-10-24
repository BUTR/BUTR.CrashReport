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
}