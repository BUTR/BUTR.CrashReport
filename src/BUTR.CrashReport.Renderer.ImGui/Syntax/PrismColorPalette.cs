using BUTR.CrashReport.ImGui.Enums;
using BUTR.CrashReport.ImGui.Structures;

using ImGuiColorTextEditNet;

using System.Buffers.Text;
using System.Numerics;

namespace BUTR.CrashReport.Renderer.ImGui.Syntax;

internal class PrismColorPalette : ColorPalette
{
    private static Vector4 ToRgba(bool isDarkTheme, ReadOnlySpan<byte> hexColorDark, ReadOnlySpan<byte> hexColorLight) =>
        ToRgba(isDarkTheme ? hexColorDark : hexColorLight);
    private static Vector4 ToRgba(bool isDarkTheme, ReadOnlySpan<byte> hexColorDark, ref readonly Vector4 colorLight) =>
        isDarkTheme ? ToRgba(hexColorDark) : colorLight;
    private static Vector4 ToRgba(bool isDarkTheme, ref readonly Vector4 colorDark, ReadOnlySpan<byte> hexColorLight) =>
        isDarkTheme ? colorDark : ToRgba(hexColorLight);
    private static ref readonly Vector4 ToRgba(bool isDarkTheme, ref readonly Vector4 colorDark, ref readonly Vector4 colorLight) =>
        ref isDarkTheme ? ref colorDark : ref colorLight;

    private static Vector4 ToRgba(ReadOnlySpan<byte> hexColor)
    {
        if (hexColor.StartsWith("#"u8))
            hexColor = hexColor.Slice(1);

        // Parse hexadecimal values for red, green, blue, and alpha components
        if (!Utf8Parser.TryParse(hexColor.Slice(0, 2), out byte r, out _, 'x')) r = 0;
        if (!Utf8Parser.TryParse(hexColor.Slice(2, 2), out byte g, out _, 'x')) g = 0;
        if (!Utf8Parser.TryParse(hexColor.Slice(4, 2), out byte b, out _, 'x')) b = 0;
        if (hexColor.Length != 8 || !Utf8Parser.TryParse(hexColor.Slice(6, 2), out byte a, out _, 'x')) a = 255;

        // Normalize the color values from 0-255 to 0-1 range
        return new Vector4(r / 255f, g / 255f, b / 255f, a / 255f);
    }

    public static ColorPalette FromToken(string? tokenName)
    {
        var all = GetAll<PrismColorPalette>();
        for (var i = 0; i < all.Length; i++)
        {
            if (all[i] is PrismColorPalette prismColorPalette && prismColorPalette._tokenName == tokenName)
                return prismColorPalette;
        }
        return Default;
    }

    public static ColorPaletteIndex DefaultOrTomorrowNight<TColors>(bool isDarkTheme, TColors colors) where TColors : IRangeAccessor<Vector4, ImGuiCol> => CodeParser.BasePalette(isDarkTheme, colors).With(new()
    {
#pragma warning disable format // @formatter:off
        [Background]            = ToRgba(isDarkTheme, "#2d2d2d"u8, "#f5f2f0"u8),

        [Keyword]               = ToRgba(isDarkTheme, "#cc99cd"u8, "#0077aa"u8),
        [BuiltIn]               = ToRgba(isDarkTheme, "#cc99cd"u8, "#669900"u8),
        [ClassName]             = ToRgba(isDarkTheme, "#f8c555"u8, "#DD4A68"u8),
        [Function]              = ToRgba(isDarkTheme, "#f08d49"u8, "#DD4A68"u8),
        [Boolean]               = ToRgba(isDarkTheme, "#f08d49"u8, "#990055"u8),
        [Number]                = ToRgba(isDarkTheme, "#f08d49"u8, "#990055"u8),
        [String]                = ToRgba(isDarkTheme, "#7ec699"u8, "#669900"u8),
        [Char]                  = ToRgba(isDarkTheme, "#7ec699"u8, "#669900"u8),
        [Symbol]                = ToRgba(isDarkTheme, "#f8c555"u8, "#990055"u8),
        [Regex]                 = ToRgba(isDarkTheme, "#7ec699"u8, "#ee9900"u8),
        [Url]                   = ToRgba(isDarkTheme, "#67cdcc"u8, "#9a6e3a"u8),
        [Operator]              = ToRgba(isDarkTheme, "#67cdcc"u8, "#9a6e3a"u8),
        [Variable]              = ToRgba(isDarkTheme, "#7ec699"u8, "#ee9900"u8),
        [Constant]              = ToRgba(isDarkTheme, "#f8c555"u8, "#990055"u8),
        [Property]              = ToRgba(isDarkTheme, "#f8c555"u8, "#990055"u8),
        [Punctuation]           = ToRgba(isDarkTheme, "#cccccc"u8, "#999999"u8),
        [Important]             = ToRgba(isDarkTheme, "#cc99cd"u8, "#ee9900"u8),
        [Comment]               = ToRgba(isDarkTheme, "#999999"u8, "#708090"u8),

        [Tag]                   = ToRgba(isDarkTheme, "#e2777a"u8, "#990055"u8),
        [AttrName]              = ToRgba(isDarkTheme, "#e2777a"u8, "#669900"u8),
        [AttrValue]             = ToRgba(isDarkTheme, "#7ec699"u8, "#0077aa"u8),
        [Namespace]             = ToRgba(isDarkTheme, "#e2777a"u8, in colors[ImGuiCol.Text]),
        [Prolog]                = ToRgba(isDarkTheme, "#999999"u8, "#708090"u8),
        [DocType]               = ToRgba(isDarkTheme, "#999999"u8, "#708090"u8),
        [CData]                 = ToRgba(isDarkTheme, "#999999"u8, "#708090"u8),
        [Entity]                = ToRgba(isDarkTheme, "#67cdcc"u8, "#9a6e3a"u8),

        [Bold]                  = ToRgba(isDarkTheme, in colors[ImGuiCol.Text], in colors[ImGuiCol.Text]),
        [Italic]                = ToRgba(isDarkTheme, in colors[ImGuiCol.Text], in colors[ImGuiCol.Text]),

        [ATRule]                = ToRgba(isDarkTheme, "#cc99cd"u8, "#0077aa"u8),
        [Selector]              = ToRgba(isDarkTheme, "#cc99cd"u8, "#669900"u8),

        [Inserted]              = ToRgba(isDarkTheme, "#008000"u8, "#669900"u8),
        [Deleted]               = ToRgba(isDarkTheme, "#e2777a"u8, "#990055"u8),

        [Directive]             = ToRgba(isDarkTheme, in colors[ImGuiCol.Text], in colors[ImGuiCol.Text]),
        [Preprocessor]          = ToRgba(isDarkTheme, in colors[ImGuiCol.Text], in colors[ImGuiCol.Text]),

        [Attribute]             = ToRgba(isDarkTheme, in colors[ImGuiCol.Text], in colors[ImGuiCol.Text]),
        [ConstructorInvocation] = ToRgba(isDarkTheme, in colors[ImGuiCol.Text], in colors[ImGuiCol.Text]),
        [GenericMethod]         = ToRgba(isDarkTheme, in colors[ImGuiCol.Text], in colors[ImGuiCol.Text]),
        [InterpolationString]   = ToRgba(isDarkTheme, in colors[ImGuiCol.Text], in colors[ImGuiCol.Text]),
        [NamedParameter]        = ToRgba(isDarkTheme, in colors[ImGuiCol.Text], in colors[ImGuiCol.Text]),
        [Range]                 = ToRgba(isDarkTheme, in colors[ImGuiCol.Text], in colors[ImGuiCol.Text]),
        [ReturnType]            = ToRgba(isDarkTheme, in colors[ImGuiCol.Text], in colors[ImGuiCol.Text]),
        [TypeExpression]        = ToRgba(isDarkTheme, in colors[ImGuiCol.Text], in colors[ImGuiCol.Text]),
        [TypeList]              = ToRgba(isDarkTheme, in colors[ImGuiCol.Text], in colors[ImGuiCol.Text]),

        //[FunctionName]          = ToRgba(isDarkTheme, "#6196cc"u8) : colors[ImGuiCol.Text],
#pragma warning restore format // @formatter:on
    });

    // General Purpose
    public static PrismColorPalette Keyword { get; } = new(nameof(Keyword), "keyword");
    public static PrismColorPalette BuiltIn { get; } = new(nameof(BuiltIn), "builtin");
    public static PrismColorPalette ClassName { get; } = new(nameof(ClassName), "class-name");
    public static PrismColorPalette Function { get; } = new(nameof(Function), "function");
    public static PrismColorPalette Boolean { get; } = new(nameof(Boolean), "boolean");
    public static PrismColorPalette Number { get; } = new(nameof(Number), "number");
    public static PrismColorPalette String { get; } = new(nameof(String), "string");
    public static PrismColorPalette Char { get; } = new(nameof(Char), "char");
    public static PrismColorPalette Symbol { get; } = new(nameof(Symbol), "symbol");
    public static PrismColorPalette Regex { get; } = new(nameof(Regex), "regex");
    public static PrismColorPalette Url { get; } = new(nameof(Url), "url");
    public static PrismColorPalette Operator { get; } = new(nameof(Operator), "operator");
    public static PrismColorPalette Variable { get; } = new(nameof(Variable), "variable");
    public static PrismColorPalette Constant { get; } = new(nameof(Constant), "constant");
    public static PrismColorPalette Property { get; } = new(nameof(Property), "property");
    public static PrismColorPalette Punctuation { get; } = new(nameof(Punctuation), "punctuation");
    public static PrismColorPalette Important { get; } = new(nameof(Important), "important");
    public static PrismColorPalette Comment { get; } = new(nameof(Comment), "comment");

    // Markup
    public static PrismColorPalette Tag { get; } = new(nameof(Tag), "tag");
    public static PrismColorPalette AttrName { get; } = new(nameof(AttrName), "attr-name");
    public static PrismColorPalette AttrValue { get; } = new(nameof(AttrValue), "attr-value");
    public static PrismColorPalette Namespace { get; } = new(nameof(Namespace), "namespace");
    public static PrismColorPalette Prolog { get; } = new(nameof(Prolog), "prolog");
    public static PrismColorPalette DocType { get; } = new(nameof(DocType), "doctype");
    public static PrismColorPalette CData { get; } = new(nameof(CData), "cdata");
    public static PrismColorPalette Entity { get; } = new(nameof(Entity), "entity");

    // Document-markup
    public static PrismColorPalette Bold { get; } = new(nameof(Bold), "bold");
    public static PrismColorPalette Italic { get; } = new(nameof(Italic), "italic");

    // Stylesheets
    public static PrismColorPalette ATRule { get; } = new(nameof(ATRule), "atrule");
    public static PrismColorPalette Selector { get; } = new(nameof(Selector), "selector");

    // Diff
    public static PrismColorPalette Inserted { get; } = new(nameof(Inserted), "inserted");
    public static PrismColorPalette Deleted { get; } = new(nameof(Deleted), "deleted");

    // C-like
    public static PrismColorPalette Directive { get; } = new(nameof(Directive), "directive");
    public static PrismColorPalette Preprocessor { get; } = new(nameof(Preprocessor), "preprocessor");

    // CSharp
    public static PrismColorPalette Attribute { get; } = new(nameof(Attribute), "attribute");
    public static PrismColorPalette ConstructorInvocation { get; } = new(nameof(ConstructorInvocation), "constructor-invocation");
    public static PrismColorPalette GenericMethod { get; } = new(nameof(GenericMethod), "generic-method");
    public static PrismColorPalette InterpolationString { get; } = new(nameof(InterpolationString), "interpolation-string");
    public static PrismColorPalette NamedParameter { get; } = new(nameof(NamedParameter), "named-parameter");
    public static PrismColorPalette Range { get; } = new(nameof(Range), "range");
    public static PrismColorPalette ReturnType { get; } = new(nameof(ReturnType), "return-type");
    public static PrismColorPalette TypeExpression { get; } = new(nameof(TypeExpression), "type-expression");
    public static PrismColorPalette TypeList { get; } = new(nameof(TypeList), "type-list");


    private readonly string _tokenName;
    private PrismColorPalette(string uniqueName, string tokenName) : base(uniqueName)
    {
        _tokenName = tokenName;
    }
}