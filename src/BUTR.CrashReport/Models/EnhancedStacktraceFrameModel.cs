using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

/// <summary>
/// <inheritdoc cref="BUTR.CrashReport.StacktraceEntry"/>
/// </summary>
public record EnhancedStacktraceFrameModel
{
    /// <summary>
    /// TODO: Is for some reason the same as the FrameDescription
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// <inheritdoc cref="BUTR.CrashReport.StacktraceEntry.StackFrameDescription"/>
    /// </summary>
    /// <returns><inheritdoc cref="BUTR.CrashReport.StacktraceEntry.StackFrameDescription"/></returns>
    public required string FrameDescription { get; set; }

    /// <summary>
    /// <inheritdoc cref="BUTR.CrashReport.StacktraceEntry.MethodFromStackframeIssue"/>
    /// </summary>
    /// <returns><inheritdoc cref="BUTR.CrashReport.StacktraceEntry.MethodFromStackframeIssue"/></returns>
    public required bool MethodFromStackframeIssue { get; set; }

    /// <summary>
    /// <inheritdoc cref="BUTR.CrashReport.StacktraceEntry.ILOffset"/>
    /// </summary>
    /// <returns><inheritdoc cref="BUTR.CrashReport.StacktraceEntry.ILOffset"/></returns>
    public required int? ILOffset { get; set; }

    /// <summary>
    /// <inheritdoc cref="BUTR.CrashReport.StacktraceEntry.NativeOffset"/>
    /// </summary>
    /// <returns><inheritdoc cref="BUTR.CrashReport.StacktraceEntry.NativeOffset"/></returns>
    public required int? NativeOffset { get; set; }

    /// <summary>
    /// <inheritdoc cref="BUTR.CrashReport.StacktraceEntry.Method"/>
    /// </summary>
    /// <returns><inheritdoc cref="BUTR.CrashReport.StacktraceEntry.Method"/></returns>
    public required EnhancedStacktraceFrameMethod OriginalMethod { get; set; }

    /// <summary>
    /// <inheritdoc cref="BUTR.CrashReport.StacktraceEntry.PatchMethods"/>
    /// </summary>
    /// <returns><inheritdoc cref="BUTR.CrashReport.StacktraceEntry.PatchMethods"/></returns>
    public required IReadOnlyList<EnhancedStacktraceFrameMethod> PatchMethods { get; set; } = new List<EnhancedStacktraceFrameMethod>();

    /// <summary>
    /// <inheritdoc cref="CrashReportModel.AdditionalMetadata"/>
    /// </summary>
    /// <returns><inheritdoc cref="CrashReportModel.AdditionalMetadata"/></returns>
    public required IReadOnlyList<MetadataModel> AdditionalMetadata { get; set; } = new List<MetadataModel>();
}