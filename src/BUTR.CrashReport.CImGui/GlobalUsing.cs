#if NET6_0_OR_GREATER
global using IntPtrByte = BUTR.CrashReport.Native.Pointer<byte>;
global using IntPtrInt32 = BUTR.CrashReport.Native.Pointer<int>;
global using IntPtrIntPtr = BUTR.CrashReport.Native.Pointer<System.IntPtr>;
global using IntPtrVoid = BUTR.CrashReport.Native.Pointer;
global using IntPtrVector2 = BUTR.CrashReport.Native.Pointer<System.Numerics.Vector2>;
global using IntPtrVector4 = BUTR.CrashReport.Native.Pointer<System.Numerics.Vector4>;
global using IntPtrImDrawData = BUTR.CrashReport.Native.Pointer<ImGuiNET.ImDrawData>;
global using IntPtrImGuiIO = BUTR.CrashReport.Native.Pointer<ImGuiNET.ImGuiIO>;
global using IntPtrImGuiViewport = BUTR.CrashReport.Native.Pointer<ImGuiNET.ImGuiViewport>;
global using IntPtrImGuiStyle = BUTR.CrashReport.Native.Pointer<ImGuiNET.ImGuiStyle>;
global using IntPtrImFont = BUTR.CrashReport.Native.Pointer<ImGuiNET.ImFont>;
global using IntPtrImFontAtlas = BUTR.CrashReport.Native.Pointer<ImGuiNET.ImFontAtlas>;
global using IntPtrImFontConfig = BUTR.CrashReport.Native.Pointer<ImGuiNET.ImFontConfig>;
global using IntPtrImDrawList = BUTR.CrashReport.Native.Pointer<ImGuiNET.ImDrawList>;
global using IntPtrImGuiListClipper = BUTR.CrashReport.Native.Pointer<ImGuiNET.ImGuiListClipper>;
#else
global using IntPtrByte = BUTR.CrashReport.Native.Pointer;
global using IntPtrImDrawData = BUTR.CrashReport.Native.Pointer;
global using IntPtrImDrawList = BUTR.CrashReport.Native.Pointer;
global using IntPtrImFont = BUTR.CrashReport.Native.Pointer;
global using IntPtrImFontAtlas = BUTR.CrashReport.Native.Pointer;
global using IntPtrImFontConfig = BUTR.CrashReport.Native.Pointer;
global using IntPtrImGuiIO = BUTR.CrashReport.Native.Pointer;
global using IntPtrImGuiListClipper = BUTR.CrashReport.Native.Pointer;
global using IntPtrImGuiStyle = BUTR.CrashReport.Native.Pointer;
global using IntPtrImGuiViewport = BUTR.CrashReport.Native.Pointer;
global using IntPtrInt32 = BUTR.CrashReport.Native.Pointer;
global using IntPtrIntPtr = BUTR.CrashReport.Native.Pointer;
global using IntPtrVector2 = BUTR.CrashReport.Native.Pointer;
global using IntPtrVector4 = BUTR.CrashReport.Native.Pointer;
global using IntPtrVoid = BUTR.CrashReport.Native.Pointer;
#endif