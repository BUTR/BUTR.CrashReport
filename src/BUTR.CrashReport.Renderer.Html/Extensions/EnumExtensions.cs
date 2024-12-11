using System;
using System.Collections.Generic;
using System.Linq;

namespace BUTR.CrashReport.Renderer.Html.Extensions;

internal static class EnumExtensions
{
    public static IEnumerable<TEnum> GetFlags<TEnum>(this TEnum input) where TEnum : struct, Enum =>
#if NET6_0_OR_GREATER
        Enum.GetValues<TEnum>().Where(@enum => input.HasFlag(@enum));
#else
        Enum.GetValues(input.GetType()).OfType<TEnum>().Where(@enum => input.HasFlag(@enum));
#endif
}