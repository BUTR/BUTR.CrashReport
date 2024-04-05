using BUTR.CrashReport.Renderer.Utils;

using HonkPerf.NET.RefLinq.Enumerators;

using System.Collections.Generic;

namespace BUTR.CrashReport.Renderer.Extensions;

internal static class RefLinqExtensions
{
    public static RefLinqEnumerable<T, IListEnumerator<T>> ToRefLinq<T>(this IList<T> c) => new(new(c));
}