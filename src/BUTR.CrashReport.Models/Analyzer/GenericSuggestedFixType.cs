using System;

namespace BUTR.CrashReport.Models.Analyzer;

/// <summary>
/// Represents the type of a generic fix.
/// </summary>
[Flags]
public enum GenericSuggestedFixType
{
    /// <summary>
    /// No fix.
    /// </summary>
    None = 0,

    /// <summary>
    /// Perform game file verification.
    /// </summary>
    VerifyFiles = 1,
}