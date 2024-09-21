namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents an imported type reference.
/// </summary>
public sealed record AssemblyImportedTypeReferenceModel
{
    /// <summary>
    /// <inheritdoc cref="AsmResolver.DotNet.TypeReference.Name"/>
    /// </summary>
    /// <returns><inheritdoc cref="AsmResolver.DotNet.TypeReference.Name"/></returns>
    public required string Name { get; set; }

    /// <summary>
    /// <inheritdoc cref="AsmResolver.DotNet.TypeReference.Namespace"/>
    /// </summary>
    /// <returns><inheritdoc cref="AsmResolver.DotNet.TypeReference.Namespace"/></returns>
    public required string Namespace { get; set; }

    /// <summary>
    /// <inheritdoc cref="AsmResolver.DotNet.TypeReference.FullName"/>
    /// </summary>
    /// <returns><inheritdoc cref="AsmResolver.DotNet.TypeReference.FullName"/></returns>
    public required string FullName { get; set; }

    /// <inheritdoc />
    public bool Equals(AssemblyImportedTypeReferenceModel? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name &&
               Namespace == other.Namespace &&
               FullName == other.FullName;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Name.GetHashCode();
            hashCode = (hashCode * 397) ^ Namespace.GetHashCode();
            hashCode = (hashCode * 397) ^ FullName.GetHashCode();
            return hashCode;
        }
    }
}