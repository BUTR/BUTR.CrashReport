using BUTR.CrashReport.Models;

using System;
using System.Collections.Generic;

namespace BUTR.CrashReport.Renderer.WinForms;

[Flags]
public enum CrashReportRendererCapabilities
{
    None = 0,
    HasSaveFiles = 1 << 1,
    HasScreenshots = 1 << 2,
    HasMiniDump = 1 << 3,
    Upload = 1 << 4,
}

public interface ICrashReportRendererUtilities
{
    CrashReportRendererCapabilities Capabilities { get; }

    void Upload(CrashReportModel crashReport, ICollection<LogSourceModel> logSources);

    string CopyAsHtml(CrashReportModel crashReport, ICollection<LogSourceModel> logSources);

    void SaveAsHtml(CrashReportModel crashReport, ICollection<LogSourceModel> logSources, bool addMiniDump, bool addLatestSave, bool addScreenshots);
    void SaveAsZip(CrashReportModel crashReport, ICollection<LogSourceModel> logSources);
}