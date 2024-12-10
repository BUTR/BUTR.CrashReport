using BUTR.CrashReport.ImGui.Structures;

namespace BUTR.CrashReport.ImGui;

public interface IImGuiWithImGuiListClipper<TImGuiListClipperRef>
    where TImGuiListClipperRef : IImGuiListClipper
{
    void CreateImGuiListClipper(out TImGuiListClipperRef listClipperRef);
}