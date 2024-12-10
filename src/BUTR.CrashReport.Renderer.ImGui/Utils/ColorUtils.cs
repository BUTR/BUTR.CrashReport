using System.Numerics;
using System.Runtime.CompilerServices;

namespace BUTR.CrashReport.Renderer.ImGui.Utils;

internal static class ColorUtils
{
    private const MethodImplOptions AggressiveOptimization = (MethodImplOptions) 512;

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public static Vector4 FromColor(byte r, byte g, byte b, byte a) => new((float) r / 255f, (float) g / 255f, (float) b / 255f, (float) a / 255f);
}