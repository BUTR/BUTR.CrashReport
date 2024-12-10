using BUTR.CrashReport.ImGui.Utils;

using ImGuiNET;

using Silk.NET.Input;
using Silk.NET.Input.Extensions;
using Silk.NET.Maths;

using System.Drawing;
using System.Numerics;

namespace BUTR.CrashReport.Renderer.ImGui.Implementation.CImGui.Controller;

partial class ImGuiController
{
    private void WindowResized(Vector2D<int> size)
    {
        _windowsWidth = (uint) size.X;
        _windowsHeight = (uint) size.Y;
    }

    private void OnKeyEvent(IKeyboard keyboard, Key keycode, int scancode, bool down)
    {
        var imGuiKey = KeyToImGuiKey(keycode);
        if (imGuiKey != ImGuiKey.None)
        {
            _imgui.GetIO(out var io);
            io.AddKeyEvent(imGuiKey, down);
            io.SetKeyEventNativeData(imGuiKey, (int) keycode, scancode);
        }
    }

    private void OnKeyChar(IKeyboard keyboard, char c)
    {
        //_pressedChars.Add(c);
        _imgui.GetIO(out var io);
        io.AddInputCharacter(c);
    }

    private void MouseOnScroll(IMouse mouse, ScrollWheel data)
    {
        _imgui.GetIO(out var io);
        io.AddMouseWheelEvent(data.X, data.Y);
    }

    private void MouseOnMouseDown(IMouse mouse, MouseButton button)
    {
        if (button is < 0 or > (MouseButton) ImGuiMouseButton.COUNT)
            return;

        _imgui.GetIO(out var io);
        io.AddMouseButtonEvent((ImGuiMouseButton) button, true);
    }

    private void MouseOnMouseUp(IMouse mouse, MouseButton button)
    {
        if (button is < 0 or > (MouseButton) ImGuiMouseButton.COUNT)
            return;

        _imgui.GetIO(out var io);
        io.AddMouseButtonEvent((ImGuiMouseButton) button, false);
    }

    private void MouseOnMouseMove(IMouse mouse, Vector2 pos)
    {
        _imgui.GetIO(out var io);

        ref var mousePosBackup = ref io.MousePos;

        /*
        var focused = _glfw.GetWindowAttrib(_windowPtr, WindowAttributeGetter.Focused);
        if (!focused)
        {
            _io.MousePos.X = -float.MaxValue;
            _io.MousePos.Y = -float.MaxValue;
            return;
        }
        */

        if (io.WantSetMousePos)
        {
            mouse.Position = mousePosBackup;
        }
        else
        {
            io.MousePos.X = pos.X;
            io.MousePos.Y = pos.Y;
        }
    }


    public void Update(double delta)
    {
        var oldCtx = _imgui.GetCurrentContext();
        if (oldCtx != _context)
        {
            _imgui.SetCurrentContext(_context);
        }

        if (_frameBegun)
        {
            _imgui.Render();
        }

        SetPerFrameImGuiData(delta);

        UpdateMouseCursor();
        //UpdateMouse();
        //UpdateKeyboard();

        _frameBegun = true;
        _imgui.NewFrame();

        if (oldCtx != _context)
        {
            _imgui.SetCurrentContext(oldCtx);
        }
    }

    /// <summary>
    /// Sets per-frame data based on the associated window.
    /// This is called by Update(float).
    /// </summary>
    private void SetPerFrameImGuiData(double delta)
    {
        _imgui.GetIO(out var io);

        io.DisplaySize.X = _windowsWidth;
        io.DisplaySize.Y = _windowsHeight;

        if (_windowsWidth > 0 && _windowsHeight > 0)
        {
            io.DisplayFramebufferScale.X = (float) _view.FramebufferSize.X / _windowsWidth;
            io.DisplayFramebufferScale.Y = (float) _view.FramebufferSize.Y / _windowsHeight;
        }

        io.DeltaTime = (float) delta;
    }

    private void UpdateMouseCursor()
    {
        _imgui.GetIO(out var io);

        var flag = (io.ConfigFlags & ImGuiConfigFlags.NoMouseCursorChange) != 0;
        if (flag || Mouse.Cursor.CursorMode == CursorMode.Disabled) return;

        var imguiCursor = _imgui.GetMouseCursor();
        if (imguiCursor == BUTR.CrashReport.ImGui.Enums.ImGuiMouseCursor.None || io.MouseDrawCursor)
        {
            Mouse.Cursor.CursorMode = CursorMode.Hidden;
        }
        else
        {
            Mouse.Cursor.StandardCursor = _mouseCursors[(int) imguiCursor] != StandardCursor.Default ? _mouseCursors[(int) imguiCursor] : _mouseCursors[(int) ImGuiMouseCursor.Arrow];
            Mouse.Cursor.CursorMode = CursorMode.Normal;
        }
    }

    private void UpdateMouse()
    {
        _imgui.GetIO(out var io);

        using var mouseState = _input.Mice[0].CaptureState();

        io.GetMouseDown(out var mouseDown);
        mouseDown[ImGuiMouseButton.Left] = mouseState.IsButtonPressed(MouseButton.Left);
        mouseDown[ImGuiMouseButton.Right] = mouseState.IsButtonPressed(MouseButton.Right);
        mouseDown[ImGuiMouseButton.Middle] = mouseState.IsButtonPressed(MouseButton.Middle);

        var point = new Point((int) mouseState.Position.X, (int) mouseState.Position.Y);
        io.MousePos = new Vector2(point.X, point.Y);

        ref var wheel = ref mouseState.GetScrollWheels()[0];
        io.MouseWheel = wheel.Y;
        io.MouseWheelH = wheel.X;
    }

    private void UpdateKeyboard()
    {
        _imgui.GetIO(out var io);

        var cs = _pressedChars.AsSpan();
        for (var i = 0; i < cs.Length; i++)
            io.AddInputCharacter(cs[i]);
        _pressedChars.Clear();

        io.KeyCtrl = Keyboard.IsKeyPressed(Key.ControlLeft) || Keyboard.IsKeyPressed(Key.ControlRight);
        io.KeyAlt = Keyboard.IsKeyPressed(Key.AltLeft) || Keyboard.IsKeyPressed(Key.AltRight);
        io.KeyShift = Keyboard.IsKeyPressed(Key.ShiftLeft) || Keyboard.IsKeyPressed(Key.ShiftRight);
        io.KeySuper = Keyboard.IsKeyPressed(Key.SuperLeft) || Keyboard.IsKeyPressed(Key.SuperRight);
    }
}