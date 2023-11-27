using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents the list of MonoMod detours for a given original method.
/// </summary>
public sealed record MonoModDetoursModel
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
    public required IReadOnlyList<MonoModDetourModel> Detours { get; set; } = new List<MonoModDetourModel>();

    /// <summary>
    /// <inheritdoc cref="CrashReportModel.AdditionalMetadata"/>
    /// </summary>
    /// <returns><inheritdoc cref="CrashReportModel.AdditionalMetadata"/></returns>
    public required IReadOnlyList<MetadataModel> AdditionalMetadata { get; set; } = new List<MetadataModel>();
}