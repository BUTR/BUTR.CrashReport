using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace BUTR.CrashReport.Memory.Utils;

public static unsafe class Utf8Utils
{
    private static readonly byte[] EmptyUtf8 = "\0"u8.ToArray();
    private static readonly LiteralSpan<byte> EmptyUtf8Span = "\0"u8;

    public static string ToString(ReadOnlySpan<byte> utf8)
    {
        if (utf8.IsEmpty)
            return string.Empty;

#if NET6_0_OR_GREATER
        return Encoding.UTF8.GetString(utf8);
#else
        return Encoding.UTF8.GetString((byte*) Unsafe.AsPointer(ref MemoryMarshal.GetReference(utf8)), utf8.Length);
#endif
    }

    public static int Utf16ToUtf8(string utf16, Span<byte> utf8)
    {
        if (string.IsNullOrEmpty(utf16) || utf8.IsEmpty)
            return 1;

#if NET6_0_OR_GREATER
        return Encoding.UTF8.GetBytes(utf16, utf8);
#else
        fixed (char* utf16Ptr = utf16)
        {
            return Encoding.UTF8.GetBytes(
                utf16Ptr,
                utf16.Length,
                (byte*) Unsafe.AsPointer(ref MemoryMarshal.GetReference(utf8)),
                utf8.Length);
        }
#endif
    }

    public static int Utf16ToUtf8(ReadOnlySpan<char> utf16, Span<byte> utf8)
    {
        if (utf16.IsEmpty || utf8.IsEmpty)
            return 1;

#if NET6_0_OR_GREATER
        return Encoding.UTF8.GetBytes(utf16, utf8);
#else
        fixed (char* utf16Ptr = utf16)
        {
            return Encoding.UTF8.GetBytes(
                utf16Ptr,
                utf16.Length,
                (byte*) Unsafe.AsPointer(ref MemoryMarshal.GetReference(utf8)),
                utf8.Length);
        }
#endif
    }

    public static byte[] ToUtf8Array(string value)
    {
        if (string.IsNullOrEmpty(value))
            return EmptyUtf8;

        var length = Encoding.UTF8.GetByteCount(value) + 1;
#if NET6_0_OR_GREATER
        var array = GC.AllocateUninitializedArray<byte>(length);
#else
        var array = new byte[length];
#endif

        var charSpan = value.AsSpan();
        var arraySpan = array.AsSpan();

        var lengthWritten = Utf16ToUtf8(charSpan, arraySpan);
        array[lengthWritten] = 0;
        return array;
    }

    public static byte[] ToUtf8Array(ReadOnlySpan<char> value)
    {
        if (value.IsEmpty)
            return EmptyUtf8;

        var length = Encoding.UTF8.GetMaxByteCount(value.Length) + 1;
#if NET6_0_OR_GREATER
        var array = GC.AllocateUninitializedArray<byte>(length);
#else
        var array = new byte[length];
#endif

        var charSpan = value;
        var arraySpan = array.AsSpan();

        var lengthWritten = Utf16ToUtf8(charSpan, arraySpan);
        array[lengthWritten] = 0;
        return array;
    }

    public static string Utf8ToUtf16(Span<byte> utf8)
    {
        if (utf8.IsEmpty)
            return string.Empty;

#if NET6_0_OR_GREATER
        return Encoding.UTF8.GetString(utf8);
#else
        fixed (byte* utf8Ptr = utf8)
        {
            return Encoding.UTF8.GetString(utf8Ptr, utf8.Length);
        }
#endif
    }
}