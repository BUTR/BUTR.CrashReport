﻿namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents a harmony patch.
/// </summary>
public record MethodEntryHarmony : MethodEntry
{
    /// <summary>
    /// The harmony patch.
    /// </summary>
    public required HarmonyPatch Patch { get; set; }
}