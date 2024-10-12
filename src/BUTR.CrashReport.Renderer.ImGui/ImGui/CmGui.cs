using BUTR.CrashReport.Renderer.ImGui.UnsafeUtils;

using System.Numerics;
using System.Runtime.CompilerServices;

namespace BUTR.CrashReport.Renderer.ImGui.ImGui;

internal partial class CmGui
{
    private const MethodImplOptions AggressiveOptimization = (MethodImplOptions) 512;

    private static readonly Vector2 Zero2 = Vector2.Zero;
    private static readonly Vector3 Zero3 = Vector3.Zero;
    private static readonly Vector4 Zero4 = Vector4.Zero;
}