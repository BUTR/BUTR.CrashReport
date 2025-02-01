using BUTR.CrashReport.ImGui.Structures;
using BUTR.CrashReport.Memory.Utils;

using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace BUTR.CrashReport.ImGui.Extensions;

partial class IImGuiExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public static void AddText<TImDrawListRef>(this TImDrawListRef imGui, ref readonly Vector2 pos, uint col, ReadOnlySpan<char> utf16Data)
        where TImDrawListRef : IImDrawList
    {
        var utf8ByteCount = Encoding.UTF8.GetMaxByteCount(utf16Data.Length) + 1;
        var tempMemory = utf8ByteCount > 2048 ? MemoryPool<byte>.Shared.Rent(utf8ByteCount) : null;
        var utf8 = utf8ByteCount <= 2048 ? stackalloc byte[utf8ByteCount] : tempMemory!.Memory.Span;
        var length = Utf8Utils.Utf16ToUtf8(utf16Data, utf8) + 1;
        utf8[length] = 0;

        imGui.AddText(in pos, col, utf8.Slice(0, length));
        tempMemory?.Dispose();
    }
}