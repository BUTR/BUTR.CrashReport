using BUTR.CrashReport.ImGui.Structures;

namespace BUTR.CrashReport.ImGui;

public interface IImGuiWithImGuiIO<TImGuiIORef>
    where TImGuiIORef : IImGuiIO
{
    void GetIO(out TImGuiIORef ioRef);
}