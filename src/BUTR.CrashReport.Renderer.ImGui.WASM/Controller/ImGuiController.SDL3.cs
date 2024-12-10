#if SDL3
using BUTR.CrashReport.ImGui.Enums;

using System.Numerics;

namespace BUTR.CrashReport.Renderer.ImGui.WASM.Controller;

partial class ImGuiGLRenderer
{
    private readonly IntPtr[] _mouseCursors = new IntPtr[(int) ImGuiMouseCursor.COUNT];

    private IntPtr MouseLastCursor;

    private ulong _time;

    private Vector2 GetWindowDevicePixelRatio()
    {
        SDL_GetWindowSize(_window, out var w, out var h);
        SDL_GetWindowSizeInPixels(_window, out var wp, out var hp);
        return new Vector2((float) wp / (float) w, (float) hp / (float) h);
    }

    public void Init()
    {
        _imgui.GetIO(out var io);
        
        io.BackendFlags |= ImGuiNET.ImGuiBackendFlags.HasMouseCursors;       // We can honor GetMouseCursor() values (optional)
        io.BackendFlags |= ImGuiNET.ImGuiBackendFlags.HasSetMousePos;        // We can honor io.WantSetMousePos requests (optional, rarely used)

        _mouseCursors[(int) ImGuiNET.ImGuiMouseCursor.Arrow] = SDL_CreateSystemCursor(SDL_SystemCursor.SDL_SYSTEM_CURSOR_DEFAULT);
        _mouseCursors[(int) ImGuiNET.ImGuiMouseCursor.TextInput] = SDL_CreateSystemCursor(SDL_SystemCursor.SDL_SYSTEM_CURSOR_TEXT);
        _mouseCursors[(int) ImGuiNET.ImGuiMouseCursor.ResizeAll] = SDL_CreateSystemCursor(SDL_SystemCursor.SDL_SYSTEM_CURSOR_MOVE);
        _mouseCursors[(int) ImGuiNET.ImGuiMouseCursor.ResizeNS] = SDL_CreateSystemCursor(SDL_SystemCursor.SDL_SYSTEM_CURSOR_NS_RESIZE);
        _mouseCursors[(int) ImGuiNET.ImGuiMouseCursor.ResizeEW] = SDL_CreateSystemCursor(SDL_SystemCursor.SDL_SYSTEM_CURSOR_EW_RESIZE);
        _mouseCursors[(int) ImGuiNET.ImGuiMouseCursor.ResizeNESW] = SDL_CreateSystemCursor(SDL_SystemCursor.SDL_SYSTEM_CURSOR_NESW_RESIZE);
        _mouseCursors[(int) ImGuiNET.ImGuiMouseCursor.ResizeNWSE] = SDL_CreateSystemCursor(SDL_SystemCursor.SDL_SYSTEM_CURSOR_NWSE_RESIZE);
        _mouseCursors[(int) ImGuiNET.ImGuiMouseCursor.Hand] = SDL_CreateSystemCursor(SDL_SystemCursor.SDL_SYSTEM_CURSOR_POINTER);
        _mouseCursors[(int) ImGuiNET.ImGuiMouseCursor.NotAllowed] = SDL_CreateSystemCursor(SDL_SystemCursor.SDL_SYSTEM_CURSOR_NOT_ALLOWED);
        
        SDL_SetHint(SDL_HINT_MOUSE_FOCUS_CLICKTHROUGH, "1");
        SDL_SetHint(SDL_HINT_MOUSE_AUTO_CAPTURE, "0");
    }
    
    public void NewFrame()
    {
        _imgui.GetIO(out var io);

        // Setup display size (every frame to accommodate for window resizing)
        SDL_GetWindowSize(_window, out var w, out var h);
        SDL_GetWindowSizeInPixels(_window, out var displayW, out var displayH);
        
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
        io.DeltaTime = _time > 0 ? (float)((double)(currentTime - _time) / frequency) : (float) (1.0f / 60.0f);
        if (io.DeltaTime <= 0)
            io.DeltaTime = 0.016f;
        _time = currentTime;

        UpdateMouseData();
        UpdateMouseCursor();
            
        //_imgui.NewFrame();
    }

    public unsafe bool ProcessEvent(SDL_Event evt)
    {
        _imgui.GetIO(out var io);
        switch ((SDL_EventType) evt.type)
        {
            case SDL_EventType.SDL_EVENT_MOUSE_MOTION:
            {
                if (evt.motion.windowID != SDL_GetWindowID(_window)) return false;

                var scale = GetWindowDevicePixelRatio();
                
                //io.AddMouseSourceEvent(evt.motion.which == SDL_TOUCH_MOUSEID ? 0 : 1, evt.motion.x * scale.X, evt.motion.y * scale.Y);
                io.AddMousePosEvent(evt.motion.x * scale.X, evt.motion.y * scale.Y);
                return true;
            }
            case SDL_EventType.SDL_EVENT_MOUSE_WHEEL:
            {
                if (evt.wheel.windowID != SDL_GetWindowID(_window)) return false;;

                var wheel_x = -evt.wheel.x;
                var wheel_y = evt.wheel.y;
                
                //io.AddMouseSourceEvent(evt->wheel.which == SDL_TOUCH_MOUSEID ? ImGuiMouseSource_TouchScreen : ImGuiMouseSource_Mouse);
                io.AddMouseWheelEvent(wheel_x, wheel_y);
                return true;
            }
            case SDL_EventType.SDL_EVENT_MOUSE_BUTTON_DOWN:
            case SDL_EventType.SDL_EVENT_MOUSE_BUTTON_UP:
            {
                if (evt.button.windowID != SDL_GetWindowID(_window)) return false;;
                
                var mouse_button = (int) ImGuiNET.ImGuiMouseButton.COUNT;
                if (evt.button.button == (int) SDL_MouseButtonFlags.SDL_BUTTON_LMASK) { mouse_button = (int) ImGuiNET.ImGuiMouseButton.Left; }
                if (evt.button.button == (int) SDL_MouseButtonFlags.SDL_BUTTON_RMASK) { mouse_button = (int) ImGuiNET.ImGuiMouseButton.Right; }
                if (evt.button.button == (int) SDL_MouseButtonFlags.SDL_BUTTON_MMASK) { mouse_button = (int) ImGuiNET.ImGuiMouseButton.Middle; }
                if (evt.button.button == (int) SDL_MouseButtonFlags.SDL_BUTTON_X1MASK) { mouse_button = 3; }
                if (evt.button.button == (int) SDL_MouseButtonFlags.SDL_BUTTON_X2MASK) { mouse_button = 4; }
                if (mouse_button == (int) ImGuiNET.ImGuiMouseButton.COUNT) return false;;
                
                //io.AddMouseSourceEvent(evt.button.which == SDL_TOUCH_MOUSEID ? ImGuiMouseSource_TouchScreen : ImGuiMouseSource_Mouse);
                io.AddMouseButtonEvent((ImGuiNET.ImGuiMouseButton) mouse_button, evt.type == (uint) SDL_EventType.SDL_EVENT_MOUSE_BUTTON_DOWN);
                return true;
            }
            case SDL_EventType.SDL_EVENT_TEXT_INPUT:
            {
                if (evt.text.windowID != SDL_GetWindowID(_window)) return false;
                
                io.AddInputCharacterUTF8(evt.text.text);
                return true;
            }
            case SDL_EventType.SDL_EVENT_KEY_DOWN:
            case SDL_EventType.SDL_EVENT_KEY_UP:
            {
                if (evt.key.windowID != SDL_GetWindowID(_window)) return false;;

                UpdateKeyModifiers(evt.key.mod);
                
                var key = MapSDLKeyToImGuiKey((SDL_Keycode) evt.key.key, evt.key.scancode);
                
                io.AddKeyEvent(key, evt.type == (uint) SDL_EventType.SDL_EVENT_KEY_DOWN);
                io.SetKeyEventNativeData(key, (int) evt.key.key, (int) evt.key.scancode, (int) evt.key.scancode);
                return true;
            }
            /*
            case SDL_EventType.SDL_EVENT_WINDOW_MOUSE_ENTER:
            {
                if (evt.window.windowID != SDL_GetWindowID(_window)) return false;;

                return true;
            }
            case SDL_EventType.SDL_EVENT_WINDOW_MOUSE_LEAVE:
            {
                if (evt.window.windowID != SDL_GetWindowID(_window)) return false;;

                return true;
            }
            */
            case SDL_EventType.SDL_EVENT_WINDOW_FOCUS_GAINED:
            case SDL_EventType.SDL_EVENT_WINDOW_FOCUS_LOST:
            {
                if (evt.window.windowID != SDL_GetWindowID(_window)) return false;;
                
                io.AddFocusEvent(evt.type == (uint) SDL_EventType.SDL_EVENT_WINDOW_FOCUS_GAINED);
                
                return true;
            }
            /*
            case SDL_EventType.SDL_EVENT_GAMEPAD_ADDED:
            case SDL_EventType.SDL_EVENT_GAMEPAD_REMOVED:
            {
                WantUpdateGamepadsList = true;
                return true;
            }
            */
        }
        return false;
    }

    private static ImGuiNET.ImGuiKey MapSDLKeyToImGuiKey(SDL_Keycode keycode, SDL_Scancode scancode)
    {
        // Keypad doesn't have individual key values in SDL3
        switch (scancode)
        {
            case SDL_Scancode.SDL_SCANCODE_KP_0: return ImGuiNET.ImGuiKey.Keypad0;
            case SDL_Scancode.SDL_SCANCODE_KP_1: return ImGuiNET.ImGuiKey.Keypad1;
            case SDL_Scancode.SDL_SCANCODE_KP_2: return ImGuiNET.ImGuiKey.Keypad2;
            case SDL_Scancode.SDL_SCANCODE_KP_3: return ImGuiNET.ImGuiKey.Keypad3;
            case SDL_Scancode.SDL_SCANCODE_KP_4: return ImGuiNET.ImGuiKey.Keypad4;
            case SDL_Scancode.SDL_SCANCODE_KP_5: return ImGuiNET.ImGuiKey.Keypad5;
            case SDL_Scancode.SDL_SCANCODE_KP_6: return ImGuiNET.ImGuiKey.Keypad6;
            case SDL_Scancode.SDL_SCANCODE_KP_7: return ImGuiNET.ImGuiKey.Keypad7;
            case SDL_Scancode.SDL_SCANCODE_KP_8: return ImGuiNET.ImGuiKey.Keypad8;
            case SDL_Scancode.SDL_SCANCODE_KP_9: return ImGuiNET.ImGuiKey.Keypad9;
            case SDL_Scancode.SDL_SCANCODE_KP_PERIOD: return ImGuiNET.ImGuiKey.KeypadDecimal;
            case SDL_Scancode.SDL_SCANCODE_KP_DIVIDE: return ImGuiNET.ImGuiKey.KeypadDivide;
            case SDL_Scancode.SDL_SCANCODE_KP_MULTIPLY: return ImGuiNET.ImGuiKey.KeypadMultiply;
            case SDL_Scancode.SDL_SCANCODE_KP_MINUS: return ImGuiNET.ImGuiKey.KeypadSubtract;
            case SDL_Scancode.SDL_SCANCODE_KP_PLUS: return ImGuiNET.ImGuiKey.KeypadAdd;
            case SDL_Scancode.SDL_SCANCODE_KP_ENTER: return ImGuiNET.ImGuiKey.KeypadEnter;
            case SDL_Scancode.SDL_SCANCODE_KP_EQUALS: return ImGuiNET.ImGuiKey.KeypadEqual;
            default: break;
        }
        switch (keycode)
        {
            case SDL_Keycode.SDLK_TAB: return ImGuiNET.ImGuiKey.Tab;
            case SDL_Keycode.SDLK_LEFT: return ImGuiNET.ImGuiKey.LeftArrow;
            case SDL_Keycode.SDLK_RIGHT: return ImGuiNET.ImGuiKey.RightArrow;
            case SDL_Keycode.SDLK_UP: return ImGuiNET.ImGuiKey.UpArrow;
            case SDL_Keycode.SDLK_DOWN: return ImGuiNET.ImGuiKey.DownArrow;
            case SDL_Keycode.SDLK_PAGEUP: return ImGuiNET.ImGuiKey.PageUp;
            case SDL_Keycode.SDLK_PAGEDOWN: return ImGuiNET.ImGuiKey.PageDown;
            case SDL_Keycode.SDLK_HOME: return ImGuiNET.ImGuiKey.Home;
            case SDL_Keycode.SDLK_END: return ImGuiNET.ImGuiKey.End;
            case SDL_Keycode.SDLK_INSERT: return ImGuiNET.ImGuiKey.Insert;
            case SDL_Keycode.SDLK_DELETE: return ImGuiNET.ImGuiKey.Delete;
            case SDL_Keycode.SDLK_BACKSPACE: return ImGuiNET.ImGuiKey.Backspace;
            case SDL_Keycode.SDLK_SPACE: return ImGuiNET.ImGuiKey.Space;
            case SDL_Keycode.SDLK_RETURN: return ImGuiNET.ImGuiKey.Enter;
            case SDL_Keycode.SDLK_ESCAPE: return ImGuiNET.ImGuiKey.Escape;
            case SDL_Keycode.SDLK_APOSTROPHE: return ImGuiNET.ImGuiKey.Apostrophe;
            case SDL_Keycode.SDLK_COMMA: return ImGuiNET.ImGuiKey.Comma;
            case SDL_Keycode.SDLK_MINUS: return ImGuiNET.ImGuiKey.Minus;
            case SDL_Keycode.SDLK_PERIOD: return ImGuiNET.ImGuiKey.Period;
            case SDL_Keycode.SDLK_SLASH: return ImGuiNET.ImGuiKey.Slash;
            case SDL_Keycode.SDLK_SEMICOLON: return ImGuiNET.ImGuiKey.Semicolon;
            case SDL_Keycode.SDLK_EQUALS: return ImGuiNET.ImGuiKey.Equal;
            case SDL_Keycode.SDLK_LEFTBRACKET: return ImGuiNET.ImGuiKey.LeftBracket;
            case SDL_Keycode.SDLK_BACKSLASH: return ImGuiNET.ImGuiKey.Backslash;
            case SDL_Keycode.SDLK_RIGHTBRACKET: return ImGuiNET.ImGuiKey.RightBracket;
            case SDL_Keycode.SDLK_GRAVE: return ImGuiNET.ImGuiKey.GraveAccent;
            case SDL_Keycode.SDLK_CAPSLOCK: return ImGuiNET.ImGuiKey.CapsLock;
            case SDL_Keycode.SDLK_SCROLLLOCK: return ImGuiNET.ImGuiKey.ScrollLock;
            case SDL_Keycode.SDLK_NUMLOCKCLEAR: return ImGuiNET.ImGuiKey.NumLock;
            case SDL_Keycode.SDLK_PRINTSCREEN: return ImGuiNET.ImGuiKey.PrintScreen;
            case SDL_Keycode.SDLK_PAUSE: return ImGuiNET.ImGuiKey.Pause;
            case SDL_Keycode.SDLK_LCTRL: return ImGuiNET.ImGuiKey.LeftCtrl;
            case SDL_Keycode.SDLK_LSHIFT: return ImGuiNET.ImGuiKey.LeftShift;
            case SDL_Keycode.SDLK_LALT: return ImGuiNET.ImGuiKey.LeftAlt;
            case SDL_Keycode.SDLK_LGUI: return ImGuiNET.ImGuiKey.LeftSuper;
            case SDL_Keycode.SDLK_RCTRL: return ImGuiNET.ImGuiKey.RightCtrl;
            case SDL_Keycode.SDLK_RSHIFT: return ImGuiNET.ImGuiKey.RightShift;
            case SDL_Keycode.SDLK_RALT: return ImGuiNET.ImGuiKey.RightAlt;
            case SDL_Keycode.SDLK_RGUI: return ImGuiNET.ImGuiKey.RightSuper;
            case SDL_Keycode.SDLK_APPLICATION: return ImGuiNET.ImGuiKey.Menu;
            case SDL_Keycode.SDLK_0: return ImGuiNET.ImGuiKey._0;
            case SDL_Keycode.SDLK_1: return ImGuiNET.ImGuiKey._1;
            case SDL_Keycode.SDLK_2: return ImGuiNET.ImGuiKey._2;
            case SDL_Keycode.SDLK_3: return ImGuiNET.ImGuiKey._3;
            case SDL_Keycode.SDLK_4: return ImGuiNET.ImGuiKey._4;
            case SDL_Keycode.SDLK_5: return ImGuiNET.ImGuiKey._5;
            case SDL_Keycode.SDLK_6: return ImGuiNET.ImGuiKey._6;
            case SDL_Keycode.SDLK_7: return ImGuiNET.ImGuiKey._7;
            case SDL_Keycode.SDLK_8: return ImGuiNET.ImGuiKey._8;
            case SDL_Keycode.SDLK_9: return ImGuiNET.ImGuiKey._9;
            case SDL_Keycode.SDLK_A: return ImGuiNET.ImGuiKey.A;
            case SDL_Keycode.SDLK_B: return ImGuiNET.ImGuiKey.B;
            case SDL_Keycode.SDLK_C: return ImGuiNET.ImGuiKey.C;
            case SDL_Keycode.SDLK_D: return ImGuiNET.ImGuiKey.D;
            case SDL_Keycode.SDLK_E: return ImGuiNET.ImGuiKey.E;
            case SDL_Keycode.SDLK_F: return ImGuiNET.ImGuiKey.F;
            case SDL_Keycode.SDLK_G: return ImGuiNET.ImGuiKey.G;
            case SDL_Keycode.SDLK_H: return ImGuiNET.ImGuiKey.H;
            case SDL_Keycode.SDLK_I: return ImGuiNET.ImGuiKey.I;
            case SDL_Keycode.SDLK_J: return ImGuiNET.ImGuiKey.J;
            case SDL_Keycode.SDLK_K: return ImGuiNET.ImGuiKey.K;
            case SDL_Keycode.SDLK_L: return ImGuiNET.ImGuiKey.L;
            case SDL_Keycode.SDLK_M: return ImGuiNET.ImGuiKey.M;
            case SDL_Keycode.SDLK_N: return ImGuiNET.ImGuiKey.N;
            case SDL_Keycode.SDLK_O: return ImGuiNET.ImGuiKey.O;
            case SDL_Keycode.SDLK_P: return ImGuiNET.ImGuiKey.P;
            case SDL_Keycode.SDLK_Q: return ImGuiNET.ImGuiKey.Q;
            case SDL_Keycode.SDLK_R: return ImGuiNET.ImGuiKey.R;
            case SDL_Keycode.SDLK_S: return ImGuiNET.ImGuiKey.S;
            case SDL_Keycode.SDLK_T: return ImGuiNET.ImGuiKey.T;
            case SDL_Keycode.SDLK_U: return ImGuiNET.ImGuiKey.U;
            case SDL_Keycode.SDLK_V: return ImGuiNET.ImGuiKey.V;
            case SDL_Keycode.SDLK_W: return ImGuiNET.ImGuiKey.W;
            case SDL_Keycode.SDLK_X: return ImGuiNET.ImGuiKey.X;
            case SDL_Keycode.SDLK_Y: return ImGuiNET.ImGuiKey.Y;
            case SDL_Keycode.SDLK_Z: return ImGuiNET.ImGuiKey.Z;
            case SDL_Keycode.SDLK_F1: return ImGuiNET.ImGuiKey.F1;
            case SDL_Keycode.SDLK_F2: return ImGuiNET.ImGuiKey.F2;
            case SDL_Keycode.SDLK_F3: return ImGuiNET.ImGuiKey.F3;
            case SDL_Keycode.SDLK_F4: return ImGuiNET.ImGuiKey.F4;
            case SDL_Keycode.SDLK_F5: return ImGuiNET.ImGuiKey.F5;
            case SDL_Keycode.SDLK_F6: return ImGuiNET.ImGuiKey.F6;
            case SDL_Keycode.SDLK_F7: return ImGuiNET.ImGuiKey.F7;
            case SDL_Keycode.SDLK_F8: return ImGuiNET.ImGuiKey.F8;
            case SDL_Keycode.SDLK_F9: return ImGuiNET.ImGuiKey.F9;
            case SDL_Keycode.SDLK_F10: return ImGuiNET.ImGuiKey.F10;
            case SDL_Keycode.SDLK_F11: return ImGuiNET.ImGuiKey.F11;
            case SDL_Keycode.SDLK_F12: return ImGuiNET.ImGuiKey.F12;
            case SDL_Keycode.SDLK_F13: return ImGuiNET.ImGuiKey.F13;
            case SDL_Keycode.SDLK_F14: return ImGuiNET.ImGuiKey.F14;
            case SDL_Keycode.SDLK_F15: return ImGuiNET.ImGuiKey.F15;
            case SDL_Keycode.SDLK_F16: return ImGuiNET.ImGuiKey.F16;
            case SDL_Keycode.SDLK_F17: return ImGuiNET.ImGuiKey.F17;
            case SDL_Keycode.SDLK_F18: return ImGuiNET.ImGuiKey.F18;
            case SDL_Keycode.SDLK_F19: return ImGuiNET.ImGuiKey.F19;
            case SDL_Keycode.SDLK_F20: return ImGuiNET.ImGuiKey.F20;
            case SDL_Keycode.SDLK_F21: return ImGuiNET.ImGuiKey.F21;
            case SDL_Keycode.SDLK_F22: return ImGuiNET.ImGuiKey.F22;
            case SDL_Keycode.SDLK_F23: return ImGuiNET.ImGuiKey.F23;
            case SDL_Keycode.SDLK_F24: return ImGuiNET.ImGuiKey.F24;
            case SDL_Keycode.SDLK_AC_BACK: return ImGuiNET.ImGuiKey.AppBack;
            case SDL_Keycode.SDLK_AC_FORWARD: return ImGuiNET.ImGuiKey.AppForward;
            default: break;
    }
    return ImGuiNET.ImGuiKey.None;
}
    
    private void UpdateKeyModifiers(SDL_Keymod sdl_key_mods)
    {
        _imgui.GetIO(out var io);
        io.AddKeyEvent(ImGuiNET.ImGuiKey.ModCtrl, sdl_key_mods.HasFlag(SDL_Keymod.SDL_KMOD_CTRL));
        io.AddKeyEvent(ImGuiNET.ImGuiKey.ModShift, sdl_key_mods.HasFlag(SDL_Keymod.SDL_KMOD_SHIFT));
        io.AddKeyEvent(ImGuiNET.ImGuiKey.ModAlt, sdl_key_mods.HasFlag(SDL_Keymod.SDL_KMOD_ALT));
        io.AddKeyEvent(ImGuiNET.ImGuiKey.ModSuper, sdl_key_mods.HasFlag(SDL_Keymod.SDL_KMOD_GUI));
    }
    
    private void UpdateMouseData()
    {
        _imgui.GetIO(out var io);

        var is_app_focused = SDL_GetWindowFlags(_window).HasFlag(SDL_WindowFlags.SDL_WINDOW_INPUT_FOCUS); // SDL 2.0.3 and non-windowed systems: single-viewport only
        if (is_app_focused)
        {
            // (Optional) Set OS mouse position from Dear ImGui if requested (rarely used, only when io.ConfigNavMoveSetMousePos is enabled by user)
            if (io.WantSetMousePos)
                SDL_WarpMouseInWindow(_window, (int)io.MousePos.X, (int)io.MousePos.Y);
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
            SDL_HideCursor();
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
            SDL_ShowCursor();
        }
    }
}
#endif