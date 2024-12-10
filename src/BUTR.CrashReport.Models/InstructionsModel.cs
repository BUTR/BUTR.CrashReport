using System.Collections.Generic;
using System.Linq;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Instructions to be displayed.
/// </summary>
public sealed record InstructionsModel
{
    /// <summary>
    /// Instructions to be displayed.
    /// </summary>
    public IList<string> Instructions { get; set; } = new List<string>();

    /// <summary>
    /// Highlighted instructions.
    /// </summary>
    public InstructionsHighlightModel? Highlight { get; set; }

    /// <inheritdoc />
    public bool Equals(InstructionsModel? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Instructions.SequenceEqual(other.Instructions) &&
               Highlight == other.Highlight;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Instructions.GetHashCode();
            hashCode = (hashCode * 397) ^ Highlight?.GetHashCode() ?? 0;
            return hashCode;
        }
    }
}