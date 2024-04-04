using BUTR.CrashReport.Decompilers.Utils;
using BUTR.CrashReport.Models;

using System.Collections.Generic;
using System.Linq;

namespace BUTR.CrashReport.Extensions;

/// <summary>
/// Extensions for <inheritdoc cref="BUTR.CrashReport.Models.ModuleModel"/>
/// </summary>
public static class ModuleModelExtensions
{
    /// <summary>
    /// Gets whether the module contains an assembly reference.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="assemblies">The list of available assemblies</param>
    /// <param name="assemblyReferences">The assembly references to search for. Supports wildcard</param>
    public static bool ContainsAssemblyReferences(this ModuleModel model, IEnumerable<AssemblyModel> assemblies, string[] assemblyReferences) => assemblies.Where(x => x.ModuleId == model.Id)
        .SelectMany(x => x.ImportedAssemblyReferences)
        .Any(x => assemblyReferences.Any(y => FileSystemName.MatchesSimpleExpression(y, x.Name)));

    /// <summary>
    /// Gets whether the module contains an type reference.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="assemblies">The list of available assemblies</param>
    /// <param name="typeReferences">The type references to search for. Supports wildcard</param>
    public static bool ContainsTypeReferences(this ModuleModel model, IEnumerable<AssemblyModel> assemblies, string[] typeReferences) => assemblies.Where(x => x.ModuleId == model.Id)
        .SelectMany(x => x.ImportedTypeReferences)
        .Any(x => typeReferences.Any(y => FileSystemName.MatchesSimpleExpression(y, x.FullName)));

    /// <summary>
    /// Gets whether the module contains an assembly reference.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="assemblies">The list of available assemblies</param>
    /// <param name="assemblyReferences">The assembly references to search for. Supports wildcard</param>
    public static bool ContainsAssemblyReferences(this LoaderPluginModel model, IEnumerable<AssemblyModel> assemblies, string[] assemblyReferences) => assemblies.Where(x => x.LoaderPluginId == model.Id)
        .SelectMany(x => x.ImportedAssemblyReferences)
        .Any(x => assemblyReferences.Any(y => FileSystemName.MatchesSimpleExpression(y, x.Name)));

    /// <summary>
    /// Gets whether the module contains an type reference.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="assemblies">The list of available assemblies</param>
    /// <param name="typeReferences">The type references to search for. Supports wildcard</param>
    public static bool ContainsTypeReferences(this LoaderPluginModel model, IEnumerable<AssemblyModel> assemblies, string[] typeReferences) => assemblies.Where(x => x.LoaderPluginId == model.Id)
        .SelectMany(x => x.ImportedTypeReferences)
        .Any(x => typeReferences.Any(y => FileSystemName.MatchesSimpleExpression(y, x.FullName)));
}