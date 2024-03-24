using BUTR.CrashReport.Models;

using System.Collections.Generic;

namespace BUTR.CrashReport.Interfaces;

/// <summary>
/// Represents a filter that can be used to filter out irrelevant stack trace frames from a crash report.
/// </summary>
public interface IStacktraceFilter
{
    /// <summary>
    /// Filters out stack trace frames that are not relevant to the crash report.
    /// </summary>
    IEnumerable<StacktraceEntry> Filter(ICollection<StacktraceEntry> stacktraceEntries);
}