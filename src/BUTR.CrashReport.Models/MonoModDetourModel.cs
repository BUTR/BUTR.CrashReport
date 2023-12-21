using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents a MonoMod detour.
/// </summary>
public sealed record MonoModDetourModel
{
    /// <summary>
    /// The type of the detour.
    /// </summary>
    public required MonoModDetourModelType Type { get; set; }

    /// <summary>
    /// The <see cref="AssemblyModel.Name"/> of the assembly that contains the patch.
    /// </summary>
    public required string? AssemblyName { get; set; }

    /// <summary>
    /// <inheritdoc cref="MonoMod.RuntimeDetour.DetourConfig.Id"/>
    /// </summary>
    /// <returns><inheritdoc cref="MonoMod.RuntimeDetour.DetourConfig.Id"/></returns>
    public required string Id { get; set; }

    /// <summary>
    /// <inheritdoc cref="MonoMod.RuntimeDetour.DetourConfig.Priority"/>
    /// </summary>
    /// <returns><inheritdoc cref="MonoMod.RuntimeDetour.DetourConfig.Priority"/></returns>
    public required int Priority { get; set; }

    /// <summary>
    /// <inheritdoc cref="MonoMod.RuntimeDetour.DetourConfig.Before"/>
    /// </summary>
    /// <returns><inheritdoc cref="MonoMod.RuntimeDetour.DetourConfig.Before"/></returns>
    public required IReadOnlyList<string> Before { get; set; } = new List<string>();

    /// <summary>
    /// <inheritdoc cref="MonoMod.RuntimeDetour.DetourConfig.After"/>
    /// </summary>
    /// <returns><inheritdoc cref="MonoMod.RuntimeDetour.DetourConfig.After"/></returns>
    public required IReadOnlyList<string> After { get; set; } = new List<string>();

    /// <summary>
    /// <inheritdoc cref="CrashReportModel.AdditionalMetadata"/>
    /// </summary>
    /// <returns><inheritdoc cref="CrashReportModel.AdditionalMetadata"/></returns>
    public required IReadOnlyList<MetadataModel> AdditionalMetadata { get; set; } = new List<MetadataModel>();
}