using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents a method from stack trace.
/// </summary>
public sealed record EnhancedStacktraceFrameModel
{
    /// <summary>
    /// <inheritdoc cref="System.Diagnostics.StackFrame.ToString"/>
    /// </summary>
    /// <returns><inheritdoc cref="System.Diagnostics.StackFrame.ToString"/></returns>
    public required string FrameDescription { get; set; }

    /// <summary>
    /// Whether there was an issue with getting the data from the stackframe.
    /// </summary>
    public required bool MethodFromStackframeIssue { get; set; }

    /// <summary>
    /// <inheritdoc cref="System.Diagnostics.StackFrame.GetILOffset"/>
    /// </summary>
    /// <returns><inheritdoc cref="System.Diagnostics.StackFrame.GetILOffset"/></returns>
    public required int? ILOffset { get; set; }

    /// <summary>
    /// <inheritdoc cref="System.Diagnostics.StackFrame.GetNativeOffset"/>
    /// </summary>
    /// <returns><inheritdoc cref="System.Diagnostics.StackFrame.GetNativeOffset"/></returns>
    public required int? NativeOffset { get; set; }

    /// <summary>
    /// The method from the stacktrace frame that is being executed.
    /// </summary>
    public required MethodExecuting ExecutingMethod { get; set; }

    /// <summary>
    /// The original method that might be patched.
    /// </summary>
    public required MethodSimple? OriginalMethod { get; set; }

    /// <summary>
    /// The list of patch methods that are applied to the method.
    /// </summary>
    public required IList<MethodSimple> PatchMethods { get; set; } = new List<MethodSimple>();

    /// <summary>
    /// <inheritdoc cref="CrashReportModel.AdditionalMetadata"/>
    /// </summary>
    /// <returns><inheritdoc cref="CrashReportModel.AdditionalMetadata"/></returns>
    public required IList<MetadataModel> AdditionalMetadata { get; set; } = new List<MetadataModel>();

    /// <inheritdoc />
    public bool Equals(EnhancedStacktraceFrameModel? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return FrameDescription == other.FrameDescription && MethodFromStackframeIssue == other.MethodFromStackframeIssue && ILOffset == other.ILOffset && NativeOffset == other.NativeOffset && ExecutingMethod.Equals(other.ExecutingMethod) && Equals(OriginalMethod, other.OriginalMethod) && PatchMethods.Equals(other.PatchMethods) && AdditionalMetadata.Equals(other.AdditionalMetadata);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = FrameDescription.GetHashCode();
            hashCode = (hashCode * 397) ^ MethodFromStackframeIssue.GetHashCode();
            hashCode = (hashCode * 397) ^ ILOffset.GetHashCode();
            hashCode = (hashCode * 397) ^ NativeOffset.GetHashCode();
            hashCode = (hashCode * 397) ^ ExecutingMethod.GetHashCode();
            hashCode = (hashCode * 397) ^ (OriginalMethod != null ? OriginalMethod.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ PatchMethods.GetHashCode();
            hashCode = (hashCode * 397) ^ AdditionalMetadata.GetHashCode();
            return hashCode;
        }
    }
}