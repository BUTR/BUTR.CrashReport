using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents the exception of the crash report.
/// </summary>
public record ExceptionModel
{
    /// <summary>
    /// The assembly identity of the assembly. Is associated with the source of the exception.
    /// </summary>
    public required AssemblyIdModel? SourceAssemblyId { get; set; }

    /// <summary>
    /// <inheritdoc cref="ModuleModel.Id"/> Is associated with the source of the exception.
    /// </summary>
    /// <returns><inheritdoc cref="ModuleModel.Id"/></returns>
    public required string? SourceModuleId { get; set; }

    /// <summary>
    /// <inheritdoc cref="LoaderPluginModel.Id"/> Is associated with the source of the exception.
    /// </summary>
    /// <returns><inheritdoc cref="LoaderPluginModel.Id"/></returns>
    public required string? SourceLoaderPluginId { get; set; }

    /// <summary>
    /// The type full name of the exception.
    /// </summary>
    public required string Type { get; set; }

    /// <summary>
    /// <inheritdoc cref="System.Exception.Message"/>
    /// </summary>
    /// <returns><inheritdoc cref="System.Exception.Message"/></returns>
    public required string Message { get; set; }

    /// <summary>
    /// <inheritdoc cref="System.Exception.StackTrace"/>
    /// </summary>
    /// <returns><inheritdoc cref="System.Exception.StackTrace"/></returns>
    public required string CallStack { get; set; }

    /// <summary>
    /// The nested exception model.
    /// </summary>
    /// <returns>The inner exception model.</returns>
    public required ExceptionModel? InnerException { get; set; }

    /// <summary>
    /// <inheritdoc cref="CrashReportModel.AdditionalMetadata"/>
    /// </summary>
    /// <returns><inheritdoc cref="CrashReportModel.AdditionalMetadata"/></returns>
    public required IList<MetadataModel> AdditionalMetadata { get; set; } = new List<MetadataModel>();
}