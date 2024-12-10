using BUTR.CrashReport.ImGui;
using BUTR.CrashReport.ImGui.Enums;
using BUTR.CrashReport.ImGui.Extensions;
using BUTR.CrashReport.ImGui.Structures;

using System;

namespace ImGuiColorTextEditNet;

internal class StandardKeyboardInput<TImGuiIORef> : ITextEditorKeyboardInput
    where TImGuiIORef : IImGuiIO
{
    private readonly IImGui _imGui;
    private readonly IImGuiWithImGuiIO<TImGuiIORef> _imGuiWithImGuiIO;
    private readonly ITextEditor _editor;
    public StandardKeyboardInput(IImGui imGui, IImGuiWithImGuiIO<TImGuiIORef> imGuiWithImGuiIO, ITextEditor editor)
    {
        _imGui = imGui;
        _imGuiWithImGuiIO = imGuiWithImGuiIO;
        _editor = editor ?? throw new ArgumentNullException(nameof(editor));
    }

    public void HandleKeyboardInputs()
    {
        if (!_imGui.IsWindowFocused())
            return;

        _imGuiWithImGuiIO.GetIO(out var io);
        var shift = _imGui.IsKeyDown(ImGuiKey.LeftShift) || _imGui.IsKeyDown(ImGuiKey.RightShift);
        var ctrl = io.ConfigMacOSXBehaviors
            ? _imGui.IsKeyDown(ImGuiKey.LeftSuper) || _imGui.IsKeyDown(ImGuiKey.RightSuper)
            : _imGui.IsKeyDown(ImGuiKey.LeftCtrl) || _imGui.IsKeyDown(ImGuiKey.RightCtrl);

        io.WantCaptureKeyboard = true;
        io.WantTextInput = true;

        switch (ctrl, shift)
        {
            case (false, _) when _imGui.IsKeyPressed(ImGuiKey.UpArrow): _editor.Movement.MoveUp(1, shift); break;
            case (false, _) when _imGui.IsKeyPressed(ImGuiKey.DownArrow): _editor.Movement.MoveDown(1, shift); break;
            case (_, _) when _imGui.IsKeyPressed(ImGuiKey.LeftArrow): _editor.Movement.MoveLeft(1, shift, ctrl); break;
            case (_, _) when _imGui.IsKeyPressed(ImGuiKey.RightArrow): _editor.Movement.MoveRight(1, shift, ctrl); break;
            case (_, _) when _imGui.IsKeyPressed(ImGuiKey.PageUp): _editor.Movement.MoveUp(_editor.Renderer.PageSize - 4, shift); break;
            case (_, _) when _imGui.IsKeyPressed(ImGuiKey.PageDown): _editor.Movement.MoveDown(_editor.Renderer.PageSize - 4, shift); break;
            case (true, _) when _imGui.IsKeyPressed(ImGuiKey.Home): _editor.Movement.MoveToStartOfFile(shift); break;
            case (true, _) when _imGui.IsKeyPressed(ImGuiKey.End): _editor.Movement.MoveToEndOfFile(shift); break;
            case (false, _) when _imGui.IsKeyPressed(ImGuiKey.Home): _editor.Movement.MoveToStartOfLine(shift); break;
            case (false, _) when _imGui.IsKeyPressed(ImGuiKey.End): _editor.Movement.MoveToEndOfLine(shift); break;
            case (true, false) when _imGui.IsKeyPressed(ImGuiKey.A): _editor.Selection.SelectAll(); break;
            case (true, false) when _imGui.IsKeyPressed(ImGuiKey.C):
            {
                using var memory = _editor.Selection.GetSelectedText();
                _imGui.SetClipboardText(memory.Memory.Span);
                break;
            }
        }
    }
}