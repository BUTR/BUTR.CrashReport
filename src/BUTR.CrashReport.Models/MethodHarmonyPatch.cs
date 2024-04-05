using System.Diagnostics.CodeAnalysis;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents a Harmony patch method.
/// </summary>
public sealed record MethodHarmonyPatch : MethodSimple
{
    /// <summary>
    /// The type of the patch.
    /// </summary>
    public required HarmonyPatchType PatchType { get; set; }

    /// <summary>
    /// Main constructor
    /// </summary>
    public MethodHarmonyPatch() { }

    /// <summary>
    /// The copy constructor
    /// </summary>
    [SetsRequiredMembers]
    public MethodHarmonyPatch(MethodSimple methodSimple, HarmonyPatchType patchType)
    {
        AssemblyId = methodSimple.AssemblyId;
        ModuleId = methodSimple.ModuleId;
        LoaderPluginId = methodSimple.LoaderPluginId;
        MethodDeclaredTypeName = methodSimple.MethodDeclaredTypeName;
        MethodName = methodSimple.MethodName;
        MethodFullDescription = methodSimple.MethodFullDescription;
        MethodParameters = methodSimple.MethodParameters;
        ILInstructions = methodSimple.ILInstructions;
        CSharpILMixedInstructions = methodSimple.CSharpILMixedInstructions;
        CSharpInstructions = methodSimple.CSharpInstructions;
        AdditionalMetadata = methodSimple.AdditionalMetadata;
        PatchType = patchType;
    }

    /// <inheritdoc />
    public bool Equals(MethodHarmonyPatch? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return base.Equals(other) && PatchType == other.PatchType;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            return (base.GetHashCode() * 397) ^ (int) PatchType;
        }
    }
}