namespace BUTR.CrashReport.Models.Diagnostics;

/// <summary>
/// Defines the position in the stack trace where a pattern should be matched.
/// </summary>
public enum StacktraceMatchPosition
{
    /// <summary>
    /// Matches at any position in the stack trace.
    /// </summary>
    Any,

    /// <summary>
    /// Matches at a specific index in the stack trace.
    /// </summary>
    AtIndex,

    /// <summary>
    /// Matches before a specific index in the stack trace.
    /// </summary>
    BeforeIndex,

    /// <summary>
    /// Matches after a specific index in the stack trace.
    /// </summary>
    AfterIndex,

    /// <summary>
    /// Matches at the start of the stack trace (same as AtIndex with Index 0).
    /// </summary>
    AtStart,

    /// <summary>
    /// Matches at the end of the stack trace.
    /// </summary>
    AtEnd,
}