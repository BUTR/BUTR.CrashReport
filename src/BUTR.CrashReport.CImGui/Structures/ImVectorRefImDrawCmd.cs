using System.Runtime.CompilerServices;

namespace ImGui.Structures;

public readonly unsafe ref struct ImVectorRefImDrawCmd
{
    public readonly int Size;
    public readonly int Capacity;
    public readonly ImGuiNET.ImDrawCmd* Data;

    public ImVectorRefImDrawCmd(ImGuiNET.ImVector vector)
    {
        Size = vector.Size;
        Capacity = vector.Capacity;
        Data = (ImGuiNET.ImDrawCmd*) vector.Data;
    }

    public ImVectorRefImDrawCmd(int size, int capacity, IntPtr data)
    {
        Size = size;
        Capacity = capacity;
        Data = (ImGuiNET.ImDrawCmd*) data;
    }

    public ref ImGuiNET.ImDrawCmd this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            Guard.ThrowIndexOutOfRangeException(index, Size);
            return ref *(Data + index);
        }
    }
}