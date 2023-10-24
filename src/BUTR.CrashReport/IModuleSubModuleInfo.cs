namespace BUTR.CrashReport;

/// <summary>
/// Represents a SubModule
/// </summary>
public interface IModuleSubModuleInfo
{
    /// <summary>
    /// <inheritdoc cref="BUTR.CrashReport.Models.ModuleSubModuleModel.AssemblyName"/>
    /// </summary>
    /// <returns><inheritdoc cref="BUTR.CrashReport.Models.ModuleSubModuleModel.AssemblyName"/></returns>
    string AssemblyName { get; }

    /// <summary>
    /// The assemblies that are linked to the submodule
    /// </summary>
    string[] Dependencies { get; }
}