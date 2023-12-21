namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents an imported type reference.
/// </summary>
public record AssemblyImportedTypeReferenceModel
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
}