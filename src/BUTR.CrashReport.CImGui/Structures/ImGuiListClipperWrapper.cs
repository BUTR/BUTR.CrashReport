using BUTR.CrashReport.ImGui.Structures;

using System.Runtime.CompilerServices;

namespace ImGui.Structures;

public readonly unsafe struct ImGuiListClipperWrapper : IImGuiListClipper
{
    private CmGui ImGui { get; }

    public ImGuiNET.ImGuiListClipper* NativePtr { get; }

    public ImGuiListClipperWrapper(CmGui imGui, ImGuiNET.ImGuiListClipper* nativePtr)
    {
        ImGui = imGui;
        NativePtr = nativePtr;
    }

    public ref IntPtr Ctx => ref Unsafe.AsRef<IntPtr>(&NativePtr->Ctx);
    public ref int DisplayStart => ref Unsafe.AsRef<int>(&NativePtr->DisplayStart);
    public ref int DisplayEnd => ref Unsafe.AsRef<int>(&NativePtr->DisplayEnd);
    public ref int ItemsCount => ref Unsafe.AsRef<int>(&NativePtr->ItemsCount);
    public ref float ItemsHeight => ref Unsafe.AsRef<float>(&NativePtr->ItemsHeight);
    public ref float StartPosY => ref Unsafe.AsRef<float>(&NativePtr->StartPosY);
    public ref double StartSeekOffsetY => ref Unsafe.AsRef<double>(&NativePtr->StartSeekOffsetY);
    public IntPtr TempData { get => (IntPtr) NativePtr->TempData; set => NativePtr->TempData = (void*) value; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Begin(int items_count)
    {
        float items_height = -1f;
        ImGui.ImGuiListClipper_Begin(NativePtr, items_count, items_height);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Begin(int items_count, float items_height) => ImGui.ImGuiListClipper_Begin(NativePtr, items_count, items_height);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void End() => ImGui.ImGuiListClipper_End(NativePtr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Step() => ImGui.ImGuiListClipper_Step(NativePtr) > 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Destroy() => ImGui.ImGuiListClipper_destroy(NativePtr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose() => Destroy();
}