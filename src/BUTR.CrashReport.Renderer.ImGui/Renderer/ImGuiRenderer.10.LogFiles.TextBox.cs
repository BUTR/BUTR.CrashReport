#if !TEXT_EDITOR
using BUTR.CrashReport.ImGui.Enums;
using BUTR.CrashReport.ImGui.Extensions;
using BUTR.CrashReport.Memory;
using BUTR.CrashReport.Models;

using Cysharp.Text;

namespace BUTR.CrashReport.Renderer.ImGui.Renderer;

partial class ImGuiRenderer
{
    // ReSharper disable once HeapView.ObjectAllocation
    protected static readonly LiteralSpan<byte>[] _logLevelNamesUtf8 =
    [
        "   \0"u8, // None
        "VRB\0"u8, // Verbose
        "DBG\0"u8, // Debug
        "INF\0"u8, // Information
        "WRN\0"u8, // Warning
        "ERR\0"u8, // Error
        "FTL\0"u8, // Fatal
    ];
}

partial class ImGuiRenderer<TImGuiIORef, TImGuiViewportRef, TImDrawListRef, TImGuiStyleRef, TColorsRangeAccessorRef, TImGuiListClipperRef>
{
    private void RenderLogFileTextBox(int logSourceIdx, LogSourceModel logSource)
    {
        _imGuiWithImGuiStyle.GetStyle(out var style);
        style.GetColors(out var colors);

        var longestApplicationLength = _logSourceMaxApplicationLengths[logSourceIdx];
        var longestTypeLength = _logSourceMaxTypeLengths[logSourceIdx];
        var hasDates = _logSourceHasDates[logSourceIdx];
        var hasType = _logSourceHasType[logSourceIdx];

        _imGuiWithImGuiListClipper.CreateImGuiListClipper(out var clipper);
        using var _ = clipper;

        clipper.Begin(logSource.Logs.Count, _imgui.GetTextLineHeightWithSpacing());

        while (clipper.Step())
        {
            for (var j = clipper.DisplayStart; j < clipper.DisplayEnd; ++j)
            {
                var logEntry = logSource.Logs[j];
                var toAppendApplication = longestApplicationLength - logEntry.Application.Length;
                var toAppendType = longestTypeLength - logEntry.Type.Length;

                var date = logEntry.Date;
                var color = logEntry.Level switch
                {
                    LogLevel.Fatal => Fatal,
                    LogLevel.Error => Error,
                    LogLevel.Warning => Warn,
                    LogLevel.Information => Info,
                    LogLevel.Debug => Debug,
                    LogLevel.Verbose => Debug,
                    _ => colors[ImGuiCol.Text],
                };
                var level = Clamp(logEntry.Level, LogLevel.None, LogLevel.Fatal);

                if (hasDates)
                {
                    _imgui.Text(ref date);
                    _imgui.SameLine();
                    _imgui.Text(" [\0"u8);
                }
                else
                {
                    _imgui.Text("[\0"u8);
                }

                _imgui.SameLine();
                _imgui.Text(logEntry.Application);
                _imgui.SameLine();
                _imgui.Text("]\0"u8);
                _imgui.SameLine();
                _imgui.PadRight(toAppendApplication);
                if (hasType)
                {
                    _imgui.Text(" [\0"u8);
                    _imgui.SameLine();
                    _imgui.Text(logEntry.Type);
                    _imgui.SameLine();
                    _imgui.Text("]\0"u8);
                    _imgui.SameLine();
                }
                _imgui.PadRight(toAppendType);
                _imgui.Text(" [\0"u8);
                _imgui.SameLine();
                _imgui.TextColored(_logLevelNamesUtf8[level], in color);
                _imgui.SameLine();
                _imgui.Text("]: \0"u8);
                _imgui.SameLine();
                _imgui.TextWrapped(logEntry.Message);
            }
        }

        clipper.End();
    }

    private void RenderLogsFileTextBoxContextMenu(LogSourceModel logSource)
    {
        _imgui.PushId(logSource.Name);
        if (_imgui.BeginPopupContextWindow("Context Menu\0"u8))
        {
            if (_imgui.MenuItem("Copy All\0"u8))
            {
                CopyLogSourceToClipboard(logSource);
            }

            _imgui.EndPopup();
        }
        _imgui.PopId();
    }

    private void CopyLogSourceToClipboard(LogSourceModel logSource)
    {
        using var sb = ZString.CreateUtf8StringBuilder();
        for (var i = 0; i < logSource.Logs.Count; i++)
        {
            var log = logSource.Logs[i];
            sb.Append(log.Date.ToString("O"));
            sb.Append(" [");
            sb.Append(log.Application);
            sb.Append("]");
            sb.Append(" [");
            sb.Append(log.Type);
            sb.Append("]");
            sb.Append(" [");
            sb.Append(log.Level);
            sb.Append("]: ");
            sb.Append(log.Message);
            sb.AppendLine();
        }
        _imgui.SetClipboardText(sb.AsSpan());
    }
}
#endif