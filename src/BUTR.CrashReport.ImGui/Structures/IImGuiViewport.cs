using System.Numerics;

namespace BUTR.CrashReport.ImGui.Structures;

public interface IImGuiViewport
{
    ref uint ID { get; }
    ref Vector2 WorkPos { get; }
    ref Vector2 WorkSize { get; }
}