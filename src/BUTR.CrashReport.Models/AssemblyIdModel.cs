using BUTR.CrashReport.Models.Utils;

using System;
using System.Reflection;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents an assembly identity.
/// </summary>
public sealed record AssemblyIdModel : IEquatable<AssemblyModel>, IEquatable<AssemblyName>
{
    /// <summary>
    /// Creates a new instance of <see cref="AssemblyIdModel"/> from an <see cref="AssemblyName"/>.
    /// </summary>
    /// <param name="assemblyName"></param>
    /// <returns></returns>
    public static AssemblyIdModel FromAssembly(AssemblyName assemblyName) => new()
    {
        Name = assemblyName.Name,
        Version = assemblyName.Version.ToString(),
        PublicKeyToken = AssemblyUtils.PublicKeyAsString(assemblyName.GetPublicKeyToken()),
    };

    /// <summary>
    /// <inheritdoc cref="System.Reflection.AssemblyName.Name"/>
    /// </summary>
    /// <returns><inheritdoc cref="System.Reflection.AssemblyName.Name"/></returns>
    public required string Name { get; set; }

    /// <summary>
    /// <inheritdoc cref="System.Reflection.AssemblyName.Version"/>
    /// </summary>
    /// <returns>A string that represents the major, minor, build, and revision numbers of the assembly.</returns>
    public required string? Version { get; set; }

    /// <summary>
    /// <inheritdoc cref="System.Reflection.AssemblyName.GetPublicKeyToken"/>
    /// </summary>
    /// <returns>A hex string that contains the public key token.</returns>
    public required string? PublicKeyToken { get; set; }

    /// <inheritdoc />
    public bool Equals(AssemblyIdModel? other) => other is not null &&
                                                  Name == other.Name &&
                                                  ((Version is null || other.Version is null) || Version == other.Version) &&
                                                  PublicKeyToken == other.PublicKeyToken;

    /// <inheritdoc />
    public bool Equals(AssemblyModel? other) => other is not null && other.Id.Equals(this);

    /// <inheritdoc />
    public bool Equals(AssemblyName? other) => other is not null &&
                                               Name == other.Name &&
                                               (Version is null || Version == other.Version.ToString()) &&
                                               PublicKeyToken == AssemblyUtils.PublicKeyAsString(other.GetPublicKeyToken());
}