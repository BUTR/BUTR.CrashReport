using System.Collections.Generic;
using System.Linq;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents the actual executing method of a stack trace frame. Can the the original method or a patched method.
/// </summary>
public sealed record MethodExecutingModel : MethodSimpleModel
{
    /// <summary>
    /// The native code of the method that was compiled by the JIT.
    /// </summary>
    public required IList<string> NativeInstructions { get; set; } = new List<string>();

    /// <inheritdoc />
    public bool Equals(MethodExecutingModel? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return base.Equals(other) &&
               NativeInstructions.SequenceEqual(other.NativeInstructions);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            return (base.GetHashCode() * 397) ^ NativeInstructions.GetHashCode();
        }
    }
}