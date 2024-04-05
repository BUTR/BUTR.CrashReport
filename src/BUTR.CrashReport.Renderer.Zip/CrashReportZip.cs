using System.IO;
using System.IO.Compression;

namespace BUTR.CrashReport.Renderer.Zip;

public static class CrashReportArchiveRenderer
{
    public static Stream Build(Stream crashReportJson, Stream logsJson, Stream miniDump, Stream saveFile, Stream screenshot)
    {
        var ms = new MemoryStream();

        using var archive = new ZipArchive(ms, ZipArchiveMode.Create, true);
        crashReportJson.Seek(0, SeekOrigin.Begin);
        var crashReportFile = archive.CreateEntry("crashreport.json");
        using (var crashReportStream = crashReportFile.Open())
        {
            crashReportJson.CopyTo(crashReportStream);
        }

        if (logsJson != Stream.Null)
        {
            var logsFile = archive.CreateEntry("logs.json");
            using var logsStream = logsFile.Open();
            logsJson.Seek(0, SeekOrigin.Begin);
            logsJson.CopyTo(logsStream);
        }

        if (miniDump != Stream.Null)
        {
            var miniDumpFile = archive.CreateEntry("minidump.dmp");
            using var miniDumpStream = miniDumpFile.Open();
            miniDump.Seek(0, SeekOrigin.Begin);
            miniDump.CopyTo(miniDumpStream);
        }

        if (saveFile != Stream.Null)
        {
            var saveFileFile = archive.CreateEntry("save.sav");
            using var saveFileStream = saveFileFile.Open();
            saveFile.Seek(0, SeekOrigin.Begin);
            saveFile.CopyTo(saveFileStream);
        }

        if (screenshot != Stream.Null)
        {
            var screenshotFile = archive.CreateEntry("screenshot.bmp");
            using var screenshotStream = screenshotFile.Open();
            screenshot.Seek(0, SeekOrigin.Begin);
            screenshot.CopyTo(screenshotStream);
        }

        return ms;
    }
}