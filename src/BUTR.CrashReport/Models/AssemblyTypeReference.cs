using BUTR.CrashReport.Decompilers.Models;

namespace BUTR.CrashReport.Models;

/// <summary>
/// <inheritdoc cref="AssemblyTypeReferenceInternal"/>
/// </summary>
public record AssemblyTypeReference
{
    /// <summary>
    /// <inheritdoc cref="AssemblyTypeReferenceInternal.Name"/>
    /// </summary>
    /// <returns><inheritdoc cref="AssemblyTypeReferenceInternal.Name"/></returns>
    public required string Name { get; set; }

    /// <summary>
    /// <inheritdoc cref="AssemblyTypeReferenceInternal.Namespace"/>
    /// </summary>
    /// <returns><inheritdoc cref="AssemblyTypeReferenceInternal.Namespace"/></returns>
    public required string Namespace { get; set; }

    /// <summary>
    /// <inheritdoc cref="AssemblyTypeReferenceInternal.FullName"/>
    /// </summary>
    /// <returns><inheritdoc cref="AssemblyTypeReferenceInternal.FullName"/></returns>
    public required string FullName { get; set; }
}