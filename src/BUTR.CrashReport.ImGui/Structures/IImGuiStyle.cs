using BUTR.CrashReport.ImGui.Enums;

using System.Numerics;

namespace BUTR.CrashReport.ImGui.Structures;

public interface IImGuiStyle<TColorsRangeAccessor> : IDisposable
    where TColorsRangeAccessor : IRangeAccessor<Vector4, ImGuiCol>
{
    ref float Alpha { get; }
    void GetColors(out TColorsRangeAccessor colors);
}