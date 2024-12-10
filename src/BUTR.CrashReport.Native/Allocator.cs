using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BUTR.CrashReport.Native;

public sealed class Allocator : IDisposable
{
    private readonly List<IntPtr> _allocated = new();

    public unsafe void* Alloc(int size)
    {
#if NETSTANDARD2_0
        var ptr = (void*) Marshal.AllocHGlobal(size);
#else
        var ptr = NativeMemory.Alloc((UIntPtr) size);
#endif
        _allocated.Add(new IntPtr(ptr));
        return ptr;
    }

    public unsafe T* Alloc<T>(T value) where T : unmanaged
    {
#if NETSTANDARD2_0
        var ptr = (T*) Marshal.AllocHGlobal(Unsafe.SizeOf<T>());
#else
        var ptr = (T*) NativeMemory.Alloc((UIntPtr) Unsafe.SizeOf<T>());
#endif
        _allocated.Add(new IntPtr(ptr));
        Unsafe.Write(ptr, value);
        return ptr;
    }

    public unsafe T* Alloc<T>(ReadOnlySpan<T> value) where T : unmanaged
    {
#if NETSTANDARD2_0
        var ptr = (T*) Marshal.AllocHGlobal(Unsafe.SizeOf<T>());
#else
        var ptr = (T*) NativeMemory.Alloc((UIntPtr) Unsafe.SizeOf<T>());
#endif
        _allocated.Add(new IntPtr(ptr));
        value.CopyTo(new Span<T>(ptr, value.Length));
        return ptr;
    }

    public void Dispose()
    {
        foreach (var ptr in _allocated)
        {
            Marshal.FreeHGlobal(ptr);
        }
    }
}