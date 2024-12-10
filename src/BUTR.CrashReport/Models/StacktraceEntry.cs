using System;
using System.Collections.Generic;
using System.Reflection;

namespace BUTR.CrashReport.Models;

/// <summary>
/// <inheritdoc cref="BUTR.CrashReport.Models.EnhancedStacktraceFrameModel"/>
/// </summary>
public sealed record StacktraceEntry
{
    /// <summary>
    /// <inheritdoc cref="BUTR.CrashReport.Models.EnhancedStacktraceFrameModel.ExecutingMethod"/>
    /// </summary>
    /// <returns><inheritdoc cref="BUTR.CrashReport.Models.EnhancedStacktraceFrameModel.ExecutingMethod"/></returns>
    public required MethodBase Method { get; set; }

    /// <summary>
    /// <inheritdoc cref="BUTR.CrashReport.Models.EnhancedStacktraceFrameModel.OriginalMethod"/>
    /// </summary>
    /// <returns><inheritdoc cref="BUTR.CrashReport.Models.EnhancedStacktraceFrameModel.OriginalMethod"/></returns>
    public required MethodEntry? OriginalMethod { get; set; }

    /// <summary>
    /// The module that holds the method. Can be null.
    /// </summary>
    public required IModuleInfo? ModuleInfo { get; set; }

    /// <summary>
    /// The loader plugin that holds the method. Can be null.
    /// </summary>
    public required ILoaderPluginInfo? LoaderPluginInfo { get; set; }

    /// <summary>
    /// <inheritdoc cref="BUTR.CrashReport.Models.EnhancedStacktraceFrameModel.ILOffset"/>
    /// </summary>
    /// <returns><inheritdoc cref="BUTR.CrashReport.Models.EnhancedStacktraceFrameModel.ILOffset"/></returns>
    public required int ILOffset { get; set; }

    /// <summary>
    /// <inheritdoc cref="BUTR.CrashReport.Models.EnhancedStacktraceFrameModel.NativeOffset"/>
    /// </summary>
    /// <returns><inheritdoc cref="BUTR.CrashReport.Models.EnhancedStacktraceFrameModel.NativeOffset"/></returns>
    public required int NativeOffset { get; set; }

    /// <summary>
    /// <inheritdoc cref="BUTR.CrashReport.Models.EnhancedStacktraceFrameModel.NativeOffset"/>
    /// </summary>
    /// <returns><inheritdoc cref="BUTR.CrashReport.Models.EnhancedStacktraceFrameModel.NativeOffset"/></returns>
    public required IntPtr NativeCodePtr { get; set; }

    /// <summary>
    /// <inheritdoc cref="BUTR.CrashReport.Models.EnhancedStacktraceFrameModel.FrameDescription"/>
    /// </summary>
    /// <returns><inheritdoc cref="BUTR.CrashReport.Models.EnhancedStacktraceFrameModel.FrameDescription"/></returns>
    public required string StackFrameDescription { get; set; }

    /// <summary>
    /// <inheritdoc cref="BUTR.CrashReport.Models.EnhancedStacktraceFrameModel.PatchMethods"/>
    /// </summary>
    /// <returns><inheritdoc cref="BUTR.CrashReport.Models.EnhancedStacktraceFrameModel.PatchMethods"/></returns>
    public required MethodRuntimePatchEntry[] PatchMethods { get; set; }
}