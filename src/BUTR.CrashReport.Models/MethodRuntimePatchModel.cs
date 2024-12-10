namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents a patch method.
/// </summary>
public sealed record MethodRuntimePatchModel : MethodModel
{
    /// <summary>
    /// The provider of the patch.
    /// </summary>
    public required string Provider { get; set; }

    /// <summary>
    /// The type of the patch.
    /// </summary>
    public required string Type { get; set; }

    /// <inheritdoc />
    public bool Equals(MethodRuntimePatchModel? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return base.Equals(other) &&
               Provider.Equals(other.Provider) &&
               Type.Equals(other.Type);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = base.GetHashCode();
            hashCode = (hashCode * 397) ^ Provider.GetHashCode();
            hashCode = (hashCode * 397) ^ Type.GetHashCode();
            return hashCode;
        }
    }
}