using BUTR.CrashReport.ImGui.Structures;

using System.Numerics;
using System.Runtime.CompilerServices;

namespace ImGui.Structures;

public readonly unsafe struct ImGuiViewportWrapper : IImGuiViewport
{
    private CmGui ImGui { get; }

    public ImGuiNET.ImGuiViewport* NativePtr { get; }

    public ImGuiViewportWrapper(CmGui imGui, ImGuiNET.ImGuiViewport* nativePtr)
    {
        ImGui = imGui;
        NativePtr = nativePtr;
    }

    public ref uint ID => ref Unsafe.AsRef<uint>(&NativePtr->ID);
    public ref Vector2 WorkPos => ref Unsafe.AsRef<Vector2>(&NativePtr->WorkPos);
    public ref Vector2 WorkSize => ref Unsafe.AsRef<Vector2>(&NativePtr->WorkSize);
}