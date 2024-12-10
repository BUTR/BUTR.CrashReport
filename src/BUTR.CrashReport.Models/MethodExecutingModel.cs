namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents the actual executing method of a stack trace frame. Can the the original method or a patched method.
/// </summary>
public sealed record MethodExecutingModel : MethodModel
{
    /// <summary>
    /// The native code of the method that was compiled by the JIT.
    /// </summary>
    public required InstructionsModel NativeInstructions { get; set; }

    /// <inheritdoc />
    public bool Equals(MethodExecutingModel? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return base.Equals(other) &&
               NativeInstructions.Equals(other.NativeInstructions);
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