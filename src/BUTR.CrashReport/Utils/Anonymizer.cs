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
    private class VariableList : List<KeyValuePair<string, string>>
    {
        public void Add(string? path, string name)
        {
            if (path is null) return;
            Add(new KeyValuePair<string, string>(path.NormalizePath(), name));
        }
    }

    private static string NormalizePath(this string path) => path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

    private static readonly List<string> AnonymizationPathsSimple = new()
    {
        "steamapps",
        ".local",
    };

    // https://renenyffenegger.ch/notes/Windows/development/environment-variables/index
    // https://gitlab.steamos.cloud/steamrt/steam-runtime-tools/-/blob/main/pressure-vessel/wrap.1.md
    private static readonly VariableList AnonymizationPaths = new()
    {
        { Environment.ExpandEnvironmentVariables("%SystemRoot%"), "SystemRoot" }, // same as windir
        { Environment.ExpandEnvironmentVariables("%ProgramData%"), "ProgramData" }, // same as %ALLUSERSPROFILE%
        { Environment.ExpandEnvironmentVariables("%CommonProgramFiles%"), "CommonProgramFiles" },
        { Environment.ExpandEnvironmentVariables("%CommonProgramFiles(x86)%"), "CommonProgramFiles(x86)" },
        { Environment.ExpandEnvironmentVariables("%CommonProgramW6432%"), "CommonProgramW6432" },
        { Environment.ExpandEnvironmentVariables("%ProgramFiles%"), "ProgramFiles" },
        { Environment.ExpandEnvironmentVariables("%ProgramFiles(x86)%"), "ProgramFiles(x86)" },
        { Environment.ExpandEnvironmentVariables("%ProgramW6432%"), "ProgramW6432" },
        { Environment.ExpandEnvironmentVariables("%AppData%"), "AppData" },
        { Environment.ExpandEnvironmentVariables("%LocalAppData%"), "LocalAppData" },
        { Environment.ExpandEnvironmentVariables("%HomeShare%"), "HomeShare" },
        { Environment.ExpandEnvironmentVariables("%OneDrive%"), "OneDrive" },
        { Environment.ExpandEnvironmentVariables("%TEMP%"), "Temp" },
        { Environment.ExpandEnvironmentVariables("%TMP%"), "Temp" },

        { Environment.GetEnvironmentVariable("STEAM_COMPAT_CLIENT_INSTALL_PATH"), "STEAM_COMPAT_CLIENT_INSTALL_PATH" },
        { Environment.GetEnvironmentVariable("STEAM_COMPAT_INSTALL_PATH"), "STEAM_COMPAT_INSTALL_PATH" },
        { Environment.GetEnvironmentVariable("STEAM_COMPAT_DATA_PATH"), "STEAM_COMPAT_DATA_PATH" },
        { Environment.GetEnvironmentVariable("STEAM_COMPAT_SHADER_PATH"), "STEAM_COMPAT_SHADER_PATH" },

        { Environment.GetEnvironmentVariable("LIBGL_DRIVERS_PATH"), "LIBGL_DRIVERS_PATH" },
        { Environment.GetEnvironmentVariable("LIBVA_DRIVERS_PATH"), "LIBVA_DRIVERS_PATH" },

        { Environment.GetEnvironmentVariable("LD_LIBRARY_PATH"), "LD_LIBRARY_PATH" },

        { Environment.GetEnvironmentVariable("XDG_CACHE_HOME"), "XDG_CACHE_HOME" },
        { Environment.GetEnvironmentVariable("XDG_CONFIG_HOME"), "XDG_CONFIG_HOME" },
        { Environment.GetEnvironmentVariable("XDG_DATA_HOME"), "XDG_DATA_HOME" },

        { Environment.ExpandEnvironmentVariables("%UserProfile%"), "UserProfile" },
        { Environment.ExpandEnvironmentVariables("%SystemDrive%"), "SystemDrive" },
        { Environment.ExpandEnvironmentVariables("%HomeDrive%"), "HomeDrive" },
    };

    private static readonly VariableList AnonymizationPathsMultiColon = new()
    {
        { Environment.GetEnvironmentVariable("STEAM_COMPAT_LIBRARY_PATHS"), "STEAM_COMPAT_LIBRARY_PATHS" },
        { Environment.GetEnvironmentVariable("STEAM_COMPAT_MOUNTS"), "STEAM_COMPAT_MOUNTS" },
        { Environment.GetEnvironmentVariable("STEAM_COMPAT_TOOL_PATHS"), "STEAM_COMPAT_TOOL_PATHS" },
    };

    /// <summary>
    /// Anonymizes the path.
    /// </summary>
    /// <param name="path">Path to be anonymized.</param>
    /// <returns>The anonymized path.</returns>
    public static string AnonymizePath(string path)
    {
        var normalizedPath = path;
        var entries = SplitWithIndex(normalizedPath, Path.DirectorySeparatorChar);

        foreach (var kv in AnonymizationPaths)
        {
            if (string.IsNullOrEmpty(kv.Key)) continue;
            if (TryAnonymizePath(normalizedPath, kv.Key!, kv.Value, out var anonymizedPath) && !string.IsNullOrEmpty(anonymizedPath))
                return anonymizedPath!;
        }

        foreach (var kv in AnonymizationPathsMultiColon)
        {
            if (string.IsNullOrEmpty(kv.Key)) continue;
            if (TryAnonymizePathsColonSeparated(normalizedPath, kv.Key!, kv.Value, out var anonymizedPath) && !string.IsNullOrEmpty(anonymizedPath))
                return anonymizedPath!;
        }

        foreach (var simplePath in AnonymizationPathsSimple)
        {
            if (entries.FirstOrDefault(x => x.Substring.Equals(simplePath, StringComparison.OrdinalIgnoreCase)) is { } entrySimple)
                return path.Substring(entrySimple.Index);
        }

        return path;
    }

    private static bool TryAnonymizePath(string normalizedPath, string variablePath, string replacePath, out string? anonymizedPath)
    {
        if (normalizedPath.StartsWith(variablePath, StringComparison.OrdinalIgnoreCase))
        {
            anonymizedPath = Path.Combine(replacePath, normalizedPath.Substring(variablePath.Length + 1));
            return true;
        }

        anonymizedPath = null;
        return false;
    }

    private static bool TryAnonymizePathsColonSeparated(string normalizedPath, string variablePaths, string replacePath, out string? anonymizedPath)
    {
        var variablePathsSplit = variablePaths.Split(':');
        if (variablePathsSplit.Length == 0) variablePathsSplit = [variablePaths];
        for (var i = 0; i < variablePathsSplit.Length; i++)
        {
            var variablePath = variablePathsSplit[i];
            if (TryAnonymizePath(normalizedPath, variablePath, $"{replacePath}_{i}", out anonymizedPath))
                return true;
        }

        anonymizedPath = null;
        return false;
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