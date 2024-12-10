using BUTR.CrashReport.ImGui.Utils;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ImGuiColorTextEditNet;

public class ColorPaletteIndexCreator
{
    public List<(ushort, Vector4)> Indices { get; init; } = new();

    public Vector4 this[ColorPalette key] { set => Indices.Add((key.ColorIndex, value)); }

}

public class ColorPaletteIndex
{
    private readonly List<Vector4> _indices = new();

    public int Length => _indices.Count;

    public ref readonly Vector4 this[ushort key] => ref IndicesAsSpan()[key];

    public Vector4 this[ColorPalette key]
    {
        get => this[key.ColorIndex];
        set => _indices.Insert(key.ColorIndex, value);
    }

    public Span<Vector4> IndicesAsSpan() => _indices.AsSpan();

    public ColorPaletteIndex With(ColorPaletteIndexCreator colorPaletteIndex)
    {
        var maxValue = colorPaletteIndex.Indices.Max(x => x.Item1) + 1;

        var copy = new ColorPaletteIndex { _indices = { Capacity = maxValue } };

        for (var i = 0; i < maxValue; i++)
            copy._indices.Add(Vector4.Zero);

        for (var i = 0; i < _indices.Count; i++)
            copy._indices[i] = _indices[i];

        foreach (var (index, color) in colorPaletteIndex.Indices.OrderBy(x => x.Item1))
            copy._indices[index] = color;

        return copy;
    }
}