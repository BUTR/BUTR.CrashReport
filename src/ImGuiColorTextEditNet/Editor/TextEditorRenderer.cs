using BUTR.CrashReport.ImGui;
using BUTR.CrashReport.ImGui.Enums;
using BUTR.CrashReport.ImGui.Extensions;
using BUTR.CrashReport.ImGui.Structures;

using System;
using System.Buffers;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text;

namespace ImGuiColorTextEditNet.Editor;

internal class TextEditorRenderer
{
    protected const float LineSpacing = 1.0f;
    protected const int LeftMargin = 10;
    protected const int CursorBlinkPeriodMs = 800;

    protected static readonly Vector4 MagentaVec4 = new(1.0f, 1.0f, 1.0f, 1.0f);
    protected static readonly uint MagentaUInt = 0xff00ffff;
}

internal class TextEditorRenderer<TImDrawListRef, TImGuiStyleRef, TColorsRangeAccessorRef, TImGuiListClipperRef> : TextEditorRenderer, ITextEditorRenderer
    where TImDrawListRef : IImDrawList

    where TImGuiStyleRef : IImGuiStyle<TColorsRangeAccessorRef>
    where TColorsRangeAccessorRef : IRangeAccessor<Vector4, ImGuiCol>

    where TImGuiListClipperRef : IImGuiListClipper
{
    private readonly IImGui _imGui;
    private readonly IImGuiWithImDrawList<TImDrawListRef> _imGuiWithImDrawList;
    private readonly IImGuiWithImGuiStyle<TImGuiStyleRef, TColorsRangeAccessorRef> _imGuiWithImGuiStyle;
    private readonly IImGuiWithImGuiListClipper<TImGuiListClipperRef> _imGuiWithImGuiListClipper;
    private readonly TextEditorSelection _selection;
    private readonly TextEditorText _text;
    private readonly StringBuilder _lineBuffer = new();
    private readonly List<uint> _palette = new();

    private Vector2 _charAdvance;
    private DateTime _startTime = DateTime.UtcNow;
    private float _textStart = 20.0f; // position (in pixels) where a code line starts relative to the left of the TextEditor.
    private uint[]? _uintPalette;
    private ColorPaletteIndex? _vec4Palette;
    private bool _paletteDirty;
    private float _lastAlpha;


    public ColorPaletteIndex PaletteIndex => _vec4Palette!;
    public uint[] Palette
    {
        get => _palette.ToArray();
        set
        {
            _palette.Clear();
            _palette.AddRange(value);
            _paletteDirty = true;
        }
    }

    public int PageSize
    {
        get
        {
            var height = _imGui.GetWindowHeight() - 20.0f;
            return (int) Math.Floor(height / _charAdvance.Y);
        }
    }

    public ITextEditorKeyboardInput? KeyboardInput { get; init; }
    public ITextEditorMouseInput? MouseInput { get; init; }

    private float fontSize;
    private float spaceSize;

    internal TextEditorRenderer(
        IImGui imGui,
        IImGuiWithImDrawList<TImDrawListRef> imGuiWithImDrawList,
        IImGuiWithImGuiStyle<TImGuiStyleRef, TColorsRangeAccessorRef> imGuiWithImGuiStyle,
        IImGuiWithImGuiListClipper<TImGuiListClipperRef> imGuiWithImGuiListClipper,
        ITextEditor editor)
    {
        _selection = editor.Selection;
        _text = editor.Text;
        Palette = [];
        _imGui = imGui;
        _imGuiWithImDrawList = imGuiWithImDrawList;
        _imGuiWithImGuiStyle = imGuiWithImGuiStyle;
        _imGuiWithImGuiListClipper = imGuiWithImGuiListClipper;
    }

    private uint ColorUInt(ushort index) => _uintPalette == null || index >= _uintPalette.Length
        ? MagentaUInt
        : _uintPalette[index];

    private uint ColorUInt(ColorPalette colorPalette) => _uintPalette == null || colorPalette.ColorIndex >= _uintPalette.Length
        ? MagentaUInt
        : _uintPalette[colorPalette.ColorIndex];

    private ref readonly Vector4 ColorVec(ColorPalette colorPalette) => ref _vec4Palette == null || colorPalette.ColorIndex > _vec4Palette.Length
        ? ref MagentaVec4
        : ref _vec4Palette[colorPalette.ColorIndex];

    public void SetPalette<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicConstructors)] TColorPalette>(ColorPaletteIndex colorPaletteIndex) where TColorPalette : ColorPalette
    {
        var values = ColorPalette.GetAll<TColorPalette>();
        if (values.Length != colorPaletteIndex.Length)
            throw new ArgumentException("Palette must contain all values of the enum.");

        _palette.Clear();
        for (var index = 0; index < values.Length; index++)
            _palette.Add(_imGui.ColorConvertFloat4ToU32(in colorPaletteIndex[values[index].ColorIndex]));

        _paletteDirty = true;
    }

    private bool _isInitialized;
    public void Initialize()
    {
        _imGui.CalcTextSize('#', out var fontSizeV2);
        fontSize = fontSizeV2.X;

        _imGui.CalcTextSize(' ', out var spaceSizeV2);
        spaceSize = spaceSizeV2.X;
    }

    public void Render(ReadOnlySpan<byte> title, ref readonly Vector2 size)
    {
        if (!_isInitialized)
        {
            Initialize();
            _isInitialized = true;
        }

        Vector4 background;
        if (_vec4Palette == null)
            _imGui.ColorConvertU32ToFloat4(_palette[ColorPalette.Background], out background);
        else
            background = ColorVec(ColorPalette.Background);
        var itemSpacing = new Vector2(0.0f, 0.0f);

        _imGui.PushStyleColor(ImGuiCol.ChildBg, in background);
        _imGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, in itemSpacing);

        _imGui.BeginChild(title, in size, ImGuiChildFlags.AutoResizeY, ImGuiWindowFlags.HorizontalScrollbar |
                                                                       ImGuiWindowFlags.AlwaysHorizontalScrollbar |
                                                                       ImGuiWindowFlags.NoMove);

        KeyboardInput?.HandleKeyboardInputs();

        MouseInput?.HandleMouseInputs();

        RenderInner();

        if (_text.PendingScrollRequest != null)
        {
            if (_text.PendingScrollRequest.Value < _text.LineCount)
                EnsurePositionVisible(new Coordinates(_text.PendingScrollRequest.Value, 0));
            _imGui.SetWindowFocus();
            _text.PendingScrollRequest = null;
        }

        _imGui.EndChild();

        _imGui.PopStyleVar();
        _imGui.PopStyleColor();
    }

    private void RenderInner()
    {
        /* Compute _charAdvance regarding to scaled font size (Ctrl + mouse wheel)*/
        _charAdvance = new Vector2(fontSize, _imGui.GetTextLineHeightWithSpacing() * LineSpacing);

        _imGuiWithImGuiStyle.GetStyle(out var style);
        var alpha = style.Alpha;
        if (Math.Abs(_lastAlpha - alpha) > float.Epsilon)
        {
            _paletteDirty = true;
            _lastAlpha = alpha;
        }

        /* Update palette with the current alpha from style */
        if (_paletteDirty)
        {
            _uintPalette = _palette.ToArray();
            var vec4Palette = new Vector4[_palette.Count];

            for (var i = 0; i < Palette.Length; ++i)
            {
                _imGui.ColorConvertU32ToFloat4(_palette[i], out var color);
                color.W *= alpha;
                vec4Palette[i] = color;
                _uintPalette[i] = _imGui.ColorConvertFloat4ToU32(in color);
            }

            _vec4Palette = new ColorPaletteIndex().With(new ColorPaletteIndexCreator
            {
                Indices = vec4Palette.Select((x, i) => ((ushort) i, x)).ToList(),
            });

            _paletteDirty = false;
        }

        //Util.Assert(_lineBuffer.Length == 0);

        var longest = _textStart;
        _imGuiWithImDrawList.GetWindowDrawList(out var drawList);
        _imGui.GetCursorScreenPos(out var cursorScreenPos);
        var scrollX = _imGui.GetScrollX();
        var globalLineMax = _text.LineCount;

        // Deduce _textStart by evaluating _lines size (global lineMax) plus two spaces as text width
        using var buf = GetLineNumberCache(globalLineMax, out var bufLength);
        _imGui.CalcTextSize(buf.Memory.Span.Slice(0, bufLength), out var bufSize);
        _textStart = bufSize.X + LeftMargin + spaceSize;

        if (globalLineMax != 0)
        {
            _imGuiWithImGuiListClipper.CreateImGuiListClipper(out var clipper);
            using var _ = clipper;

            clipper.Begin(globalLineMax, _imGui.GetTextLineHeightWithSpacing());
            while (clipper.Step())
            {
                RenderWithClipper(in clipper, in drawList, in cursorScreenPos, scrollX, ref longest, spaceSize);
            }
            clipper.End();
        }

        // Expand the window size to fit the longest line
        var vec2 = new Vector2(longest + 2, 0f);
        _imGui.Dummy(in vec2);
    }

    private void RenderWithClipper(ref readonly TImGuiListClipperRef clipper, ref readonly TImDrawListRef drawList, ref readonly Vector2 cursorScreenPos, float scrollX, ref float longestLineLength, float spaceSize)
    {
        for (var lineIdx = clipper.DisplayStart; lineIdx < clipper.DisplayEnd; ++lineIdx)
        {
            var lineStartScreenPos = cursorScreenPos with { Y = cursorScreenPos.Y + lineIdx * _charAdvance.Y };
            var textScreenPos = lineStartScreenPos with { X = lineStartScreenPos.X + _textStart };

            var line = _text.GetLine(lineIdx);
            var lineLength = _textStart + TextDistanceToLineStart(new(lineIdx, _text.GetLineMaxColumn(lineIdx)));
            longestLineLength = Math.Max(lineLength, longestLineLength);

            var lineStartCoord = new Coordinates(lineIdx, 0);
            var lineEndCoord = new Coordinates(lineIdx, _text.GetLineMaxColumn(lineIdx));

            // Draw selection for the current line
            var sstart = float.NegativeInfinity;
            var ssend = float.NegativeInfinity;

            //Util.Assert(_selection.Start <= _selection.End);
            if (_selection.Start <= lineEndCoord)
                sstart = _selection.Start > lineStartCoord ? TextDistanceToLineStart(in _selection.State.Start) : 0.0f;
            if (_selection.End > lineStartCoord)
                ssend = TextDistanceToLineStart(in _selection.End < lineEndCoord ? ref _selection.State.End : ref lineEndCoord);

            if (_selection.End.Line > lineIdx)
                ssend += _charAdvance.X;

            if (!float.IsNegativeInfinity(sstart) && !float.IsNegativeInfinity(ssend) && sstart < ssend)
            {
                var vstart = lineStartScreenPos with { X = lineStartScreenPos.X + _textStart + sstart };
                var vend = new Vector2(lineStartScreenPos.X + _textStart + ssend, lineStartScreenPos.Y + _charAdvance.Y);
                drawList.AddRectFilled(in vstart, in vend, ColorUInt(ColorPalette.Selection), 1.0f);
            }

            var start = lineStartScreenPos with { X = lineStartScreenPos.X + scrollX };

            if (_text.ExceptionLines.Contains(lineIdx + 1))
            {
                /*
                var y = new Coordinates(lineIdx, 0);
                while (_text.IsOnWordBoundary(y))
                    y = y with { Column = y.Column + 1 };
                y = y with { Column = y.Column - 1 };
                var tstart = TextDistanceToLineStart(y);
                */
                var tstart = 0;
                var estart = start with { X = start.X + _textStart + tstart };
                var end = new Vector2(
                    lineStartScreenPos.X + lineLength + 2.0f * scrollX,
                    lineStartScreenPos.Y + _charAdvance.Y);

                drawList.AddRectFilled(in estart, in end, ColorUInt(ColorPalette.ErrorMarker), 1f);

                if (_imGui.IsMouseHoveringRect(in lineStartScreenPos, in end))
                {
                    _imGui.BeginTooltip();
                    _imGui.PushStyleColor(ImGuiCol.Text, in ColorVec(ColorPalette.ErrorText));
                    _imGui.Text("Exception\0"u8);
                    _imGui.PopStyleColor();
                    _imGui.EndTooltip();
                }
            }

            // Draw line number (right aligned)
            using var buf = GetLineNumberCache(lineIdx + 1, out var bufLength);

            _imGui.CalcTextSize(buf.Memory.Span.Slice(0, bufLength), out var bufSize);
            var lineNoWidth = bufSize.X;
            var pos = lineStartScreenPos with { X = lineStartScreenPos.X + _textStart - lineNoWidth };
            drawList.AddText(in pos, ColorUInt(ColorPalette.LineNumber), buf.Memory.Span.Slice(0, bufLength));

            if (_selection.Cursor.Line == lineIdx)
            {
                var focused = _imGui.IsWindowFocused();

                // Highlight the current line (where the cursor is)
                if (!_selection.HasSelection)
                {
                    var sstart2 = start with { X = start.X + _textStart };
                    var end = new Vector2(start.X + lineLength - scrollX, start.Y + _charAdvance.Y);
                    drawList.AddRectFilled(in sstart2, in end, ColorUInt(focused ? ColorPalette.CurrentLineFill : ColorPalette.CurrentLineFillInactive), 1.0f);

                    drawList.AddRect(in start, in end, ColorUInt(ColorPalette.CurrentLineEdge), 1.0f);
                }

                // Render the cursor
                if (focused)
                {
                    var timeEnd = DateTime.UtcNow;
                    var elapsed = timeEnd - _startTime;
                    if (elapsed.Milliseconds > CursorBlinkPeriodMs / 2)
                    {
                        var width = 1.0f;
                        //var cindex = _text.GetCharacterIndex(_selection.Cursor);
                        var cx = TextDistanceToLineStart(in _selection.Cursor);

                        var cstart = lineStartScreenPos with { X = textScreenPos.X + cx };
                        var cend = new Vector2(textScreenPos.X + cx + width, lineStartScreenPos.Y + _charAdvance.Y);
                        drawList.AddRectFilled(in cstart, in cend, ColorUInt(ColorPalette.Cursor), 1.0f);
                        if (elapsed.Milliseconds > CursorBlinkPeriodMs)
                            _startTime = timeEnd;
                    }
                }
            }

            // Render colorized text
            var prevColor = line.Length == 0 ? ColorUInt(ColorPalette.Default) : ColorUInt(line[0].ColorIndex);
            var bufferOffset = new Vector2();

            for (var i = 0; i < line.Length;)
            {
                var glyph = line[i];
                var color = ColorUInt(glyph.ColorIndex);

                if ((color != prevColor || glyph.Char is '\t' or ' ') && _lineBuffer.Length != 0)
                {
                    var newOffset = new Vector2(textScreenPos.X + bufferOffset.X, textScreenPos.Y + bufferOffset.Y);
                    DrawText(in drawList, in newOffset, prevColor, _lineBuffer, out var textSize);
                    bufferOffset.X += textSize.X;
                    _lineBuffer.Clear();
                }
                prevColor = color;

                switch (glyph.Char)
                {
                    case '\t':
                        bufferOffset.X = (1.0f + (float) Math.Floor((1.0f + bufferOffset.X) / (_text.TabSize * spaceSize))) * (_text.TabSize * spaceSize);
                        i++;
                        break;
                    case ' ':
                        bufferOffset.X += spaceSize;
                        i++;
                        break;
                    default:
                        _lineBuffer.Append(line[i++].Char);
                        break;
                }
            }

            if (_lineBuffer.Length != 0)
            {
                var newOffset = new Vector2(textScreenPos.X + bufferOffset.X, textScreenPos.Y + bufferOffset.Y);
                DrawText(in drawList, in newOffset, prevColor, _lineBuffer, out _);
                _lineBuffer.Clear();
            }
        }
    }

    private void DrawText(ref readonly TImDrawListRef drawList, ref readonly Vector2 offset, uint color, StringBuilder sb, out Vector2 textSize)
    {
        var length = sb.Length;
        var tempMemory = length > 1024 ? MemoryPool<char>.Shared.Rent(length) : null;
        var temp = length <= 1024 ? stackalloc char[length] : tempMemory!.Memory.Span;

        var i = 0;
        foreach (var chunk in sb.GetChunks())
        {
            chunk.Span.CopyTo(temp.Slice(i));
            i += chunk.Length;
        }

        _imGui.CalcTextSize(temp, out textSize);
        drawList.AddText(in offset, color, temp);
        
        tempMemory?.Dispose();
    }

    private float TextDistanceToLineStart(ref readonly Coordinates position)
    {
        var line = _text.GetLine(position.Line);
        var distance = 0.0f;

        var colIndex = _text.GetCharacterIndex(in position);
        for (var i = 0; i < line.Length && i < colIndex;)
        {
            var c = line[i].Char;
            if (c == '\t')
            {
                distance = (1.0f + (float) Math.Floor((1.0f + distance) / (_text.TabSize * spaceSize))) * (_text.TabSize * spaceSize);
            }
            else
            {
                // distance + _charWidthCache.Get(c);
                _imGui.CalcTextSize(c, out var charSizeV2);
                distance = distance + charSizeV2.X;
            }

            i++;
        }

        return distance;
    }

    private void EnsurePositionVisible(ref readonly Coordinates pos)
    {
        var scrollX = _imGui.GetScrollX();
        var scrollY = _imGui.GetScrollY();

        var height = _imGui.GetWindowHeight();
        var width = _imGui.GetWindowWidth();

        var top = 1 + (int) Math.Ceiling(scrollY / _charAdvance.Y);
        var bottom = (int) Math.Ceiling((scrollY + height) / _charAdvance.Y);

        var left = (int) Math.Ceiling(scrollX / _charAdvance.X);
        var right = (int) Math.Ceiling((scrollX + width) / _charAdvance.X);

        var len = TextDistanceToLineStart(in pos);

        if (pos.Line < top)
            _imGui.SetScrollY(Math.Max(0.0f, (pos.Line - 1) * _charAdvance.Y));
        if (pos.Line > bottom - 4)
            _imGui.SetScrollY(Math.Max(0.0f, (pos.Line + 4) * _charAdvance.Y - height));
        if (len + _textStart < left + 4)
            _imGui.SetScrollX(Math.Max(0.0f, len + _textStart - 4));
        if (len + _textStart > right - 4)
            _imGui.SetScrollX(Math.Max(0.0f, len + _textStart + 4 - width));
    }

    public void ScreenPosToCoordinates(ref readonly Vector2 position, out Coordinates coordinates)
    {
        _imGui.GetCursorScreenPos(out var origin);
        var local = new Vector2(position.X - origin.X, position.Y - origin.Y);

        var lineCount = _text.LineCount;
        var lineNo = Math.Max(0, (int) Math.Floor(local.Y / _charAdvance.Y));
        var columnCoord = 0;

        if (lineNo >= lineCount)
        {
            coordinates = new Coordinates(lineNo, columnCoord);
            _text.SanitizeCoordinates(in coordinates, out coordinates);
            return;
        }

        var line = _text.GetLine(lineNo);

        var columnIndex = 0;
        var columnX = 0.0f;

        while (columnIndex < line.Length)
        {
            float columnWidth;

            if (line[columnIndex].Char == '\t')
            {
                var oldX = columnX;
                var newColumnX = (1.0f + (float) Math.Floor((1.0f + columnX) / (_text.TabSize * spaceSize))) * (_text.TabSize * spaceSize);
                columnWidth = newColumnX - oldX;
                if (_textStart + columnX + columnWidth * 0.5f > local.X)
                    break;

                columnX = newColumnX;
                columnCoord = columnCoord / _text.TabSize * _text.TabSize + _text.TabSize;
                columnIndex++;
            }
            else
            {
                _imGui.CalcTextSize(line[columnIndex++].Char, out var charSizeV2);
                columnWidth = charSizeV2.X;
                // TODO:
                //columnWidth = _charWidthCache.Get(line[columnIndex++].Char);
                if (_textStart + columnX + columnWidth * 0.5f > local.X)
                    break;

                columnX += columnWidth;
                columnCoord++;
            }
        }

        coordinates = new Coordinates(lineNo, columnCoord);
        _text.SanitizeCoordinates(in coordinates, out coordinates);
    }

    private static IMemoryOwner<byte> GetLineNumberCache(int lineIdx, out int length)
    {
        // We support up to 9999 lines
        var buffer = MemoryPool<byte>.Shared.Rent(6);
        var bufferUtf8 = buffer.Memory.Span;
        Utf8Formatter.TryFormat(lineIdx, bufferUtf8, out length);
        bufferUtf8[length++] = (byte) ' ';
        bufferUtf8[length++] = 0;
        return buffer;
    }
}