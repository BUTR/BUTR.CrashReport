using BUTR.CrashReport.ImGui.Utils;

using System;
using System.Collections.Generic;

namespace ImGuiColorTextEditNet;

internal sealed class Line
{
    public static Line Empty => new(ReadOnlySpan<char>.Empty);

    public List<Glyph> Glyphs { get; init; }

    public Line(List<Glyph> glyphs)
    {
        Glyphs = glyphs ?? throw new ArgumentNullException(nameof(glyphs));
    }

    public Line(ReadOnlySpan<char> text)
    {
        Glyphs = new();
        for (var i = 0; i < text.Length; i++)
            Glyphs.Add(new Glyph(text[i], 0));
    }

    public Span<Glyph> AsSpan() => Glyphs.AsSpan();
}