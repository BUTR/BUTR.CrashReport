using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ImGui.Structures;

[StructLayout(LayoutKind.Sequential)]
public readonly unsafe struct ImVectorRefImDrawVert
{
    public readonly int Size;
    public readonly int Capacity;
    public readonly ImGuiNET.ImDrawVert* Data;
    public UIntPtr SizeInBytes => (UIntPtr) (Size * sizeof(ImGuiNET.ImDrawVert));

    public ImVectorRefImDrawVert(ImGuiNET.ImVector vector)
    {
        Size = vector.Size;
        Capacity = vector.Capacity;
        Data = (ImGuiNET.ImDrawVert*) vector.Data;
    }

    public ImVectorRefImDrawVert(int size, int capacity, IntPtr data)
    {
        Size = size;
        Capacity = capacity;
        Data = (ImGuiNET.ImDrawVert*) data;
    }

    public ref ImGuiNET.ImDrawVert this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            Guard.ThrowIndexOutOfRangeException(index, Size);
            return ref *(Data + index);
        }
    }

    public Span<ImGuiNET.ImDrawVert> AsSpan() => new(Data, Size);
}