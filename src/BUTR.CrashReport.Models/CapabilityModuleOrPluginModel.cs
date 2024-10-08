﻿namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents the functionality that is used by the module or plugin.
/// </summary>
public sealed record CapabilityModuleOrPluginModel
{
    /// <summary>
    /// The name of the capability.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// An optional description of the capability.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Creates a new instance of <see cref="CapabilityModuleOrPluginModel"/>.
    /// </summary>
    public CapabilityModuleOrPluginModel(string name) => Name = name;

    /// <inheritdoc />
    public bool Equals(CapabilityModuleOrPluginModel? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name &&
               Description == other.Description;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            return (Name.GetHashCode() * 397) ^ Description.GetHashCode();
        }
    }
}