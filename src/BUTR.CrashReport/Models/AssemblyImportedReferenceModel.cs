namespace BUTR.CrashReport.Models;

public record AssemblyImportedReferenceModel
{
    public required string Name { get; set; }
    public required string Version { get; set; }
    public required string? Culture { get; set; }
    public required string? PublicKeyToken { get; set; }
    public string GetFullName() => AssemblyNameFormatter.ComputeDisplayName(Name, Version, Culture, PublicKeyToken);
}