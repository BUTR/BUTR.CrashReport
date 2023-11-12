using System;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents attributes for a .NET assembly.
/// </summary>
[Flags]
public enum AssemblyModelType
{
    /// <summary>
    /// Unknown assembly origin
    /// </summary>
    Unclassified = 0,

    /// <summary>
    /// Dynamic assembly origin
    /// </summary>
    Dynamic = 1,

    /// <summary>
    /// GAC assembly origin
    /// </summary>
    GAC = 2,

    /// <summary>
    /// System assembly
    /// </summary>
    System = 4,

    /// <summary>
    /// Game core assembly
    /// </summary>
    GameCore = 8,

    /// <summary>
    /// Game module assembly
    /// </summary>
    GameModule = 16,

    /// <summary>
    /// Custom module assembly
    /// </summary>
    Module = 32,
}