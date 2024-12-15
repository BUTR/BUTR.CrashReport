using BUTR.CrashReport.Models;
using BUTR.CrashReport.Renderer.Html;

using System.Runtime.InteropServices.JavaScript;

namespace BUTR.CrashReport.Renderer.ImGui.WASM;

internal sealed partial class CrashReportRendererUtilities : ICrashReportRendererUtilities
{
    [JSImport("saveFile", "interop")]
    private static partial void SaveFile(byte[] data, string fileName);

    [JSImport("writeClipboard", "interop")]
    private static partial void WriteClipboard(string data);

    [JSImport("isDarkMode", "interop")]
    private static partial bool IsDarkMode();

    public bool IsDefaultDarkMode => IsDarkMode();

    private readonly CrashReportRendererCapabilities _capabilities =
#if WINDOWS
        CrashReportRendererCapabilities.Dialogs |
#endif
        CrashReportRendererCapabilities.CopyAsHtml |
        CrashReportRendererCapabilities.Dialogs;

    public CrashReportRendererCapabilities Capabilities => _capabilities;

    public CrashReportRendererUtilities(CrashReportModel model, LogSourceModel[] logs)
    {
        if (!string.IsNullOrEmpty(model.Metadata.LoaderPluginProviderName))
        {
            _capabilities |= CrashReportRendererCapabilities.PluginLoader;
        }

        if (logs.Length > 0)
        {
            _capabilities |= CrashReportRendererCapabilities.Logs;
        }
    }

    public void Upload(CrashReportModel crashReport, ICollection<LogSourceModel> logSources) { }

    public void CopyAsHtml(CrashReportModel crashReport, ICollection<LogSourceModel> logSources)
    {
        var reportAsHtml = CrashReportHtml.Build(crashReport, logSources);
        WriteClipboard(reportAsHtml);
    }

    public void SaveAsHtml(CrashReportModel crashReport, ICollection<LogSourceModel> logSources, bool addMiniDump, bool addLatestSave, bool addScreenshots, Stream stream)
    {
        var reportAsHtml = CrashReportHtml.Build(crashReport, logSources);
        CreatorHtml.Create(crashReport, reportAsHtml, stream);
        SaveFile(((MemoryStream) stream).ToArray(), "crashreport.html");
    }

    public void SaveAsZip(CrashReportModel crashReport, ICollection<LogSourceModel> logSources, Stream stream)
    {
        CreatorZip.Create(crashReport, logSources, stream);
        SaveFile(((MemoryStream) stream).ToArray(), "crashreport.zip");
    }

    public Stream SaveFileDialog(string filter, string defaultPath)
    {
        return new MemoryStream();
    }
}