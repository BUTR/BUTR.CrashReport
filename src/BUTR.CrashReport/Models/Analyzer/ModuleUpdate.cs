namespace BUTR.CrashReport.Models.Analyzer;

public sealed record ModuleUpdate
{
    public required string ModuleId { get; set; }
    public required string ModuleVersion { get; set; }
    public required bool ModuleInvolved { get; set; }
}