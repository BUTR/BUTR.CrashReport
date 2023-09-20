using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace BUTR.CrashReport;

public interface ICrashReportHelper
{
    StackTrace FromException(Exception exception);

    IEnumerable<StacktraceEntry> Filter(ICollection<StacktraceEntry> stacktraceEntries);

    IEnumerable<IModuleInfo> GetLoadedModules();

    IModuleInfo? GetModuleByType(Type? type);

    IEnumerable<IModuleSubModuleInfo> GetSubModules(IModuleInfo moduleInfo);

    IEnumerable<Assembly> Assemblies();
}