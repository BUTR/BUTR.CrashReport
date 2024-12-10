using System.Reflection;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents a method.
/// </summary>
public record MethodEntry
{
    /// <summary>
    /// The actual method.
    /// </summary>
    public required MethodBase Method { get; set; }

    /// <summary>
    /// <inheritdoc cref="AssemblyIdModel"/>
    /// </summary>
    public required AssemblyIdModel? AssemblyId { get; set; }

    /// <summary>
    /// <inheritdoc cref="StacktraceEntry.ModuleInfo"/>
    /// </summary>
    /// <returns><inheritdoc cref="StacktraceEntry.ModuleInfo"/></returns>
    public required IModuleInfo? ModuleInfo { get; set; }

    /// <summary>
    /// <inheritdoc cref="StacktraceEntry.LoaderPluginInfo"/>
    /// </summary>
    /// <returns><inheritdoc cref="StacktraceEntry.LoaderPluginInfo"/></returns>
    public required ILoaderPluginInfo? LoaderPluginInfo { get; set; }
}