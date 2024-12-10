using System.Runtime.InteropServices;

namespace ImGui.Structures;

[StructLayout(LayoutKind.Sequential)]
public ref struct Vector2Ref(float X, float Y)
{
    public float X = X;
    public float Y = Y;
}