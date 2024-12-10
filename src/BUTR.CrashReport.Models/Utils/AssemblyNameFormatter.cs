using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BUTR.CrashReport.Models.Utils;

internal static class AssemblyNameFormatter
{
    public static StringBuilder ComputeDisplayName(string? name, string? version, string? cultureName, string? publicKeyToken)
    {
        if (name == string.Empty)
            throw new FileLoadException();

        var sb = new StringBuilder();
        if (name != null)
        {
            sb.AppendQuoted(name);
        }

        if (version != null)
        {
            sb.Append(", Version=");
            sb.Append(version);
        }

        if (cultureName != null)
        {
            if (cultureName == string.Empty)
                cultureName = "neutral";
            sb.Append(", Culture=");
            sb.AppendQuoted(cultureName);
        }

        if (publicKeyToken != null)
        {
            if (publicKeyToken == string.Empty)
                publicKeyToken = "null";
            sb.Append(", PublicKeyToken=").Append(publicKeyToken);
        }

        // NOTE: By design (desktop compat) AssemblyName.FullName and ToString() do not include ProcessorArchitecture.

        return sb;
    }

    private static void AppendQuoted(this StringBuilder sb, string s)
    {
        var needsQuoting = false;
        const char quoteChar = '\"';

        //@todo: App-compat: You can use double or single quotes to quote a name, and Fusion (or rather the IdentityAuthority) picks one
        // by some algorithm. Rather than guess at it, I'll just use double-quote consistently.
        if (s != s.Trim() || s.Contains("\"") || s.Contains("\'"))
            needsQuoting = true;

        if (needsQuoting)
            sb.Append(quoteChar);

        for (var i = 0; i < s.Length; i++)
        {
            var addedEscape = false;
            foreach (var kv in EscapeSequences)
            {
                var key = kv.Key;
                var escapeReplacement = kv.Value;

                if (s[i] != escapeReplacement[0])
                    continue;
                if (s.Length - i < escapeReplacement.Length)
                    continue;
                if (s.Substring(i, escapeReplacement.Length).SequenceEqual(escapeReplacement))
                {
                    sb.Append('\\');
                    sb.Append(key);
                    addedEscape = true;
                }
            }

            if (!addedEscape)
                sb.Append(s[i]);
        }

        if (needsQuoting)
            sb.Append(quoteChar);
    }

    public static string GetVersion(Version version)
    {
        var sb = new StringBuilder();
        var canonicalizedVersion = version.CanonicalizeVersion();

        if (canonicalizedVersion.Major == ushort.MaxValue) return sb.ToString();
        sb.Append(canonicalizedVersion.Major);

        if (canonicalizedVersion.Minor == ushort.MaxValue) return sb.ToString();
        sb.Append('.');
        sb.Append(canonicalizedVersion.Minor);

        if (canonicalizedVersion.Build == ushort.MaxValue) return sb.ToString();
        sb.Append('.');
        sb.Append(canonicalizedVersion.Build);

        if (canonicalizedVersion.Revision == ushort.MaxValue) return sb.ToString();
        sb.Append('.');
        sb.Append(canonicalizedVersion.Revision);

        return sb.ToString();
    }

    private static Version CanonicalizeVersion(this Version version)
    {
        var major = (ushort) version.Major;
        var minor = (ushort) version.Minor;
        var build = (ushort) version.Build;
        var revision = (ushort) version.Revision;

        if (major == version.Major && minor == version.Minor && build == version.Build && revision == version.Revision)
            return version;

        return new Version(major, minor, build, revision);
    }

    private static readonly KeyValuePair<char, string>[] EscapeSequences =
    [
        new('\\', "\\"),
        new(',', ","),
        new('=', "="),
        new('\'', "'"),
        new('\"', "\""),
        new('n', Environment.NewLine),
        new('t', "\t"),
    ];
}