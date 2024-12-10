namespace BUTR.CrashReport.ImGui.Structures;

public interface IRangeAccessor<T> where T : struct
{
    ref T this[int index] { get; }
}

public interface IRangeAccessor<T, in TEnum> where T : struct where TEnum : Enum
{
    ref T this[TEnum index] { get; }
}