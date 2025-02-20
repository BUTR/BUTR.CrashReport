using System;

namespace BUTR.CrashReport.AutomatedRemediation;

/// <summary>
/// Represents the type of generic fix.
/// </summary>
[Flags]
public enum CrashGenericFixType
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