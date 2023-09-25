using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

public sealed record HarmonyPatchModel
{
    public required string Owner { get; set; }
    public required string Namespace { get; set; }
    public required int Index { get; set; }
    public required int Priority { get; set; }
    public required IReadOnlyList<string> Before { get; set; }
    public required IReadOnlyList<string> After { get; set; }
    public required HarmonyPatchModelType Type { get; set; }
}