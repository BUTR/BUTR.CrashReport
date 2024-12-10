using System.Numerics;
using System.Runtime.CompilerServices;

namespace ImGui.Structures;

public readonly unsafe ref struct ImDrawDataWrapper
{
    private CmGui ImGui { get; }

    public ImGuiNET.ImDrawData* NativePtr { get; }

    public ImDrawDataWrapper(CmGui imGui, ImGuiNET.ImDrawData* nativePtr)
    {
        ImGui = imGui;
        NativePtr = nativePtr;
    }

    public ref bool Valid => ref Unsafe.AsRef<bool>(&NativePtr->Valid);
    public ref int CmdListsCount => ref Unsafe.AsRef<int>(&NativePtr->CmdListsCount);
    public ref int TotalIdxCount => ref Unsafe.AsRef<int>(&NativePtr->TotalIdxCount);
    public ref int TotalVtxCount => ref Unsafe.AsRef<int>(&NativePtr->TotalVtxCount);
    public ref Vector2 DisplayPos => ref Unsafe.AsRef<Vector2>(&NativePtr->DisplayPos);
    public ref Vector2 DisplaySize => ref Unsafe.AsRef<Vector2>(&NativePtr->DisplaySize);
    public ref Vector2 FramebufferScale => ref Unsafe.AsRef<Vector2>(&NativePtr->FramebufferScale);

    public void GetOwnerViewport(out ImGuiViewportWrapper ownerViewport) => ownerViewport = new(ImGui, NativePtr->OwnerViewport);
    public void GetCmdLists(out ImVectorRefImDrawListRef cmdLists) => cmdLists = new(NativePtr->CmdLists);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddDrawList(ImGuiNET.ImDrawListPtr drawList) => ImGui.ImDrawData_AddDrawList(NativePtr, drawList.NativePtr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DeIndexAllBuffers() => ImGui.ImDrawData_DeIndexAllBuffers(NativePtr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ScaleClipRects(Vector2 fbScale) => ImGui.ImDrawData_ScaleClipRects(NativePtr, fbScale);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() => ImGui.ImDrawData_Clear(NativePtr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Destroy() => ImGui.ImDrawData_destroy(NativePtr);

    public void Dispose() => Destroy();
}