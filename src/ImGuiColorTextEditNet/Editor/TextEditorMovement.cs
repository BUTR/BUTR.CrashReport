﻿using System;

namespace ImGuiColorTextEditNet.Editor;

internal class TextEditorMovement
{
    private readonly TextEditorSelection _selection;
    private readonly TextEditorText _text;

    internal TextEditorMovement(TextEditorSelection selection, TextEditorText text)
    {
        _selection = selection ?? throw new ArgumentNullException(nameof(selection));
        _text = text ?? throw new ArgumentNullException(nameof(text));
    }

    public void MoveUp(int amount = 1, bool isSelecting = false)
    {
        var oldPos = _selection.Cursor;
        var newPos = _selection.Cursor;
        newPos.Line = Math.Max(0, _selection.Cursor.Line - amount);

        if (oldPos == newPos)
            return;

        _selection.Cursor = newPos;
        if (isSelecting)
        {
            if (oldPos == _selection.InteractiveStart)
                _selection.InteractiveStart = _selection.Cursor;
            else if (oldPos == _selection.InteractiveEnd)
                _selection.InteractiveEnd = _selection.Cursor;
            else
            {
                _selection.InteractiveStart = _selection.Cursor;
                _selection.InteractiveEnd = oldPos;
            }
        }
        else
            _selection.InteractiveStart = _selection.InteractiveEnd = _selection.Cursor;

        _selection.Select(in _selection.InteractiveStart, in _selection.InteractiveEnd);
        _text.PendingScrollRequest = _selection.Cursor.Line;
    }

    public void MoveDown(int amount = 1, bool isSelecting = false)
    {
        //Util.Assert(_selection.Cursor.Column >= 0);
        var oldPos = _selection.Cursor;
        var newPos = _selection.Cursor;
        newPos.Line = Math.Max(0, Math.Min(_text.LineCount - 1, _selection.Cursor.Line + amount));

        if (newPos == oldPos)
            return;

        _selection.Cursor = newPos;

        if (isSelecting)
        {
            if (oldPos == _selection.InteractiveEnd)
                _selection.InteractiveEnd = _selection.Cursor;
            else if (oldPos == _selection.InteractiveStart)
                _selection.InteractiveStart = _selection.Cursor;
            else
            {
                _selection.InteractiveStart = oldPos;
                _selection.InteractiveEnd = _selection.Cursor;
            }
        }
        else
            _selection.InteractiveStart = _selection.InteractiveEnd = _selection.Cursor;

        _selection.Select(in _selection.InteractiveStart, in _selection.InteractiveEnd);
        _text.PendingScrollRequest = _selection.Cursor.Line;
    }

    public void MoveLeft(int amount = 1, bool isSelecting = false, bool isWordMode = false)
    {
        if (_text.LineCount == 0)
            return;

        var oldPos = _selection.Cursor;
        _selection.GetActualCursorCoordinates(out _selection.Cursor);
        var line = _selection.Cursor.Line;
        var cindex = _text.GetCharacterIndex(in _selection.Cursor);

        while (amount-- > 0)
        {
            if (cindex == 0)
            {
                if (line > 0)
                {
                    --line;
                    cindex = _text.LineCount > line ? _text.GetLine(line).Length : 0;
                }
            }
            else
                --cindex;

            _selection.Cursor = new(line, _text.GetCharacterColumn(line, cindex));
            if (isWordMode)
            {
                _text.FindWordStart(in _selection.Cursor, out _selection.Cursor);
                cindex = _text.GetCharacterIndex(in _selection.Cursor);
            }
        }

        _selection.Cursor = new(line, _text.GetCharacterColumn(line, cindex));

        //Util.Assert(_selection.Cursor.Column >= 0);
        if (isSelecting)
        {
            if (oldPos == _selection.InteractiveStart)
                _selection.InteractiveStart = _selection.Cursor;
            else if (oldPos == _selection.InteractiveEnd)
                _selection.InteractiveEnd = _selection.Cursor;
            else
            {
                _selection.InteractiveStart = _selection.Cursor;
                _selection.InteractiveEnd = oldPos;
            }
        }
        else
            _selection.InteractiveStart = _selection.InteractiveEnd = _selection.Cursor;

        _selection.Select(in _selection.InteractiveStart, in _selection.InteractiveEnd, isSelecting && isWordMode ? SelectionMode.Word : SelectionMode.Normal);
        _text.PendingScrollRequest = _selection.Cursor.Line;
    }

    public void MoveRight(int amount = 1, bool isSelecting = false, bool isWordMode = false)
    {
        var oldPos = _selection.Cursor;

        if (_text.LineCount == 0 || oldPos.Line >= _text.LineCount)
            return;

        var cindex = _text.GetCharacterIndex(in _selection.Cursor);
        while (amount-- > 0)
        {
            var lindex = _selection.Cursor.Line;
            var line = _text.GetLine(lindex);
            if (cindex >= line.Length)
            {
                if (_selection.Cursor.Line < _text.LineCount - 1)
                    _selection.Cursor = new(Math.Max(0, Math.Min(_text.LineCount - 1, _selection.Cursor.Line + 1)), 0);
                else
                    return;
            }
            else
            {
                cindex++;
                _selection.Cursor = new(lindex, _text.GetCharacterColumn(lindex, cindex));
                if (isWordMode)
                    _text.FindNextWord(in _selection.Cursor, out _selection.Cursor);
            }
        }

        if (isSelecting)
        {
            if (oldPos == _selection.InteractiveEnd)
                _text.SanitizeCoordinates(in _selection.Cursor, out _selection.InteractiveEnd);
            else if (oldPos == _selection.InteractiveStart)
                _selection.InteractiveStart = _selection.Cursor;
            else
            {
                _selection.InteractiveStart = oldPos;
                _selection.InteractiveEnd = _selection.Cursor;
            }
        }
        else
            _selection.InteractiveStart = _selection.InteractiveEnd = _selection.Cursor;

        _selection.Select(in _selection.InteractiveStart, in _selection.InteractiveEnd, isSelecting && isWordMode ? SelectionMode.Word : SelectionMode.Normal);
        _text.PendingScrollRequest = _selection.Cursor.Line;
    }

    public void MoveToStartOfFile(bool isSelecting = false)
    {
        var oldPos = _selection.Cursor;
        _selection.Cursor = new(0, 0);

        if (_selection.Cursor == oldPos)
            return;

        if (isSelecting)
        {
            _selection.InteractiveEnd = oldPos;
            _selection.InteractiveStart = _selection.Cursor;
        }
        else
            _selection.InteractiveStart = _selection.InteractiveEnd = _selection.Cursor;

        _selection.Select(in _selection.InteractiveStart, in _selection.InteractiveEnd);
        _text.PendingScrollRequest = _selection.Cursor.Line;
    }

    public void MoveToEndOfFile(bool isSelecting = false)
    {
        var oldPos = _selection.Cursor;
        var newPos = new Coordinates(_text.LineCount - 1, 0);
        _selection.Cursor = newPos;

        if (isSelecting)
        {
            _selection.InteractiveStart = oldPos;
            _selection.InteractiveEnd = newPos;
        }
        else
            _selection.InteractiveStart = _selection.InteractiveEnd = newPos;

        _selection.Select(in _selection.InteractiveStart, in _selection.InteractiveEnd);
        _text.PendingScrollRequest = _selection.Cursor.Line;
    }

    public void MoveToStartOfLine(bool isSelecting = false)
    {
        var oldPos = _selection.Cursor;
        _selection.Cursor = new(_selection.Cursor.Line, 0);

        if (_selection.Cursor != oldPos)
        {
            if (isSelecting)
            {
                if (oldPos == _selection.InteractiveStart)
                    _selection.InteractiveStart = _selection.Cursor;
                else if (oldPos == _selection.InteractiveEnd)
                    _selection.InteractiveEnd = _selection.Cursor;
                else
                {
                    _selection.InteractiveStart = _selection.Cursor;
                    _selection.InteractiveEnd = oldPos;
                }
            }
            else
                _selection.InteractiveStart = _selection.InteractiveEnd = _selection.Cursor;
            _selection.Select(in _selection.InteractiveStart, in _selection.InteractiveEnd);
        }

        _text.PendingScrollRequest = _selection.Cursor.Line;
    }

    public void MoveToEndOfLine(bool isSelecting = false)
    {
        var oldPos = _selection.Cursor;
        _selection.Cursor = new(_selection.Cursor.Line, _text.GetLineMaxColumn(oldPos.Line));

        if (_selection.Cursor == oldPos)
            return;

        if (isSelecting)
        {
            if (oldPos == _selection.InteractiveEnd)
                _selection.InteractiveEnd = _selection.Cursor;
            else if (oldPos == _selection.InteractiveStart)
                _selection.InteractiveStart = _selection.Cursor;
            else
            {
                _selection.InteractiveStart = oldPos;
                _selection.InteractiveEnd = _selection.Cursor;
            }
        }
        else
            _selection.InteractiveStart = _selection.InteractiveEnd = _selection.Cursor;

        _selection.Select(in _selection.InteractiveStart, in _selection.InteractiveEnd);
        _text.PendingScrollRequest = _selection.Cursor.Line;
    }
}