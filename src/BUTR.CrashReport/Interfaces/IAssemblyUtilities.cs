using BUTR.CrashReport.Models;

using System.Collections.Generic;
using System.Reflection;

namespace BUTR.CrashReport.Interfaces;

/// <summary>
/// Provides functionality related to assemblies.
/// </summary>
public interface IAssemblyUtilities
{
    /// <summary>
    /// Provides the implementation for getting the assemblies present in the process.
    /// </summary>
    IEnumerable<Assembly> Assemblies();

    /// <summary>
    /// Gets the module for the assembly if there is one
    /// </summary>
    IModuleInfo? GetAssemblyModule(CrashReportInfo crashReport, Assembly assembly);

    /// <summary>
    /// Gets the module for the assembly if there is one
    /// </summary>
    ILoaderPluginInfo? GetAssemblyPlugin(CrashReportInfo crashReport, Assembly assembly);

    /// <summary>
    /// Gets the type of the assembly
    /// </summary>
    AssemblyModelType GetAssemblyType(AssemblyModelType type, CrashReportInfo crashReport, Assembly assembly);
}