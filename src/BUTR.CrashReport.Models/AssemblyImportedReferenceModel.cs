namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents an imported assembly reference.
/// </summary>
public sealed record AssemblyImportedReferenceModel
{
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
    /// The empty default constructor.
    /// </summary>
    public AssemblyImportedReferenceModel() { }

    /// <inheritdoc />
    public bool Equals(AssemblyImportedReferenceModel? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name &&
               Version == other.Version &&
               Culture == other.Culture &&
               PublicKeyToken == other.PublicKeyToken;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Name.GetHashCode();
            hashCode = (hashCode * 397) ^ Version.GetHashCode();
            hashCode = (hashCode * 397) ^ (Culture != null ? Culture.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (PublicKeyToken != null ? PublicKeyToken.GetHashCode() : 0);
            return hashCode;
        }
    }
}