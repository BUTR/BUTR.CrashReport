using System.IO.Compression;

namespace BUTR.CrashReport.Renderer.ImGui.WASM.Extensions;

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