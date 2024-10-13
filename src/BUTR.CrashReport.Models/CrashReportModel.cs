using System;
using System.Collections.Generic;
using System.Linq;

namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents the main model of a crash report.
/// </summary>
public sealed record CrashReportModel
{
    /// <summary>
    /// The id of the crash report.
    /// </summary>
    public required Guid Id { get; set; }

    /// <summary>
    /// The version of the crash report.
    /// </summary>
    public required byte Version { get; set; }

    /// <summary>
    /// The exception that caused the crash.
    /// </summary>
    public required ExceptionModel Exception { get; set; }

    /// <summary>
    /// The metadata of the crash report.
    /// </summary>
    public required CrashReportMetadataModel Metadata { get; set; }

    /// <summary>
    /// The list of modules that are loaded in the process.
    /// </summary>
    public required IList<ModuleModel> Modules { get; set; } = new List<ModuleModel>();

    /// <summary>
    /// The list of involved modules in the crash.
    /// </summary>
    public required IList<InvolvedModuleOrPluginModel> InvolvedModules { get; set; } = new List<InvolvedModuleOrPluginModel>();

    /// <summary>
    /// The enhanced stack trace frames.
    /// </summary>
    public required IList<EnhancedStacktraceFrameModel> EnhancedStacktrace { get; set; } = new List<EnhancedStacktraceFrameModel>();

    /// <summary>
    /// The list of assemblies that are present.
    /// </summary>
    public required IList<AssemblyModel> Assemblies { get; set; } = new List<AssemblyModel>();

    /// <summary>
    /// The list of native modules that are present.
    /// </summary>
    public required IList<NativeAssemblyModel> NativeModules { get; set; } = new List<NativeAssemblyModel>();
    
    /// <summary>
    /// The list of runtime patches that are present.
    /// </summary>
    public required IList<RuntimePatchesModel> RuntimePatches { get; set; } = new List<RuntimePatchesModel>();

    /*
    /// <summary>
    /// The list of MonoMod patches that are present.
    /// </summary>
    public required IList<MonoModPatchesModel> MonoModPatches { get; set; } = new List<MonoModPatchesModel>();

    /// <summary>
    /// The list of Harmony patches that are present.
    /// </summary>
    public required IList<HarmonyPatchesModel> HarmonyPatches { get; set; } = new List<HarmonyPatchesModel>();
    */

    /// <summary>
    /// The list of loader plugins that are present.
    /// </summary>
    public required IList<LoaderPluginModel> LoaderPlugins { get; set; } = new List<LoaderPluginModel>();

    /// <summary>
    /// The list of involved loader plugins in the crash.
    /// </summary>
    public required IList<InvolvedModuleOrPluginModel> InvolvedLoaderPlugins { get; set; } = new List<InvolvedModuleOrPluginModel>();

    /// <summary>
    /// Additional metadata associated with the model.
    /// </summary>
    /// <returns>A key:value list of metadatas.</returns>
    public required IList<MetadataModel> AdditionalMetadata { get; set; } = new List<MetadataModel>();

    /// <inheritdoc />
    public bool Equals(CrashReportModel? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id.Equals(other.Id) &&
               Version == other.Version &&
               Exception.Equals(other.Exception) &&
               Metadata.Equals(other.Metadata) &&
               Modules.SequenceEqual(other.Modules) &&
               InvolvedModules.SequenceEqual(other.InvolvedModules) &&
               EnhancedStacktrace.SequenceEqual(other.EnhancedStacktrace) &&
               Assemblies.SequenceEqual(other.Assemblies) &&
               RuntimePatches.SequenceEqual(other.RuntimePatches) &&
               //HarmonyPatches.SequenceEqual(other.HarmonyPatches) &&
               //HarmonyPatches.SequenceEqual(other.HarmonyPatches) &&
               LoaderPlugins.SequenceEqual(other.LoaderPlugins) &&
               InvolvedLoaderPlugins.SequenceEqual(other.InvolvedLoaderPlugins) &&
               AdditionalMetadata.SequenceEqual(other.AdditionalMetadata);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Id.GetHashCode();
            hashCode = (hashCode * 397) ^ Version.GetHashCode();
            hashCode = (hashCode * 397) ^ Exception.GetHashCode();
            hashCode = (hashCode * 397) ^ Metadata.GetHashCode();
            hashCode = (hashCode * 397) ^ Modules.GetHashCode();
            hashCode = (hashCode * 397) ^ InvolvedModules.GetHashCode();
            hashCode = (hashCode * 397) ^ EnhancedStacktrace.GetHashCode();
            hashCode = (hashCode * 397) ^ Assemblies.GetHashCode();
            hashCode = (hashCode * 397) ^ RuntimePatches.GetHashCode();
            //hashCode = (hashCode * 397) ^ HarmonyPatches.GetHashCode();
            //hashCode = (hashCode * 397) ^ HarmonyPatches.GetHashCode();
            hashCode = (hashCode * 397) ^ LoaderPlugins.GetHashCode();
            hashCode = (hashCode * 397) ^ InvolvedLoaderPlugins.GetHashCode();
            hashCode = (hashCode * 397) ^ AdditionalMetadata.GetHashCode();
            return hashCode;
        }
    }
}