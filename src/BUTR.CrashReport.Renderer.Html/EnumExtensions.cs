using System;
using System.Collections.Generic;
using System.Linq;

namespace BUTR.CrashReport.Renderer.Html;

internal static class EnumExtensions
{
    public static IEnumerable<TEnum> GetFlags<TEnum>(this TEnum input) where TEnum : Enum =>
        Enum.GetValues(input.GetType()).OfType<TEnum>().Where(@enum => input.HasFlag(@enum));
}