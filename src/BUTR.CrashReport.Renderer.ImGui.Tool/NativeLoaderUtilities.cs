using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BUTR.CrashReport.Renderer.ImGui.Tool;

internal sealed class NativeLoaderUtilities : INativeLoaderUtilities
{
    public IEnumerable<string> GetNativeLibrariesFolderPath()
    {
        try
        {
            var runtimeDirectory = Path.Combine(Environment.CurrentDirectory, "runtimes");
            var libraries = Directory.EnumerateFiles(runtimeDirectory, "*.*", SearchOption.AllDirectories);
            var folders = libraries.Select(x => Path.GetDirectoryName((string?) x)!).Distinct();
            return folders;
        }
        catch (Exception)
        {
            return [];
        }
    }
}