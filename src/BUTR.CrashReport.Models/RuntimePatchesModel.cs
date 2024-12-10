using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents a runtime patch model.
/// </summary>
public sealed class RuntimePatchesModel
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
    /// The list of MonoMod detours.
    /// </summary>
    public required IList<RuntimePatchModel> Patches { get; set; } = new List<RuntimePatchModel>();

    /// <summary>
    /// <inheritdoc cref="CrashReportModel.AdditionalMetadata"/>
    /// </summary>
    /// <returns><inheritdoc cref="CrashReportModel.AdditionalMetadata"/></returns>
    public required IList<MetadataModel> AdditionalMetadata { get; set; } = new List<MetadataModel>();
}