namespace BUTR.CrashReport.Models;

public record MetadataModel
{
    public required string Key { get; set; }
    public required string Value { get; set; }
}