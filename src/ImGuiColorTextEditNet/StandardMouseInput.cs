using BUTR.CrashReport.ImGui;
using BUTR.CrashReport.ImGui.Enums;
using BUTR.CrashReport.ImGui.Structures;

using System;

namespace ImGuiColorTextEditNet;

internal class StandardMouseInput<TImGuiIORef> : ITextEditorMouseInput
    where TImGuiIORef : IImGuiIO
{
    private readonly IImGui _imGui;
    private readonly IImGuiWithImGuiIO<TImGuiIORef> _imGuiWithImGuiIO;
    private readonly ITextEditor _editor;

    public StandardMouseInput(IImGui imGui, IImGuiWithImGuiIO<TImGuiIORef> imGuiWithImGuiIO, ITextEditor editor)
    {
        _imGui = imGui;
        _imGuiWithImGuiIO = imGuiWithImGuiIO;
        _editor = editor ?? throw new ArgumentNullException(nameof(editor));
    }

    public void HandleMouseInputs()
    {
        _imGuiWithImGuiIO.GetIO(out var io);
        var shift = io.KeyShift;
        var ctrl = io.ConfigMacOSXBehaviors ? io.KeySuper : io.KeyCtrl;
        var alt = io.ConfigMacOSXBehaviors ? io.KeyCtrl : io.KeyAlt;

        if (!_imGui.IsWindowHovered())
            return;

        _imGui.SetMouseCursor(ImGuiMouseCursor.TextInput);

        if (shift || alt)
            return;

        var click = _imGui.IsMouseClicked(0);
        var doubleClick = _imGui.IsMouseDoubleClicked(0);

        /* Left mouse button double click */
        if (doubleClick)
        {
            if (!ctrl)
            {
                _imGui.GetMousePos(out var mousePos);
                _editor.Renderer.ScreenPosToCoordinates(in mousePos, out _editor.Selection.Cursor);
                _editor.Selection.InteractiveStart = _editor.Selection.Cursor;
                _editor.Selection.InteractiveEnd = _editor.Selection.Cursor;
                _editor.Selection.Mode = _editor.Selection.Mode == SelectionMode.Line ? SelectionMode.Normal : SelectionMode.Word;
                _editor.Selection.Select(in _editor.Selection.InteractiveStart, in _editor.Selection.InteractiveEnd, _editor.Selection.Mode);
            }
        }
        else if (click) /* Left mouse button click */
        {
            _imGui.GetMousePos(out var mousePos);
            _editor.Renderer.ScreenPosToCoordinates(in mousePos, out _editor.Selection.Cursor);
            _editor.Selection.InteractiveStart = _editor.Selection.Cursor;
            _editor.Selection.InteractiveEnd = _editor.Selection.Cursor;
            _editor.Selection.Mode = ctrl ? SelectionMode.Word : SelectionMode.Normal;
            _editor.Selection.Select(in _editor.Selection.Cursor, in _editor.Selection.Cursor, _editor.Selection.Mode);
        }
        else if (_imGui.IsMouseDragging(0) && _imGui.IsMouseDown(0)) // Mouse left button dragging (=> update selection)
        {
            io.WantCaptureMouse = true;
            _imGui.GetMousePos(out var mousePos);
            _editor.Renderer.ScreenPosToCoordinates(in mousePos, out _editor.Selection.Cursor);
            if (_editor.Selection.Cursor < _editor.Selection.InteractiveStart)
            {
                _editor.Selection.InteractiveEnd = _editor.Selection.InteractiveStart;
                _editor.Selection.Select(in _editor.Selection.Cursor, in _editor.Selection.InteractiveStart, _editor.Selection.Mode);
            }
            else
            {
                _editor.Selection.InteractiveEnd = _editor.Selection.Cursor;
                _editor.Selection.Select(in _editor.Selection.InteractiveStart, in _editor.Selection.Cursor, _editor.Selection.Mode);
            }
            //_editor.Selection.InteractiveEnd = _editor.Selection.Cursor;
            //_editor.Selection.Select(in _editor.Selection.InteractiveStart, in _editor.Selection.InteractiveEnd, _editor.Selection.Mode);
        }
    }
}