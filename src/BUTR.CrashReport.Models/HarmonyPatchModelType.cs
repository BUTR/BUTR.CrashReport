namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents the type of a Harmony patch.
/// </summary>
public enum HarmonyPatchModelType
{
    /// <summary>
    /// <inheritdoc cref="HarmonyLib.HarmonyPatchType.Prefix"/>
    /// </summary>
    Prefix,

    /// <summary>
    /// <inheritdoc cref="HarmonyLib.HarmonyPatchType.Postfix"/>
    /// </summary>
    Postfix,

    /// <summary>
    /// <inheritdoc cref="HarmonyLib.HarmonyPatchType.Finalizer"/>
    /// </summary>
    Finalizer,

    /// <summary>
    /// <inheritdoc cref="HarmonyLib.HarmonyPatchType.Transpiler"/>
    /// </summary>
    Transpiler,
}