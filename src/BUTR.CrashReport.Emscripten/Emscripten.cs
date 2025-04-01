using System.Runtime.InteropServices;

namespace Emscripten;

[StructLayout(LayoutKind.Sequential)]
public readonly ref struct EmscriptenUiEvent
{
    public readonly int Detail;
    public readonly int DocumentBodyClientWidth;
    public readonly int DocumentBodyClientHeight;
    public readonly int WindowInnerWidth;
    public readonly int WindowInnerHeight;
    public readonly int WindowOuterWidth;
    public readonly int WindowOuterHeight;
    public readonly int ScrollTop;
    public readonly int ScrollLeft;
}

[StructLayout(LayoutKind.Sequential)]
public struct EmscriptenCanvasSizeChangeData
{
    public required IntPtr Window;
    public required IntPtrByte CanvasId;
}

public class Emscripten
{
    // ReSharper disable UnusedMember.Local
    public const int EM_FALSE = 0;
    public const int EM_TRUE = 1;

    public const int EM_CALLBACK_THREAD_CONTEXT_MAIN_RUNTIME_THREAD = 0x01;
    public const int EM_CALLBACK_THREAD_CONTEXT_CALLING_THREAD = 0x02;

    public const int EMSCRIPTEN_EVENT_TARGET_DOCUMENT = 1;
    public const int EMSCRIPTEN_EVENT_TARGET_WINDOW = 2;
    // ReSharper restore UnusedMember.Local

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void emscripten_set_main_loopDel(em_arg_callback_func cb, int fps, int simulate_infinite_loop);
    public emscripten_set_main_loopDel emscripten_set_main_loop = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate EMSCRIPTEN_RESULT emscripten_set_resize_callback_on_threadDel(IntPtrByte target, IntPtrEmscriptenCanvasSizeChangeData userData, int useCapture, em_ui_callback_func callback, IntPtr thread);
    public emscripten_set_resize_callback_on_threadDel emscripten_set_resize_callback_on_thread = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void custom_emscripten_get_display_usable_boundsDel(IntPtrInt width, IntPtrInt height);
    public custom_emscripten_get_display_usable_boundsDel custom_emscripten_get_display_usable_bounds = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void custom_emscripten_set_element_style_sizeDel(IntPtrByte element_id, int width, int height);
    public custom_emscripten_set_element_style_sizeDel custom_emscripten_set_element_style_size = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void custom_emscripten_open_linkDel(IntPtrByte url);
    public custom_emscripten_open_linkDel custom_emscripten_open_link = null!;
}