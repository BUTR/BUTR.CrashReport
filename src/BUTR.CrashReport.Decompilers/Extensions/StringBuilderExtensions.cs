using System;
using System.Text;

namespace BUTR.CrashReport.Decompilers.Extensions;

internal static class StringBuilderExtensions
{
    public static int IndexOf(this StringBuilder sb, ReadOnlySpan<char> value, int startIndex)
    {
        var length = value.Length;
        var maxSearchLength = sb.Length - length + 1;

        for (var i = startIndex; i < maxSearchLength; ++i)
        {
            if (sb[i] != value[0]) continue;

            var index = 1;
            while (index < length && sb[i + index] == value[index])
                ++index;

            if (index == length)
                return i;
        }

        return -1;
    }
}