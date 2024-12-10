using BUTR.CrashReport.ImGui.Utils;

using System;
using System.Collections.Generic;

namespace ImGuiColorTextEditNet;

public class GlyphBuilder
{
    private readonly List<Glyph> _glyphs = new();

    public void Append(char c, ColorPalette colorPalette) => _glyphs.Add(new Glyph(c, colorPalette.ColorIndex));

    public void Append(char c, int count, ColorPalette colorPalette)
    {
        for (var i = 0; i < count; i++)
            _glyphs.Add(new Glyph(c, colorPalette.ColorIndex));
    }
    public void Append(ReadOnlySpan<char> s, ColorPalette colorPalette)
    {
        foreach (var c in s)
            _glyphs.Add(new Glyph(c, colorPalette.ColorIndex));
    }
    public void Append(string s, ColorPalette colorPalette)
    {
        foreach (var c in s)
            _glyphs.Add(new Glyph(c, colorPalette.ColorIndex));
    }

    public void Append(char c) => _glyphs.Add(new Glyph(c, 0));

    public void Append(char c, int count)
    {
        for (var i = 0; i < count; i++)
            _glyphs.Add(new Glyph(c, 0));
    }

    public void Append(ReadOnlySpan<char> s)
    {
        foreach (var c in s)
            _glyphs.Add(new Glyph(c, 0));
    }

    public void Append(string s)
    {
        foreach (var c in s)
            _glyphs.Add(new Glyph(c, 0));
    }

    public void AppendLine() => _glyphs.Add(new Glyph('\n', 0));

    public Span<Glyph> AsSpan() => _glyphs.AsSpan();

    public void Clear() => _glyphs.Clear();
}