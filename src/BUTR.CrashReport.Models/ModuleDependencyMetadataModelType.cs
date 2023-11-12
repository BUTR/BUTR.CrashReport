namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents the type of teh dependency metadata.
/// </summary>
public enum ModuleDependencyMetadataModelType
{
    /// <summary>
    /// The depencency will load before this module.
    /// </summary>
    LoadBefore = 1,

    /// <summary>
    /// The depencency will load after this module.
    /// </summary>
    LoadAfter = 2,

    /// <summary>
    /// The depencency will prevent this module from loading.
    /// </summary>
    Incompatible = 3,
}