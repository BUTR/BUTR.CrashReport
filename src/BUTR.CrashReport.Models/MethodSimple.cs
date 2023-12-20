using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents a method.
/// </summary>
public record MethodSimple
{
    /// <summary>
    /// <inheritdoc cref="ModuleModel.Id"/>
    /// </summary>
    /// <returns><inheritdoc cref="ModuleModel.Id"/></returns>
    public required string? ModuleId { get; set; }

    /// <summary>
    /// <inheritdoc cref="System.Reflection.TypeInfo.Name"/>
    /// </summary>
    /// <returns><inheritdoc cref="System.Reflection.TypeInfo.Name"/></returns>
    public required string? MethodDeclaredTypeName { get; set; }

    /// <summary>
    /// <inheritdoc cref="System.Reflection.MethodInfo.Name"/>
    /// </summary>
    /// <returns><inheritdoc cref="System.Reflection.MethodInfo.Name"/></returns>
    public required string MethodName { get; set; }

    /// <summary>
    /// <inheritdoc cref="HarmonyLib.GeneralExtensions.FullDescription(System.Reflection.MethodBase)"/>
    /// </summary>
    /// <returns><inheritdoc cref="HarmonyLib.GeneralExtensions.FullDescription(System.Reflection.MethodBase)"/></returns>
    public required string MethodFullDescription { get; set; }

    /// <summary>
    /// The list of types that are part of the method signature.
    /// </summary>
    public required IReadOnlyList<string> MethodParameters { get; set; } = new List<string>();

    /// <summary>
    /// <inheritdoc cref="BUTR.CrashReport.StacktraceEntry.CilInstructions"/>
    /// </summary>
    /// <returns><inheritdoc cref="BUTR.CrashReport.StacktraceEntry.CilInstructions"/></returns>
    public required IReadOnlyList<string> CilInstructions { get; set; } = new List<string>();
    
    public required IReadOnlyList<string> CsharpWithCilInstructions { get; set; } = new List<string>();
    
    public required IReadOnlyList<string> CsharpInstructions { get; set; } = new List<string>();

    /// <summary>
    /// <inheritdoc cref="CrashReportModel.AdditionalMetadata"/>
    /// </summary>
    /// <returns><inheritdoc cref="CrashReportModel.AdditionalMetadata"/></returns>
    public required IReadOnlyList<MetadataModel> AdditionalMetadata { get; set; } = new List<MetadataModel>();
}