namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents a MonoMod patch.
/// </summary>
public record MethodEntryRuntimePatch : MethodEntry
{
    /// <summary>
    /// The harmony patch.
    /// </summary>
    public required RuntimePatch Patch { get; set; }
}