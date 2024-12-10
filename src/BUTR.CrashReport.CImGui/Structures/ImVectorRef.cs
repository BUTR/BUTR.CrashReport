using System.Runtime.CompilerServices;

namespace ImGui.Structures;

public readonly unsafe ref struct ImVectorRef<T> where T : unmanaged
{
    public readonly int Size;
    public readonly int Capacity;
    public readonly T* Data;

    public ImVectorRef(ImGuiNET.ImVector vector)
    {
        Size = vector.Size;
        Capacity = vector.Capacity;
        Data = (T*) vector.Data;
    }

    public ImVectorRef(int size, int capacity, IntPtr data)
    {
        Size = size;
        Capacity = capacity;
        Data = (T*) data;
    }

    public ref T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            Guard.ThrowIndexOutOfRangeException(index, Size);
            return ref Unsafe.Add(ref Unsafe.AsRef<T>(Data), index);
        }
    }
}