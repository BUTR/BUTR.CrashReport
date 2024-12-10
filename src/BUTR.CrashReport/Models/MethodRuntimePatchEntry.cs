namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents a method entry with runtime patch information.
/// </summary>
public sealed record MethodRuntimePatchEntry : MethodEntry
{
    /// <summary>
    /// <inheritdoc cref="ManagedRuntimePatch"/>
    /// </summary>
    public required ManagedRuntimePatch Patch { get; set; }
}