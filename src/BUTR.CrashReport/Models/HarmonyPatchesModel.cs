using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

public sealed record HarmonyPatchesModel
{
    public required string? OriginalMethod { get; set; }
    public required string? OriginalMethodFullName { get; set; }
    public required ICollection<HarmonyPatchModel> Prefixes { get; set; }
    public required ICollection<HarmonyPatchModel> Postfixes { get; set; }
    public required ICollection<HarmonyPatchModel> Finalizers { get; set; }
    public required ICollection<HarmonyPatchModel> Transpilers { get; set; }
}