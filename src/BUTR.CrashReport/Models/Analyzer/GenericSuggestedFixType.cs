using System;

namespace BUTR.CrashReport.Models.Analyzer;

[Flags]
public enum GenericSuggestedFixType
{
    None = 0,
    VerifyFiles = 1,
}