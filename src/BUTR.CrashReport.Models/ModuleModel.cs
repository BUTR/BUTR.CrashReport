﻿using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents a game module that is loaded into the process.
/// </summary>
public sealed record ModuleModel
{
    /// <summary>
    /// The id of the module.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// The name of the module.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// The version of the module.
    /// </summary>
    public required string Version { get; set; }

    /// <summary>
    /// Whether the module was installed by an external source like Steam Workshop.
    /// </summary>
    public required bool IsExternal { get; set; }

    /// <summary>
    /// Whether the module is an official module.
    /// </summary>
    public required bool IsOfficial { get; set; }

    /// <summary>
    /// Whether the module is a singleplayer module.
    /// </summary>
    public required bool IsSingleplayer { get; set; }

    /// <summary>
    /// Whether the module is a multiplayer module.
    /// </summary>
    public required bool IsMultiplayer { get; set; }

    /// <summary>
    /// The URL of the module. Usually the URL is either NexusMods or Steam Workshop.
    /// </summary>
    public required string? Url { get; set; }

    /// <summary>
    /// The Key:Value pair for updating the module. The Key can be 'NexusMods:%MODID%' or 'GitHub:%USER%/%REPO%'.
    /// </summary>
    public required string? UpdateInfo { get; set; }

    /// <summary>
    /// The dependencies of the module.
    /// </summary>
    public required IReadOnlyList<ModuleDependencyMetadataModel> DependencyMetadatas { get; set; } = new List<ModuleDependencyMetadataModel>();

    /// <summary>
    /// The submodules of the module.
    /// </summary>
    public required IReadOnlyList<ModuleSubModuleModel> SubModules { get; set; } = new List<ModuleSubModuleModel>();

    /// <summary>
    /// <inheritdoc cref="CrashReportModel.AdditionalMetadata"/>
    /// </summary>
    /// <returns><inheritdoc cref="CrashReportModel.AdditionalMetadata"/></returns>
    public required IReadOnlyList<MetadataModel> AdditionalMetadata { get; set; } = new List<MetadataModel>();
}