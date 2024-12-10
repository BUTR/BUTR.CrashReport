using BUTR.CrashReport.ImGui.Structures;

using System.Runtime.CompilerServices;

namespace ImGui.Structures;

public readonly unsafe struct RangeAccessorRef<T, TEnum> : IRangeAccessor<T, TEnum> where T : struct where TEnum : Enum
{
    public readonly void* Data;
    public readonly TEnum Count;

    public RangeAccessorRef(IntPtr data, TEnum count) : this(data.ToPointer(), count) { }

    public RangeAccessorRef(void* data, TEnum count)
    {
        Data = data;
        Count = count;
    }

    public ref T this[TEnum index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            Guard.ThrowIndexOutOfRangeException(index, Count);
            return ref Unsafe.Add(ref Unsafe.AsRef<T>(Data), Unsafe.As<TEnum, int>(ref index));
        }
    }
}

public readonly struct RangeAccessorRef<T> : IRangeAccessor<T> where T : struct
{
    public readonly unsafe void* Data;
    public readonly int Count;

    public unsafe RangeAccessorRef(IntPtr data, int count) : this(data.ToPointer(), count) { }

    public unsafe RangeAccessorRef(void* data, int count)
    {
        Data = data;
        Count = count;
    }

    public unsafe ref T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            Guard.ThrowIndexOutOfRangeException(index, Count);
            return ref Unsafe.Add(ref Unsafe.AsRef<T>(Data), index);
        }
    }
}