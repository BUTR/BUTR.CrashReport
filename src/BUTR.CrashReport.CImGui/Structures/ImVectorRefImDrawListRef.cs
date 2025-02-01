using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ImGui.Structures;

[StructLayout(LayoutKind.Sequential)]
public readonly unsafe ref struct ImVectorRefImDrawListRef
{
    public readonly int Size;
    public readonly int Capacity;
    public readonly ImDrawListRef* Data;

    public ImVectorRefImDrawListRef(ImGuiNET.ImVector vector)
    {
        Size = vector.Size;
        Capacity = vector.Capacity;
        Data = (ImDrawListRef*) vector.Data;
    }

    public ImVectorRefImDrawListRef(int size, int capacity, IntPtr data)
    {
        Size = size;
        Capacity = capacity;
        Data = (ImDrawListRef*) data;
    }

    public ref ImDrawListRef this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            Guard.ThrowIndexOutOfRangeException(index, Size);
            return ref *(Data + index);
        }
    }
}