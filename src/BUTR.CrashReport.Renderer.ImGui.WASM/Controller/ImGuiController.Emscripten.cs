using BUTR.CrashReport.Memory;

using Emscripten;

using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BUTR.CrashReport.Renderer.ImGui.WASM.Controller;

partial class ImGuiController
{
    private static readonly LiteralSpan<byte> CanvasIdUtf8 = "canvas\0"u8;

    // We make sure that the backbuffer is a whole number so it will be scaled down correctly
    private static void ScaleWindowSize(ref int width, ref int height, Vector2 scale)
    {
        var scaledWidth = width * scale.X;
        while (scaledWidth % 1 != 0)
        {
            width--;
            scaledWidth = width * scale.X;
        }

        var scaledHeight = height * scale.Y;
        while (scaledHeight % 1 != 0)
        {
            height--;
            scaledHeight = height * scale.Y;
        }
    }

    private unsafe void SetupEmscripten(IntPtr windowHandle)
    {
        _imgui.GetIO(out var io);

        io.BackendFlags |= ImGuiNET.ImGuiBackendFlags.HasMouseCursors;
        io.BackendFlags |= ImGuiNET.ImGuiBackendFlags.HasSetMousePos;
        io.BackendFlags |= ImGuiNET.ImGuiBackendFlags.RendererHasVtxOffset;
        io.BackendFlags |= ImGuiNET.ImGuiBackendFlags.HasGamepad;

        io.ConfigFlags |= ImGuiNET.ImGuiConfigFlags.NavEnableKeyboard;
        io.ConfigFlags |= ImGuiNET.ImGuiConfigFlags.NavEnableGamepad;
        io.ConfigFlags |= ImGuiNET.ImGuiConfigFlags.DpiEnableScaleFonts;
        io.ConfigFlags |= ImGuiNET.ImGuiConfigFlags.DpiEnableScaleViewports;
        io.ConfigFlags |= ImGuiNET.ImGuiConfigFlags.IsTouchScreen;

        int windowsWidth, windowsHeight;
        _emscripten.custom_emscripten_get_display_usable_bounds(&windowsWidth, &windowsHeight);

        var scale = GetWindowDevicePixelRatio();
        ScaleWindowSize(ref windowsWidth, ref windowsHeight, scale);

        SDL_SetWindowSize(_window, windowsWidth, windowsHeight);
        _emscripten.custom_emscripten_set_element_style_size(CanvasIdUtf8.Ptr, windowsWidth, windowsHeight);

        var onCanvasSizeChangeDataPtr = _allocator.Alloc(new EmscriptenCanvasSizeChangeData
        {
            Window = windowHandle,
            CanvasId = CanvasIdUtf8.Ptr,
        });
        _emscripten.emscripten_set_resize_callback_on_thread((byte*) EMSCRIPTEN_EVENT_TARGET_WINDOW, onCanvasSizeChangeDataPtr, 0, &EmscriptenWindowSizeChange, EM_CALLBACK_THREAD_CONTEXT_CALLING_THREAD);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static unsafe int EmscriptenWindowSizeChange(int event_type, EmscriptenUiEvent* @event, EmscriptenCanvasSizeChangeData* user_data)
    {
        if (!_instances.TryGetValue(user_data->Window, out var instanceRef) || !instanceRef.TryGetTarget(out var instance))
            return EM_FALSE;

        var windowsWidth = @event->WindowInnerWidth;
        var windowsHeight = @event->WindowInnerHeight;
        var scale = instance.GetWindowDevicePixelRatio();
        ScaleWindowSize(ref windowsWidth, ref windowsHeight, scale);

        SDL_SetWindowSize(user_data->Window, windowsWidth, windowsHeight);
        instance._emscripten.custom_emscripten_set_element_style_size(user_data->CanvasId, windowsWidth, windowsHeight);

        return EM_TRUE;
    }
}