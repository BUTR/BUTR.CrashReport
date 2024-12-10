using BUTR.CrashReport.ImGui.Utils;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ImGuiColorTextEditNet;

public class ColorPalette
{
    private static readonly Dictionary<Type, List<string>> _perTypeNameToColorIndices = new();
    private static readonly Dictionary<Type, HashSet<ColorPalette>> _perTypeColorIndices = new();

    private static readonly Dictionary<Type, List<ColorPalette>> _colorIndices = new();

    public static Span<ColorPalette> GetAll<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicConstructors)] TColorPalette>() where TColorPalette : ColorPalette
    {
        if (_colorIndices.TryGetValue(typeof(TColorPalette), out var colorIndices))
            return colorIndices.AsSpan();

        var unorderedColorIndices = new List<ColorPalette>();
        var type = typeof(TColorPalette);
#pragma warning disable IL2059
        RuntimeHelpers.RunClassConstructor(typeof(TColorPalette).TypeHandle);
#pragma warning restore IL2059
        while (type != null)
        {
            if (_perTypeColorIndices.TryGetValue(type, out var perTypeColorIndices))
            {
                foreach (var colorPalette in perTypeColorIndices)
                    unorderedColorIndices.Add(colorPalette);
            }
            type = type.BaseType;
        }

        _colorIndices[typeof(TColorPalette)] = colorIndices = unorderedColorIndices.OrderBy(x => x.ColorIndex).ToList();
        return colorIndices.AsSpan();
    }

    public static implicit operator ushort(ColorPalette colorPalette) => colorPalette.ColorIndex;

    public static ColorPalette Default { get; } = new(nameof(Default));
    public static ColorPalette Background { get; } = new(nameof(Background));
    public static ColorPalette Cursor { get; } = new(nameof(Cursor));
    public static ColorPalette Selection { get; } = new(nameof(Selection));
    public static ColorPalette ExecutingLine { get; } = new(nameof(ExecutingLine));
    public static ColorPalette LineNumber { get; } = new(nameof(LineNumber));
    public static ColorPalette CurrentLineFill { get; } = new(nameof(CurrentLineFill));
    public static ColorPalette CurrentLineFillInactive { get; } = new(nameof(CurrentLineFillInactive));
    public static ColorPalette CurrentLineEdge { get; } = new(nameof(CurrentLineEdge));
    public static ColorPalette ErrorMarker { get; } = new(nameof(ErrorMarker));
    public static ColorPalette ErrorText { get; } = new(nameof(ErrorText));


    public ushort ColorIndex { get; }

    protected ColorPalette(string uniqueName)
    {
        if (!_perTypeNameToColorIndices.TryGetValue(GetType(), out var perTypeNameToColorIndices))
            _perTypeNameToColorIndices[GetType()] = perTypeNameToColorIndices = new();

        int offset = perTypeNameToColorIndices.IndexOf(uniqueName);
        if (offset == -1)
        {
            perTypeNameToColorIndices.Add(uniqueName);
            offset = perTypeNameToColorIndices.Count - 1;
        }

        int maxBaseColorIndex = 0;
        if (_perTypeColorIndices.TryGetValue(GetType().BaseType ?? typeof(object), out var basePerTypeColorIndices))
        {
            maxBaseColorIndex = basePerTypeColorIndices.Max(x => x.ColorIndex) + 1;
        }

        ColorIndex = (ushort) (maxBaseColorIndex + offset);

        if (!_perTypeColorIndices.TryGetValue(GetType(), out var perTypeColorIndices))
            _perTypeColorIndices[GetType()] = perTypeColorIndices = [];
        perTypeColorIndices.Add(this);
    }
    protected ColorPalette(ushort colorIndex)
    {
        ColorIndex = colorIndex;

        if (!_perTypeColorIndices.TryGetValue(GetType(), out var perTypeColorIndices))
            _perTypeColorIndices[GetType()] = perTypeColorIndices = [];
        perTypeColorIndices.Add(this);
    }

    public override int GetHashCode() => ColorIndex.GetHashCode();

    public override bool Equals(object? obj) => obj is ColorPalette colorPalette && ColorIndex == colorPalette.ColorIndex;
}