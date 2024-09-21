using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BUTR.CrashReport.Utils;

/// <summary>
/// Provides various built-in anonymization methods.
/// </summary>
public static class Anonymizer
{
    private static string Windows = Environment.ExpandEnvironmentVariables("%SystemRoot%").Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
    private static string ProgramData = Environment.ExpandEnvironmentVariables("%ProgramData%").Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
    private static string ProgramFiles = Environment.ExpandEnvironmentVariables("%ProgramW6432%").Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
    private static string ProgramFilesX86 = Environment.ExpandEnvironmentVariables("%ProgramFiles(x86)%").Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

    /// <summary>
    /// Anonymizes the path.
    /// </summary>
    /// <param name="path">Path to be anonymized.</param>
    /// <returns>The anonymized path.</returns>
    public static string AnonymizePath(string path)
    {
        var normalizedPath = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        var entries = SplitWithIndex(normalizedPath, Path.DirectorySeparatorChar);

        if (entries.FirstOrDefault(x => x.Substring.Equals("steamapps", StringComparison.OrdinalIgnoreCase)) is { } entrySteamapps)
            return path.Substring(entrySteamapps.Index);

        if (normalizedPath.StartsWith(Windows, StringComparison.OrdinalIgnoreCase))
            return Path.Combine("Windows", normalizedPath.Substring(Windows.Length + 1));

        if (normalizedPath.StartsWith(ProgramData, StringComparison.OrdinalIgnoreCase))
            return Path.Combine("ProgramData", normalizedPath.Substring(ProgramData.Length + 1));

        if (normalizedPath.StartsWith(ProgramFiles, StringComparison.OrdinalIgnoreCase))
            return Path.Combine("Program Files", normalizedPath.Substring(ProgramFiles.Length + 1));

        if (normalizedPath.StartsWith(ProgramFilesX86, StringComparison.OrdinalIgnoreCase))
            return Path.Combine("Program Files (x86)", normalizedPath.Substring(ProgramFilesX86.Length + 1));

        return path;
    }

    private sealed record PathEntry(string Substring, int Index)
    {
        public string Substring { get; } = Substring;
        public int Index { get; } = Index;
    }

    private static List<PathEntry> SplitWithIndex(string input, char separator)
    {
        var result = new List<PathEntry>();
        var startIndex = 0;

        while (startIndex < input.Length)
        {
            var separatorIndex = input.IndexOf(separator, startIndex);
            if (separatorIndex == -1)
            {
                result.Add(new(input.Substring(startIndex), startIndex));
                break;
            }

            result.Add(new(input.Substring(startIndex, separatorIndex - startIndex), startIndex));
            startIndex = separatorIndex + 1;
        }

        return result;
    }
}