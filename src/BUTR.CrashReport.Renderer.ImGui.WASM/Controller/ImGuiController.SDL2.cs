#if SDL2
using BUTR.CrashReport.ImGui.Enums;

using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace BUTR.CrashReport.Renderer.ImGui.WASM.Controller;

partial class ImGuiController
{
    private readonly IntPtr[] _mouseCursors = new IntPtr[(int) ImGuiMouseCursor.COUNT];

    private IntPtr MouseLastCursor;
    private ulong _time;

    private Vector2 GetWindowDevicePixelRatio()
    {
        SDL_GetWindowSize(_window, out var w, out var h);
        SDL_GL_GetDrawableSize(_window, out var wp, out var hp);
        return new Vector2((float) wp / (float) w, (float) hp / (float) h);
    }

    public void Init()
    {
        _imgui.GetIO(out var io);

        io.BackendFlags |= ImGuiNET.ImGuiBackendFlags.HasMouseCursors;       // We can honor GetMouseCursor() values (optional)
        io.BackendFlags |= ImGuiNET.ImGuiBackendFlags.HasSetMousePos;        // We can honor io.WantSetMousePos requests (optional, rarely used)

        _mouseCursors[(int) ImGuiNET.ImGuiMouseCursor.Arrow] = SDL_CreateSystemCursor(SDL_SystemCursor.SDL_SYSTEM_CURSOR_ARROW);
        _mouseCursors[(int) ImGuiNET.ImGuiMouseCursor.TextInput] = SDL_CreateSystemCursor(SDL_SystemCursor.SDL_SYSTEM_CURSOR_IBEAM);
        _mouseCursors[(int) ImGuiNET.ImGuiMouseCursor.ResizeAll] = SDL_CreateSystemCursor(SDL_SystemCursor.SDL_SYSTEM_CURSOR_SIZEALL);
        _mouseCursors[(int) ImGuiNET.ImGuiMouseCursor.ResizeNS] = SDL_CreateSystemCursor(SDL_SystemCursor.SDL_SYSTEM_CURSOR_SIZENS);
        _mouseCursors[(int) ImGuiNET.ImGuiMouseCursor.ResizeEW] = SDL_CreateSystemCursor(SDL_SystemCursor.SDL_SYSTEM_CURSOR_SIZEWE);
        _mouseCursors[(int) ImGuiNET.ImGuiMouseCursor.ResizeNESW] = SDL_CreateSystemCursor(SDL_SystemCursor.SDL_SYSTEM_CURSOR_SIZENESW);
        _mouseCursors[(int) ImGuiNET.ImGuiMouseCursor.ResizeNWSE] = SDL_CreateSystemCursor(SDL_SystemCursor.SDL_SYSTEM_CURSOR_SIZENWSE);
        _mouseCursors[(int) ImGuiNET.ImGuiMouseCursor.Hand] = SDL_CreateSystemCursor(SDL_SystemCursor.SDL_SYSTEM_CURSOR_HAND);
        _mouseCursors[(int) ImGuiNET.ImGuiMouseCursor.NotAllowed] = SDL_CreateSystemCursor(SDL_SystemCursor.SDL_SYSTEM_CURSOR_NO);

        SDL_SetHint(SDL_HINT_MOUSE_FOCUS_CLICKTHROUGH, "1");
        //SDL_SetHint(SDL_HINT_MOUSE_AUTO_CAPTURE, "0");
    }

    public void NewFrame()
    {
        _imgui.GetIO(out var io);

        // Setup display size (every frame to accommodate for window resizing)
        SDL_GetWindowSize(_window, out var w, out var h);
        SDL_GL_GetDrawableSize(_window, out var displayW, out var displayH);

        if (SDL_GetWindowFlags(_window).HasFlag(SDL_WindowFlags.SDL_WINDOW_MINIMIZED))
            w = h = 0;

        io.DisplaySize.X = displayW;
        io.DisplaySize.Y = displayH;
        io.DisplayFramebufferScale.X = 1;
        io.DisplayFramebufferScale.Y = 1;
        /*
        if (w > 0 && h > 0)
        {
            io.DisplayFramebufferScale.X = (float) displayW / w;
            io.DisplayFramebufferScale.Y = (float) displayH / h;
        }
        */

        // Setup time step (we don't use SDL_GetTicks() because it is using millisecond resolution)
        var frequency = SDL_GetPerformanceFrequency();
        var currentTime = SDL_GetPerformanceCounter();
        io.DeltaTime = _time > 0 ? (float) ((double) (currentTime - _time) / frequency) : 1.0f / 60.0f;
        if (io.DeltaTime <= 0)
            io.DeltaTime = 0.016f;
        _time = currentTime;

        UpdateMouseData();
        UpdateMouseCursor();

        _imgui.NewFrame();
    }

    public unsafe bool ProcessEvent(SDL_Event evt)
    {
        _imgui.GetIO(out var io);
        switch (evt.type)
        {
            case SDL_EventType.SDL_MOUSEMOTION:
            {
                if (evt.motion.windowID != SDL_GetWindowID(_window)) return false;

                var scale = GetWindowDevicePixelRatio();

                //io.AddMouseSourceEvent(evt.motion.which == SDL_TOUCH_MOUSEID ? 0 : 1, evt.motion.x * scale.X, evt.motion.y * scale.Y);
                io.AddMousePosEvent(evt.motion.x * scale.X, evt.motion.y * scale.Y);
                return true;
            }
            case SDL_EventType.SDL_MOUSEWHEEL:
            {
                if (evt.wheel.windowID != SDL_GetWindowID(_window)) return false; ;

                var wheel_x = -evt.wheel.preciseX;
                var wheel_y = evt.wheel.preciseY;

                //wheel_x /= 100.0f;

                //io.AddMouseSourceEvent(evt->wheel.which == SDL_TOUCH_MOUSEID ? ImGuiMouseSource_TouchScreen : ImGuiMouseSource_Mouse);
                io.AddMouseWheelEvent(wheel_x, wheel_y);
                return true;
            }
            case SDL_EventType.SDL_MOUSEBUTTONDOWN:
            case SDL_EventType.SDL_MOUSEBUTTONUP:
            {
                if (evt.button.windowID != SDL_GetWindowID(_window)) return false; ;

                var mouse_button = ImGuiNET.ImGuiMouseButton.COUNT;
                if (evt.button.button == SDL_BUTTON_LEFT) { mouse_button = ImGuiNET.ImGuiMouseButton.Left; }
                if (evt.button.button == SDL_BUTTON_RIGHT) { mouse_button = ImGuiNET.ImGuiMouseButton.Right; }
                if (evt.button.button == SDL_BUTTON_MIDDLE) { mouse_button = ImGuiNET.ImGuiMouseButton.Middle; }
                if (evt.button.button == SDL_BUTTON_X1) { mouse_button = (ImGuiNET.ImGuiMouseButton) 3; }
                if (evt.button.button == SDL_BUTTON_X2) { mouse_button = (ImGuiNET.ImGuiMouseButton) 4; }
                if (mouse_button == ImGuiNET.ImGuiMouseButton.COUNT) return false; ;

                //io.AddMouseSourceEvent(evt.button.which == SDL_TOUCH_MOUSEID ? ImGuiMouseSource_TouchScreen : ImGuiMouseSource_Mouse);
                io.AddMouseButtonEvent(mouse_button, evt.type == SDL_EventType.SDL_MOUSEBUTTONDOWN);
                return true;
            }
            case SDL_EventType.SDL_TEXTINPUT:
            {
                if (evt.text.windowID != SDL_GetWindowID(_window)) return false; ;

                var utf8 = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(evt.text.text);
                var utf16Length = Encoding.UTF8.GetCharCount(utf8);
                Span<char> utf16 = stackalloc char[utf16Length];
                Encoding.UTF8.GetChars(utf8, utf16);

                for (var i = 0; i < utf16.Length; i++)
                    io.AddInputCharacter(utf16[i]);
                return true;
            }
            case SDL_EventType.SDL_KEYDOWN:
            case SDL_EventType.SDL_KEYUP:
            {
                if (evt.key.windowID != SDL_GetWindowID(_window)) return false; ;

                UpdateKeyModifiers(evt.key.keysym.mod);

                var key = MapSDLKeyToImGuiKey(evt.key.keysym.sym, evt.key.keysym.scancode);

                io.AddKeyEvent(key, evt.type == SDL_EventType.SDL_KEYDOWN);
                io.SetKeyEventNativeData(key, (int) evt.key.keysym.sym, (int) evt.key.keysym.scancode, (int) evt.key.keysym.scancode);
                return true;
            }
            case SDL_EventType.SDL_WINDOWEVENT:
            {
                if (evt.window.windowID != SDL_GetWindowID(_window)) return false; ;

                /*
                if (evt.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_ENTER)
                {
                    bd->MouseWindowID = evt.window.windowID;
                    bd->MouseLastLeaveFrame = 0;
                }
                if (evt.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_LEAVE)
                    bd->MouseLastLeaveFrame = ImGui::GetFrameCount() + 1;
                */
                if (evt.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_GAINED)
                    io.AddFocusEvent(true);
                else if (evt.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_LOST)
                    io.AddFocusEvent(false);
                return true;
            }
            case SDL_EventType.SDL_CONTROLLERDEVICEADDED:
            case SDL_EventType.SDL_CONTROLLERDEVICEREMOVED:
            {
                //bd->WantUpdateGamepadsList = true;
                return true;
            }
        }
        return false;
    }

    private static ImGuiNET.ImGuiKey MapSDLKeyToImGuiKey(SDL_Keycode keycode, SDL_Scancode scancod) => keycode switch
    {
        SDL_Keycode.SDLK_TAB => ImGuiNET.ImGuiKey.Tab,
        SDL_Keycode.SDLK_LEFT => ImGuiNET.ImGuiKey.LeftArrow,
        SDL_Keycode.SDLK_RIGHT => ImGuiNET.ImGuiKey.RightArrow,
        SDL_Keycode.SDLK_UP => ImGuiNET.ImGuiKey.UpArrow,
        SDL_Keycode.SDLK_DOWN => ImGuiNET.ImGuiKey.DownArrow,
        SDL_Keycode.SDLK_PAGEUP => ImGuiNET.ImGuiKey.PageUp,
        SDL_Keycode.SDLK_PAGEDOWN => ImGuiNET.ImGuiKey.PageDown,
        SDL_Keycode.SDLK_HOME => ImGuiNET.ImGuiKey.Home,
        SDL_Keycode.SDLK_END => ImGuiNET.ImGuiKey.End,
        SDL_Keycode.SDLK_INSERT => ImGuiNET.ImGuiKey.Insert,
        SDL_Keycode.SDLK_DELETE => ImGuiNET.ImGuiKey.Delete,
        SDL_Keycode.SDLK_BACKSPACE => ImGuiNET.ImGuiKey.Backspace,
        SDL_Keycode.SDLK_SPACE => ImGuiNET.ImGuiKey.Space,
        SDL_Keycode.SDLK_RETURN => ImGuiNET.ImGuiKey.Enter,
        SDL_Keycode.SDLK_ESCAPE => ImGuiNET.ImGuiKey.Escape,
        SDL_Keycode.SDLK_QUOTE => ImGuiNET.ImGuiKey.Apostrophe,
        SDL_Keycode.SDLK_COMMA => ImGuiNET.ImGuiKey.Comma,
        SDL_Keycode.SDLK_MINUS => ImGuiNET.ImGuiKey.Minus,
        SDL_Keycode.SDLK_PERIOD => ImGuiNET.ImGuiKey.Period,
        SDL_Keycode.SDLK_SLASH => ImGuiNET.ImGuiKey.Slash,
        SDL_Keycode.SDLK_SEMICOLON => ImGuiNET.ImGuiKey.Semicolon,
        SDL_Keycode.SDLK_EQUALS => ImGuiNET.ImGuiKey.Equal,
        SDL_Keycode.SDLK_LEFTBRACKET => ImGuiNET.ImGuiKey.LeftBracket,
        SDL_Keycode.SDLK_BACKSLASH => ImGuiNET.ImGuiKey.Backslash,
        SDL_Keycode.SDLK_RIGHTBRACKET => ImGuiNET.ImGuiKey.RightBracket,
        SDL_Keycode.SDLK_BACKQUOTE => ImGuiNET.ImGuiKey.GraveAccent,
        SDL_Keycode.SDLK_CAPSLOCK => ImGuiNET.ImGuiKey.CapsLock,
        SDL_Keycode.SDLK_SCROLLLOCK => ImGuiNET.ImGuiKey.ScrollLock,
        SDL_Keycode.SDLK_NUMLOCKCLEAR => ImGuiNET.ImGuiKey.NumLock,
        SDL_Keycode.SDLK_PRINTSCREEN => ImGuiNET.ImGuiKey.PrintScreen,
        SDL_Keycode.SDLK_PAUSE => ImGuiNET.ImGuiKey.Pause,
        SDL_Keycode.SDLK_KP_0 => ImGuiNET.ImGuiKey.Keypad0,
        SDL_Keycode.SDLK_KP_1 => ImGuiNET.ImGuiKey.Keypad1,
        SDL_Keycode.SDLK_KP_2 => ImGuiNET.ImGuiKey.Keypad2,
        SDL_Keycode.SDLK_KP_3 => ImGuiNET.ImGuiKey.Keypad3,
        SDL_Keycode.SDLK_KP_4 => ImGuiNET.ImGuiKey.Keypad4,
        SDL_Keycode.SDLK_KP_5 => ImGuiNET.ImGuiKey.Keypad5,
        SDL_Keycode.SDLK_KP_6 => ImGuiNET.ImGuiKey.Keypad6,
        SDL_Keycode.SDLK_KP_7 => ImGuiNET.ImGuiKey.Keypad7,
        SDL_Keycode.SDLK_KP_8 => ImGuiNET.ImGuiKey.Keypad8,
        SDL_Keycode.SDLK_KP_9 => ImGuiNET.ImGuiKey.Keypad9,
        SDL_Keycode.SDLK_KP_PERIOD => ImGuiNET.ImGuiKey.KeypadDecimal,
        SDL_Keycode.SDLK_KP_DIVIDE => ImGuiNET.ImGuiKey.KeypadDivide,
        SDL_Keycode.SDLK_KP_MULTIPLY => ImGuiNET.ImGuiKey.KeypadMultiply,
        SDL_Keycode.SDLK_KP_MINUS => ImGuiNET.ImGuiKey.KeypadSubtract,
        SDL_Keycode.SDLK_KP_PLUS => ImGuiNET.ImGuiKey.KeypadAdd,
        SDL_Keycode.SDLK_KP_ENTER => ImGuiNET.ImGuiKey.KeypadEnter,
        SDL_Keycode.SDLK_KP_EQUALS => ImGuiNET.ImGuiKey.KeypadEqual,
        SDL_Keycode.SDLK_LCTRL => ImGuiNET.ImGuiKey.LeftCtrl,
        SDL_Keycode.SDLK_LSHIFT => ImGuiNET.ImGuiKey.LeftShift,
        SDL_Keycode.SDLK_LALT => ImGuiNET.ImGuiKey.LeftAlt,
        SDL_Keycode.SDLK_LGUI => ImGuiNET.ImGuiKey.LeftSuper,
        SDL_Keycode.SDLK_RCTRL => ImGuiNET.ImGuiKey.RightCtrl,
        SDL_Keycode.SDLK_RSHIFT => ImGuiNET.ImGuiKey.RightShift,
        SDL_Keycode.SDLK_RALT => ImGuiNET.ImGuiKey.RightAlt,
        SDL_Keycode.SDLK_RGUI => ImGuiNET.ImGuiKey.RightSuper,
        SDL_Keycode.SDLK_APPLICATION => ImGuiNET.ImGuiKey.Menu,
        SDL_Keycode.SDLK_0 => ImGuiNET.ImGuiKey._0,
        SDL_Keycode.SDLK_1 => ImGuiNET.ImGuiKey._1,
        SDL_Keycode.SDLK_2 => ImGuiNET.ImGuiKey._2,
        SDL_Keycode.SDLK_3 => ImGuiNET.ImGuiKey._3,
        SDL_Keycode.SDLK_4 => ImGuiNET.ImGuiKey._4,
        SDL_Keycode.SDLK_5 => ImGuiNET.ImGuiKey._5,
        SDL_Keycode.SDLK_6 => ImGuiNET.ImGuiKey._6,
        SDL_Keycode.SDLK_7 => ImGuiNET.ImGuiKey._7,
        SDL_Keycode.SDLK_8 => ImGuiNET.ImGuiKey._8,
        SDL_Keycode.SDLK_9 => ImGuiNET.ImGuiKey._9,
        SDL_Keycode.SDLK_a => ImGuiNET.ImGuiKey.A,
        SDL_Keycode.SDLK_b => ImGuiNET.ImGuiKey.B,
        SDL_Keycode.SDLK_c => ImGuiNET.ImGuiKey.C,
        SDL_Keycode.SDLK_d => ImGuiNET.ImGuiKey.D,
        SDL_Keycode.SDLK_e => ImGuiNET.ImGuiKey.E,
        SDL_Keycode.SDLK_f => ImGuiNET.ImGuiKey.F,
        SDL_Keycode.SDLK_g => ImGuiNET.ImGuiKey.G,
        SDL_Keycode.SDLK_h => ImGuiNET.ImGuiKey.H,
        SDL_Keycode.SDLK_i => ImGuiNET.ImGuiKey.I,
        SDL_Keycode.SDLK_j => ImGuiNET.ImGuiKey.J,
        SDL_Keycode.SDLK_k => ImGuiNET.ImGuiKey.K,
        SDL_Keycode.SDLK_l => ImGuiNET.ImGuiKey.L,
        SDL_Keycode.SDLK_m => ImGuiNET.ImGuiKey.M,
        SDL_Keycode.SDLK_n => ImGuiNET.ImGuiKey.N,
        SDL_Keycode.SDLK_o => ImGuiNET.ImGuiKey.O,
        SDL_Keycode.SDLK_p => ImGuiNET.ImGuiKey.P,
        SDL_Keycode.SDLK_q => ImGuiNET.ImGuiKey.Q,
        SDL_Keycode.SDLK_r => ImGuiNET.ImGuiKey.R,
        SDL_Keycode.SDLK_s => ImGuiNET.ImGuiKey.S,
        SDL_Keycode.SDLK_t => ImGuiNET.ImGuiKey.T,
        SDL_Keycode.SDLK_u => ImGuiNET.ImGuiKey.U,
        SDL_Keycode.SDLK_v => ImGuiNET.ImGuiKey.V,
        SDL_Keycode.SDLK_w => ImGuiNET.ImGuiKey.W,
        SDL_Keycode.SDLK_x => ImGuiNET.ImGuiKey.X,
        SDL_Keycode.SDLK_y => ImGuiNET.ImGuiKey.Y,
        SDL_Keycode.SDLK_z => ImGuiNET.ImGuiKey.Z,
        SDL_Keycode.SDLK_F1 => ImGuiNET.ImGuiKey.F1,
        SDL_Keycode.SDLK_F2 => ImGuiNET.ImGuiKey.F2,
        SDL_Keycode.SDLK_F3 => ImGuiNET.ImGuiKey.F3,
        SDL_Keycode.SDLK_F4 => ImGuiNET.ImGuiKey.F4,
        SDL_Keycode.SDLK_F5 => ImGuiNET.ImGuiKey.F5,
        SDL_Keycode.SDLK_F6 => ImGuiNET.ImGuiKey.F6,
        SDL_Keycode.SDLK_F7 => ImGuiNET.ImGuiKey.F7,
        SDL_Keycode.SDLK_F8 => ImGuiNET.ImGuiKey.F8,
        SDL_Keycode.SDLK_F9 => ImGuiNET.ImGuiKey.F9,
        SDL_Keycode.SDLK_F10 => ImGuiNET.ImGuiKey.F10,
        SDL_Keycode.SDLK_F11 => ImGuiNET.ImGuiKey.F11,
        SDL_Keycode.SDLK_F12 => ImGuiNET.ImGuiKey.F12,
        SDL_Keycode.SDLK_F13 => ImGuiNET.ImGuiKey.F13,
        SDL_Keycode.SDLK_F14 => ImGuiNET.ImGuiKey.F14,
        SDL_Keycode.SDLK_F15 => ImGuiNET.ImGuiKey.F15,
        SDL_Keycode.SDLK_F16 => ImGuiNET.ImGuiKey.F16,
        SDL_Keycode.SDLK_F17 => ImGuiNET.ImGuiKey.F17,
        SDL_Keycode.SDLK_F18 => ImGuiNET.ImGuiKey.F18,
        SDL_Keycode.SDLK_F19 => ImGuiNET.ImGuiKey.F19,
        SDL_Keycode.SDLK_F20 => ImGuiNET.ImGuiKey.F20,
        SDL_Keycode.SDLK_F21 => ImGuiNET.ImGuiKey.F21,
        SDL_Keycode.SDLK_F22 => ImGuiNET.ImGuiKey.F22,
        SDL_Keycode.SDLK_F23 => ImGuiNET.ImGuiKey.F23,
        SDL_Keycode.SDLK_F24 => ImGuiNET.ImGuiKey.F24,
        SDL_Keycode.SDLK_AC_BACK => ImGuiNET.ImGuiKey.AppBack,
        SDL_Keycode.SDLK_AC_FORWARD => ImGuiNET.ImGuiKey.AppForward,
        _ => ImGuiNET.ImGuiKey.None,
    };

    private void UpdateKeyModifiers(SDL_Keymod sdl_key_mods)
    {
        _imgui.GetIO(out var io);
        io.AddKeyEvent(ImGuiNET.ImGuiKey.ModCtrl, (sdl_key_mods & SDL_Keymod.KMOD_CTRL) != 0);
        io.AddKeyEvent(ImGuiNET.ImGuiKey.ModShift, (sdl_key_mods & SDL_Keymod.KMOD_SHIFT) != 0);
        io.AddKeyEvent(ImGuiNET.ImGuiKey.ModAlt, (sdl_key_mods & SDL_Keymod.KMOD_ALT) != 0);
        io.AddKeyEvent(ImGuiNET.ImGuiKey.ModSuper, (sdl_key_mods & SDL_Keymod.KMOD_GUI) != 0);
    }

    private void UpdateMouseData()
    {
        _imgui.GetIO(out var io);

        var is_app_focused = SDL_GetWindowFlags(_window).HasFlag(SDL_WindowFlags.SDL_WINDOW_INPUT_FOCUS); // SDL 2.0.3 and non-windowed systems: single-viewport only
        if (is_app_focused)
        {
            // (Optional) Set OS mouse position from Dear ImGui if requested (rarely used, only when io.ConfigNavMoveSetMousePos is enabled by user)
            if (io.WantSetMousePos)
                SDL_WarpMouseInWindow(_window, (int) io.MousePos.X, (int) io.MousePos.Y);
        }
    }

    private void UpdateMouseCursor()
    {
        _imgui.GetIO(out var io);
        if (io.ConfigFlags.HasFlag(ImGuiNET.ImGuiConfigFlags.NoMouseCursorChange))
            return;

        var imgui_cursor = _imgui.GetMouseCursor();
        if (io.MouseDrawCursor || imgui_cursor == ImGuiMouseCursor.None)
        {
            // Hide OS mouse cursor if imgui is drawing it or if it wants no cursor
            SDL_ShowCursor(SDL_bool.SDL_FALSE);
        }
        else
        {
            // Show OS mouse cursor
            var expected_cursor = _mouseCursors.Length > (int) imgui_cursor ? _mouseCursors[(int) imgui_cursor] : _mouseCursors[(int) ImGuiMouseCursor.Arrow];
            if (MouseLastCursor != expected_cursor)
            {
                SDL_SetCursor(expected_cursor); // SDL function doesn't have an early out (see #6113)
                MouseLastCursor = expected_cursor;
            }
            SDL_ShowCursor(SDL_bool.SDL_TRUE);
        }
    }
}
#endif