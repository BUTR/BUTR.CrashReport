using System;
using System.IO;
using System.IO.Compression;

namespace BUTR.CrashReport.Renderer.ImGui.Tool;

public static class ZipArchiveEntryExtensions
{
    public static Stream TryOpen(this ZipArchiveEntry? entry)
    {
        try
        {
            return entry?.Open() ?? Stream.Null;
        }
        catch (Exception)
        {
            return Stream.Null;
        }
    }
}