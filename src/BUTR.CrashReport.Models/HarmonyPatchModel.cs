/*
using System.Collections.Generic;
using System.Linq;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents a Harmony patch.
/// </summary>
public sealed record HarmonyPatchModel
{
    /// <summary>
    /// The type of the patch.
    /// </summary>
    public required HarmonyPatchType Type { get; set; }

    /// <summary>
    /// <inheritdoc cref="ModuleModel.Id"/> Is null if not from a module.
    /// </summary>
    /// <returns><inheritdoc cref="ModuleModel.Id"/></returns>
    public required string? ModuleId { get; set; }

    /// <summary>
    /// <inheritdoc cref="LoaderPluginModel.Id"/> Is null if not from a module.
    /// </summary>
    /// <returns><inheritdoc cref="LoaderPluginModel.Id"/></returns>
    public required string? LoaderPluginId { get; set; }

    /// <summary>
    /// The <see cref="AssemblyIdModel.Name"/> of the assembly that contains the patch.
    /// </summary>
    public required AssemblyIdModel? AssemblyId { get; set; }

    /// <summary>
    /// <inheritdoc cref="HarmonyLib.Patch.owner"/>
    /// </summary>
    /// <returns><inheritdoc cref="HarmonyLib.Patch.owner"/></returns>
    public required string Owner { get; set; }

    public required string Namespace { get; set; }

    /// <summary>
    /// <inheritdoc cref="HarmonyLib.Patch.index"/>
    /// </summary>
    /// <returns><inheritdoc cref="HarmonyLib.Patch.index"/></returns>
    public required int Index { get; set; }

    /// <summary>
    /// <inheritdoc cref="HarmonyLib.Patch.priority"/>
    /// </summary>
    /// <returns><inheritdoc cref="HarmonyLib.Patch.priority"/></returns>
    public required int Priority { get; set; }

    /// <summary>
    /// <inheritdoc cref="HarmonyLib.Patch.before"/>
    /// </summary>
    /// <returns><inheritdoc cref="HarmonyLib.Patch.before"/></returns>
    public required IList<string> Before { get; set; } = new List<string>();

    /// <summary>
    /// <inheritdoc cref="HarmonyLib.Patch.after"/>
    /// </summary>
    /// <returns><inheritdoc cref="HarmonyLib.Patch.after"/></returns>
    public required IList<string> After { get; set; } = new List<string>();

    /// <summary>
    /// <inheritdoc cref="CrashReportModel.AdditionalMetadata"/>
    /// </summary>
    /// <returns><inheritdoc cref="CrashReportModel.AdditionalMetadata"/></returns>
    public required IList<MetadataModel> AdditionalMetadata { get; set; } = new List<MetadataModel>();

    /// <inheritdoc />
    public bool Equals(HarmonyPatchModel? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Type == other.Type &&
               ModuleId == other.ModuleId &&
               LoaderPluginId == other.LoaderPluginId &&
               Equals(AssemblyId, other.AssemblyId) &&
               Owner == other.Owner &&
               Namespace == other.Namespace &&
               Index == other.Index &&
               Priority == other.Priority &&
               Before.SequenceEqual(other.Before) &&
               After.SequenceEqual(other.After) &&
               AdditionalMetadata.SequenceEqual(other.AdditionalMetadata);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = (int) Type;
            hashCode = (hashCode * 397) ^ (ModuleId != null ? ModuleId.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (LoaderPluginId != null ? LoaderPluginId.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (AssemblyId != null ? AssemblyId.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ Owner.GetHashCode();
            hashCode = (hashCode * 397) ^ Namespace.GetHashCode();
            hashCode = (hashCode * 397) ^ Index;
            hashCode = (hashCode * 397) ^ Priority;
            hashCode = (hashCode * 397) ^ Before.GetHashCode();
            hashCode = (hashCode * 397) ^ After.GetHashCode();
            hashCode = (hashCode * 397) ^ AdditionalMetadata.GetHashCode();
            return hashCode;
        }
    }
}
*/