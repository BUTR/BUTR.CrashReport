using System.Collections.Generic;

namespace BUTR.CrashReport;

public interface IModuleInfo
{
    string Id { get; }
    string Version { get; }
    string UpdateInfo { get; }
    IEnumerable<IModuleSubModuleInfo> SubModules { get; }
}