using System;
using System.Globalization;

namespace BUTR.CrashReport.Models.Utils;

/// <summary>
/// Provides the assembly utilities.
/// </summary>
public static class AssemblyUtils
{
    /// <summary>
    /// Gets the public key token as string.
    /// </summary>
    /// <param name="publicKeyToken">The public key token.</param>
    /// <returns> The public key token as string.</returns>
    public static string PublicKeyAsString(byte[]? publicKeyToken) =>
        string.Join(string.Empty, Array.ConvertAll(publicKeyToken ?? [], x => x.ToString("x2", CultureInfo.InvariantCulture)));
}