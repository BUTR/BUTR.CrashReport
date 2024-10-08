﻿namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents the architecture of a native assembly.
/// </summary>
public enum AssemblyArchitectureType
{
    /// <summary>
    /// Unknown architecture.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// 
    /// </summary>
    MSIL = 1,

    /// <summary>
    /// 
    /// </summary>
    X86 = 2,

    /// <summary>
    /// 
    /// </summary>
    IA64 = 3,

    /// <summary>
    /// 
    /// </summary>
    Amd64 = 4,

    /// <summary>
    /// 
    /// </summary>
    Arm = 5,
}
/// <summary>
/// Represents the architecture of a native assembly.
/// </summary>
public enum NativeAssemblyArchitectureType
{
    /// <summary>
    /// Unknown architecture.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// 
    /// </summary>
    x86 = 1,

    /// <summary>
    /// 
    /// </summary>
    x86_64 = 2,

    /// <summary>
    /// 
    /// </summary>
    Arm = 3,

    /// <summary>
    /// AArch64
    /// </summary>
    Arm64 = 4,
}