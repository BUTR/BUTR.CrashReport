using System.Runtime.CompilerServices;

namespace ImGui;

internal static class Guard
{
    public static void ThrowIndexOutOfRangeException(int index, int count)
    {
        if (index < 0 || index >= count)
            throw new IndexOutOfRangeException();
    }
    public static void ThrowIndexOutOfRangeException<TEnum>(TEnum index, TEnum count)
    {
        var indexInt = Unsafe.As<TEnum, int>(ref index);
        var countInt = Unsafe.As<TEnum, int>(ref count);
        if (indexInt < 0 || indexInt >= countInt)
            throw new IndexOutOfRangeException();
    }
}