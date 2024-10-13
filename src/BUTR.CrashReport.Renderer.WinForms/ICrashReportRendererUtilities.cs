using System;
using BUTR.CrashReport.Models;

using System.Collections.Generic;

namespace BUTR.CrashReport.Renderer.WinForms;

[Flags]
public enum CrashReportRendererCapabilities
{
    None = 0,
    SaveAsHtml = 1 << 0,
    CopyAsHtml = 1 << 1,
    Upload = 1 << 2,
    HasSaveFiles = 1 << 3,
    HasScreenshots = 1 << 4,
    HasMiniDump = 1 << 5,
}

public interface ICrashReportRendererUtilities
{
    CrashReportRendererCapabilities Capabilities { get; }

    void Upload(CrashReportModel crashReport, ICollection<LogSourceModel> logSources);

    string CopyAsHtml(CrashReportModel crashReport, ICollection<LogSourceModel> logSources);

    void SaveCrashReportAsHtml(CrashReportModel crashReport, ICollection<LogSourceModel> logSources, bool addMiniDump, bool addLatestSave, bool addScreenshots);
}