using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents the list of Harmony patches for a given original method.
/// </summary>
public sealed record HarmonyPatchesModel
{
    /// <summary>
    /// The original <see cref="EnhanExecutingMethodEntryodDeclaredTypeName"/>.
    /// </summary>
    public required string? OriginalMethodDeclaredTypeName { get; set; }

    /// <summary>
    /// The original  <see cref="EnhanExecutingMethodEntryodName"/>.
    /// </summary>
    public required string? OriginalMethodName { get; set; }

    /// <summary>
    /// The list of Harmony patches.
    /// </summary>
    public required IReadOnlyList<HarmonyPatchModel> Patches { get; set; } = new List<HarmonyPatchModel>();

    /// <summary>
    /// <inheritdoc cref="CrashReportModel.AdditionalMetadata"/>
    /// </summary>
    /// <returns><inheritdoc cref="CrashReportModel.AdditionalMetadata"/></returns>
    public required IReadOnlyList<MetadataModel> AdditionalMetadata { get; set; } = new List<MetadataModel>();
}