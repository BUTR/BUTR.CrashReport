using CommandLine;

using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;

namespace BUTR.CrashReport.Renderer.Html.Tool;

public static class Program
{
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetProcessDPIAware();

    public static async Task<int> Main(string[] args)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            SetProcessDPIAware();
        }

        HtmlOptions? parsedOptions = null;

        try
        {
            var parser = Parser.Default
                .ParseArguments<HtmlOptions>(args);

            parser = await parser
                .WithParsedAsync<HtmlOptions>(async options =>
                {
                    parsedOptions = options;

                    var stream = new Uri(options.ArchiveFile).IsFile
                        ? File.OpenRead(options.ArchiveFile)
                        : await new HttpClient().GetStreamAsync(options.ArchiveFile);

                    using var archive = new ZipArchive(stream, ZipArchiveMode.Read, false);

                    await using var jsonStream = archive.GetEntry("crashreport.json").TryOpen();
                    await using var logsStream = archive.GetEntry("logs.json").TryOpen();
                    if (jsonStream == Stream.Null) return;

                    using var minidumpMemoryStream = new MemoryStream();
                    await using var minidumpZipStream = new GZipStream(minidumpMemoryStream, CompressionMode.Compress, true);
                    await using var minidumpStream = archive.GetEntry("minidump.dmp").TryOpen();
                    if (minidumpStream != Stream.Null) await minidumpStream.CopyToAsync(minidumpZipStream);
                    var minidump = Convert.ToBase64String(minidumpMemoryStream.ToArray());

                    using var saveFileMemoryStream = new MemoryStream();
                    await using var saveFileZipStream = new GZipStream(saveFileMemoryStream, CompressionMode.Compress, true);
                    await using var saveFileStream = archive.GetEntry("save.sav").TryOpen();
                    if (saveFileStream != Stream.Null) await saveFileStream.CopyToAsync(saveFileZipStream);
                    var saveFile = Convert.ToBase64String(saveFileMemoryStream.ToArray());

                    using var screenshotMemoryStream = new MemoryStream();
                    await using var screenshotStream = archive.GetEntry("screenshot.bmp").TryOpen();
                    if (screenshotStream != Stream.Null) await screenshotStream.CopyToAsync(screenshotMemoryStream);
                    var screenshot = Convert.ToBase64String(screenshotMemoryStream.ToArray());

                    var crashReportJson = await new StreamReader(jsonStream).ReadToEndAsync();
                    var crashReport = JsonSerializer.Deserialize(crashReportJson, CustomJsonSerializerContext.Default.CrashReportModel)!;
                    var logs = logsStream != Stream.Null ? JsonSerializer.Deserialize(logsStream, CustomJsonSerializerContext.Default.LogSourceModelArray)! : [];

                    var html = CrashReportHtml.AddData(CrashReportHtml.Build(crashReport, logs), crashReportJson, minidump, saveFile, screenshot);

                    var output = options.OutputFile ?? Path.Combine(Path.GetDirectoryName(options.ArchiveFile)!, $"{Path.GetFileNameWithoutExtension(options.ArchiveFile)}.html");
                    await File.WriteAllTextAsync(output, html);
                });

            parser = parser
                .WithNotParsed(e => { Console.Write("INVALID COMMAND"); });

            if (parser.Errors.Any()) return 1;

            return 0;
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("The input file could not be found");
            if (parsedOptions is not null)
                Console.WriteLine($"File: '{parsedOptions.ArchiveFile}'");
            return 1;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred:");
            Console.WriteLine(ex);
            return 1;
        }
    }
}