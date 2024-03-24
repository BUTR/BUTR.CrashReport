using BUTR.CrashReport.Models;

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

    /// <summary>
    /// Returns the original method for a given patch method.
    /// </summary>
    /// <param name="replacement">The patch method</param>
    MethodBase? GetOriginalMethod(MethodInfo replacement);

    /// <summary>
    /// Returns the method from a stackframe.
    /// </summary>
    MethodBase? GetMethodFromStackframe(StackFrame frame);
}