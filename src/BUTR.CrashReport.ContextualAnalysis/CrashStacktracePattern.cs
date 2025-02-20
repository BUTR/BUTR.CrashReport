using System.Linq;

namespace BUTR.CrashReport.ContextualAnalysis;

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
    public string[] ArgumentTypes { get; set; } = [];

    /// <summary>
    /// The generic type parameters of the method, if applicable.
    /// </summary>
    public string[] TypeParameters { get; set; } = [];

    /// <summary>
    /// The position in the stack trace where this pattern should match.
    /// </summary>
    public StacktraceMatchPosition Position { get; set; }

    /// <summary>
    /// The specific index in the stack trace for position-based matching.
    /// </summary>
    public int? Index { get; set; }

    /// <inheritdoc />
    public bool Equals(CrashStacktracePattern? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Type == other.Type &&
               Method == other.Method &&
               ArgumentTypes.SequenceEqual(other.ArgumentTypes) &&
               TypeParameters.SequenceEqual(other.TypeParameters) &&
               Position == other.Position &&
               Index == other.Index;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = (Type != null ? Type.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (Method != null ? Method.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (ArgumentTypes != null ? ArgumentTypes.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (TypeParameters != null ? TypeParameters.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (Position != null ? Position.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (Index != null ? Index.GetHashCode() : 0);
            return hashCode;
        }
    }
}