using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace ImGuiColorTextEditNet.Editor;

internal interface ITextEditorRenderer
{
    int PageSize { get; }
    void ScreenPosToCoordinates(ref readonly Vector2 position, out Coordinates coordinates);
    void Render(ReadOnlySpan<byte> title, ref readonly Vector2 size);
    void SetPalette<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicConstructors)] TColorPalette>(ColorPaletteIndex colorPaletteIndex) where TColorPalette : ColorPalette;
}