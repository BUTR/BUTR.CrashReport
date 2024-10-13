using System;
using System.Collections.Generic;
using System.Reflection;

namespace BUTR.CrashReport.Models;

public record StackFrameRuntimePatch
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
    public required IList<RuntimePatch> Patches { get; set; }

    public required int ILOffset { get; set; }

    public required int NativeILOffset { get; set; }

    public required IntPtr NativeCodePtr { get; set; }
        
    /// <summary>
    /// Deconstructs the object.
    /// </summary>
    public void Deconstruct(out MethodBase? original, out MethodBase? executing, out IList<RuntimePatch> patches, out int ilOffset, out int nativeILOffset, out IntPtr nativeCodePtr)
    {
        original = OriginalMethod;
        executing = ExecutingMethod;
        patches = Patches;
        ilOffset = ILOffset;
        nativeILOffset = NativeILOffset;
        nativeCodePtr = NativeCodePtr;
    }
}