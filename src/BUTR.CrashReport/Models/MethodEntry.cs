using System.Reflection;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents a method.
/// </summary>
public abstract record MethodEntry
{
    /// <summary>
    /// The Harmony patch method.
    /// </summary>
    public required MethodBase Method { get; set; }

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

    /// <summary>
    /// <inheritdoc cref="MethodSimpleModel.ILInstructions"/>
    /// </summary>
    /// <returns><inheritdoc cref="MethodSimpleModel.ILInstructions"/></returns>
    public required string[] ILInstructions { get; set; }

    /// <summary>
    /// <inheritdoc cref="MethodSimpleModel.CSharpILMixedInstructions"/>
    /// </summary>
    /// <returns><inheritdoc cref="MethodSimpleModel.CSharpILMixedInstructions"/></returns>
    public required string[] CSharpILMixedInstructions { get; set; }

    /// <summary>
    /// <inheritdoc cref="MethodSimpleModel.CSharpInstructions"/>
    /// </summary>
    /// <returns><inheritdoc cref="MethodSimpleModel.CSharpInstructions"/></returns>
    public required string[] CSharpInstructions { get; set; }
}