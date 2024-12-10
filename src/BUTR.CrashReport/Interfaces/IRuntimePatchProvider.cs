using BUTR.CrashReport.Models;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace BUTR.CrashReport.Interfaces;

/// <summary>
/// Provides information about runtime patches.
/// </summary>
public interface IRuntimePatchProvider
{
    /// <summary>
    /// Gets all patches.
    /// </summary>
    IList<ManagedRuntimePatch> GetAllManagedPatches();

    /// <summary>
    /// Gets all patches.
    /// </summary>
    IList<NativeRuntimePatch> GetAllNativePatches();

    /// <summary>
    /// Gets patches for the specified method.
    /// </summary>
    MethodWithRuntimePatches? GetPatchInfo(MethodBase method);
}