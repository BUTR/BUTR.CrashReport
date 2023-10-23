using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

public record EnhancedStacktraceFrameMethod
{
    public required string? ModuleId { get; set; }
    public required string? MethodDeclaredTypeName { get; set; }
    public required string MethodName { get; set; }
    public required string MethodFullDescription { get; set; }
    public required IReadOnlyList<string> MethodParameters { get; set; } = new List<string>();
    public required IReadOnlyList<string> NativeInstructions { get; set; } = new List<string>();
    public required IReadOnlyList<string> CilInstructions { get; set; } = new List<string>();
    public required IReadOnlyList<MetadataModel> AdditionalMetadata { get; set; } = new List<MetadataModel>();
}