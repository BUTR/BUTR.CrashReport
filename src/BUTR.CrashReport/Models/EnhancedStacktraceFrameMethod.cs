using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

public record EnhancedStacktraceFrameMethod
{
    public required string Module { get; set; }
    public required string MethodFullName { get; set; }
    public required string Method { get; set; }
    public required IReadOnlyList<string> MethodParameters { get; set; }
    public required IReadOnlyList<string> NativeInstructions { get; set; }
    public required IReadOnlyList<string> CilInstructions { get; set; }
}