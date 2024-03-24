using BUTR.CrashReport.Models;

using System;
using System.Collections.Generic;

namespace BUTR.CrashReport.Interfaces;

/// <summary>
/// Represents the loader plugin information.
/// </summary>
public interface ILoaderPluginProvider
{
    /// <summary>
    /// Provides the implementation for getting the loaded loader plugin in the process.
    /// </summary>
    ICollection<ILoaderPluginInfo> GetLoadedLoaderPlugins();

    /// <summary>
    /// Provides the implementation for getting the loader plugin based on the type.
    /// </summary>
    ILoaderPluginInfo? GetLoaderPluginByType(Type? type);
}