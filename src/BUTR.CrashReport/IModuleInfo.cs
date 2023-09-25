using System.Collections.Generic;

namespace BUTR.CrashReport;

public interface IModuleInfo
{
    string Id { get; }
    IEnumerable<IModuleSubModuleInfo> SubModules { get; }
}