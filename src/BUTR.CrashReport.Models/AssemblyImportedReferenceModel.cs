using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents an imported assembly reference.
/// </summary>
public record AssemblyImportedReferenceModel
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
}