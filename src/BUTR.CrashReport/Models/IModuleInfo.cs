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
}