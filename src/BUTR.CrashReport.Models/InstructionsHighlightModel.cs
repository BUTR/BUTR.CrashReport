namespace BUTR.CrashReport.Models;

/// <summary>
/// Model for instructions highlighting.
/// </summary>
public sealed record InstructionsHighlightModel
{
    /// <summary>
    /// Start line of the instruction.
    /// </summary>
    public int StartLine { get; set; }

    /// <summary>
    /// Start column of the instruction.
    /// </summary>
    public int StartColumn { get; set; }

    /// <summary>
    /// End line of the instruction.
    /// </summary>
    public int EndLine { get; set; }

    /// <summary>
    /// End column of the instruction.
    /// </summary>
    public int EndColumn { get; set; }

    /// <inheritdoc />
    public bool Equals(InstructionsHighlightModel? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return StartLine == other.StartLine &&
               StartColumn == other.StartColumn &&
               EndLine == other.EndLine &&
               EndColumn == other.EndColumn;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = StartLine.GetHashCode();
            hashCode = (hashCode * 397) ^ StartColumn.GetHashCode();
            hashCode = (hashCode * 397) ^ EndLine.GetHashCode();
            hashCode = (hashCode * 397) ^ EndColumn.GetHashCode();
            return hashCode;
        }
    }
}