namespace BUTR.CrashReport;

public interface IModuleSubModuleInfo
{
    string AssemblyName { get; }
    string[] Dependencies { get; }
}