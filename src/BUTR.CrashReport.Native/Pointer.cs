using System.Runtime.CompilerServices;

namespace BUTR.CrashReport.Native;

public readonly unsafe record struct Pointer
{
    public static implicit operator Pointer(void* ptr) => new(ptr);
    public static implicit operator void*(Pointer ptr) => (void*) ptr._value;
    public static implicit operator Pointer(IntPtr ptr) => new(ptr.ToPointer());
    public static implicit operator IntPtr(Pointer ptr) => ptr._value;

    private readonly nint _value;

    private Pointer(void* ptr) => _value = (nint) ptr;
}

public readonly unsafe record struct Pointer<T> where T : unmanaged
{
    public static explicit operator IntPtr(Pointer<T> intPtr) => Unsafe.As<Pointer<T>, IntPtr>(ref intPtr);
    public static implicit operator Pointer<T>(IntPtr intPtr) => Unsafe.As<IntPtr, Pointer<T>>(ref intPtr);

    public static implicit operator Pointer<T>(T* ptr) => new(ptr);
    public static explicit operator T*(Pointer<T> ptr) => (T*) ptr._value;

    private readonly nint _value;

    private Pointer(T* ptr) => _value = (nint) ptr;
}