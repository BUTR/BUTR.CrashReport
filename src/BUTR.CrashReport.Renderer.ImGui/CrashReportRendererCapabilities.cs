namespace BUTR.CrashReport.Renderer.ImGui;

[Flags]
public enum CrashReportRendererCapabilities
{
    None = 0,
    HasSaveFiles = 1 << 1,
    HasScreenshots = 1 << 2,
    HasMiniDump = 1 << 3,
    CopyAsHtml = 1 << 4,
    Upload = 1 << 5,
    PluginLoader = 1 << 6,
    Logs = 1 << 7,
    Dialogs = 1 << 8,
    CloseAndContinue = 1 << 9,
}