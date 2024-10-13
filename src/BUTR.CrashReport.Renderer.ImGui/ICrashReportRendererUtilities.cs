using System;
using BUTR.CrashReport.Models;

using System.Collections.Generic;

namespace BUTR.CrashReport.Renderer.ImGui;

[Flags]
public enum CrashReportRendererCapabilities
{
    None = 0,
    SaveAsHtml = 1 << 0,
    SaveAsZip = 1 << 1,
    CopyAsHtml = 1 << 2,
    Upload = 1 << 3,
    HasSaveFiles = 1 << 4,
    HasScreenshots = 1 << 5,
    HasMiniDump = 1 << 6,
    PluginLoader = 1 << 7,
    Logs = 1 << 8,
}

public interface ICrashReportRendererUtilities
{
    CrashReportRendererCapabilities Capabilities { get; }
    
    IEnumerable<string> GetNativeLibrariesFolderPath();

    void Upload(CrashReportModel crashReport, ICollection<LogSourceModel> logSources);

    void CopyAsHtml(CrashReportModel crashReport, ICollection<LogSourceModel> logSources);

    // TODO: Right now we rely on the implementation to provide a dialog and do the save
    // We should instead do the dialog within ImGui and then provide here the path/stream of where to save the file
    void SaveAsHtml(CrashReportModel crashReport, ICollection<LogSourceModel> logSources, bool addMiniDump, bool addLatestSave, bool addScreenshots);
    void SaveAsZip(CrashReportModel crashReport, ICollection<LogSourceModel> logSources);
}