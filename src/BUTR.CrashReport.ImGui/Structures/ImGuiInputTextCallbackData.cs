using BUTR.CrashReport.ImGui.Enums;

namespace BUTR.CrashReport.ImGui.Structures;

public ref struct ImGuiInputTextCallbackData<TData> where TData : struct
{
    public ImGuiInputTextFlags EventFlag;
    public ImGuiInputTextFlags Flags;
    public ushort EventChar;
    public ImGuiKey EventKey;
    public int CursorPos;
    public int SelectionStart;
    public int SelectionEnd;

    public TData Data;
}
public ref struct ImGuiInputTextCallbackInt64Data
{
    public ImGuiInputTextFlags EventFlag;
    public ImGuiInputTextFlags Flags;
    public ushort EventChar;
    public ImGuiKey EventKey;
    public int CursorPos;
    public int SelectionStart;
    public int SelectionEnd;

    public Int64 Data;
}