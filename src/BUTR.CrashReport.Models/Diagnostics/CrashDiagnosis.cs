namespace BUTR.CrashReport.Models.Diagnostics;

/// <summary>
/// Represents a diagnosis of a crash, providing possible causes and solutions.
/// </summary>
public sealed record CrashDiagnosis
{
    /// <summary>
    /// The criteria used to match the crash details.
    /// </summary>
    public required CrashMatchCriteria MatchCriteria { get; set; }

    /// <summary>
    /// A human-readable title of the diagnosed issue.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// A human-readable description of the diagnosed issue.
    /// </summary>
    public required string Issue { get; set; }

    /// <summary>
    /// A suggested solution or workaround for resolving the diagnosed issue.
    /// </summary>
    public required string Solution { get; set; }
}