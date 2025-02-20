namespace BUTR.CrashReport.Models.Diagnostics;

/// <summary>
/// Represents criteria used to match crash details for diagnosis.
/// </summary>
public sealed record CrashMatchCriteria
{
    /// <summary>
    /// The type of exception that caused the crash, if available.
    /// </summary>
    public string? ExceptionType { get; set; }

    /// <summary>
    /// A message or substring that may appear in the invariant part of the exception message.
    /// </summary>
    public string? InvariantMessageContains { get; set; }

    /// <summary>
    /// The source of the exception, such as the assembly or module name.
    /// </summary>
    public string? Source { get; set; }

    /// <summary>
    /// The HRESULT error code associated with the exception, if available.
    /// </summary>
    public int? HResult { get; set; }

    /// <summary>
    /// A collection of stack trace patterns used to identify the crash cause.
    /// </summary>
    public CrashStacktracePattern[] StacktracePatterns { get; set; } = [];

    /// <summary>
    /// <inheritdoc cref="ModuleModel.Id"/> Is associated with the source of the exception.
    /// </summary>
    /// <returns><inheritdoc cref="ModuleModel.Id"/></returns>
    public string? SourceModuleId { get; set; }

    /// <summary>
    /// The list of available modules.
    /// </summary>
    public CrashModuleIdOrPluginPattern[] AvailableModules { get; set; } = [];

    /// <summary>
    /// <inheritdoc cref="LoaderPluginModel.Id"/> Is associated with the source of the exception.
    /// </summary>
    /// <returns><inheritdoc cref="LoaderPluginModel.Id"/></returns>
    public string? SourceLoaderPluginId { get; set; }

    /// <summary>
    /// The list of available loader plugins.
    /// </summary>
    public CrashModuleIdOrPluginPattern[] AvailableLoaderPlugins { get; set; } = [];
}