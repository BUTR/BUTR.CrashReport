namespace BUTR.CrashReport.Native;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class DelegateLoaderAttribute : Attribute
{
    public Type TypeToWrap { get; }
    public bool UseDelegateTypeName { get; }

    public DelegateLoaderAttribute(Type typeToWrap, bool useDelegateTypeName = false)
    {
        TypeToWrap = typeToWrap;
        UseDelegateTypeName = useDelegateTypeName;
    }
}