using BUTR.CrashReport.Models;

using System.Linq;

namespace BUTR.CrashReport.ContextualAnalysis;

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

    /// <inheritdoc />
    public bool Equals(CrashMatchCriteria? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return ExceptionType == other.ExceptionType &&
               InvariantMessageContains == other.InvariantMessageContains &&
               Source == other.Source &&
               HResult == other.HResult &&
               StacktracePatterns.SequenceEqual(other.StacktracePatterns) &&
               SourceModuleId == other.SourceModuleId &&
               AvailableModules.SequenceEqual(other.AvailableModules) &&
               SourceLoaderPluginId == other.SourceLoaderPluginId &&
               AvailableLoaderPlugins.SequenceEqual(other.AvailableLoaderPlugins);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = (ExceptionType != null ? ExceptionType.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (InvariantMessageContains != null ? InvariantMessageContains.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (Source != null ? Source.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (HResult != null ? HResult.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (StacktracePatterns != null ? StacktracePatterns.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (SourceModuleId != null ? SourceModuleId.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (AvailableModules != null ? AvailableModules.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (SourceLoaderPluginId != null ? SourceLoaderPluginId.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (AvailableLoaderPlugins != null ? AvailableLoaderPlugins.GetHashCode() : 0);
            return hashCode;
        }
    }
}