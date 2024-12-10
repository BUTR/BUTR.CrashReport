using BUTR.CrashReport.ImGui.Enums;
using BUTR.CrashReport.ImGui.Structures;

using System.Numerics;

namespace BUTR.CrashReport.ImGui;

public interface IImGui<TImGuiIO, TImGuiViewport, TImDrawList, TImGuiStyle, out TColorsRangeAccessor, TImGuiListClipper> : IImGui,
    IImGuiWithImGuiIO<TImGuiIO>,
    IImGuiWithImGuiViewport<TImGuiViewport>,
    IImGuiWithImDrawList<TImDrawList>,
    IImGuiWithImGuiStyle<TImGuiStyle, TColorsRangeAccessor>,
    IImGuiWithImGuiListClipper<TImGuiListClipper>

    where TImGuiIO : IImGuiIO

    where TImGuiViewport : IImGuiViewport

    where TImDrawList : IImDrawList

    where TImGuiStyle : IImGuiStyle<TColorsRangeAccessor>
    where TColorsRangeAccessor : IRangeAccessor<Vector4, ImGuiCol>

    where TImGuiListClipper : IImGuiListClipper;