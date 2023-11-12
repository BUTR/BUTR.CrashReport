using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents an involved module info.
/// </summary>
public record InvolvedModuleModel
{
    /// <summary>
    /// <inheritdoc cref="ModuleModel.Id"/>
    /// </summary>
    /// <returns><inheritdoc cref="ModuleModel.Id"/></returns>
    public required string ModuleId { get; set; }

    /// <summary>
    /// <inheritdoc cref="EnhancedStacktraceFrameModel.Name"/>
    /// </summary>
    /// <returns><inheritdoc cref="EnhancedStacktraceFrameModel.Name"/></returns>
    public required string EnhancedStacktraceFrameName { get; set; }

    /// <summary>
    /// <inheritdoc cref="CrashReportModel.AdditionalMetadata"/>
    /// </summary>
    /// <returns><inheritdoc cref="CrashReportModel.AdditionalMetadata"/></returns>
    public required IReadOnlyList<MetadataModel> AdditionalMetadata { get; set; } = new List<MetadataModel>();
}