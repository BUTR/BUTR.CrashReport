using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents the metadata for a crash report.
/// </summary>
public sealed record CrashReportMetadataModel
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

    /// <inheritdoc />
    public bool Equals(CrashReportMetadataModel? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return GameName == other.GameName && GameVersion == other.GameVersion && LoaderPluginProviderName == other.LoaderPluginProviderName && LoaderPluginProviderVersion == other.LoaderPluginProviderVersion && LauncherType == other.LauncherType && LauncherVersion == other.LauncherVersion && Runtime == other.Runtime && AdditionalMetadata.Equals(other.AdditionalMetadata);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = (GameName != null ? GameName.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ GameVersion.GetHashCode();
            hashCode = (hashCode * 397) ^ (LoaderPluginProviderName != null ? LoaderPluginProviderName.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (LoaderPluginProviderVersion != null ? LoaderPluginProviderVersion.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (LauncherType != null ? LauncherType.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (LauncherVersion != null ? LauncherVersion.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (Runtime != null ? Runtime.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ AdditionalMetadata.GetHashCode();
            return hashCode;
        }
    }
}