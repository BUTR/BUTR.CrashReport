using System.Runtime.CompilerServices;

namespace BUTR.CrashReport.ImGui.Utils;

public static class CollectionsMarshal
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan<T>(this List<T>? list) => CollectionsMarshal<T>.AsSpan(list);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan<T>(this IList<T>? list) => CollectionsMarshal<T>.AsSpan(list);
}

public static class CollectionsMarshal<T>
{
    private static readonly Func<List<T>, T[]> GetItems;
    private static readonly Func<List<T>, int> GetSize;

    static CollectionsMarshal()
    {
        GetItems = FieldAccessor.CompileFieldGetter<List<T>, T[]>("_items");
        GetSize = FieldAccessor.CompileFieldGetter<List<T>, int>("_size");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan(List<T>? list) => list is null ? default : new Span<T>(GetItems(list), 0, GetSize(list));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan(IList<T>? list)
    {
        if (list is null)
            return default;

        if (list is List<T> listT)
            return AsSpan(listT);

        if (list is T[] array)
            return new Span<T>(array);

        throw new NotSupportedException("Only List<T> and T[] are supported.");
    }
}