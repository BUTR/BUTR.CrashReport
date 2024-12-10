using BUTR.CrashReport.ImGui.Enums;
using BUTR.CrashReport.ImGui.Extensions;

namespace BUTR.CrashReport.Renderer.ImGui.Renderer;

partial class ImGuiRenderer<TImGuiIORef, TImGuiViewportRef, TImDrawListRef, TImGuiStyleRef, TColorsRangeAccessorRef, TImGuiListClipperRef>
{
    private int[] _logSourceMaxApplicationLengths = [];
    private int[] _logSourceMaxTypeLengths = [];
    private bool[] _logSourceHasDates = [];
    private bool[] _logSourceHasType = [];

    private void InitializeLogFiles()
    {
        _logSourceMaxApplicationLengths = new int[_logSources.Count];
        for (var i = 0; i < _logSources.Count; i++)
        {
            if (_logSources[i].Logs.Count == 0) _logSourceMaxApplicationLengths[i] = 0;
            else _logSourceMaxApplicationLengths[i] = _logSources[i].Logs.Select(x => x.Application.Length).Max();
        }

        _logSourceMaxTypeLengths = new int[_logSources.Count];
        for (var i = 0; i < _logSources.Count; i++)
        {
            if (_logSources[i].Logs.Count == 0) _logSourceMaxTypeLengths[i] = 0;
            else _logSourceMaxTypeLengths[i] = _logSources[i].Logs.Select(x => x.Type.Length).Max();
        }

        _logSourceHasDates = new bool[_logSources.Count];
        for (var i = 0; i < _logSources.Count; i++)
        {
            if (_logSources[i].Logs.Count == 0) _logSourceHasDates[i] = false;
            else _logSourceHasDates[i] = _logSources[i].Logs.All(x => x.Date != DateTimeOffset.MinValue);
        }

        _logSourceHasType = new bool[_logSources.Count];
        for (var i = 0; i < _logSources.Count; i++)
        {
            if (_logSources[i].Logs.Count == 0) _logSourceHasType[i] = false;
            else _logSourceHasType[i] = _logSources[i].Logs.All(x => !string.IsNullOrEmpty(x.Type));
        }

#if TEXT_EDITOR
        InitializeLogFilesTextEditor();
#endif
    }

    private void RenderLogFiles()
    {
        for (var i = 0; i < _logSources.Count; i++)
        {
            var logSource = _logSources[i];

            _imgui.Unindent();

            if (_imgui.TreeNode(logSource.Name, ImGuiTreeNodeFlags.DefaultOpen))
            {
                if (logSource.Logs.Count == 0) continue;

                _imgui.Unindent();

#if TEXT_EDITOR
                RenderLogFileTextEditor(i, logSource);
#else
                RenderLogFileTextBox(i, logSource);
#endif

                _imgui.Indent();

                _imgui.TreePop();
            }

#if !TEXT_EDITOR
            RenderLogsFileTextBoxContextMenu(logSource);
#endif

            _imgui.Indent();
        }
    }
}