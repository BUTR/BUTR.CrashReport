using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

public sealed record HarmonyPatchModel
{
    public required HarmonyPatchModelType Type { get; set; }
    public required string Owner { get; set; }
    public required string Namespace { get; set; }
    public required int Index { get; set; }
    public required int Priority { get; set; }
    public required IReadOnlyList<string> Before { get; set; } = new List<string>();
    public required IReadOnlyList<string> After { get; set; } = new List<string>();
    public required IReadOnlyList<MetadataModel> AdditionalMetadata { get; set; } = new List<MetadataModel>();
}