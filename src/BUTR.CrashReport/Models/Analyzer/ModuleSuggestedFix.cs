namespace BUTR.CrashReport.Models.Analyzer;

public sealed record ModuleSuggestedFix
{
    public required string ModuleId { get; set; }
    public required ModuleSuggestedFixType Type { get; set; }
}