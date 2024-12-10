namespace ImGuiColorTextEditNet;

public readonly struct Glyph
{
    public readonly char Char;
    public readonly ushort ColorIndex;

    public Glyph(char aChar, ushort colorIndex)
    {
        Char = aChar;
        ColorIndex = colorIndex;
    }
}