using System;
using System.IO;
using System.IO.Compression;

namespace BUTR.CrashReport.Renderer.Zip;

public class CrashReportZipOptions
{
    public string SaveExtension { get; set; } = "sav";
    public string ScreenshotExtension { get; set; } = "jpg";
}

public static class CrashReportZip
{
    public static Stream Build(Stream crashReportJson, Stream logsJson, Stream miniDump, Stream saveFile, Stream screenshot, CrashReportZipOptions? options = null)
    {
        static void CopyTo(Stream source, Stream destination)
        {
            if (source == Stream.Null)
                return;
            
            source.Seek(0, SeekOrigin.Begin);
            source.CopyTo(destination);
        }

        return BuildLazy(
            x => CopyTo(crashReportJson, x),
            x => CopyTo(logsJson, x),
            x => CopyTo(miniDump, x),
            x => CopyTo(saveFile, x),
            x => CopyTo(screenshot, x),
            options);
    }
    
    public static Stream BuildLazy(Action<Stream> crashReportJson, Action<Stream>? logsJson, Action<Stream>? miniDump, Action<Stream>? saveFile, Action<Stream>? screenshot, CrashReportZipOptions? options = null)
    {
        options ??= new CrashReportZipOptions();

        var ms = new MemoryStream();

        using var archive = new ZipArchive(ms, ZipArchiveMode.Create, true);
        var crashReportFile = archive.CreateEntry("crashreport.json");
        using (var crashReportStream = crashReportFile.Open())
        {
            crashReportJson(crashReportStream);
        }

        if (logsJson != null)
        {
            var logsFile = archive.CreateEntry("logs.json");
            using var logsStream = logsFile.Open();
            logsJson(logsStream);
        }

        if (miniDump != null)
        {
            var miniDumpFile = archive.CreateEntry("minidump.dmp");
            using var miniDumpStream = miniDumpFile.Open();
            miniDump(miniDumpStream);
        }

        if (saveFile != null)
        {
            var saveFileFile = archive.CreateEntry($"save.{options.SaveExtension}");
            using var saveFileStream = saveFileFile.Open();
            saveFile(saveFileStream);
        }

        if (screenshot != null)
        {
            var screenshotFile = archive.CreateEntry($"screenshot.{options.ScreenshotExtension}");
            using var screenshotStream = screenshotFile.Open();
            screenshot(screenshotStream);
        }

        return ms;
    }
}