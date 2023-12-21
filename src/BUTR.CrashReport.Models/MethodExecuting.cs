using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents the actual executing method of a stack trace frame. Can the the original method or a patched method.
/// </summary>
public record MethodExecuting : MethodSimple
{
    /// <summary>
    /// The native code of the method that was compiled by the JIT.
    /// </summary>
    public required IReadOnlyList<string> NativeInstructions { get; set; } = new List<string>();
}