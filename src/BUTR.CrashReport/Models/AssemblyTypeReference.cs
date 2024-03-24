namespace BUTR.CrashReport.Models;

/// <summary>
/// <inheritdoc cref="BUTR.CrashReport.Decompilers.Utils.AssemblyTypeReferenceInternal"/>
/// </summary>
public record AssemblyTypeReference
{
    /// <summary>
    /// <inheritdoc cref="BUTR.CrashReport.Decompilers.Utils.AssemblyTypeReferenceInternal.Name"/>
    /// </summary>
    /// <returns><inheritdoc cref="BUTR.CrashReport.Decompilers.Utils.AssemblyTypeReferenceInternal.Name"/></returns>
    public required string Name { get; set; }

    /// <summary>
    /// <inheritdoc cref="BUTR.CrashReport.Decompilers.Utils.AssemblyTypeReferenceInternal.Namespace"/>
    /// </summary>
    /// <returns><inheritdoc cref="BUTR.CrashReport.Decompilers.Utils.AssemblyTypeReferenceInternal.Namespace"/></returns>
    public required string Namespace { get; set; }

    /// <summary>
    /// <inheritdoc cref="BUTR.CrashReport.Decompilers.Utils.AssemblyTypeReferenceInternal.FullName"/>
    /// </summary>
    /// <returns><inheritdoc cref="BUTR.CrashReport.Decompilers.Utils.AssemblyTypeReferenceInternal.FullName"/></returns>
    public required string FullName { get; set; }
}