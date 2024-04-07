using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BUTR.CrashReport.Renderer.Html.Extensions;

internal static class StringBuilderExtensions
{
    public static string EscapeGenerics(this string str) => str.Replace("<", "&lt;").Replace(">", "&gt;");

    public static StringBuilder AppendIf(this StringBuilder builder, Func<StringBuilder, StringBuilder> lambda) => lambda(builder);

    public static StringBuilder AppendJoin(this StringBuilder builder, string separator, IEnumerable<string> lines) => AppendJoinIf(builder, true, separator, lines.ToArray());
    public static StringBuilder AppendJoin(this StringBuilder builder, char separator, IEnumerable<string> lines) => AppendJoinIf(builder, true, separator, lines.ToArray());
    public static StringBuilder AppendJoinIf(this StringBuilder builder, bool condition, string separator, IReadOnlyList<string> lines)
    {
        if (!condition) return builder;

        for (var i = 0; i < lines.Count; i++)
        {
            builder.Append(lines[i]);
            if (lines.Count - 1 != i) builder.Append(separator);
        }
        return builder;
    }
    public static StringBuilder AppendJoinIf(this StringBuilder builder, bool condition, char separator, IReadOnlyList<string> lines)
    {
        if (!condition) return builder;

        for (var i = 0; i < lines.Count; i++)
        {
            builder.Append(lines[i]);
            if (lines.Count - 1 != i) builder.Append(separator);
        }
        return builder;
    }
    public static StringBuilder AppendIf(this StringBuilder builder, bool condition, string? value) => condition ? builder.Append(value) : builder;
    public static StringBuilder AppendIf(this StringBuilder builder, bool condition, char value) => condition ? builder.Append(value) : builder;
    public static StringBuilder AppendIf(this StringBuilder builder, bool condition, int value) => condition ? builder.Append(value) : builder;
    public static StringBuilder AppendIf(this StringBuilder builder, bool condition, StringBuilder value) => condition ? builder.Append(value) : builder;
    public static StringBuilder AppendLineIf(this StringBuilder builder, bool condition) => condition ? builder.AppendLine() : builder;
    public static StringBuilder AppendLineIf(this StringBuilder builder, bool condition, string value) => condition ? builder.AppendLine(value) : builder;

    public static StringBuilder AppendIf(this StringBuilder builder, bool condition, Func<StringBuilder, StringBuilder> lambda) => condition ? lambda(builder) : builder;
   
    public static StringBuilder AppendSb(this StringBuilder builder, Func<StringBuilder, StringBuilder> lambda) => lambda(builder);
}