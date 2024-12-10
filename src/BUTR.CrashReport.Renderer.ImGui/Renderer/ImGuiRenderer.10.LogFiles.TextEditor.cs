#if TEXT_EDITOR
using BUTR.CrashReport.ImGui.Enums;
using BUTR.CrashReport.Models;
using BUTR.CrashReport.Renderer.ImGui.Syntax;

using ImGuiColorTextEditNet;

namespace BUTR.CrashReport.Renderer.ImGui.Renderer;

partial class ImGuiRenderer
{
    // ReSharper disable once HeapView.ObjectAllocation
    protected static readonly string[] _logLevelNamesUtf16 =
    [
        "   ", // None
        "VRB", // Verbose
        "DBG", // Debug
        "INF", // Information
        "WRN", // Warning
        "ERR", // Error
        "FTL", // Fatal
    ];
}

partial class ImGuiRenderer<TImGuiIORef, TImGuiViewportRef, TImDrawListRef, TImGuiStyleRef, TColorsRangeAccessorRef, TImGuiListClipperRef>
{
    private static ColorPaletteIndex LogPalette(bool isDarkTheme, TColorsRangeAccessorRef colors) => CodeParser.BasePalette(isDarkTheme, colors).With(new()
    {
        [LogColorPalette.Date] = colors[ImGuiCol.Text],
        [LogColorPalette.Application] = colors[ImGuiCol.Text],
        [LogColorPalette.Type] = colors[ImGuiCol.Text],
        [LogColorPalette.Message] = colors[ImGuiCol.Text],
        [LogColorPalette.Debug] = Debug,
        [LogColorPalette.Info] = Info,
        [LogColorPalette.Warn] = Warn,
        [LogColorPalette.Error] = Error,
        [LogColorPalette.Fatal] = Fatal,
    });

    private TextEditor<LogColorPalette, TImGuiIORef, TImDrawListRef, TImGuiStyleRef, TColorsRangeAccessorRef, TImGuiListClipperRef>[] _logsEditor = [];

    private void InitializeLogFilesTextEditor()
    {
        _logsEditor = new TextEditor<LogColorPalette, TImGuiIORef, TImDrawListRef, TImGuiStyleRef, TColorsRangeAccessorRef, TImGuiListClipperRef>[_logSources.Count];
        for (var i = 0; i < _logSources.Count; i++)
        {
            var logSource = _logSources[i];

            var hasDates = _logSourceHasDates[i];
            var longestApplicationLength = _logSourceMaxApplicationLengths[i];
            var hasType = _logSourceHasType[i];
            var longestTypeLength = _logSourceMaxTypeLengths[i];

            _logsEditor[i] = new TextEditor<LogColorPalette, TImGuiIORef, TImDrawListRef, TImGuiStyleRef, TColorsRangeAccessorRef, TImGuiListClipperRef>(_imgui, _imGuiWithImGuiIO, _imGuiWithImDrawList, _imGuiWithImGuiStyle, _imGuiWithImGuiListClipper);
            for (var j = 0; j < logSource.Logs.Count; j++)
            {
                var log = logSource.Logs[j];

                var sb = _logsEditor[i].CreateGlyphBuilder();
                if (hasDates)
                {
                    sb.Append(log.Date.ToString("O"));
                    sb.Append(" [");
                }
                else
                {
                    sb.Append("[");
                }
                sb.Append(log.Application);
                sb.Append("]");
                sb.Append(' ', longestApplicationLength - log.Application.Length);
                sb.Append(" [");
                if (hasType)
                {
                    sb.Append(log.Type);
                    sb.Append("]");
                    sb.Append(' ', longestTypeLength - log.Type.Length);
                    sb.Append(" [");
                }
                var color = log.Level switch
                {
                    LogLevel.Fatal => LogColorPalette.Fatal,
                    LogLevel.Error => LogColorPalette.Error,
                    LogLevel.Warning => LogColorPalette.Warn,
                    LogLevel.Information => LogColorPalette.Info,
                    LogLevel.Debug => LogColorPalette.Debug,
                    LogLevel.Verbose => LogColorPalette.Debug,
                    _ => ColorPalette.Default,
                };
                sb.Append(_logLevelNamesUtf16[(int) log.Level], color);
                sb.Append("]: ");
                sb.Append(log.Message);
                if (j < logSource.Logs.Count - 1)
                    sb.AppendLine();

                _logsEditor[i].AddGlyphs(sb.AsSpan());
            }
        }

        _onDarkModeChanged += isDarkTheme =>
        {
            _imGuiWithImGuiStyle.GetStyle(out var style);
            style.GetColors(out var colors);
            for (var i = 0; i < _logsEditor.Length; i++)
                _logsEditor[i].SetPalette(LogPalette(isDarkTheme, colors));
        };
    }

    private void RenderLogFileTextEditor(int logSourceIdx, LogSourceModel logSource)
    {
        _logsEditor[logSourceIdx].Render(logSource.Name);
    }
}
#endif