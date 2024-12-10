using System.Numerics;

namespace BUTR.CrashReport.ImGui.Structures;

public interface IImDrawList : IDisposable
{
    void AddText(ref readonly Vector2 pos, uint col, ReadOnlySpan<byte> utf8Data);

    void AddRect(ref readonly Vector2 min, ref readonly Vector2 max, uint col, float rounding);
    void AddRectFilled(ref readonly Vector2 min, ref readonly Vector2 _max, uint col, float rounding);
}