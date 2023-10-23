namespace BUTR.CrashReport.Models;

public record AssemblyImportedTypeReferenceModel
{
    public required string Name { get; set; }
    public required string Namespace { get; set; }
    public required string FullName { get; set; }
}