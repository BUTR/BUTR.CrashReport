using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ImGui.Structures;

[StructLayout(LayoutKind.Sequential)]
public readonly unsafe struct ImVectorRefUInt16
{
    public readonly int Size;
    public readonly int Capacity;
    public readonly UInt16* Data;
    public UIntPtr SizeInBytes => (UIntPtr) (Size * sizeof(UInt16));

    public ImVectorRefUInt16(ImGuiNET.ImVector vector)
    {
        Size = vector.Size;
        Capacity = vector.Capacity;
        Data = (UInt16*) vector.Data;
    }

    public ImVectorRefUInt16(int size, int capacity, IntPtr data)
    {
        Size = size;
        Capacity = capacity;
        Data = (UInt16*) data;
    }

    public ref UInt16 this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            Guard.ThrowIndexOutOfRangeException(index, Size);
            return ref *(Data + index);
        }
    }

    public Span<UInt16> AsSpan() => new(Data, Size);
}