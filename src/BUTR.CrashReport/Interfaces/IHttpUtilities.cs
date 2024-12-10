namespace BUTR.CrashReport.Interfaces;

/// <summary>
/// Provides functionality related to http.
/// </summary>
public interface IHttpUtilities
{
    /// <summary>
    /// Used by the decompilation engine to get the content of a source linked source code file.
    /// Returns null if the content could not be retrieved or HTTP is not supported.
    /// </summary>
    string? GetStringFromUrl(string url);
}