using BUTR.CrashReport.ImGui.Enums;
using BUTR.CrashReport.Memory.Utils;

using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace BUTR.CrashReport.ImGui.Extensions;

partial class IImGuiExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public static void PushId(this IImGui imGui, string utf16Label)
    {
        var utf8ByteCount = Encoding.UTF8.GetMaxByteCount(utf16Label.Length) + 1;
        var tempMemory = utf8ByteCount > 2048 ? MemoryPool<byte>.Shared.Rent(utf8ByteCount) : null;
        var utf8 = utf8ByteCount <= 2048 ? stackalloc byte[utf8ByteCount] : tempMemory!.Memory.Span;
        var length = Utf8Utils.Utf16ToUtf8(utf16Label, utf8) + 1;
        utf8[length] = 0;

        imGui.PushId(utf8.Slice(0, length));
        tempMemory?.Dispose();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public static void Text(this IImGui imGui, string utf16Label)
    {
        var utf8ByteCount = Encoding.UTF8.GetMaxByteCount(utf16Label.Length) + 1;
        var tempMemory = utf8ByteCount > 2048 ? MemoryPool<byte>.Shared.Rent(utf8ByteCount) : null;
        var utf8 = utf8ByteCount <= 2048 ? stackalloc byte[utf8ByteCount] : tempMemory!.Memory.Span;
        var length = Utf8Utils.Utf16ToUtf8(utf16Label, utf8) + 1;
        utf8[length] = 0;

        imGui.Text(utf8.Slice(0, length));
        tempMemory?.Dispose();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public static void TextWrapped(this IImGui imGui, string utf16Label)
    {
        var utf8ByteCount = Encoding.UTF8.GetMaxByteCount(utf16Label.Length) + 1;
        var tempMemory = utf8ByteCount > 2048 ? MemoryPool<byte>.Shared.Rent(utf8ByteCount) : null;
        var utf8 = utf8ByteCount <= 2048 ? stackalloc byte[utf8ByteCount] : tempMemory!.Memory.Span;
        var length = Utf8Utils.Utf16ToUtf8(utf16Label, utf8) + 1;
        utf8[length] = 0;

        imGui.TextWrapped(utf8.Slice(0, length));
        tempMemory?.Dispose();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public static bool SmallButton(this IImGui imGui, string utf16Label)
    {
        imGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 3f);

        var utf8ByteCount = Encoding.UTF8.GetMaxByteCount(utf16Label.Length) + 1;
        var tempMemory = utf8ByteCount > 2048 ? MemoryPool<byte>.Shared.Rent(utf8ByteCount) : null;
        var utf8 = utf8ByteCount <= 2048 ? stackalloc byte[utf8ByteCount] : tempMemory!.Memory.Span;
        var length = Utf8Utils.Utf16ToUtf8(utf16Label, utf8) + 1;
        utf8[length] = 0;

        var result = imGui.SmallButton(utf8.Slice(0, length));
        tempMemory?.Dispose();
        imGui.PopStyleVar();
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public static void TextLinkOpenURL(this IImGui imGui, string utf16Label, string utf16Url)
    {
        var utf8LabelByteCount = Encoding.UTF8.GetMaxByteCount(utf16Label.Length) + 1;
        var tempMemoryLabel = utf8LabelByteCount > 2048 ? MemoryPool<byte>.Shared.Rent(utf8LabelByteCount) : null;
        var utf8Label = utf8LabelByteCount <= 2048 ? stackalloc byte[utf8LabelByteCount] : tempMemoryLabel!.Memory.Span;
        var labelLength = Utf8Utils.Utf16ToUtf8(utf16Label, utf8Label) + 1;
        utf8Label[labelLength] = 0;

        var utf8UrlByteCount = Encoding.UTF8.GetMaxByteCount(utf16Url.Length) + 1;
        var tempMemoryUrl = utf8LabelByteCount > 2048 ? MemoryPool<byte>.Shared.Rent(utf8UrlByteCount) : null;
        var utf8Url = utf8UrlByteCount <= 2048 ? stackalloc byte[utf8UrlByteCount] : tempMemoryUrl!.Memory.Span;
        var urlLength = Utf8Utils.Utf16ToUtf8(utf16Url, utf8Url) + 1;
        utf8Url[urlLength] = 0;

        imGui.TextLinkOpenURL(utf8Label.Slice(0, labelLength), utf8Url.Slice(0, urlLength));
        tempMemoryLabel?.Dispose();
        tempMemoryUrl?.Dispose();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public static bool Selectable(this IImGui imGui, string utf16Label, ref bool isSelected, ImGuiSelectableFlags flags)
    {
        var utf8ByteCount = Encoding.UTF8.GetMaxByteCount(utf16Label.Length) + 1;
        var tempMemory = utf8ByteCount > 2048 ? MemoryPool<byte>.Shared.Rent(utf8ByteCount) : null;
        var utf8 = utf8ByteCount <= 2048 ? stackalloc byte[utf8ByteCount] : tempMemory!.Memory.Span;
        var length = Utf8Utils.Utf16ToUtf8(utf16Label, utf8) + 1;
        utf8[length] = 0;

        var result = imGui.Selectable(utf8.Slice(0, length), ref isSelected, flags);
        tempMemory?.Dispose();
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public static bool TreeNode(this IImGui imGui, string utf16Label)
    {
        var utf8ByteCount = Encoding.UTF8.GetMaxByteCount(utf16Label.Length) + 1;
        var tempMemory = utf8ByteCount > 2048 ? MemoryPool<byte>.Shared.Rent(utf8ByteCount) : null;
        var utf8 = utf8ByteCount <= 2048 ? stackalloc byte[utf8ByteCount] : tempMemory!.Memory.Span;
        var length = Utf8Utils.Utf16ToUtf8(utf16Label, utf8) + 1;
        utf8[length] = 0;

        var result = imGui.TreeNode(utf8.Slice(0, length), ImGuiTreeNodeFlags.None);
        tempMemory?.Dispose();
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public static bool TreeNode(this IImGui imGui, string utf16Label, ImGuiTreeNodeFlags flags)
    {
        var utf8ByteCount = Encoding.UTF8.GetMaxByteCount(utf16Label.Length) + 1;
        var tempMemory = utf8ByteCount > 2048 ? MemoryPool<byte>.Shared.Rent(utf8ByteCount) : null;
        var utf8 = utf8ByteCount <= 2048 ? stackalloc byte[utf8ByteCount] : tempMemory!.Memory.Span;
        var length = Utf8Utils.Utf16ToUtf8(utf16Label, utf8) + 1;
        utf8[length] = 0;

        var result = imGui.TreeNode(utf8.Slice(0, length), flags);
        tempMemory?.Dispose();
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public static bool BeginChild(this IImGui imGui, string utf16Label, ref readonly Vector2 size, ImGuiChildFlags child_flags, ImGuiWindowFlags window_flags)
    {
        var utf8ByteCount = Encoding.UTF8.GetMaxByteCount(utf16Label.Length) + 1;
        var tempMemory = utf8ByteCount > 2048 ? MemoryPool<byte>.Shared.Rent(utf8ByteCount) : null;
        var utf8 = utf8ByteCount <= 2048 ? stackalloc byte[utf8ByteCount] : tempMemory!.Memory.Span;
        var length = Utf8Utils.Utf16ToUtf8(utf16Label, utf8) + 1;
        utf8[length] = 0;

        imGui.PushStyleVar(ImGuiStyleVar.ChildRounding, 5f);
        var result = imGui.BeginChild(utf8.Slice(0, length), in size, child_flags, window_flags);
        tempMemory?.Dispose();
        imGui.PopStyleVar();
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public static void SetClipboardText(this IImGui imGui, ReadOnlySpan<char> utf16Data)
    {
        var utf8ByteCount = Encoding.UTF8.GetMaxByteCount(utf16Data.Length) + 1;
        var tempMemory = utf8ByteCount > 2048 ? MemoryPool<byte>.Shared.Rent(utf8ByteCount) : null;
        var utf8 = utf8ByteCount <= 2048 ? stackalloc byte[utf8ByteCount] : tempMemory!.Memory.Span;
        var length = Utf8Utils.Utf16ToUtf8(utf16Data, utf8) + 1;
        utf8[length] = 0;

        imGui.SetClipboardText(utf8.Slice(0, length));
        tempMemory?.Dispose();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public static void SetClipboardText(this IImGui imGui, string utf16Data)
    {
        var utf8ByteCount = Encoding.UTF8.GetMaxByteCount(utf16Data.Length) + 1;
        var tempMemory = utf8ByteCount > 2048 ? MemoryPool<byte>.Shared.Rent(utf8ByteCount) : null;
        var utf8 = utf8ByteCount <= 2048 ? stackalloc byte[utf8ByteCount] : tempMemory!.Memory.Span;
        var length = Utf8Utils.Utf16ToUtf8(utf16Data, utf8) + 1;
        utf8[length] = 0;

        imGui.SetClipboardText(utf8.Slice(0, length));
        tempMemory?.Dispose();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public static void CalcTextSize(this IImGui imGui, char utf16, out Vector2 size)
    {
        Span<char> utf16Data = stackalloc char[] { utf16 };
        Span<byte> utf8 = stackalloc byte[5];
        var length = Utf8Utils.Utf16ToUtf8(utf16Data, utf8) + 1;
        utf8[length] = 0;
        
        imGui.CalcTextSize(utf8.Slice(0, length), out size);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public static void CalcTextSize(this IImGui imGui, ReadOnlySpan<char> utf16Data, out Vector2 size)
    {
        var utf8ByteCount = Encoding.UTF8.GetMaxByteCount(utf16Data.Length) + 1;
        var tempMemory = utf8ByteCount > 2048 ? MemoryPool<byte>.Shared.Rent(utf8ByteCount) : null;
        var utf8 = utf8ByteCount <= 2048 ? stackalloc byte[utf8ByteCount] : tempMemory!.Memory.Span;
        var length = Utf8Utils.Utf16ToUtf8(utf16Data, utf8) + 1;
        utf8[length] = 0;

        imGui.CalcTextSize(utf8.Slice(0, length), out size);
        tempMemory?.Dispose();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public static void CalcTextSize(this IImGui imGui, string utf16Data, out Vector2 size)
    {
        var utf8ByteCount = Encoding.UTF8.GetMaxByteCount(utf16Data.Length) + 1;
        var tempMemory = utf8ByteCount > 2048 ? MemoryPool<byte>.Shared.Rent(utf8ByteCount) : null;
        var utf8 = utf8ByteCount <= 2048 ? stackalloc byte[utf8ByteCount] : tempMemory!.Memory.Span;
        var length = Utf8Utils.Utf16ToUtf8(utf16Data, utf8) + 1;
        utf8[length] = 0;

        imGui.CalcTextSize(utf8.Slice(0, length), out size);
        tempMemory?.Dispose();
    }
}