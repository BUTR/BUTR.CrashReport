namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents a loader plugin.
/// </summary>
public interface ILoaderPluginInfo
{
    /// <summary>
    /// <inheritdoc cref="BUTR.CrashReport.Models.LoaderPluginModel.Id"/>
    /// </summary>
    /// <returns><inheritdoc cref="BUTR.CrashReport.Models.LoaderPluginModel.Id"/></returns>
    string Id { get; }

    /// <summary>
    /// <inheritdoc cref="BUTR.CrashReport.Models.LoaderPluginModel.Version"/>
    /// </summary>
    /// <returns><inheritdoc cref="BUTR.CrashReport.Models.LoaderPluginModel.Version"/></returns>
    string? Version { get; }

    /// <summary>
    /// <inheritdoc cref="BUTR.CrashReport.Models.LoaderPluginModel.UpdateInfo"/>
    /// </summary>
    /// <returns><inheritdoc cref="BUTR.CrashReport.Models.LoaderPluginModel.UpdateInfo"/></returns>
    string? UpdateInfo { get; }
}