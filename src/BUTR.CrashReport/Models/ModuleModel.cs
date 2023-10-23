using System;
using System.Collections.Generic;
using System.Linq;
using BUTR.CrashReport.Utils;

namespace BUTR.CrashReport.Models;

public sealed record ModuleModel
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Version { get; set; }
    public required bool IsExternal { get; set; }
    public required bool IsOfficial { get; set; }
    public required bool IsSingleplayer { get; set; }
    public required bool IsMultiplayer { get; set; }
    public required string? Url { get; set; }
    public required string? UpdateInfo { get; set; }
    public required IReadOnlyList<ModuleDependencyMetadataModel> DependencyMetadatas { get; set; } = new List<ModuleDependencyMetadataModel>();
    public required IReadOnlyList<ModuleSubModuleModel> SubModules { get; set; } = new List<ModuleSubModuleModel>();
    public required IReadOnlyList<MetadataModel> AdditionalMetadata { get; set; } = new List<MetadataModel>();

    public IEnumerable<AssemblyModel> GetAllAssemblies(IReadOnlyList<AssemblyModel> assemblies) => assemblies.Where(x => x.ModuleId == Id);

    public bool ContainsAssemblyReferences(IReadOnlyList<AssemblyModel> assemblies, string[] assemblyReferences) => GetAllAssemblies(assemblies)
        .SelectMany(x => x.ImportedAssemblyReferences)
        .Any(x => assemblyReferences.Any(y => FileSystemName.MatchesSimpleExpression(y.AsSpan(), x.Name.AsSpan())));

    public bool ContainsTypeReferences(IReadOnlyList<AssemblyModel> assemblies, string[] typeReferences) => GetAllAssemblies(assemblies)
        .SelectMany(x => x.ImportedTypeReferences)
        .Any(x => typeReferences.Any(y => FileSystemName.MatchesSimpleExpression(y.AsSpan(), x.FullName.AsSpan())));
}