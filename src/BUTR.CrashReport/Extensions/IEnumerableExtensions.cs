using System;
using System.Collections.Generic;
using System.Linq;

namespace BUTR.CrashReport.Extensions;

public static class IEnumerableExtensions
{
    public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> items, Func<T, TKey> property) => items.GroupBy(property).Select(x => x.First());
}