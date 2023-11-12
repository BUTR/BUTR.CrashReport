using System;

namespace BUTR.CrashReport.Models.Analyzer;

/// <summary>
/// Represents the type of a module fix.
/// </summary>
[Flags]
public enum ModuleSuggestedFixType
{
    /// <summary>
    /// No fix.
    /// </summary>
    None = 0,

    /// <summary>
    /// Update the module.
    /// </summary>
    Update = 1,

    /// <summary>
    /// Downgrade the module.
    /// </summary>
    Downgrade = 2,

    /// <summary>
    /// Remove the module.
    /// </summary>
    Remove = 4,
}