using BUTR.CrashReport.Models;

using System.Collections.Generic;

namespace BUTR.CrashReport.Interfaces;

/// <summary>
/// Converts the data interfaces to models.
/// </summary>
public interface IModelConverter
{
    /// <summary>
    /// Converts the loaded modules to module models.
    /// </summary>
    List<ModuleModel> ToModuleModels(CrashReportInfo crashReportInfo, ICollection<IModuleInfo> loadedModules);

    /// <summary>
    /// Converts the loaded assemblies to assembly models.
    /// </summary>
    List<LoaderPluginModel> ToLoaderPluginModels(CrashReportInfo crashReportInfo, ICollection<ILoaderPluginInfo> loadedLoaderPlugins);
}