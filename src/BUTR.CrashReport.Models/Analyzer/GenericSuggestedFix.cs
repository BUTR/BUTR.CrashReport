namespace BUTR.CrashReport.Models.Analyzer;

/// <summary>
/// Represents a generic crash report fix.
/// </summary>
public sealed record GenericSuggestedFix
{
    /// <summary>
    /// The type of suggested fix.
    /// </summary>
    public required GenericSuggestedFixType Type { get; set; }
}