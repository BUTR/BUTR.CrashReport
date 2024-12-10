namespace BUTR.CrashReport.Models;

/// <summary>
/// The type of involvement of a module or plugin in a crash.
/// </summary>
public enum InvolvedModuleOrPluginType
{
    /// <summary>
    /// The module or plugin is a directly involved in the crash.
    /// </summary>
    Direct,

    /// <summary>
    /// The module or plugin is a patch for a method that is involved in the crash.
    /// </summary>
    Patch,
}