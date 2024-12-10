namespace BUTR.CrashReport.Decompilers.Models;

/// <summary>
/// <inheritdoc cref="AssemblyTypeReferenceInternal"/>
/// </summary>
public record AssemblyTypeReferenceInternal
{
    /// <summary>
    /// <inheritdoc cref="Name"/>
    /// </summary>
    /// <returns><inheritdoc cref="Name"/></returns>
    public required string Name { get; set; }

    /// <summary>
    /// <inheritdoc cref="Namespace"/>
    /// </summary>
    /// <returns><inheritdoc cref="Namespace"/></returns>
    public required string Namespace { get; set; }

    /// <summary>
    /// <inheritdoc cref="FullName"/>
    /// </summary>
    /// <returns><inheritdoc cref="FullName"/></returns>
    public required string FullName { get; set; }
}