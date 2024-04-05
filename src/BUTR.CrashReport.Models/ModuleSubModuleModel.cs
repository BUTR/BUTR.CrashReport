using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents a game submodule that is loaded into the process.
/// </summary>
public sealed record ModuleSubModuleModel
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

    /// <inheritdoc />
    public bool Equals(ModuleSubModuleModel? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name && Equals(AssemblyId, other.AssemblyId) && Entrypoint == other.Entrypoint && AdditionalMetadata.Equals(other.AdditionalMetadata);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Name.GetHashCode();
            hashCode = (hashCode * 397) ^ (AssemblyId != null ? AssemblyId.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ Entrypoint.GetHashCode();
            hashCode = (hashCode * 397) ^ AdditionalMetadata.GetHashCode();
            return hashCode;
        }
    }
}