using System.Collections.Generic;
using System.Linq;

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

    /// <inheritdoc />
    public bool Equals(LoaderPluginModel? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id &&
               Name == other.Name &&
               Version == other.Version &&
               Equals(UpdateInfo, other.UpdateInfo) &&
               Dependencies.SequenceEqual(other.Dependencies) &&
               Capabilities.SequenceEqual(other.Capabilities) &&
               AdditionalMetadata.SequenceEqual(other.AdditionalMetadata);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Id.GetHashCode();
            hashCode = (hashCode * 397) ^ Name.GetHashCode();
            hashCode = (hashCode * 397) ^ (Version != null ? Version.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (UpdateInfo != null ? UpdateInfo.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ Dependencies.GetHashCode();
            hashCode = (hashCode * 397) ^ Capabilities.GetHashCode();
            hashCode = (hashCode * 397) ^ AdditionalMetadata.GetHashCode();
            return hashCode;
        }
    }
}