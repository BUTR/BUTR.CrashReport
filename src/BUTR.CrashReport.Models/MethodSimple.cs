using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents a method.
/// </summary>
public record MethodSimple
{
    /// <summary>
    /// The assembly identity of the assembly that contains the method.
    /// </summary>
    public required AssemblyIdModel? AssemblyId { get; set; }

    /// <summary>
    /// <inheritdoc cref="ModuleModel.Id"/>
    /// </summary>
    /// <returns><inheritdoc cref="ModuleModel.Id"/></returns>
    public required string? ModuleId { get; set; }

    /// <summary>
    /// <inheritdoc cref="LoaderPluginModel.Id"/>
    /// </summary>
    /// <returns><inheritdoc cref="LoaderPluginModel.Id"/></returns>
    public required string? LoaderPluginId { get; set; }

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
    public required IList<string> MethodParameters { get; set; } = new List<string>();

    /// <summary>
    /// The Common Intermediate Language (CIL/IL) representation of the method.
    /// </summary>
    public required IList<string> ILInstructions { get; set; } = new List<string>();

    /// <summary>
    /// The C# and Common Intermediate Language (CIL/IL) representation of the method.
    /// </summary>
    public required IList<string> CSharpILMixedInstructions { get; set; } = new List<string>();

    /// <summary>
    /// The C# representation the method.
    /// </summary>
    public required IList<string> CSharpInstructions { get; set; } = new List<string>();

    /// <summary>
    /// <inheritdoc cref="CrashReportModel.AdditionalMetadata"/>
    /// </summary>
    /// <returns><inheritdoc cref="CrashReportModel.AdditionalMetadata"/></returns>
    public required IList<MetadataModel> AdditionalMetadata { get; set; } = new List<MetadataModel>();
}