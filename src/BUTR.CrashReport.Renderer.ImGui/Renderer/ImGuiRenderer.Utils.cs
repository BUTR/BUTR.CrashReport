using System.Runtime.CompilerServices;

namespace BUTR.CrashReport.Renderer.ImGui.Renderer;

public partial class ImGuiRenderer
{
    protected const MethodImplOptions AggressiveOptimization = (MethodImplOptions) 512;

    protected static void SetNestedDictionary<TDictionary, TKey, TNestedKey, TValue>(TDictionary methodDict, TKey key, TNestedKey nestedKey, TValue value)
        where TDictionary : IDictionary<TKey, Dictionary<TNestedKey, TValue>>, new() where TNestedKey : notnull
    {
        if (!methodDict.TryGetValue(key, out var nestedDict))
            methodDict[key] = nestedDict = new Dictionary<TNestedKey, TValue>();
        nestedDict[nestedKey] = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    protected static int Clamp<TEnum>(TEnum n, TEnum min, TEnum max) where TEnum : Enum
    {
        var nInt = Unsafe.As<TEnum, int>(ref n);
        var minInt = Unsafe.As<TEnum, int>(ref min);
        var maxInt = Unsafe.As<TEnum, int>(ref max);

        if (nInt < minInt) return minInt;
        if (nInt > maxInt) return maxInt;
        return nInt;
    }
}