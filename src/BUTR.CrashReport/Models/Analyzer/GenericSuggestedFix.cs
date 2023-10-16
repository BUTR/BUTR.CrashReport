namespace BUTR.CrashReport.Models.Analyzer;

public sealed record GenericSuggestedFix
{
    public required GenericSuggestedFixType Type { get; set; }
}