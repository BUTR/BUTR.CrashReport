using BUTR.CrashReport.Models;

namespace BUTR.CrashReport.Renderer.ImGui;

public interface ICrashReportRendererUtilities
{
    bool IsDefaultDarkMode { get; }

    CrashReportRendererCapabilities Capabilities { get; }

    void Upload(CrashReportModel crashReport, ICollection<LogSourceModel> logSources);

    void CopyAsHtml(CrashReportModel crashReport, ICollection<LogSourceModel> logSources);

    void SaveAsHtml(CrashReportModel crashReport, ICollection<LogSourceModel> logSources, bool addMiniDump, bool addLatestSave, bool addScreenshots, Stream stream);
    void SaveAsZip(CrashReportModel crashReport, ICollection<LogSourceModel> logSources, Stream stream);

    Stream SaveFileDialog(string filter, string defaultPath);
}