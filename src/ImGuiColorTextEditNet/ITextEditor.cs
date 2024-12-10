using ImGuiColorTextEditNet.Editor;

namespace ImGuiColorTextEditNet;

internal interface ITextEditor
{
    TextEditorText Text { get; }
    TextEditorSelection Selection { get; }
    TextEditorMovement Movement { get; }
    ITextEditorRenderer Renderer { get; }
}