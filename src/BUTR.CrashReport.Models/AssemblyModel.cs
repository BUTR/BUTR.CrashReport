using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents a .NET assembly.
/// </summary>
public record AssemblyModel
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
    /// <inheritdoc cref="AssemblyIdModel"/>
    /// </summary>
    public required AssemblyIdModel Id { get; set; }

    /// <summary>
    /// <inheritdoc cref="System.Reflection.AssemblyName.CultureName"/>
    /// </summary>
    /// <returns><inheritdoc cref="System.Reflection.AssemblyName.CultureName"/></returns>
    public required string? CultureName { get; set; }

    /// <summary>
    /// <inheritdoc cref="System.Reflection.AssemblyName.ProcessorArchitecture"/>
    /// </summary>
    /// <returns><inheritdoc cref="System.Reflection.AssemblyName.ProcessorArchitecture"/></returns>
    public required string Architecture { get; set; }

    /// <summary>
    /// The hash of the assembly.
    /// </summary>
    public required string Hash { get; set; }

    /// <summary>
    /// The anonymized path to the assembly.
    /// </summary>
    public required string AnonymizedPath { get; set; }

    /// <summary>
    /// The detected types for the assembly.
    /// </summary>
    public required AssemblyModelType Type { get; set; }

    /// <summary>
    /// The list of imported type references from the assembly.
    /// </summary>
    public required IList<AssemblyImportedTypeReferenceModel> ImportedTypeReferences { get; set; } = new List<AssemblyImportedTypeReferenceModel>();

    /// <summary>
    /// The list of imported assembly references from the assembly.
    /// </summary>
    public required IList<AssemblyImportedReferenceModel> ImportedAssemblyReferences { get; set; } = new List<AssemblyImportedReferenceModel>();

    /// <summary>
    /// <inheritdoc cref="CrashReportModel.AdditionalMetadata"/>
    /// </summary>
    /// <returns><inheritdoc cref="CrashReportModel.AdditionalMetadata"/></returns>
    public required IList<MetadataModel> AdditionalMetadata { get; set; } = new List<MetadataModel>();
}