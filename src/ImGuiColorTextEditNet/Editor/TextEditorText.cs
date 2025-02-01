using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace ImGuiColorTextEditNet.Editor;

internal class TextEditorText
{
    private readonly List<Line> _lines = [];
    private int _tabSize = 4;

    public int LineCount => _lines.Count;
    public int? PendingScrollRequest { get; set; }

    public int TabSize { get => _tabSize; set => _tabSize = Math.Max(0, Math.Min(32, value)); }

    public HashSet<int> ExceptionLines { get; } = new();

    public ReadOnlySpan<Glyph> GetLine(int index) => _lines[index].AsSpan();

    public IMemoryOwner<char> GetText(ref readonly Coordinates startPos, ref readonly Coordinates endPos)
    {
        var lstart = startPos.Line;
        var lend = endPos.Line;
        var istart = GetCharacterIndex(in startPos);
        var iend = GetCharacterIndex(in endPos);
        var s = 0;

        for (var i = lstart; i < lend; i++)
            s += _lines[i].Glyphs.Count;
        if (lstart == lend)
            s += iend - istart;

        var j = 0;
        var buffer = MemoryPool<char>.Shared.Rent(s + s / 8);
        while (istart < iend || lstart < lend)
        {
            if (lstart >= _lines.Count)
                break;

            var line = _lines[lstart].AsSpan();
            if (istart < line.Length)
            {
                buffer.Memory.Span[j++] = line[istart].Char;
                istart++;
            }
            else
            {
                istart = 0;
                ++lstart;
                if (lstart < _lines.Count)
                {
                    Environment.NewLine.AsSpan().CopyTo(buffer.Memory.Span.Slice(j, Environment.NewLine.Length));
                    j += Environment.NewLine.Length;
                }
            }
        }

        return buffer;
    }

    public string GetWordAt(ref readonly Coordinates position)
    {
        FindWordStart(in position, out var start);
        FindWordEnd(in position, out var end);

        var sb = new StringBuilder();

        var istart = GetCharacterIndex(in start);
        var iend = GetCharacterIndex(in end);

        var line = _lines[position.Line].AsSpan();
        for (var it = istart; it < iend; ++it)
            sb.Append(line[it].Char);

        return sb.ToString();
    }

    public int GetCharacterIndex(ref readonly Coordinates position)
    {
        if (position.Line >= _lines.Count)
            return -1;

        var line = _lines[position.Line].AsSpan();
        var c = 0;
        var i = 0;

        for (; i < line.Length && c < position.Column;)
        {
            c = line[i].Char == '\t' ? c / TabSize * TabSize + TabSize : c + 1;
            i++;
        }

        return i;
    }

    public int GetCharacterColumn(int lineNumber, int columnNumber)
    {
        if (lineNumber >= _lines.Count)
            return 0;

        var line = _lines[lineNumber].AsSpan();
        var col = 0;
        var i = 0;

        while (i < columnNumber && i < line.Length)
        {
            var c = line[i].Char;
            i++;
            col = c == '\t' ? col / TabSize * TabSize + TabSize : col + 1;
        }

        return col;
    }

    public int GetLineMaxColumn(int lineNumber)
    {
        if (lineNumber >= _lines.Count)
            return 0;

        var line = _lines[lineNumber].AsSpan();
        var col = 0;

        for (var i = 0; i < line.Length;)
        {
            var c = line[i].Char;
            col = c == '\t' ? col / TabSize * TabSize + TabSize : col + 1;
            i++;
        }

        return col;
    }

    public bool IsOnWordBoundary(ref readonly Coordinates position)
    {
        if (position.Line >= _lines.Count || position.Column == 0)
            return true;

        var line = _lines[position.Line].AsSpan();
        var cindex = GetCharacterIndex(in position);
        if (cindex >= line.Length)
            return true;

        return line[cindex].ColorIndex != line[cindex - 1].ColorIndex;
    }

    public void SanitizeCoordinates(ref readonly Coordinates value, out Coordinates sanitizedValue)
    {
        var line = value.Line;
        var column = value.Column;
        if (line >= _lines.Count)
        {
            if (_lines.Count == 0)
            {
                line = 0;
                column = 0;
            }
            else
            {
                line = _lines.Count - 1;
                column = GetLineMaxColumn(line);
            }
        }
        else
        {
            column = _lines.Count == 0 ? 0 : Math.Min(column, GetLineMaxColumn(line));
        }

        sanitizedValue = new(line, column);
    }

    public void FindWordStart(ref readonly Coordinates position, out Coordinates wordStart)
    {
        if (position.Line >= _lines.Count)
        {
            wordStart = position;
            return;
        }

        var line = _lines[position.Line].AsSpan();
        var cindex = GetCharacterIndex(in position);

        if (cindex >= line.Length)
        {
            wordStart = position;
            return;
        }

        while (cindex > 0 && char.IsWhiteSpace(line[cindex].Char))
            --cindex;

        var cstart = line[cindex].ColorIndex;
        while (cindex > 0)
        {
            var c = line[cindex].Char;
            if ((c & 0xC0) != 0x80) // not UTF code sequence 10xxxxxx
            {
                if (c <= 32 && char.IsWhiteSpace(c))
                {
                    cindex++;
                    break;
                }
                if (cstart != line[cindex - 1].ColorIndex)
                    break;
            }
            --cindex;
        }

        wordStart = new(position.Line, GetCharacterColumn(position.Line, cindex));
    }

    public void FindWordEnd(ref readonly Coordinates position, out Coordinates wordEnd)
    {
        if (position.Line >= _lines.Count)
        {
            wordEnd = position;
            return;
        }

        var line = _lines[position.Line].AsSpan();
        var cindex = GetCharacterIndex(in position);

        if (cindex >= line.Length)
        {
            wordEnd = position;
            return;
        }

        var prevspace = char.IsWhiteSpace(line[cindex].Char);
        var cstart = line[cindex].ColorIndex;
        while (cindex < line.Length)
        {
            var c = line[cindex].Char;
            if (cstart != line[cindex].ColorIndex)
                break;

            if (prevspace != char.IsWhiteSpace(c))
            {
                if (char.IsWhiteSpace(c))
                    while (cindex < line.Length && char.IsWhiteSpace(line[cindex].Char))
                        ++cindex;
                break;
            }
            cindex++;
        }
        wordEnd = new(position.Line, GetCharacterColumn(position.Line, cindex));
    }

    public void FindNextWord(ref readonly Coordinates from, out Coordinates nextWord)
    {
        var at = from;
        if (at.Line >= _lines.Count)
        {
            nextWord = at;
            return;
        }

        // skip to the next non-word character
        var isword = false;
        var skip = false;
        var cindex = GetCharacterIndex(in from);
        {
            var line = _lines[at.Line].AsSpan();
            if (cindex < line.Length)
            {
                isword = char.IsLetterOrDigit(line[cindex].Char);
                skip = isword;
            }
        }

        while (!isword || skip)
        {
            if (at.Line >= _lines.Count)
            {
                var l = Math.Max(0, _lines.Count - 1);
                nextWord = new(l, GetLineMaxColumn(l));
                return;
            }

            var line = _lines[at.Line].AsSpan();
            if (cindex < line.Length)
            {
                isword = char.IsLetterOrDigit(line[cindex].Char);

                if (isword && !skip)
                {
                    nextWord = new(at.Line, GetCharacterColumn(at.Line, cindex));
                    return;
                }

                if (!isword)
                    skip = false;

                cindex++;
            }
            else
            {
                cindex = 0;
                ++at.Line;
                skip = false;
                isword = false;
            }
        }

        nextWord = at;
    }

    public void AddGlyphs(Span<Glyph> glyphs)
    {
        Line line;
        if (_lines.Count == 0)
            _lines.Add(line = Line.Empty);
        else
            line = _lines[_lines.Count - 1];

        for (var i = 0; i < glyphs.Length; i++)
        {
            if (glyphs[i].Char == '\n')
            {
                _lines.Add(line = Line.Empty);
            }
            else
            {
                line.Glyphs.Add(glyphs[i]);
            }
        }
    }
}