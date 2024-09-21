using System.Collections.Generic;
using System.Linq;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents a native assembly.
/// </summary>
public sealed record NativeAssemblyModel
{
    /// <summary>
    /// <inheritdoc cref="AssemblyIdModel"/>
    /// </summary>
    public required AssemblyIdModel Id { get; set; }

    /// <summary>
    /// <inheritdoc cref="NativeAssemblyArchitectureType"/>
    /// </summary>
    /// <returns><inheritdoc cref="NativeAssemblyArchitectureType"/></returns>
    public required NativeAssemblyArchitectureType Architecture { get; set; }

    /// <summary>
    /// The hash of the assembly.
    /// </summary>
    public required string Hash { get; set; }

    /// <summary>
    /// The anonymized path to the assembly.
    /// </summary>
    public required string AnonymizedPath { get; set; }

    /// <summary>
    /// <inheritdoc cref="CrashReportModel.AdditionalMetadata"/>
    /// </summary>
    /// <returns><inheritdoc cref="CrashReportModel.AdditionalMetadata"/></returns>
    public required IList<MetadataModel> AdditionalMetadata { get; set; } = new List<MetadataModel>();

    /// <inheritdoc />
    public bool Equals(NativeAssemblyModel? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id &&
               Architecture == other.Architecture &&
               Hash == other.Hash &&
               AnonymizedPath == other.AnonymizedPath &&
               AdditionalMetadata.SequenceEqual(other.AdditionalMetadata);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Id.GetHashCode();
            hashCode = (hashCode * 397) ^ Architecture.GetHashCode();
            hashCode = (hashCode * 397) ^ Hash.GetHashCode();
            hashCode = (hashCode * 397) ^ AnonymizedPath.GetHashCode();
            hashCode = (hashCode * 397) ^ AdditionalMetadata.GetHashCode();
            return hashCode;
        }
    }
}