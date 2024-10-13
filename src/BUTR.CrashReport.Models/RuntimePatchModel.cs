using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

public sealed class RuntimePatchModel
{
    /// <summary>
    /// <inheritdoc cref="ModuleModel.Id"/> Is null if not from a module.
    /// </summary>
    /// <returns><inheritdoc cref="ModuleModel.Id"/></returns>
    public required string? ModuleId { get; set; }

    /// <summary>
    /// <inheritdoc cref="LoaderPluginModel.Id"/> Is null if not from a module.
    /// </summary>
    /// <returns><inheritdoc cref="LoaderPluginModel.Id"/></returns>
    public required string? LoaderPluginId { get; set; }

    /// <summary>
    /// The <see cref="AssemblyIdModel.Name"/> of the assembly that contains the patch.
    /// </summary>
    public required AssemblyIdModel? AssemblyId { get; set; }
    
    public required string Provider { get; set; }
    
    public required string Type { get; set; }

    public required string FullName { get; set; }
    
    /// <summary>
    /// <inheritdoc cref="CrashReportModel.AdditionalMetadata"/>
    /// </summary>
    /// <returns><inheritdoc cref="CrashReportModel.AdditionalMetadata"/></returns>
    public required IList<MetadataModel> AdditionalMetadata { get; set; } = new List<MetadataModel>();
}