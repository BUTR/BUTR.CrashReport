using BUTR.CrashReport.ImGui.Structures;

using System.Diagnostics;
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
        fixed (byte* utf8DataPtr = utf8Data)
        {
            var ptrStart = utf8DataPtr;
            var ptrEnd = (byte*) Unsafe.Add<byte>(ptrStart, utf8Data.Length - 1);
            Debug.Assert(*ptrEnd == 0, "string must be null-terminated");
            ImGui.ImDrawList_AddText_Vec2(NativePtr, pos, col, ptrStart, ptrEnd);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddRect(ref readonly Vector2 min, ref readonly Vector2 max, uint col, float rounding) =>
        ImGui.ImDrawList_AddRect(NativePtr, min, max, col, rounding, ImGuiNET.ImDrawFlags.None, 1f);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddRectFilled(ref readonly Vector2 min, ref readonly Vector2 max, uint col, float rounding) =>
        ImGui.ImDrawList_AddRectFilled(NativePtr, min, max, col, rounding, ImGuiNET.ImDrawFlags.None);

    public void Dispose() { }
}