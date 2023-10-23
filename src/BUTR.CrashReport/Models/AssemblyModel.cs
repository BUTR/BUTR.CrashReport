using System.Collections.Generic;
using BUTR.CrashReport.Utils;

namespace BUTR.CrashReport.Models;

public record AssemblyModel
{
    public required string? ModuleId { get; set; }
    public required string Name { get; set; }
    public required string Version { get; set; }
    public required string? Culture { get; set; }
    public required string? PublicKeyToken { get; set; }
    public required string Architecture { get; set; }
    public required string Hash { get; set; }
    public required string AnonymizedPath { get; set; }
    public required AssemblyModelType Type { get; set; }
    public required IReadOnlyList<AssemblyImportedTypeReferenceModel> ImportedTypeReferences { get; set; } = new List<AssemblyImportedTypeReferenceModel>();
    public required IReadOnlyList<AssemblyImportedReferenceModel> ImportedAssemblyReferences { get; set; } = new List<AssemblyImportedReferenceModel>();
    public required IReadOnlyList<MetadataModel> AdditionalMetadata { get; set; } = new List<MetadataModel>();
    public string GetFullName() => AssemblyNameFormatter.ComputeDisplayName(Name, Version, Culture, PublicKeyToken);
}