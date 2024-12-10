using BUTR.CrashReport.ImGui;
using BUTR.CrashReport.ImGui.Enums;

using ImGui.Structures;

using System.Numerics;
using System.Runtime.CompilerServices;

namespace ImGui;

public partial class CmGui : IImGui<ImGuiIOWrapper, ImGuiViewportWrapper, ImDrawListWrapper, ImGuiStyleWrapper, RangeAccessorRef<Vector4, ImGuiCol>, ImGuiListClipperWrapper>
{
    private const MethodImplOptions AggressiveOptimization = (MethodImplOptions) 512;

    private static readonly Vector2 Zero2 = Vector2.Zero;
    private static readonly Vector3 Zero3 = Vector3.Zero;
    private static readonly Vector4 Zero4 = Vector4.Zero;

    public void Dispose() { }
}