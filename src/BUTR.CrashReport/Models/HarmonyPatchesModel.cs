using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

public sealed record HarmonyPatchesModel
{
    public required string? OriginalMethod { get; set; }
    public required string? OriginalMethodFullName { get; set; }
    public required IReadOnlyList<HarmonyPatchModel> Prefixes { get; set; }
    public required IReadOnlyList<HarmonyPatchModel> Postfixes { get; set; }
    public required IReadOnlyList<HarmonyPatchModel> Finalizers { get; set; }
    public required IReadOnlyList<HarmonyPatchModel> Transpilers { get; set; }
}