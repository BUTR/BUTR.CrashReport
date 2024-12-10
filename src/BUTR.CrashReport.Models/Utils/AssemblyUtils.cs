using System.Globalization;
using System.Text;

namespace BUTR.CrashReport.Models.Utils;

/// <summary>
/// Provides the assembly utilities.
/// </summary>
internal static class AssemblyUtils
{
    /// <summary>
    /// Gets the public key token as string.
    /// </summary>
    /// <param name="publicKeyToken">The public key token.</param>
    /// <returns> The public key token as string.</returns>
    public static string PublicKeyAsString(byte[]? publicKeyToken)
    {
        var sb = new StringBuilder();
        foreach (var b in publicKeyToken ?? [])
            sb.Append(b.ToString("x2", CultureInfo.InvariantCulture));
        return sb.ToString();
    }
}