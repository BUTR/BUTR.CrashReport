using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents a plugin from a Loader
/// </summary>
public sealed record LoaderPluginModel
{
    /// <summary>
    /// The unique identifier of the plugin
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// The name of the plugin
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// The version of the plugin
    /// </summary>
    public required string? Version { get; set; }

    /// <summary>
    /// The information for updating the module.
    /// </summary>
    public required UpdateInfoModuleOrLoaderPlugin? UpdateInfo { get; set; }

    /// <summary>
    /// The plugin dependencies of the plugin
    /// </summary>
    public required IList<DependencyMetadataModel> Dependencies { get; set; } = new List<DependencyMetadataModel>();

    /// <summary>
    /// The capabilities, if there are any.
    /// </summary>
    public required IList<CapabilityModuleOrPluginModel> Capabilities { get; set; } = new List<CapabilityModuleOrPluginModel>();

    /// <summary>
    /// <inheritdoc cref="CrashReportModel.AdditionalMetadata"/>
    /// </summary>
    /// <returns><inheritdoc cref="CrashReportModel.AdditionalMetadata"/></returns>
    public required IList<MetadataModel> AdditionalMetadata { get; set; } = new List<MetadataModel>();
}