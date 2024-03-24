using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents a game submodule that is loaded into the process.
/// </summary>
public record ModuleSubModuleModel
{
    /// <summary>
    /// The name of the SubModule.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// The main assembly of the SubModule.
    /// </summary>
    public required AssemblyIdModel? AssemblyId { get; set; }

    /// <summary>
    /// The entry point of the assembly. Can be a method or a type full name.
    /// </summary>
    public required string Entrypoint { get; set; }

    /// <summary>
    /// <inheritdoc cref="CrashReportModel.AdditionalMetadata"/>
    /// </summary>
    /// <returns><inheritdoc cref="CrashReportModel.AdditionalMetadata"/></returns>
    public required IList<MetadataModel> AdditionalMetadata { get; set; } = new List<MetadataModel>();
}