using System.Collections.Generic;
using System.Reflection;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents a runtime patch.
/// </summary>
public sealed record ManagedRuntimePatch
{
    /// <summary>
    /// The name of the patch provider. Could be Harmony or MonoMod
    /// </summary>
    public required string PatchProvider { get; set; }

    /// <summary>
    /// The type of the patch. Could be Prefix, Postfix, Finalizer or Transpiler, Detour, ILHook
    /// </summary>
    public required string PatchType { get; set; }

    /// <summary>
    /// The original method that was patched
    /// </summary>
    public required MethodBase Original { get; set; }

    /// <summary>
    /// The patched method
    /// </summary>
    public required MethodBase Patch { get; set; }

    /// <summary>
    /// <inheritdoc cref="CrashReportModel.AdditionalMetadata"/>
    /// </summary>
    /// <returns><inheritdoc cref="CrashReportModel.AdditionalMetadata"/></returns>
    public required IList<MetadataModel> AdditionalMetadata { get; set; } = new List<MetadataModel>();
}