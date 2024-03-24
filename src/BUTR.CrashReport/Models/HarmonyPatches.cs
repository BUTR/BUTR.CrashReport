using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

/// <summary>
/// <inheritdoc cref="HarmonyLib.Patches"/>
/// </summary>
public record HarmonyPatches
{
    /// <summary>
    /// <inheritdoc cref="HarmonyLib.Patches.Prefixes"/>
    /// </summary>
    public required IList<HarmonyPatch> Prefixes { get; set; }
    
    /// <summary>
    /// <inheritdoc cref="HarmonyLib.Patches.Postfixes"/>
    /// </summary>
    public required IList<HarmonyPatch> Postfixes { get; set; }
    
    /// <summary>
    /// <inheritdoc cref="HarmonyLib.Patches.Finalizers"/>
    /// </summary>
    public required IList<HarmonyPatch> Finalizers { get; set; }
    
    /// <summary>
    /// <inheritdoc cref="HarmonyLib.Patches.Transpilers"/>
    /// </summary>
    public required IList<HarmonyPatch> Transpilers { get; set; }
}