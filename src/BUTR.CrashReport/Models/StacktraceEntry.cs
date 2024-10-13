using System.Reflection;

namespace BUTR.CrashReport.Models;

/// <summary>
/// <inheritdoc cref="BUTR.CrashReport.Models.EnhancedStacktraceFrameModel"/>
/// </summary>
public record StacktraceEntry
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
    public required MethodEntrySimple? OriginalMethod { get; set; }

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
    public required int? ILOffset { get; set; }

    /// <summary>
    /// <inheritdoc cref="BUTR.CrashReport.Models.EnhancedStacktraceFrameModel.NativeOffset"/>
    /// </summary>
    /// <returns><inheritdoc cref="BUTR.CrashReport.Models.EnhancedStacktraceFrameModel.NativeOffset"/></returns>
    public required int? NativeOffset { get; set; }

    /// <summary>
    /// <inheritdoc cref="BUTR.CrashReport.Models.EnhancedStacktraceFrameModel.FrameDescription"/>
    /// </summary>
    /// <returns><inheritdoc cref="BUTR.CrashReport.Models.EnhancedStacktraceFrameModel.FrameDescription"/></returns>
    public required string StackFrameDescription { get; set; }

    /// <summary>
    /// <inheritdoc cref="MethodExecutingModel.NativeInstructions"/>
    /// </summary>
    /// <returns><inheritdoc cref="MethodExecutingModel.NativeInstructions"/></returns>
    public required string[] NativeInstructions { get; set; }

    /// <summary>
    /// <inheritdoc cref="MethodSimpleModel.ILInstructions"/>
    /// </summary>
    /// <returns><inheritdoc cref="MethodSimpleModel.ILInstructions"/></returns>
    public required string[] ILInstructions { get; set; }

    /// <summary>
    /// <inheritdoc cref="MethodSimpleModel.CSharpILMixedInstructions"/>
    /// </summary>
    /// <returns><inheritdoc cref="MethodSimpleModel.CSharpILMixedInstructions"/></returns>
    public required string[] CSharpILMixedInstructions { get; set; }

    /// <summary>
    /// <inheritdoc cref="MethodSimpleModel.CSharpInstructions"/>
    /// </summary>
    /// <returns><inheritdoc cref="MethodSimpleModel.CSharpInstructions"/></returns>
    public required string[] CSharpInstructions { get; set; }

    /// <summary>
    /// <inheritdoc cref="BUTR.CrashReport.Models.EnhancedStacktraceFrameModel.PatchMethods"/>
    /// </summary>
    /// <returns><inheritdoc cref="BUTR.CrashReport.Models.EnhancedStacktraceFrameModel.PatchMethods"/></returns>
    public required MethodEntry[] PatchMethods { get; set; }
}