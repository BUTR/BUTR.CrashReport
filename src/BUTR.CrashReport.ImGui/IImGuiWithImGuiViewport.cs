using BUTR.CrashReport.ImGui.Structures;

namespace BUTR.CrashReport.ImGui;

public interface IImGuiWithImGuiViewport<TImGuiViewportRef>
    where TImGuiViewportRef : IImGuiViewport
{
    void GetMainViewport(out TImGuiViewportRef viewportRef);
}