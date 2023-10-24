using System;

namespace BUTR.CrashReport.Bannerlord.Parser.Extensions;

internal static class StringExtensions
{
    public static string[] Split(this string str, string separator) => str.Split(new[] { separator }, StringSplitOptions.None);

    public static string[] Split(this string str, string separator, StringSplitOptions stringSplitOptions) => str.Split(new[] { separator }, stringSplitOptions);

    public static string[] Split(this string str, char separator, StringSplitOptions stringSplitOptions) => str.Split(new[] { separator }, stringSplitOptions);
}