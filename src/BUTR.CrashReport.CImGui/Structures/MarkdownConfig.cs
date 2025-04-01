using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ImGui.Structures;

[StructLayout(LayoutKind.Sequential)]
public readonly struct MarkdownLinkCallbackData
{
    public readonly IntPtrByte Text; // Pointer to text between []
    public readonly int TextLength;
    public readonly IntPtrByte Link; // Pointer to text between ()
    public readonly int LinkLength;
    public readonly IntPtrVoid UserData;
    public readonly byte IsImage;
}

[StructLayout(LayoutKind.Sequential)]
public readonly struct MarkdownTooltipCallbackData
{
    public readonly MarkdownLinkCallbackData LinkData;
    public readonly IntPtr LinkIcon;
}

[StructLayout(LayoutKind.Sequential)]
public ref struct MarkdownImageData
{
    public byte IsValid;
    public byte UseLinkCallback;
    public IntPtr UserTextureId; // Equivalent to ImTextureID
    public Vector2Ref Size;
    public Vector2Ref UV0;
    public Vector2Ref UV1;
    public Vector4Ref TintColor;
    public Vector4Ref BorderColor;
}

public enum MarkdownFormatType
{
    NormalText,
    Heading,
    UnorderedList,
    Link,
    Emphasis,
}

[StructLayout(LayoutKind.Sequential)]
public readonly struct MarkdownFormatInfo
{
    public readonly MarkdownFormatType Type;
    public readonly int Level;
    public readonly byte IsItemHovered;
    public readonly IntPtrMarkdownConfig Config; // Pointer to MarkdownConfig
}

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MarkdownLinkCallback(IntPtrMarkdownLinkCallbackData data);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MarkdownTooltipCallback(IntPtrMarkdownTooltipCallbackData data);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate MarkdownImageData MarkdownImageCallback(IntPtrMarkdownLinkCallbackData data);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MarkdownFormatCallback(IntPtrMarkdownFormatInfo markdownFormatInfo, byte isStart);

[StructLayout(LayoutKind.Sequential)]
public struct MarkdownHeadingFormat
{
    public IntPtrImFont Font; // ImFont* is an IntPtr in ImGui.NET
    public byte IsSeparator;
}


[StructLayout(LayoutKind.Sequential)]
public struct MarkdownConfig
{
    public IntPtr LinkCallback;
    public IntPtr TooltipCallback;
    public IntPtr ImageCallback;
    public IntPtrByte LinkIcon;
    public MarkdownHeadingFormat Heading1;
    public MarkdownHeadingFormat Heading2;
    public MarkdownHeadingFormat Heading3;
    public IntPtrVoid UserData;
    public IntPtr FormatCallback;
}

public readonly unsafe ref struct MarkdownConfigRef
{
    public readonly MarkdownConfig* NativePtr;

    public MarkdownConfigRef(MarkdownConfig* nativePtr) => NativePtr = nativePtr;

    public MarkdownLinkCallback? LinkCallback
    {
        get => NativePtr->LinkCallback == IntPtr.Zero ? null : Marshal.GetDelegateForFunctionPointer<MarkdownLinkCallback>((IntPtr) NativePtr->LinkCallback);
        set => NativePtr->LinkCallback = value is null ? IntPtr.Zero : Marshal.GetFunctionPointerForDelegate(value);
    }
    public MarkdownTooltipCallback? TooltipCallback
    {
        get => NativePtr->TooltipCallback == IntPtr.Zero ? null : Marshal.GetDelegateForFunctionPointer<MarkdownTooltipCallback>(NativePtr->TooltipCallback);
        set => NativePtr->TooltipCallback = value is null ? IntPtr.Zero : Marshal.GetFunctionPointerForDelegate(value);
    }
    public MarkdownImageCallback? ImageCallback
    {
        get => NativePtr->ImageCallback == IntPtr.Zero ? null : Marshal.GetDelegateForFunctionPointer<MarkdownImageCallback>(NativePtr->ImageCallback);
        set => NativePtr->ImageCallback = value is null ? IntPtr.Zero : Marshal.GetFunctionPointerForDelegate(value);
    }
    //public ref byte LinkIcon => ref Unsafe.AsRef<byte>(&NativePtr->LinkIcon);
    public ReadOnlySpan<byte> LinkIcon
    {
        get
        {
#if NET5_0_OR_GREATER
            return MemoryMarshal.CreateReadOnlySpanFromNullTerminated((byte*) NativePtr->LinkIcon);
#endif
            var len = new ReadOnlySpan<byte>((void*) NativePtr->LinkIcon, int.MaxValue).IndexOf((byte) '\0');
            return new ReadOnlySpan<byte>((void*) NativePtr->LinkIcon, len);
        }
        set => NativePtr->LinkIcon = (byte*) Unsafe.AsPointer(ref MemoryMarshal.GetReference(value));
    }

    public ref MarkdownHeadingFormat Heading1 => ref Unsafe.AsRef<MarkdownHeadingFormat>(&NativePtr->Heading1);
    public ref MarkdownHeadingFormat Heading2 => ref Unsafe.AsRef<MarkdownHeadingFormat>(&NativePtr->Heading2);
    public ref MarkdownHeadingFormat Heading3 => ref Unsafe.AsRef<MarkdownHeadingFormat>(&NativePtr->Heading3);
    public ref IntPtr UserData => ref Unsafe.AsRef<IntPtr>(&NativePtr->UserData);
    public MarkdownFormatCallback? FormatCallback
    {
        get => NativePtr->FormatCallback == IntPtr.Zero ? null : Marshal.GetDelegateForFunctionPointer<MarkdownFormatCallback>(NativePtr->FormatCallback);
        set => NativePtr->FormatCallback = value is null ? IntPtr.Zero : Marshal.GetFunctionPointerForDelegate(value);
    }
}