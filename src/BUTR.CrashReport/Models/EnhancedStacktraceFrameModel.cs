using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

public record EnhancedStacktraceFrameModel
{
    public required string Name { get; set; }
    public required string FrameDescription { get; set; }
    public required bool MethodFromStackframeIssue { get; set; }
    public required int? ILOffset { get; set; }
    public required int? NativeOffset { get; set; }
    public required EnhancedStacktraceFrameMethod OriginalMethod { get; set; }
    public required IReadOnlyList<EnhancedStacktraceFrameMethod> PatchMethods { get; set; } = new List<EnhancedStacktraceFrameMethod>();
    public required IReadOnlyList<MetadataModel> AdditionalMetadata { get; set; } = new List<MetadataModel>();
}