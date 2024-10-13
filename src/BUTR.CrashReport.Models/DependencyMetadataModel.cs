using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents the dependency metadata for a module.
/// </summary>
public sealed record DependencyMetadataModel
{
    /// <summary>
    /// <inheritdoc cref="ModuleModel.Id"/> Is null if not from a module.
    /// <inheritdoc cref="LoaderPluginModel.Id"/> Is null if not from a plugin
    /// </summary>
    public required string ModuleOrPluginId { get; set; }

    /// <summary>
    /// The dependency type.
    /// </summary>
    public required DependencyMetadataType Type { get; set; }

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

    /// <inheritdoc />
    public bool Equals(DependencyMetadataModel? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return ModuleOrPluginId == other.ModuleOrPluginId &&
               Type == other.Type &&
               IsOptional == other.IsOptional &&
               Version == other.Version &&
               VersionRange == other.VersionRange &&
               AdditionalMetadata.Equals(other.AdditionalMetadata);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = ModuleOrPluginId.GetHashCode();
            hashCode = (hashCode * 397) ^ (int) Type;
            hashCode = (hashCode * 397) ^ IsOptional.GetHashCode();
            hashCode = (hashCode * 397) ^ (Version != null ? Version.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (VersionRange != null ? VersionRange.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ AdditionalMetadata.GetHashCode();
            return hashCode;
        }
    }
}