using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents the actual executing method of a stack trace frame. Can the the original method or a patched method.
/// </summary>
public record MethodExecuting : MethodSimple
{
    /// <summary>
    /// <inheritdoc cref="BUTR.CrashReport.StacktraceEntry.NativeInstructions"/>
    /// </summary>
    /// <returns><inheritdoc cref="BUTR.CrashReport.StacktraceEntry.NativeInstructions"/></returns>
    public required IReadOnlyList<string> NativeInstructions { get; set; } = new List<string>();
}