using System;
using System.Collections.Generic;
using System.Reflection;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents a stack frame runtime patch.
/// </summary>
public sealed record MethodWithRuntimePatches
{
    /// <summary>
    /// <inheritdoc cref="EnhancedStacktraceFrameModel.OriginalMethod"/>
    /// </summary>
    public required MethodBase? OriginalMethod { get; set; }

    /// <summary>
    /// <inheritdoc cref="EnhancedStacktraceFrameModel.ExecutingMethod"/>
    /// </summary>
    public required MethodBase? ExecutingMethod { get; set; }

    /// <summary>
    /// <inheritdoc cref="EnhancedStacktraceFrameModel.PatchMethods"/>
    /// </summary>
    public required IList<ManagedRuntimePatch> Patches { get; set; }

    /// <summary>
    /// The native code pointer.
    /// </summary>
    public required IntPtr NativeCodePtr { get; set; }

    /// <summary>
    /// Deconstructs the object.
    /// </summary>
    public void Deconstruct(out MethodBase? original, out MethodBase? executing, out IList<ManagedRuntimePatch> patches, out IntPtr nativeCodePtr)
    {
        original = OriginalMethod;
        executing = ExecutingMethod;
        patches = Patches;
        nativeCodePtr = NativeCodePtr;
    }
}