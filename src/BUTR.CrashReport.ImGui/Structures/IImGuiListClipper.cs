namespace BUTR.CrashReport.ImGui.Structures;

public interface IImGuiListClipper : IDisposable
{
    ref int DisplayStart { get; }
    ref int DisplayEnd { get; }

    void Begin(int items_count);
    void Begin(int items_count, float items_height);
    void End();
    bool Step();
}