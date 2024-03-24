namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents a SubModule
/// </summary>
public interface IModuleSubModuleInfo
{
    /// <summary>
    /// <inheritdoc cref="BUTR.CrashReport.Models.AssemblyIdModel.Name"/>
    /// </summary>
    /// <returns><inheritdoc cref="BUTR.CrashReport.Models.AssemblyIdModel.Name"/></returns>
    string AssemblyFile { get; }

    /// <summary>
    /// The assemblies that are linked to the submodule
    /// </summary>
    string[] Dependencies { get; }
}