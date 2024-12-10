using BUTR.CrashReport.ImGui.Structures;

using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ImGui.Structures;

public readonly unsafe struct ImDrawListWrapper : IImDrawList
{
    private CmGui ImGui { get; }

    public ImGuiNET.ImDrawList* NativePtr { get; }

    public ImDrawListWrapper(CmGui imGui, ImGuiNET.ImDrawList* nativePtr)
    {
        ImGui = imGui;
        NativePtr = nativePtr;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddText(ref readonly Vector2 pos, uint col, ReadOnlySpan<byte> utf8Data)
    {
        var ptrStart = (byte*) Unsafe.AsPointer(ref MemoryMarshal.GetReference(utf8Data));
        var endPtr = (byte*) Unsafe.Add<byte>(ptrStart, utf8Data.Length - 1);
        ImGui.ImDrawList_AddText_Vec2(NativePtr, pos, col, ptrStart, endPtr);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddRect(ref readonly Vector2 min, ref readonly Vector2 max, uint col, float rounding) =>
        ImGui.ImDrawList_AddRect(NativePtr, min, max, col, rounding, ImGuiNET.ImDrawFlags.None, 1f);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddRectFilled(ref readonly Vector2 min, ref readonly Vector2 max, uint col, float rounding) =>
        ImGui.ImDrawList_AddRectFilled(NativePtr, min, max, col, rounding, ImGuiNET.ImDrawFlags.None);

    public void Dispose() { }
}