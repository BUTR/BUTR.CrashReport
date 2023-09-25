using System;
using System.Collections.Generic;
using System.Reflection;

namespace BUTR.CrashReport;

public interface ICrashReportHelper
{
    IEnumerable<StacktraceEntry> Filter(ICollection<StacktraceEntry> stacktraceEntries);

    IEnumerable<IModuleInfo> GetLoadedModules();

    IModuleInfo? GetModuleByType(Type? type);

    IEnumerable<Assembly> Assemblies();
}