using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents an involved module info.
/// </summary>
public record InvolvedModuleOrPluginModel
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
}