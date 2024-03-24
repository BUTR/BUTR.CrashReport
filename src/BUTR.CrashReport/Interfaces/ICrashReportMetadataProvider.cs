using BUTR.CrashReport.Models;

namespace BUTR.CrashReport.Interfaces;

/// <summary>
/// Provides metadata for a crash report.
/// </summary>
public interface ICrashReportMetadataProvider
{
    /// <summary>
    /// Gets the metadata for a crash report.
    /// </summary>
    CrashReportMetadataModel GetCrashReportMetadataModel(CrashReportInfo crashReport);
}