using System.Runtime.InteropServices;

namespace ImGui.Structures;

[StructLayout(LayoutKind.Sequential)]
public ref struct Vector4Ref(float X, float Y, float Z, float W)
{
    public float X = X;
    public float Y = Y;
    public float Z = Z;
    public float W = W;
}