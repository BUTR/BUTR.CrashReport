using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

public sealed record HarmonyPatchesModel
{
    public required string? OriginalMethodDeclaredTypeName { get; set; }
    public required string? OriginalMethodName { get; set; }
    public required IReadOnlyList<HarmonyPatchModel> Patches { get; set; } = new List<HarmonyPatchModel>();
    public required IReadOnlyList<MetadataModel> AdditionalMetadata { get; set; } = new List<MetadataModel>();
}