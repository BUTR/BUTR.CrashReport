using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents a module.
/// </summary>
public interface IModuleInfo
{
    /// <summary>
    /// <inheritdoc cref="BUTR.CrashReport.Models.ModuleModel.Id"/>
    /// </summary>
    /// <returns><inheritdoc cref="BUTR.CrashReport.Models.ModuleModel.Id"/></returns>
    string Id { get; }

    /// <summary>
    /// <inheritdoc cref="BUTR.CrashReport.Models.ModuleModel.Version"/>
    /// </summary>
    /// <returns><inheritdoc cref="BUTR.CrashReport.Models.ModuleModel.Version"/></returns>
    string Version { get; }

    /// <summary>
    /// <inheritdoc cref="BUTR.CrashReport.Models.ModuleModel.UpdateInfo"/>
    /// </summary>
    /// <returns><inheritdoc cref="BUTR.CrashReport.Models.ModuleModel.UpdateInfo"/></returns>
    string UpdateInfo { get; }

    /// <summary>
    /// The list SubModules of the Module.
    /// </summary>
    IEnumerable<IModuleSubModuleInfo> SubModules { get; }
}