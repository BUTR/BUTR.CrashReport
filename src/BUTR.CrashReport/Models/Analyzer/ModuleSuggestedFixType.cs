using System;

namespace BUTR.CrashReport.Models.Analyzer;

[Flags]
public enum ModuleSuggestedFixType
{
    None = 0,
    Update = 1,
    Downgrade = 2,
    Remove = 4,
}