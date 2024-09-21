using System.Collections.Generic;
using System.Linq;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents a game module that is loaded into the process.
/// </summary>
public sealed record ModuleModel
{
    /// <summary>
    /// The id of the module.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// The name of the module.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// The version of the module.
    /// </summary>
    public required string Version { get; set; }

    /// <summary>
    /// Whether the module was installed by an external source like Steam Workshop.
    /// </summary>
    public required bool IsExternal { get; set; }

    /// <summary>
    /// Whether the module is an official module.
    /// </summary>
    public required bool IsOfficial { get; set; }

    /// <summary>
    /// Whether the module is a singleplayer module.
    /// </summary>
    public required bool IsSingleplayer { get; set; }

    /// <summary>
    /// Whether the module is a multiplayer module.
    /// </summary>
    public required bool IsMultiplayer { get; set; }

    /// <summary>
    /// The URL of the module. Usually the URL is either NexusMods or Steam Workshop.
    /// </summary>
    public required string? Url { get; set; }

    /// <summary>
    /// The information for updating the module.
    /// </summary>
    public required UpdateInfoModuleOrLoaderPlugin? UpdateInfo { get; set; }

    /// <summary>
    /// The dependencies of the module, if there are any.
    /// </summary>
    public required IList<DependencyMetadataModel> DependencyMetadatas { get; set; } = new List<DependencyMetadataModel>();

    /// <summary>
    /// The submodules of the module, if there are any.
    /// </summary>
    public required IList<ModuleSubModuleModel> SubModules { get; set; } = new List<ModuleSubModuleModel>();

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
    public bool Equals(ModuleModel? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id &&
               Name == other.Name &&
               Version == other.Version &&
               IsExternal == other.IsExternal &&
               IsOfficial == other.IsOfficial &&
               IsSingleplayer == other.IsSingleplayer &&
               IsMultiplayer == other.IsMultiplayer &&
               Url == other.Url &&
               Equals(UpdateInfo, other.UpdateInfo) &&
               DependencyMetadatas.SequenceEqual(other.DependencyMetadatas) &&
               SubModules.SequenceEqual(other.SubModules) &&
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
            hashCode = (hashCode * 397) ^ Version.GetHashCode();
            hashCode = (hashCode * 397) ^ IsExternal.GetHashCode();
            hashCode = (hashCode * 397) ^ IsOfficial.GetHashCode();
            hashCode = (hashCode * 397) ^ IsSingleplayer.GetHashCode();
            hashCode = (hashCode * 397) ^ IsMultiplayer.GetHashCode();
            hashCode = (hashCode * 397) ^ (Url != null ? Url.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (UpdateInfo != null ? UpdateInfo.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ DependencyMetadatas.GetHashCode();
            hashCode = (hashCode * 397) ^ SubModules.GetHashCode();
            hashCode = (hashCode * 397) ^ Capabilities.GetHashCode();
            hashCode = (hashCode * 397) ^ AdditionalMetadata.GetHashCode();
            return hashCode;
        }
    }
}