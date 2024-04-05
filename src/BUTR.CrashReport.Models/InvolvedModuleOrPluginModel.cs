using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents an involved module info.
/// </summary>
public sealed record InvolvedModuleOrPluginModel
{
    /// <summary>
    /// <inheritdoc cref="ModuleModel.Id"/>
    /// </summary>
    /// <returns><inheritdoc cref="ModuleModel.Id"/></returns>
    public required string ModuleOrLoaderPluginId { get; set; }

    /// <summary>
    /// <inheritdoc cref="EnhancedStacktraceFrameModel.FrameDescription"/>
    /// </summary>
    /// <returns><inheritdoc cref="EnhancedStacktraceFrameModel.FrameDescription"/></returns>
    public required string EnhancedStacktraceFrameName { get; set; }

    /// <summary>
    /// <inheritdoc cref="CrashReportModel.AdditionalMetadata"/>
    /// </summary>
    /// <returns><inheritdoc cref="CrashReportModel.AdditionalMetadata"/></returns>
    public required IList<MetadataModel> AdditionalMetadata { get; set; } = new List<MetadataModel>();

    /// <inheritdoc />
    public bool Equals(InvolvedModuleOrPluginModel? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return ModuleOrLoaderPluginId == other.ModuleOrLoaderPluginId && EnhancedStacktraceFrameName == other.EnhancedStacktraceFrameName && AdditionalMetadata.Equals(other.AdditionalMetadata);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = ModuleOrLoaderPluginId.GetHashCode();
            hashCode = (hashCode * 397) ^ EnhancedStacktraceFrameName.GetHashCode();
            hashCode = (hashCode * 397) ^ AdditionalMetadata.GetHashCode();
            return hashCode;
        }
    }
}