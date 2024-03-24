using BUTR.CrashReport.Models;

using System;
using System.Collections.Generic;

namespace BUTR.CrashReport.Interfaces;

/// <summary>
/// Provides the implementation for getting the module information.
/// </summary>
public interface IModuleProvider
{
    /// <summary>
    /// Provides the implementation for getting the loaded modules in the process.
    /// </summary>
    /// <returns></returns>
    ICollection<IModuleInfo> GetLoadedModules();

    /// <summary>
    /// Provides the implementation for getting the module based on the type.
    /// </summary>
    IModuleInfo? GetModuleByType(Type? type);
}