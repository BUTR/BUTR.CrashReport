using System;
using System.Buffers;

namespace ImGuiColorTextEditNet.Editor;

internal class TextEditorSelection
{
    private readonly TextEditorText _text;
    private SelectionState _state;

    internal SelectionMode Mode = SelectionMode.Normal;
    internal Coordinates InteractiveStart;
    internal Coordinates InteractiveEnd;

    internal TextEditorSelection(TextEditorText text)
    {
        _text = text ?? throw new ArgumentNullException(nameof(text));
    }

    public IMemoryOwner<char> GetSelectedText() => _text.GetText(in _state.Start, in _state.End);
    internal void GetActualCursorCoordinates(out Coordinates cursorCoordinates) => _text.SanitizeCoordinates(in Cursor, out cursorCoordinates);

    internal ref SelectionState State => ref _state;

    public ref Coordinates Cursor => ref _state.Cursor;

    public Coordinates Start
    {
        get => _state.Start;
        set
        {
            _text.SanitizeCoordinates(in value, out _state.Start);
            if (_state.Start > _state.End)
                (_state.Start, _state.End) = (_state.End, _state.Start);
        }
    }

    public Coordinates End
    {
        get => _state.End;
        set
        {
            _text.SanitizeCoordinates(in value, out _state.End);
            if (_state.Start > _state.End)
                (_state.Start, _state.End) = (_state.End, _state.Start);
        }
    }

    public void SelectAll() => Select(new(0, 0), new(_text.LineCount, 0));
    public bool HasSelection => End > Start;

    public void Select(ref readonly Coordinates start, ref readonly Coordinates end, SelectionMode mode = SelectionMode.Normal)
    {
        _text.SanitizeCoordinates(in start, out _state.Start);
        _text.SanitizeCoordinates(in end, out _state.End);

        switch (mode)
        {
            case SelectionMode.Normal:
                break;

            case SelectionMode.Word:
            {
                _text.FindWordStart(in _state.Start, out _state.Start);
                _text.SanitizeCoordinates(in _state.Start, out _state.Start);
                if (!_text.IsOnWordBoundary(in _state.End))
                {
                    _text.FindWordStart(in _state.End, out _state.End);
                    _text.FindWordEnd(in _state.End, out _state.End);
                    _text.SanitizeCoordinates(in _state.End, out _state.End);
                }
                break;
            }

            case SelectionMode.Line:
            {
                Start = new(Start.Line, 0);
                End = new(End.Line, _text.GetLineMaxColumn(End.Line));
                break;
            }
        }
    }
}