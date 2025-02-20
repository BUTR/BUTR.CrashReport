namespace BUTR.CrashReport.ContextualAnalysis;

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

    /// <inheritdoc />
    public bool Equals(CrashDiagnosis? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Equals(MatchCriteria, other.MatchCriteria) &&
               Title == other.Title &&
               Issue == other.Issue &&
               Solution == other.Solution;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = (MatchCriteria != null ? MatchCriteria.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (Title != null ? Title.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (Issue != null ? Issue.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (Solution != null ? Solution.GetHashCode() : 0);
            return hashCode;
        }
    }
}