namespace BUTR.CrashReport.Native;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class PInvokeDelegateLoaderAttribute : Attribute
{
    public Type TypeToWrap { get; }
    public string DllName { get; }
    public bool UseDelegateTypeName { get; }

    public PInvokeDelegateLoaderAttribute(Type typeToWrap, string dllName, bool useDelegateTypeName = false)
    {
        TypeToWrap = typeToWrap;
        DllName = dllName;
        UseDelegateTypeName = useDelegateTypeName;
    }
}