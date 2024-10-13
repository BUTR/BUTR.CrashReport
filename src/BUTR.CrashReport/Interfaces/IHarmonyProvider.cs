﻿/*
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace BUTR.CrashReport.Interfaces;

/// <summary>
/// Provides information about Harmony patches.
/// </summary>
public interface IHarmonyProvider
{
    /// <summary>
    /// Returns all patched methods.
    /// </summary>
    IEnumerable<MethodBase> GetAllPatchedMethods();
    
    /// <summary>
    /// Returns the patch information for a given method.
    /// </summary>
    /// <param name="originalMethod"></param>
    HarmonyPatches? GetPatchInfo(MethodBase originalMethod);

    HarmonyPatches? GetPatchInfo(StackFrame frame, IModuleProvider moduleProvider, ILoaderPluginProvider loaderPluginProvider);

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