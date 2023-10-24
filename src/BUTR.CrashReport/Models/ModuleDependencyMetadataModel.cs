using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents the dependency metadata for a module.
/// </summary>
public record ModuleDependencyMetadataModel
{
    /// <summary>
    /// <inheritdoc cref="ModuleModel.Id"/>
    /// </summary>
    /// <returns><inheritdoc cref="ModuleModel.Id"/></returns>
    public required string ModuleId { get; set; }

    /// <summary>
    /// The dependency type.
    /// </summary>
    public required ModuleDependencyMetadataModelType Type { get; set; }

    /// <summary>
    /// Whether the dependency is required.
    /// </summary>
    public required bool IsOptional { get; set; }

    /// <summary>
    /// The minimal version of the dependency.
    /// </summary>
    public required string? Version { get; set; }

    /// <summary>
    /// The minimal and maximum version of the dependency.
    /// </summary>
    public required string? VersionRange { get; set; }

    /// <summary>
    /// <inheritdoc cref="CrashReportModel.AdditionalMetadata"/>
    /// </summary>
    /// <returns><inheritdoc cref="CrashReportModel.AdditionalMetadata"/></returns>
    public required IReadOnlyList<MetadataModel> AdditionalMetadata { get; set; } = new List<MetadataModel>();
}