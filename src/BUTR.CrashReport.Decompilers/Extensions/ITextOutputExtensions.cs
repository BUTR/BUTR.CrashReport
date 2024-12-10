using ICSharpCode.Decompiler;

using System;

namespace BUTR.CrashReport.Decompilers.Extensions;

internal static class ITextOutputExtensions
{
    public static void Write(this ITextOutput textOutput, ReadOnlySpan<char> line)
    {
        for (var i = 0; i < line.Length; i++)
            textOutput.Write(line[i]);
    }
    public static void WriteLine(this ITextOutput textOutput, ReadOnlySpan<char> line)
    {
        for (var i = 0; i < line.Length; i++)
            textOutput.Write(line[i]);
        textOutput.WriteLine();
    }
}