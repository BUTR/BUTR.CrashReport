using BUTR.CrashReport.Models;

using System.Collections.Generic;

namespace BUTR.CrashReport.Renderer.WinForms;

public interface ICrashReportRendererUtilities
{
    void Upload(CrashReportModel crashReport, ICollection<LogSource> logSources);

    string CopyAsHtml(CrashReportModel crashReport, ICollection<LogSource> logSources);

    void SaveCrashReportAsHtml(CrashReportModel crashReport, ICollection<LogSource> logSources, bool addMiniDump, bool addLatestSave, bool addScreenshots);
}