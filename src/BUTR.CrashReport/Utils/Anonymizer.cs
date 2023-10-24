using System;

namespace BUTR.CrashReport.Utils;

/// <summary>
/// Provides various anonymization methods.
/// </summary>
public static class Anonymizer
{
    /// <summary>
    /// Anonymizes the path.
    /// </summary>
    /// <param name="path">Path to be anonymized.</param>
    /// <returns>The anonymized path.</returns>
    public static string AnonymizePath(string path)
    {
        if (path.IndexOf("steamapps", StringComparison.OrdinalIgnoreCase) is var idxSteam and not -1)
            return path.Substring(idxSteam);

        if (path.IndexOf("Mount & Blade II Bannerlord", StringComparison.OrdinalIgnoreCase) is var idxRoot and not -1)
            return path.Substring(idxRoot);

        if (path.IndexOf("Windows", StringComparison.OrdinalIgnoreCase) is var idxWindows and not -1)
            return path.Substring(idxWindows);

        return path;
    }
}