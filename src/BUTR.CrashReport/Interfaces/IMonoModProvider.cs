/*
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using BUTR.CrashReport.Models;

namespace BUTR.CrashReport.Interfaces;

public interface IMonoModProvider
{
    /// <summary>
    /// Returns all patched methods.
    /// </summary>
    IEnumerable<MethodBase> GetAllPatchedMethods();
    
    MonoModPatches? GetPatchInfo(MethodBase originalMethod);
    
    MonoModPatches? GetPatchInfo(StackFrame frame, IModuleProvider moduleProvider, ILoaderPluginProvider loaderPluginProvider);

    /// <summary>
    /// Returns the actual executing method from a stackframe.
    /// </summary>
    MethodInfo? GetExecutingMethod(StackFrame frame);
    
    /// <summary>
    /// Returns the original method for a given patch method.
    /// </summary>
    /// <param name="frame">The frame</param>
    MethodBase? GetOriginalMethod(StackFrame frame);
    
    /// <summary>
    /// Returns the JIT compiled (native) start address of a method.
    /// </summary>
    IntPtr GetNativeMethodBody(MethodBase method);
}
*/