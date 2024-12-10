using BUTR.CrashReport.ImGui.Structures;

namespace BUTR.CrashReport.ImGui;

public interface IImGuiWithImDrawList<TImDrawList>
    where TImDrawList : IImDrawList
{
    void GetWindowDrawList(out TImDrawList drawListRef);
}