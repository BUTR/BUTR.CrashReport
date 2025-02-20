namespace BUTR.CrashReport.Models.Diagnostics;

/// <summary>
/// Represents a pattern used to match specific elements in a stack trace for crash analysis.
/// </summary>
public sealed record CrashStacktracePattern
{
    /// <summary>
    /// The type where the crash occurred, if applicable.
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// The method name that appears in the stack trace, if applicable.
    /// </summary>
    public string? Method { get; set; }

    /// <summary>
    /// The arguments of the method, if applicable.
    /// </summary>
    public string[]? ArgumentTypes { get; set; }

    /// <summary>
    /// The generic type parameters of the method, if applicable.
    /// </summary>
    public string[]? TypeParameters { get; set; }

    /// <summary>
    /// The position in the stack trace where this pattern should match.
    /// </summary>
    public StacktraceMatchPosition Position { get; set; }

    /// <summary>
    /// The specific index in the stack trace for position-based matching.
    /// </summary>
    public int? Index { get; set; }
}