namespace BUTR.CrashReport.Models;

/// <summary>
/// <inheritdoc cref="BUTR.CrashReport.AssemblyTypeReference"/>
/// </summary>
public record AssemblyImportedTypeReferenceModel
{
    /// <summary>
    /// <inheritdoc cref="BUTR.CrashReport.AssemblyTypeReference.Name"/>
    /// </summary>
    /// <returns><inheritdoc cref="BUTR.CrashReport.AssemblyTypeReference.Name"/></returns>
    public required string Name { get; set; }

    /// <summary>
    /// <inheritdoc cref="BUTR.CrashReport.AssemblyTypeReference.Namespace"/>
    /// </summary>
    /// <returns><inheritdoc cref="BUTR.CrashReport.AssemblyTypeReference.Namespace"/></returns>
    public required string Namespace { get; set; }

    /// <summary>
    /// <inheritdoc cref="BUTR.CrashReport.AssemblyTypeReference.FullName"/>
    /// </summary>
    /// <returns><inheritdoc cref="BUTR.CrashReport.AssemblyTypeReference.FullName"/></returns>
    public required string FullName { get; set; }
}