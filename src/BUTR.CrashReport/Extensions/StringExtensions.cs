using System;

namespace BUTR.CrashReport.Extensions;

public static class StringExtensions
{
    public static string[] Split(this string str, string separator)
    {
        return str.Split(new[] { separator }, StringSplitOptions.None);
    }
    public static string[] Split(this string str, string separator, StringSplitOptions stringSplitOptions)
    {
        return str.Split(new[] { separator }, stringSplitOptions);
    }
    public static string[] Split(this string str, char separator, StringSplitOptions stringSplitOptions)
    {
        return str.Split(new[] { separator }, stringSplitOptions);
    }
}