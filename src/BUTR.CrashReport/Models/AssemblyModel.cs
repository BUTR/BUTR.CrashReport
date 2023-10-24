using BUTR.CrashReport.Utils;

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
    /// <inheritdoc cref="System.Reflection.AssemblyName.Name"/>
    /// </summary>
    /// <returns><inheritdoc cref="System.Reflection.AssemblyName.Name"/></returns>
    public required string Name { get; set; }

    /// <summary>
    /// <inheritdoc cref="System.Reflection.AssemblyName.Version"/>
    /// </summary>
    /// <returns>A string that represents the major, minor, build, and revision numbers of the assembly.</returns>
    public required string Version { get; set; }

    /// <summary>
    /// <inheritdoc cref="System.Reflection.AssemblyName.CultureName"/>
    /// </summary>
    /// <returns><inheritdoc cref="System.Reflection.AssemblyName.CultureName"/></returns>
    public required string? Culture { get; set; }

    /// <summary>
    /// <inheritdoc cref="System.Reflection.AssemblyName.GetPublicKeyToken"/>
    /// </summary>
    /// <returns>A hex string that contains the public key token.</returns>
    public required string? PublicKeyToken { get; set; }

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
    public required IReadOnlyList<AssemblyImportedTypeReferenceModel> ImportedTypeReferences { get; set; } = new List<AssemblyImportedTypeReferenceModel>();

    /// <summary>
    /// The list of imported assembly references from the assembly.
    /// </summary>
    public required IReadOnlyList<AssemblyImportedReferenceModel> ImportedAssemblyReferences { get; set; } = new List<AssemblyImportedReferenceModel>();

    /// <summary>
    /// <inheritdoc cref="CrashReportModel.AdditionalMetadata"/>
    /// </summary>
    /// <returns><inheritdoc cref="CrashReportModel.AdditionalMetadata"/></returns>
    public required IReadOnlyList<MetadataModel> AdditionalMetadata { get; set; } = new List<MetadataModel>();

    /// <summary>
    /// <inheritdoc cref="System.Reflection.AssemblyName.FullName"/>
    /// </summary>
    /// <returns><inheritdoc cref="System.Reflection.AssemblyName.FullName"/></returns>
    public string GetFullName() => AssemblyNameFormatter.ComputeDisplayName(Name, Version, Culture, PublicKeyToken);
}