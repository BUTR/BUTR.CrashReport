using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents the actual executing method of a stack trace frame. Can the the original method or a patched method.
/// </summary>
public sealed record MethodExecuting : MethodSimple
{
    /// <summary>
    /// The native code of the method that was compiled by the JIT.
    /// </summary>
    public required IList<string> NativeInstructions { get; set; } = new List<string>();

    /// <inheritdoc />
    public bool Equals(MethodExecuting? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return base.Equals(other) && NativeInstructions.Equals(other.NativeInstructions);
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