using BUTR.CrashReport.ImGui.Enums;
using BUTR.CrashReport.ImGui.Structures;

using System.Numerics;

namespace BUTR.CrashReport.ImGui;

public interface IImGuiWithImGuiStyle<TImGuiStyleRef, out TColorsRangeAccessorRef>
    where TImGuiStyleRef : IImGuiStyle<TColorsRangeAccessorRef>
    where TColorsRangeAccessorRef : IRangeAccessor<Vector4, ImGuiCol>
{
    void GetStyle(out TImGuiStyleRef styleRef);
}