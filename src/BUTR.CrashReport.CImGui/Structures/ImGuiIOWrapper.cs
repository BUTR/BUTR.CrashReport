using BUTR.CrashReport.ImGui.Structures;

using System.Numerics;
using System.Runtime.CompilerServices;

namespace ImGui.Structures;

public readonly unsafe struct ImGuiIOWrapper : IImGuiIO
{
    private CmGui ImGui { get; }

    public ImGuiNET.ImGuiIO* NativePtr { get; }

    public ImGuiIOWrapper(CmGui imGui, ImGuiNET.ImGuiIO* nativePtr)
    {
        ImGui = imGui;
        NativePtr = nativePtr;
    }

    public ref bool KeyCtrl => ref Unsafe.AsRef<bool>(&NativePtr->KeyCtrl);
    public ref bool KeyShift => ref Unsafe.AsRef<bool>(&NativePtr->KeyShift);
    public ref bool KeyAlt => ref Unsafe.AsRef<bool>(&NativePtr->KeyAlt);
    public ref bool KeySuper => ref Unsafe.AsRef<bool>(&NativePtr->KeySuper);
    public ref bool ConfigMacOSXBehaviors => ref Unsafe.AsRef<bool>(&NativePtr->ConfigMacOSXBehaviors);
    public ref bool WantCaptureKeyboard => ref Unsafe.AsRef<bool>(&NativePtr->WantCaptureKeyboard);
    public ref bool WantCaptureMouse => ref Unsafe.AsRef<bool>(&NativePtr->WantCaptureMouse);
    public ref bool WantTextInput => ref Unsafe.AsRef<bool>(&NativePtr->WantTextInput);

    public ref ImGuiNET.ImGuiConfigFlags ConfigFlags => ref Unsafe.AsRef<ImGuiNET.ImGuiConfigFlags>(&NativePtr->ConfigFlags);
    public ref ImGuiNET.ImGuiBackendFlags BackendFlags => ref Unsafe.AsRef<ImGuiNET.ImGuiBackendFlags>(&NativePtr->BackendFlags);
    public ref Vector2 DisplaySize => ref Unsafe.AsRef<Vector2>(&NativePtr->DisplaySize);
    public ref float DeltaTime => ref Unsafe.AsRef<float>(&NativePtr->DeltaTime);
    public ref Vector2 DisplayFramebufferScale => ref Unsafe.AsRef<Vector2>(&NativePtr->DisplayFramebufferScale);
    public ref bool MouseDrawCursor => ref Unsafe.AsRef<bool>(&NativePtr->MouseDrawCursor);
    public ref bool WantSetMousePos => ref Unsafe.AsRef<bool>(&NativePtr->WantSetMousePos);
    public ref Vector2 MousePos => ref Unsafe.AsRef<Vector2>(&NativePtr->MousePos);
    public ref float MouseWheel => ref Unsafe.AsRef<float>(&NativePtr->MouseWheel);
    public ref float MouseWheelH => ref Unsafe.AsRef<float>(&NativePtr->MouseWheelH);

    public void GetFonts(out ImFontAtlasWrapper fonts) => fonts = new(ImGui, NativePtr->Fonts);
    public void GetMouseDown(out RangeAccessorRef<bool, ImGuiNET.ImGuiMouseButton> mouseDown) => mouseDown = new(NativePtr->MouseDown, ImGuiNET.ImGuiMouseButton.COUNT);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddInputCharacter(uint c) => ImGui.ImGuiIO_AddInputCharacter(NativePtr, c);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddInputCharacterUTF8(byte* c) => ImGui.ImGuiIO_AddInputCharactersUTF8(NativePtr, c);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddKeyEvent(ImGuiNET.ImGuiKey key, bool down) => ImGui.ImGuiIO_AddKeyEvent(NativePtr, key, Unsafe.As<bool, byte>(ref down));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetKeyEventNativeData(ImGuiNET.ImGuiKey key, int native_keycode, int native_scancode, int native_legacy_index = -1) => ImGui.ImGuiIO_SetKeyEventNativeData(NativePtr, key, native_keycode, native_scancode, native_legacy_index);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddMouseButtonEvent(ImGuiNET.ImGuiMouseButton button, bool down) => ImGui.ImGuiIO_AddMouseButtonEvent(NativePtr, button, Unsafe.As<bool, byte>(ref down));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddMouseWheelEvent(float wheel_x, float wheel_y) => ImGui.ImGuiIO_AddMouseWheelEvent(NativePtr, wheel_x, wheel_y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddMousePosEvent(float x, float y) => ImGui.ImGuiIO_AddMousePosEvent(NativePtr, x, y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddFocusEvent(bool focused) => ImGui.ImGuiIO_AddFocusEvent(NativePtr, Unsafe.As<bool, byte>(ref focused));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Destroy() => ImGui.ImGuiIO_destroy(NativePtr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose() => Destroy();
}