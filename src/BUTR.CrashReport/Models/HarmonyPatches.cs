using System.Collections.Generic;
using System.Reflection;

namespace BUTR.CrashReport.Models;

public record RuntimePatches
{
    public required IList<RuntimePatch> Patches { get; set; }
}

public record RuntimePatch
{
    public required string PatchProvider { get; set; }
    
    public required string PatchType { get; set; }
    
    public required MethodBase Original { get; set; }
    public required MethodBase Patch { get; set; }

    /// <summary>
    /// <inheritdoc cref="CrashReportModel.AdditionalMetadata"/>
    /// </summary>
    /// <returns><inheritdoc cref="CrashReportModel.AdditionalMetadata"/></returns>
    public required IList<MetadataModel> AdditionalMetadata { get; set; } = new List<MetadataModel>();
}

/*
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

public record MonoModPatches
{
    public required IList<MonoModPatch> Detours { get; set; }
    public required IList<MonoModPatch> ILHooks { get; set; }
}

public record MonoModPatch
{
    public required MethodBase Method { get; set; }

    public required bool IsActive { get; set; }

    public required string Id { get; set; }

    public required int? Index { get; set; }
    
    public required int? MaxIndex { get; set; }
    
    public required int? GlobalIndex { get; set; }
    public required int? Priority { get; set; }

    public required int? SubPriority { get; set; }

    public required IList<string> Before { get; set; } = new List<string>();

    public required IList<string> After { get; set; } = new List<string>();
}
*/