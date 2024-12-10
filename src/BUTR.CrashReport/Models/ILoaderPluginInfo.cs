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
}