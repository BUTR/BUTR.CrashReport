// ReSharper disable RedundantUsingDirective.Global
global using EMSCRIPTEN_RESULT = int;

global using UnusedI = int;
// ReSharper restore RedundantUsingDirective.Global

#if NET6_0_OR_GREATER
global using IntPtrByte = BUTR.CrashReport.Native.Pointer<byte>;
global using IntPtrInt = BUTR.CrashReport.Native.Pointer<int>;
global using IntPtrEmscriptenCanvasSizeChangeData = BUTR.CrashReport.Native.Pointer<Emscripten.EmscriptenCanvasSizeChangeData>;
global using unsafe em_ui_callback_func = delegate* unmanaged[Cdecl]<int, Emscripten.EmscriptenUiEvent*, Emscripten.EmscriptenCanvasSizeChangeData*, int>;
global using unsafe em_arg_callback_func = delegate* unmanaged[Cdecl]<void>;
#else
global using IntPtrByte = BUTR.CrashReport.Native.Pointer;
global using IntPtrInt = BUTR.CrashReport.Native.Pointer;
global using IntPtrEmscriptenCanvasSizeChangeData = BUTR.CrashReport.Native.Pointer;
global using em_ui_callback_func = BUTR.CrashReport.Native.Pointer;
global using em_arg_callback_func = BUTR.CrashReport.Native.Pointer;
#endif