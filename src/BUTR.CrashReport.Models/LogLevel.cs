namespace BUTR.CrashReport.Models;

/// <summary>
/// Specifies the meaning and relative importance of a log event.
/// </summary>
public enum LogLevel
{
    /// <summary>
    /// Unknown log level.
    /// </summary>
    None = 0,

    /// <summary>
    /// Anything and everything you might want to know about
    /// a running block of code.
    /// </summary>
    Verbose = 1,

    /// <summary>
    /// Internal system events that aren't necessarily
    /// observable from the outside.
    /// </summary>
    Debug = 2,

    /// <summary>
    /// The lifeblood of operational intelligence - things
    /// happen.
    /// </summary>
    Information = 3,

    /// <summary>
    /// Service is degraded or endangered.
    /// </summary>
    Warning = 4,

    /// <summary>
    /// Functionality is unavailable, invariants are broken
    /// or data is lost.
    /// </summary>
    Error = 5,

    /// <summary>
    /// If you have a pager, it goes off when one of these
    /// occurs.
    /// </summary>
    Fatal = 6,
}