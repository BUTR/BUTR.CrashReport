using BUTR.CrashReport.Renderer.ImGui.Utils;

using HonkPerf.NET.RefLinq.Enumerators;

using System.Collections.Generic;

namespace BUTR.CrashReport.Renderer.ImGui.Extensions;

internal static class RefLinqExtensions
{
    public static RefLinqEnumerable<T, IListEnumerator<T>> ToRefLinq<T>(this IList<T> c) => new(new(c));
}