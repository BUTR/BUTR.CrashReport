using ImGuiColorTextEditNet;

namespace BUTR.CrashReport.Renderer.ImGui.Syntax;

internal class LogColorPalette : ColorPalette
{
    public static LogColorPalette Date { get; } = new(nameof(Date));

    public static LogColorPalette Application { get; } = new(nameof(Application));

    public static LogColorPalette Type { get; } = new(nameof(Type));

    public static LogColorPalette Message { get; } = new(nameof(Message));

    public static LogColorPalette Debug { get; } = new(nameof(Debug));
    public static LogColorPalette Info { get; } = new(nameof(Info));
    public static LogColorPalette Warn { get; } = new(nameof(Warn));
    public static LogColorPalette Error { get; } = new(nameof(Error));
    public static LogColorPalette Fatal { get; } = new(nameof(Fatal));

    private LogColorPalette(string uniqueName) : base(uniqueName) { }
}