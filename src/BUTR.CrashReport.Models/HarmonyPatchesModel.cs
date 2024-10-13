/*
using System.Collections.Generic;
using System.Linq;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents the list of Harmony patches for a given original method.
/// </summary>
public sealed record HarmonyPatchesModel
{
    /// <summary>
    /// The original method type name.
    /// </summary>
    public required string? OriginalMethodDeclaredTypeName { get; set; }

    /// <summary>
    /// The original method name.
    /// </summary>
    public required string? OriginalMethodName { get; set; }

    /// <summary>
    /// The list of Harmony patches.
    /// </summary>
    public required IList<HarmonyPatchModel> Patches { get; set; } = new List<HarmonyPatchModel>();

    /// <summary>
    /// <inheritdoc cref="CrashReportModel.AdditionalMetadata"/>
    /// </summary>
    /// <returns><inheritdoc cref="CrashReportModel.AdditionalMetadata"/></returns>
    public required IList<MetadataModel> AdditionalMetadata { get; set; } = new List<MetadataModel>();

    /// <inheritdoc />
    public bool Equals(HarmonyPatchesModel? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return OriginalMethodDeclaredTypeName == other.OriginalMethodDeclaredTypeName &&
               OriginalMethodName == other.OriginalMethodName &&
               Patches.SequenceEqual(other.Patches) &&
               AdditionalMetadata.SequenceEqual(other.AdditionalMetadata);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = (OriginalMethodDeclaredTypeName != null ? OriginalMethodDeclaredTypeName.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (OriginalMethodName != null ? OriginalMethodName.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ Patches.GetHashCode();
            hashCode = (hashCode * 397) ^ AdditionalMetadata.GetHashCode();
            return hashCode;
        }
    }
}
*/