using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents the metadata for a crash report.
/// </summary>
public record CrashReportMetadataModel
{
    /// <summary>
    /// The game name.
    /// </summary>
    public required string? GameName { get; set; }

    /// <summary>
    /// The game version.
    /// </summary>
    public required string GameVersion { get; set; }

    /// <summary>
    /// The loader plugin provider name that was used to launch the game.
    /// </summary>
    public required string? LoaderPluginProviderName { get; set; }

    /// <summary>
    /// The loader plugin provider version that was used to launch the game.
    /// </summary>
    public required string? LoaderPluginProviderVersion { get; set; }

    /// <summary>
    /// The launcher type that was used to launch the game. Usually it's the process name.
    /// </summary>
    public required string? LauncherType { get; set; }

    /// <summary>
    /// The launcher version that was used to launch the game. Usually it's the process version.
    /// </summary>
    public required string? LauncherVersion { get; set; }

    /// <summary>
    /// The .NET runtime version that was used to launch the game.
    /// </summary>
    public required string? Runtime { get; set; }

    /// <summary>
    /// <inheritdoc cref="CrashReportModel.AdditionalMetadata"/>
    /// </summary>
    /// <returns><inheritdoc cref="CrashReportModel.AdditionalMetadata"/></returns>
    public required IList<MetadataModel> AdditionalMetadata { get; set; } = new List<MetadataModel>();
}