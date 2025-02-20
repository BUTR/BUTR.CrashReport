using System.Collections.Generic;
using System.Linq;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents a method.
/// </summary>
public record MethodModel
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
    /// The list of generic type parameters that are part of the method signature.
    /// </summary>
    public required IList<string> MethodTypeParameters { get; set; } = new List<string>();

    /// <summary>
    /// The list of generic type arguments that are part of the method signature.
    /// </summary>
    public required IList<string> MethodTypeArguments { get; set; } = new List<string>();

    /// <summary>
    /// The list of types that are part of the method signature.
    /// </summary>
    public required IList<string> MethodParameters { get; set; } = new List<string>();

    /// <summary>
    /// The list of types that are part of the method signature.
    /// </summary>
    public required IList<string> MethodParameterNames { get; set; } = new List<string>();

    /// <summary>
    /// The Common Intermediate Language (CIL/IL) representation of the method.
    /// </summary>
    public required InstructionsModel? ILInstructions { get; set; }

    /// <summary>
    /// The Common Intermediate Language (CIL/IL) with C# representation of the method.
    /// </summary>
    public required InstructionsModel? ILMixedInstructions { get; set; }

    /// <summary>
    /// The C# representation the method.
    /// </summary>
    public required InstructionsModel? CSharpInstructions { get; set; }

    /// <summary>
    /// <inheritdoc cref="CrashReportModel.AdditionalMetadata"/>
    /// </summary>
    /// <returns><inheritdoc cref="CrashReportModel.AdditionalMetadata"/></returns>
    public required IList<MetadataModel> AdditionalMetadata { get; set; } = new List<MetadataModel>();

    /// <inheritdoc />
    public virtual bool Equals(MethodModel? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Equals(AssemblyId, other.AssemblyId) &&
               ModuleId == other.ModuleId &&
               LoaderPluginId == other.LoaderPluginId &&
               MethodDeclaredTypeName == other.MethodDeclaredTypeName &&
               MethodName == other.MethodName &&
               MethodFullDescription == other.MethodFullDescription &&
               MethodTypeParameters.SequenceEqual(other.MethodTypeParameters) &&
               MethodTypeArguments.SequenceEqual(other.MethodTypeArguments) &&
               MethodParameters.SequenceEqual(other.MethodParameters) &&
               ILInstructions == other.ILInstructions &&
               ILMixedInstructions == other.ILMixedInstructions &&
               CSharpInstructions == other.CSharpInstructions &&
               AdditionalMetadata.SequenceEqual(other.AdditionalMetadata);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = (AssemblyId != null ? AssemblyId.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ ModuleId?.GetHashCode() ?? 0;
            hashCode = (hashCode * 397) ^ LoaderPluginId?.GetHashCode() ?? 0;
            hashCode = (hashCode * 397) ^ MethodDeclaredTypeName?.GetHashCode() ?? 0;
            hashCode = (hashCode * 397) ^ MethodName.GetHashCode();
            hashCode = (hashCode * 397) ^ MethodFullDescription.GetHashCode();
            hashCode = (hashCode * 397) ^ MethodTypeParameters.GetHashCode();
            hashCode = (hashCode * 397) ^ MethodTypeArguments.GetHashCode();
            hashCode = (hashCode * 397) ^ MethodParameters.GetHashCode();
            hashCode = (hashCode * 397) ^ ILInstructions?.GetHashCode() ?? 0;
            hashCode = (hashCode * 397) ^ ILMixedInstructions?.GetHashCode() ?? 0;
            hashCode = (hashCode * 397) ^ CSharpInstructions?.GetHashCode() ?? 0;
            hashCode = (hashCode * 397) ^ AdditionalMetadata.GetHashCode();
            return hashCode;
        }
    }
}