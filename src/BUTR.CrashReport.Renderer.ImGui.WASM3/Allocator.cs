using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ImGuiGeneral;

internal class Allocator : IDisposable
{
    private readonly List<IntPtr> _allocated = new();
    
    public unsafe void* Alloc(int size)
    {
        var ptr = NativeMemory.Alloc((UIntPtr) size);
        _allocated.Add(new IntPtr(ptr));
        return ptr;
    }
    
    public unsafe T* Alloc<T>(T value) where T : unmanaged
    {
        var ptr = (T*) NativeMemory.Alloc((UIntPtr) Unsafe.SizeOf<T>());
        _allocated.Add(new IntPtr(ptr));
        Unsafe.Write(ptr, value);
        return ptr;
    }
    
    public unsafe T* Alloc<T>(ReadOnlySpan<T> value) where T : unmanaged
    {
        var ptr = (T*) NativeMemory.Alloc((UIntPtr) Unsafe.SizeOf<T>());
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