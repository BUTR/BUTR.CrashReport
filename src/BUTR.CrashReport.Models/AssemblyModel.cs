using System.Collections.Generic;
using System.Linq;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents a .NET assembly.
/// </summary>
public sealed record AssemblyModel
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
    public required AssemblyArchitectureType Architecture { get; set; }

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
    public required AssemblyType Type { get; set; }

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

    /// <inheritdoc />
    public bool Equals(AssemblyModel? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return ModuleId == other.ModuleId &&
               LoaderPluginId == other.LoaderPluginId &&
               Id.Equals(other.Id) &&
               CultureName == other.CultureName &&
               Architecture == other.Architecture &&
               Hash == other.Hash &&
               AnonymizedPath == other.AnonymizedPath &&
               Type == other.Type &&
               ImportedTypeReferences.SequenceEqual(other.ImportedTypeReferences) &&
               ImportedAssemblyReferences.SequenceEqual(other.ImportedAssemblyReferences) &&
               AdditionalMetadata.SequenceEqual(other.AdditionalMetadata);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = (ModuleId != null ? ModuleId.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (LoaderPluginId != null ? LoaderPluginId.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ Id.GetHashCode();
            hashCode = (hashCode * 397) ^ (CultureName != null ? CultureName.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ Architecture.GetHashCode();
            hashCode = (hashCode * 397) ^ Hash.GetHashCode();
            hashCode = (hashCode * 397) ^ AnonymizedPath.GetHashCode();
            hashCode = (hashCode * 397) ^ (int) Type;
            hashCode = (hashCode * 397) ^ ImportedTypeReferences.GetHashCode();
            hashCode = (hashCode * 397) ^ ImportedAssemblyReferences.GetHashCode();
            hashCode = (hashCode * 397) ^ AdditionalMetadata.GetHashCode();
            return hashCode;
        }
    }
}