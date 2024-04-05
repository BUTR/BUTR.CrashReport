using BUTR.CrashReport.Models;

using System.Collections.Generic;

namespace BUTR.CrashReport.Renderer;

public interface ICrashReportRendererUtilities
{
    IEnumerable<string> GetNativeLibrariesFolderPath();

    void Upload(CrashReportModel crashReport, ICollection<LogSource> logSources);

    void CopyAsHtml(CrashReportModel crashReport, ICollection<LogSource> logSources);

    // TODO: Right now we rely on the implementation to provide a dialog and do the save
    // We should instead do the dialog within ImGui and then provide here the path/stream of where to save the file
    void SaveCrashReportAsHtml(CrashReportModel crashReport, ICollection<LogSource> logSources, bool addMiniDump, bool addLatestSave, bool addScreenshots);
    void SaveCrashReportAsZip(CrashReportModel crashReport, ICollection<LogSource> logSources);
}