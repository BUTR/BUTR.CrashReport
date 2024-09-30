namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents the type of the operating system.
/// </summary>
public enum OperatingSystemType
{
    /// <summary>
    /// Unknown operating system.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Windows operating system.
    /// </summary>
    Windows = 1,

    /// <summary>
    /// Linux operating system.
    /// </summary>
    Linux = 2,

    /// <summary>
    /// MacOS operating system.
    /// </summary>
    MacOS = 3,

    /// <summary>
    /// Wine emulating a Windows operating system.
    /// </summary>
    WindowsOnWine = 4,
}