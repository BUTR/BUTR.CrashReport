namespace BUTR.CrashReport.Models;

public record AssemblyImportedReferenceModel
{
    public required string Name { get; set; }
    public required string FullName { get; set; }
    public required string Version { get; set; }
}