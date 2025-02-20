namespace BUTR.CrashReport.Models.Diagnostics;

/// <summary>
/// Represents a pattern used to match a module or plugin ID.
/// </summary>
public sealed record CrashModuleIdOrPluginPattern
{
    /// <summary>
    /// The id of the module.
    /// </summary>
    public required string Id { get; set; }
}