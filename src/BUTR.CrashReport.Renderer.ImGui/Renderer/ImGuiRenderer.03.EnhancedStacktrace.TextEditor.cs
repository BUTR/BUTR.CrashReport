#if TEXT_EDITOR
using BUTR.CrashReport.Models;
using BUTR.CrashReport.Renderer.ImGui.Syntax;

using ImGuiColorTextEditNet;

using PrismSharp.Core;

namespace BUTR.CrashReport.Renderer.ImGui.Renderer;

partial class ImGuiRenderer<TImGuiIORef, TImGuiViewportRef, TImDrawListRef, TImGuiStyleRef, TColorsRangeAccessorRef, TImGuiListClipperRef>
{
    private readonly Dictionary<MethodModel, TextEditor<PrismColorPalette, TImGuiIORef, TImDrawListRef, TImGuiStyleRef, TColorsRangeAccessorRef, TImGuiListClipperRef>[]> _methodCodeLinesEditor = new(MethodEqualityComparer.Instance);

    private void SetCodeDictionary(IDictionary<MethodModel, TextEditor<PrismColorPalette, TImGuiIORef, TImDrawListRef, TImGuiStyleRef, TColorsRangeAccessorRef, TImGuiListClipperRef>[]> methodDict, MethodModel key, CodeType codeType, InstructionsModel? instructions)
    {
        static void Handle(IEnumerable<Token> tokens, GlyphBuilder builder)
        {
            foreach (var token in tokens)
            {
                if (token is StringToken stringToken)
                {
                    var type = stringToken.Type;

                    if (stringToken.Alias.Length > 0)
                    {
                        foreach (var alias in stringToken.Alias.Reverse())
                        {
                            // If there are multiple aliases, we need to return the last one that assigns an actual color
                            var colorPalette = PrismColorPalette.FromToken(alias);
                            if (!Equals(colorPalette, ColorPalette.Default))
                            {
                                type = alias;
                                break;
                            }
                        }
                    }

                    builder.Append(stringToken.Content, PrismColorPalette.FromToken(type));
                }

                if (token is StreamToken streamToken)
                {
                    Handle(streamToken.Content, builder);
                }
            }
        }

        if (!methodDict.TryGetValue(key, out var codeArray))
            codeArray = methodDict[key] = new TextEditor<PrismColorPalette, TImGuiIORef, TImDrawListRef, TImGuiStyleRef, TColorsRangeAccessorRef, TImGuiListClipperRef>[(int) CodeType.Native + 1];

        var textEditor = codeArray[(int) codeType] = new(_imgui, _imGuiWithImGuiIO, _imGuiWithImDrawList, _imGuiWithImGuiStyle, _imGuiWithImGuiListClipper);

        var builder = new GlyphBuilder();
        var tokens = Prism.Tokenize(string.Join("\n", instructions?.Instructions ?? []), codeType switch
        {
            CodeType.IL => LanguageGrammars.cil,
            CodeType.ILMixed => LanguageGrammars.cil,
            CodeType.CSharp => LanguageGrammars.csharp,
            CodeType.Native => LanguageGrammars.nasm,
            _ => throw new ArgumentOutOfRangeException(nameof(codeType), codeType, null)
        });
        Handle(tokens, builder);
        textEditor.AddGlyphs(builder.AsSpan());

        if (instructions?.Highlight is not null)
        {
            for (var i = instructions.Highlight.StartLine; i <= instructions.Highlight.EndLine; i++)
                textEditor.AddExceptionLine(i);
        }
    }
}
#endif