namespace BUTR.CrashReport.Models;

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
    /// x86
    /// </summary>
    x86 = 1,

    /// <summary>
    /// x86_64
    /// </summary>
    x86_64 = 2,

    /// <summary>
    /// ARM
    /// </summary>
    Arm = 3,

    /// <summary>
    /// AArch64
    /// </summary>
    Arm64 = 4,
}