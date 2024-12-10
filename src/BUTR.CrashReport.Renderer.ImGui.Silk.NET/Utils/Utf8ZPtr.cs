using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BUTR.CrashReport.Renderer.ImGui.Implementation.CImGui.Utils;

/// <summary>
/// Null-terminated UTF-8 string pointer.
/// </summary>
internal readonly unsafe ref struct Utf8ZPtr
{
    private readonly void* NativePtr;
    public ref readonly byte Data => ref Unsafe.AsRef<byte>(NativePtr);

    public Utf8ZPtr(ref byte reference) { NativePtr = Unsafe.AsPointer(ref reference); }
    public Utf8ZPtr(ref readonly ReadOnlySpan<byte> span) : this(ref MemoryMarshal.GetReference(span)) { }
    public Utf8ZPtr(ref readonly Span<byte> span) : this(ref MemoryMarshal.GetReference(span)) { }
    public Utf8ZPtr(ref readonly byte[] array) : this(ref MemoryMarshal.GetReference(array.AsSpan())) { }
}