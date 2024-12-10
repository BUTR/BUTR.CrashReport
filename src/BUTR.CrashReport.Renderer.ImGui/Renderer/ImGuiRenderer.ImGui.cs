using BUTR.CrashReport.Renderer.ImGui.Utils;

using System.Numerics;

namespace BUTR.CrashReport.Renderer.ImGui.Renderer;

public partial class ImGuiRenderer
{
    protected static readonly Vector2 Zero2 = Vector2.Zero;
    protected static readonly Vector3 Zero3 = Vector3.Zero;
    protected static readonly Vector4 Zero4 = Vector4.Zero;

    protected static readonly Vector4 Black = ColorUtils.FromColor(0, 0, 0, 255);
    protected static readonly Vector4 White = ColorUtils.FromColor(255, 255, 255, 255);

    protected static readonly Vector4 Debug = ColorUtils.FromColor(54, 96, 146, 255);
    protected static readonly Vector4 Info = ColorUtils.FromColor(0, 125, 60, 255);
    protected static readonly Vector4 Warn = ColorUtils.FromColor(225, 125, 50, 255);
    protected static readonly Vector4 Error = ColorUtils.FromColor(240, 0, 0, 255);
    protected static readonly Vector4 Fatal = ColorUtils.FromColor(190, 0, 0, 255);

    protected static readonly Vector4 LightBackground = ColorUtils.FromColor(236, 236, 236, 255);
    protected static readonly Vector4 LightChildBackground = ColorUtils.FromColor(255, 255, 255, 255);

    protected static readonly Vector4 DarkBackground = ColorUtils.FromColor(30, 30, 30, 255);
    protected static readonly Vector4 DarkChildBackground = ColorUtils.FromColor(43, 43, 43, 255);

    protected static readonly Vector4 LightPlugin = ColorUtils.FromColor(255, 255, 224, 255);
    protected static readonly Vector4 DarkPlugin = ColorUtils.FromColor(30, 30, 61, 255);

    protected static readonly Vector4 LightOfficialModule = ColorUtils.FromColor(244, 252, 220, 255);
    protected static readonly Vector4 DarkOfficialModule = ColorUtils.FromColor(50, 50, 56, 255);

    protected static readonly Vector4 LightUnofficialModule = ColorUtils.FromColor(255, 255, 224, 255);
    protected static readonly Vector4 DarkUnofficialModule = ColorUtils.FromColor(53, 53, 53, 255);

    protected static readonly Vector4 LightExternalModule = ColorUtils.FromColor(255, 255, 224, 255);
    protected static readonly Vector4 DarkExternalModule = ColorUtils.FromColor(30, 30, 61, 255);

    protected static readonly Vector4 LightSubModule = ColorUtils.FromColor(248, 248, 231, 255);
    protected static readonly Vector4 DarkSubModule = ColorUtils.FromColor(70, 70, 70, 255);

    protected static readonly Vector4 Primary = ColorUtils.FromColor(151, 129, 082, 135);
    protected static readonly Vector4 Primary2 = ColorUtils.FromColor(151, 129, 082, 180);
    protected static readonly Vector4 Primary3 = ColorUtils.FromColor(151, 129, 082, 210);
    protected static readonly Vector4 Secondary = ColorUtils.FromColor(255, 230, 128, 135);
    protected static readonly Vector4 Secondary2 = ColorUtils.FromColor(255, 230, 128, 180);
    protected static readonly Vector4 Secondary3 = ColorUtils.FromColor(255, 230, 128, 210);
    //protected static readonly Vector4 PrimaryActive = ColorUtils.FromColor(174, 137, 59, 180);
}