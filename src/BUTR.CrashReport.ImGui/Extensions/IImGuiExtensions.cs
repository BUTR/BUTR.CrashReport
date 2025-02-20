using BUTR.CrashReport.ImGui.Enums;
using BUTR.CrashReport.Memory;

using System.Buffers;
using System.Buffers.Text;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace BUTR.CrashReport.ImGui.Extensions;

public static partial class IImGuiExtensions
{
    private const MethodImplOptions AggressiveOptimization = (MethodImplOptions) 512;

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public static void RenderId(this IImGui imGui, ReadOnlySpan<byte> title, string id)
    {
        imGui.Text(title);
        imGui.SameLine(0.0f, -1.0f);
        imGui.SmallButton(id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public static void Text(this IImGui imGui, bool fmt)
    {
        var @true = "true\0"u8;
        var @false = "false\0"u8;
        imGui.Text(fmt ? @true : @false);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public static void Text(this IImGui imGui, int value)
    {
        Span<byte> valueUtf8 = stackalloc byte[sizeof(int) * sizeof(char) + 1];
        Utf8Formatter.TryFormat(value, valueUtf8, out _);
        valueUtf8[valueUtf8.Length - 1] = 0;
        imGui.Text(valueUtf8);
    }

    private static readonly LiteralSpan<byte> _hexPrefix = "0x"u8;

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public static unsafe void Hex(this IImGui imGui, int value)
    {
        const int hexLength = sizeof(int) * sizeof(char);
        var hexPrefix = new ReadOnlySpan<byte>(_hexPrefix.Ptr, _hexPrefix.Length);
        Span<byte> valueUtf8 = stackalloc byte[hexPrefix.Length + (hexLength + 1)];
        hexPrefix.CopyTo(valueUtf8);
        IntToHexUtf8(value, valueUtf8.Slice(hexPrefix.Length, hexLength));
        valueUtf8[valueUtf8.Length - 1] = 0;
        imGui.Text(valueUtf8);
    }
    
    private static readonly LiteralSpan<byte> _hexChars = "0123456789ABCDEF"u8;
    private static void IntToHexUtf8(int value, Span<byte> buffer)
    {
        var i = buffer.Length; // Start from the end (right to left)

        for (var j = 0; j < 8; j++)
        {
            buffer[--i] = _hexChars[value & 0xF]; // Get last hex digit as byte
            value >>= 4; // Shift right by 4 bits
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public static void Text(this IImGui imGui, ref readonly DateTime value)
    {
        Span<byte> valueUtf8 = stackalloc byte[64];
        Utf8Formatter.TryFormat(value, valueUtf8, out var written, new StandardFormat('O'));
        valueUtf8[written] = 0;
        imGui.Text(valueUtf8.Slice(0, written));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public static void Text(this IImGui imGui, ref readonly DateTimeOffset value)
    {
        Span<byte> valueUtf8 = stackalloc byte[64];
        Utf8Formatter.TryFormat(value, valueUtf8, out var written, new StandardFormat('O'));
        valueUtf8[written] = 0;
        imGui.Text(valueUtf8.Slice(0, written));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public static void PadRight(this IImGui imGui, int toAppend)
    {
        Span<byte> padding = stackalloc byte[toAppend + 1];
        padding.Fill((byte) ' ');
        padding[toAppend] = 0;
        imGui.Text(padding);
        imGui.SameLine(0, 0);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public static bool Button(this IImGui imGui, ReadOnlySpan<byte> label, ref readonly Vector4 color, ref readonly Vector4 hovered, ref readonly Vector4 active)
    {
        imGui.PushStyleColor(ImGuiCol.Button, in color);
        imGui.PushStyleColor(ImGuiCol.ButtonHovered, in hovered);
        imGui.PushStyleColor(ImGuiCol.ButtonActive, in active);
        var result = imGui.Button(label);
        imGui.PopStyleColor(3);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public static bool SmallButtonRound(this IImGui imGui, ReadOnlySpan<byte> utf8Label)
    {
        imGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 3f);
        var result = imGui.SmallButton(utf8Label);
        imGui.PopStyleVar();
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public static bool SmallButtonRound(this IImGui imGui, string utf16Label)
    {
        imGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 3f);
        var result = imGui.SmallButton(utf16Label);
        imGui.PopStyleVar();
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public static bool CheckboxRound(this IImGui imGui, ReadOnlySpan<byte> utf8Label, ref bool isSelected)
    {
        imGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 1f);
        imGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 3f);
        var result = imGui.Checkbox(utf8Label, ref isSelected);
        imGui.PopStyleVar(2);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public static bool BeginChildRound(this IImGui imGui, ReadOnlySpan<byte> utf8Label, ref readonly Vector2 size, ImGuiChildFlags child_flags, ImGuiWindowFlags window_flags)
    {
        imGui.PushStyleVar(ImGuiStyleVar.ChildRounding, 5f);
        var result = imGui.BeginChild(utf8Label, in size, child_flags, window_flags);
        imGui.PopStyleVar();
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public static bool BeginChild(this IImGui imGui, string label, ref readonly Vector2 size, ref readonly Vector4 color, ImGuiChildFlags child_flags, ImGuiWindowFlags window_flags)
    {
        imGui.PushStyleColor(ImGuiCol.ChildBg, in color);
        var result = imGui.BeginChild(label, in size, child_flags, window_flags);
        imGui.PopStyleColor();
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public static bool BeginChild(this IImGui imGui, ReadOnlySpan<byte> strId, ref readonly Vector2 size, ref readonly Vector4 color, ImGuiChildFlags child_flags, ImGuiWindowFlags window_flags)
    {
        imGui.PushStyleColor(ImGuiCol.ChildBg, in color);
        var result = imGui.BeginChild(strId, in size, child_flags, window_flags);
        imGui.PopStyleColor();
        return result;
    }
}