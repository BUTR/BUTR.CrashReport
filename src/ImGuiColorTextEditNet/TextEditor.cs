using BUTR.CrashReport.ImGui;
using BUTR.CrashReport.ImGui.Enums;
using BUTR.CrashReport.ImGui.Structures;
using BUTR.CrashReport.Memory.Utils;

using ImGuiColorTextEditNet.Editor;

using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Text;

namespace ImGuiColorTextEditNet;

public class TextEditor<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicConstructors)] TColorPalette, TImGuiIORef, TImDrawListRef, TImGuiStyleRef, TColorsRangeAccessorRef, TImGuiListClipperRef> : ITextEditor
    where TColorPalette : ColorPalette

    where TImGuiIORef : IImGuiIO

    where TImDrawListRef : IImDrawList

    where TImGuiStyleRef : IImGuiStyle<TColorsRangeAccessorRef>
    where TColorsRangeAccessorRef : IRangeAccessor<Vector4, ImGuiCol>
    where TImGuiListClipperRef : IImGuiListClipper
{
    internal TextEditorText Text { get; }
    TextEditorText ITextEditor.Text => Text;

    internal TextEditorSelection Selection { get; }
    TextEditorSelection ITextEditor.Selection => Selection;

    internal ITextEditorRenderer Renderer { get; }
    ITextEditorRenderer ITextEditor.Renderer => Renderer;
    internal TextEditorMovement Movement { get; }
    TextEditorMovement ITextEditor.Movement => Movement;

    public TextEditor(
        IImGui imGui,
        IImGuiWithImGuiIO<TImGuiIORef> imGuiWithImGuiIO,
        IImGuiWithImDrawList<TImDrawListRef> imGuiWithImDrawList,
        IImGuiWithImGuiStyle<TImGuiStyleRef, TColorsRangeAccessorRef> imGuiWithImGuiStyle,
        IImGuiWithImGuiListClipper<TImGuiListClipperRef> imGuiWithImGuiListClipper)
    {
        Text = new TextEditorText();
        Selection = new TextEditorSelection(Text);
        Movement = new TextEditorMovement(Selection, Text);
        Renderer = new TextEditorRenderer<TImDrawListRef, TImGuiStyleRef, TColorsRangeAccessorRef, TImGuiListClipperRef>(imGui, imGuiWithImDrawList, imGuiWithImGuiStyle, imGuiWithImGuiListClipper, this)
        {
            KeyboardInput = new StandardKeyboardInput<TImGuiIORef>(imGui, imGuiWithImGuiIO, this),
            MouseInput = new StandardMouseInput<TImGuiIORef>(imGui, imGuiWithImGuiIO, this)
        };
    }

    public void Render(string utf16Title, in Vector2 size = new())
    {
        var utf8ByteCount = Encoding.UTF8.GetMaxByteCount(utf16Title.Length) + 1;
        var tempMemory = utf8ByteCount > 2048 ? MemoryPool<byte>.Shared.Rent(utf8ByteCount) : null;
        var utf8 = utf8ByteCount <= 2048 ? stackalloc byte[utf8ByteCount] : tempMemory!.Memory.Span;
        var length = Utf8Utils.Utf16ToUtf8(utf16Title, utf8);
        utf8[length] = 0;

        Renderer.Render(utf8, in size);
    }

    public void Render(ReadOnlySpan<byte> utf8Title, in Vector2 size = new()) => Renderer.Render(utf8Title, in size);

    public void AddGlyphs(Span<Glyph> glyphs) => Text.AddGlyphs(glyphs);

    public void AddExceptionLine(int line) => Text.ExceptionLines.Add(line);

    public void SetPalette(ColorPaletteIndex colorPaletteIndex) => Renderer.SetPalette<TColorPalette>(colorPaletteIndex);

    public GlyphBuilder CreateGlyphBuilder() => new();
}