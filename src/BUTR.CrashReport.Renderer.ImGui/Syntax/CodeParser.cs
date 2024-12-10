using BUTR.CrashReport.ImGui.Enums;
using BUTR.CrashReport.ImGui.Structures;
using BUTR.CrashReport.Renderer.ImGui.Utils;

using ImGuiColorTextEditNet;

using System.Numerics;

namespace BUTR.CrashReport.Renderer.ImGui.Syntax;

internal static class CodeParser
{
    private static readonly Vector4 ErrorMarker = ColorUtils.FromColor(142, 21, 25, 80);
    private static readonly Vector4 ErrorText = ColorUtils.FromColor(255, 51, 51, 255);

    public static ColorPaletteIndex BasePalette<TColors>(bool isDarkTheme, TColors colors) where TColors : IRangeAccessor<Vector4, ImGuiCol> => new()
    {
        [ColorPalette.Default] = colors[ImGuiCol.Text],
        [ColorPalette.Background] = colors[ImGuiCol.WindowBg],
        [ColorPalette.Cursor] = colors[ImGuiCol.Text],
        [ColorPalette.Selection] = colors[ImGuiCol.TextSelectedBg],
        [ColorPalette.ExecutingLine] = colors[ImGuiCol.NavWindowingHighlight],
        [ColorPalette.LineNumber] = colors[ImGuiCol.TextDisabled],
        [ColorPalette.CurrentLineFill] = colors[ImGuiCol.ButtonActive],
        [ColorPalette.CurrentLineFillInactive] = colors[ImGuiCol.Button],
        [ColorPalette.CurrentLineEdge] = colors[ImGuiCol.ButtonHovered],
        [ColorPalette.ErrorMarker] = ErrorMarker,
        [ColorPalette.ErrorText] = ErrorText,
    };
}