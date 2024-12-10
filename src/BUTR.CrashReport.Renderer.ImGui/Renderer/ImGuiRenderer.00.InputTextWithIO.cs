using BUTR.CrashReport.ImGui.Enums;
using BUTR.CrashReport.ImGui.Structures;

using System.Buffers;
using System.Diagnostics.CodeAnalysis;

namespace BUTR.CrashReport.Renderer.ImGui.Renderer;

partial class ImGuiRenderer<TImGuiIORef, TImGuiViewportRef, TImDrawListRef, TImGuiStyleRef, TColorsRangeAccessorRef, TImGuiListClipperRef>
{
    private struct UserData
    {
        public long LabelHash;
    }

    //private ImGuiInputTextCallback<UserData> _inputTextCallback = default!;
    private ImGuiInputTextInt64Callback _inputTextCallback = default!;
    private long _selectionLabelHash;
    private int _selectionStart;
    private int _selectionEnd;
    private IMemoryOwner<byte>? _selectedText;

    //private int InputTextCallback(ImGuiInputTextCallbackData<UserData> data)
    private int InputTextCallback(ImGuiInputTextCallbackInt64Data data)
    {
        //_selectionLabelHash = data.Data.LabelHash;
        _selectionLabelHash = data.Data;

        _selectionStart = data.SelectionStart;
        _selectionEnd = data.SelectionEnd;

        return 0;
    }

    private void InitializeInputTextWithIO()
    {

        _inputTextCallback = InputTextCallback;
    }

    private void RenderInputTextWithIO(ReadOnlySpan<byte> label, Span<byte> input, int lineCount)
    {
        var labelHash = Hash(label);

        //_imgui.InputTextMultiline(label, input, lineCount, _inputTextCallback, new UserData { LabelHash = labelHash });
        _imgui.InputTextMultilineInt64(label, input, lineCount, _inputTextCallback, labelHash);
        if (_imgui.IsItemClicked(ImGuiMouseButton.Left) && labelHash == _selectionLabelHash)
        {
            _selectedText = null;
        }
        if (_imgui.IsItemClicked(ImGuiMouseButton.Right) && labelHash == _selectionLabelHash && _selectionStart != _selectionEnd)
        {
            var min = Math.Min(_selectionStart, _selectionEnd);
            var max = Math.Max(_selectionStart, _selectionEnd);

            if (TryFillBuffer(input.Slice(min, max - min), ref _selectedText))
            {
                _imgui.OpenPopup("CopyMenu\0"u8, ImGuiPopupFlags.None);
            }
        }
        if ((_imgui.IsKeyDown(ImGuiKey.LeftCtrl) || _imgui.IsKeyDown(ImGuiKey.RightCtrl)) && _imgui.IsKeyPressed(ImGuiKey.C) && labelHash == _selectionLabelHash && _selectionStart != _selectionEnd)
        {
            var min = Math.Min(_selectionStart, _selectionEnd);
            var max = Math.Max(_selectionStart, _selectionEnd);

            if (TryFillBuffer(input.Slice(min, max - min), ref _selectedText))
            {
                _imgui.SetClipboardText(_selectedText.Memory.Span);
                _selectedText = null;
            }
        }

        if (_imgui.BeginPopup("CopyMenu\0"u8, ImGuiWindowFlags.NoFocusOnAppearing))
        {
            if (_selectedText is null)
            {
                _imgui.CloseCurrentPopup();
            }
            else
            {
                if (_imgui.IsWindowAppearing())
                    _imgui.BringWindowToDisplayFront(_imgui.GetCurrentWindow());

                if (_imgui.MenuItem("Copy\0"u8))
                {
                    _imgui.SetClipboardText(_selectedText.Memory.Span);
                    _selectedText = null;
                }
            }

            _imgui.EndPopup();
        }
    }

    private static bool TryFillBuffer(ReadOnlySpan<byte> toCopy, [NotNullWhen(true)] ref IMemoryOwner<byte>? buffer)
    {
        buffer ??= MemoryPool<byte>.Shared.Rent(toCopy.Length);

        if (buffer.Memory.Length < toCopy.Length)
        {
            buffer.Dispose();
            buffer = MemoryPool<byte>.Shared.Rent(toCopy.Length);
        }

        toCopy.CopyTo(buffer.Memory.Span);
        buffer.Memory.Span[toCopy.Length] = 0;

        return true;
    }

    private static long Hash(ReadOnlySpan<byte> value)
    {
        var hash = 0L;
        for (var i = 0; i < value.Length; i++)
            hash = (hash * 397) ^ value[i].GetHashCode();
        return hash;
    }
}