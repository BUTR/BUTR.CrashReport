using System;
using System.Collections.Generic;
using System.Linq;
using BUTR.CrashReport.Extensions;
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

    public IEnumerable<AssemblyModel> GetAllAssemblies(CrashReportModel crashReport)
    {
        foreach (var kv in AdditionalMetadata.Where(x => x.Key.Equals("METADATA:AdditionalAssembly")))
        {
            var splt = kv.Value.Split(" (");
            var fullName = splt[1].TrimEnd(')');
            if (crashReport.Assemblies.FirstOrDefault(x => fullName.StartsWith(x.FullName)) is { } assemblyModel) yield return assemblyModel;
        }
    }

    public bool ContainsAssemblyReferences(CrashReportModel crashReportModel, string[] assemblyReferences) => GetAllAssemblies(crashReportModel)
        .SelectMany(x => x.ImportedAssemblyReferences)
        .Any(x => assemblyReferences.Any(y => FileSystemName.MatchesSimpleExpression(y.AsSpan(), x.Name.AsSpan())));

    public bool ContainsTypeReferences(CrashReportModel crashReportModel, string[] typeReferences) => GetAllAssemblies(crashReportModel)
        .SelectMany(x => x.ImportedTypeReferences)
        .Any(x => typeReferences.Any(y => FileSystemName.MatchesSimpleExpression(y.AsSpan(), x.FullName.AsSpan())));
}