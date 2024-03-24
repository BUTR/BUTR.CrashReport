using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents the dependency metadata for a module.
/// </summary>
public record DependencyMetadataModel
{
    /// <summary>
    /// <inheritdoc cref="ModuleModel.Id"/> Is null if not from a module.
    /// <inheritdoc cref="LoaderPluginModel.Id"/> Is null if not from a plugin
    /// </summary>
    public required string ModuleOrPluginId { get; set; }

    /// <summary>
    /// The dependency type.
    /// </summary>
    public required DependencyMetadataModelType Type { get; set; }

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
    public required IList<MetadataModel> AdditionalMetadata { get; set; } = new List<MetadataModel>();
}