/*
using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents a MonoMod detour.
/// </summary>
public sealed class MonoModPatchModel
{
    /// <summary>
    /// The type of the detour.
    /// </summary>
    public required MonoModPatchModelType Type { get; set; }
    
    /// <summary>
    /// <inheritdoc cref="ModuleModel.Id"/> Is null if not from a module.
    /// </summary>
    /// <returns><inheritdoc cref="ModuleModel.Id"/></returns>
    public required string? ModuleId { get; set; }

    /// <summary>
    /// <inheritdoc cref="LoaderPluginModel.Id"/> Is null if not from a module.
    /// </summary>
    /// <returns><inheritdoc cref="LoaderPluginModel.Id"/></returns>
    public required string? LoaderPluginId { get; set; }

    /// <summary>
    /// The <see cref="AssemblyIdModel.Name"/> of the assembly that contains the patch.
    /// </summary>
    public required AssemblyIdModel? AssemblyId { get; set; }
    
    public required string Namespace { get; set; }

    /// <summary>
    /// <inheritdoc cref="MonoMod.RuntimeDetour.IsApplied"/>
    /// </summary>
    /// <returns><inheritdoc cref="MonoMod.RuntimeDetour.IsApplied"/></returns>
    public required bool IsActive { get; set; }

    /// <summary>
    /// <inheritdoc cref="MonoMod.RuntimeDetour.DetourConfig.Id"/>
    /// </summary>
    /// <returns><inheritdoc cref="MonoMod.RuntimeDetour.DetourConfig.Id"/></returns>
    public required string Id { get; set; }

    public required int? Index { get; set; }
    
    public required int? MaxIndex { get; set; }
    
    public required int? GlobalIndex { get; set; }
    
    /// <summary>
    /// <inheritdoc cref="MonoMod.RuntimeDetour.DetourConfig.Priority"/>
    /// </summary>
    /// <returns><inheritdoc cref="MonoMod.RuntimeDetour.DetourConfig.Priority"/></returns>
    public required int? Priority { get; set; }

    /// <summary>
    /// <inheritdoc cref="MonoMod.RuntimeDetour.DetourConfig.SubPriority"/>
    /// </summary>
    /// <returns><inheritdoc cref="MonoMod.RuntimeDetour.DetourConfig.SubPriority"/></returns>
    public required int? SubPriority { get; set; }

    /// <summary>
    /// <inheritdoc cref="MonoMod.RuntimeDetour.DetourConfig.Before"/>
    /// </summary>
    /// <returns><inheritdoc cref="MonoMod.RuntimeDetour.DetourConfig.Before"/></returns>
    public required IList<string> Before { get; set; } = new List<string>();

    /// <summary>
    /// <inheritdoc cref="MonoMod.RuntimeDetour.DetourConfig.After"/>
    /// </summary>
    /// <returns><inheritdoc cref="MonoMod.RuntimeDetour.DetourConfig.After"/></returns>
    public required IList<string> After { get; set; } = new List<string>();

    /// <summary>
    /// <inheritdoc cref="CrashReportModel.AdditionalMetadata"/>
    /// </summary>
    /// <returns><inheritdoc cref="CrashReportModel.AdditionalMetadata"/></returns>
    public required IList<MetadataModel> AdditionalMetadata { get; set; } = new List<MetadataModel>();
}
*/