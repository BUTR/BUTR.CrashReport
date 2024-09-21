using System.Collections.Generic;
using System.Linq;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents the exception of the crash report.
/// </summary>
public sealed record ExceptionModel
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

    /// <inheritdoc />
    public bool Equals(ExceptionModel? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Equals(SourceAssemblyId, other.SourceAssemblyId) &&
               SourceModuleId == other.SourceModuleId &&
               SourceLoaderPluginId == other.SourceLoaderPluginId &&
               Type == other.Type &&
               Message == other.Message &&
               CallStack == other.CallStack &&
               Equals(InnerException, other.InnerException) &&
               AdditionalMetadata.SequenceEqual(other.AdditionalMetadata);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = (SourceAssemblyId != null ? SourceAssemblyId.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (SourceModuleId != null ? SourceModuleId.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (SourceLoaderPluginId != null ? SourceLoaderPluginId.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ Type.GetHashCode();
            hashCode = (hashCode * 397) ^ Message.GetHashCode();
            hashCode = (hashCode * 397) ^ CallStack.GetHashCode();
            hashCode = (hashCode * 397) ^ (InnerException != null ? InnerException.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ AdditionalMetadata.GetHashCode();
            return hashCode;
        }
    }
}