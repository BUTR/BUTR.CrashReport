namespace BUTR.CrashReport.Interfaces;

/// <summary>
/// Anonymizes paths.
/// </summary>
public interface IPathAnonymizer
{
    /// <summary>
    /// Tries to handle the path and return an anonymized version of it.
    /// </summary>
    bool TryHandlePath(string path, out string anonymizedPath);
}