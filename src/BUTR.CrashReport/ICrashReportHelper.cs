using System;
using System.Collections.Generic;
using System.Reflection;

namespace BUTR.CrashReport;

/// <summary>
/// Exposes the basic function needed for the Crash Report Creator
/// </summary>
public interface ICrashReportHelper
{
    /// <summary>
    /// Filters out stack trace frames that are not relevant to the crash report.
    /// </summary>
    /// <param name="stacktraceEntries"></param>
    /// <returns></returns>
    IEnumerable<StacktraceEntry> Filter(ICollection<StacktraceEntry> stacktraceEntries);

    /// <summary>
    /// Provides the implementation for getting the loaded modules in the process.
    /// </summary>
    /// <returns></returns>
    IEnumerable<IModuleInfo> GetLoadedModules();

    /// <summary>
    /// Provides the implementation for getting the module based on the type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    IModuleInfo? GetModuleByType(Type? type);

    /// <summary>
    /// Provides the implementation for getting the assemblies present in the process.
    /// </summary>
    /// <returns></returns>
    IEnumerable<Assembly> Assemblies();
}