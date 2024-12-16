using BUTR.CrashReport.Models;
using BUTR.CrashReport.Renderer.ImGui.Implementation.CImGui;
using BUTR.CrashReport.Renderer.ImGui.Tool.Extensions;

using HtmlAgilityPack;

using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace BUTR.CrashReport.Renderer.ImGui.Tool;

public static class Program
{
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetProcessDPIAware();

    private static async Task<T?> TryDeserializeAsync<T>(Stream stream, JsonTypeInfo<T> typeInfo)
    {
        try
        {
            return await JsonSerializer.DeserializeAsync(stream, typeInfo);
        }
        catch (JsonException)
        {
            return default;
        }
    }

    public static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: CrashReport.Renderer.ImGui.Tool <file>");
            return;
        }

        var crashReportPath = args[0].Trim('"');
        var url = args.Length > 1 ? args[1] : null;
        var tenant = args.Length > 2 ? int.TryParse(args[2], out var tenantVar) ? tenantVar : 0 : 0;

        if (!File.Exists(crashReportPath))
        {
            Console.WriteLine("File does not exist.");
            return;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            SetProcessDPIAware();
        }

        // Don't use the Custom if you plan to serialize back to JSON, since we loose data
        var contract = CustomJsonSerializerContext.Default;

        using var crashReportImGui = new CrashReportImGui(new NativeLoaderUtilities(), WindowProvider.Glfw);
        void OnConsoleOnCancelKeyPress(object? o, ConsoleCancelEventArgs consoleCancelEventArgs) => crashReportImGui.Close();
        Console.CancelKeyPress += OnConsoleOnCancelKeyPress;
        switch (Path.GetExtension(crashReportPath))
        {
            case ".zip":
            {
                using var archive = new ZipArchive(File.OpenRead(crashReportPath), ZipArchiveMode.Read, false);

                await using var jsonStream = archive.GetEntry("crashreport.json").TryOpen();
                await using var logsStream = archive.GetEntry("logs.json").TryOpen();
                if (jsonStream == Stream.Null) return;

                var crashReport = await TryDeserializeAsync(jsonStream, contract.CrashReportModel);
                var logs = await TryDeserializeAsync(logsStream, contract.LogSourceModelArray) ?? [];

                crashReportImGui.ShowAndWait(crashReport!, logs, new CrashReportRendererUtilities(url, tenant, crashReport!, logs));
                break;
            }
            case ".json":
            {
                var crashReport = await TryDeserializeAsync(File.OpenRead(crashReportPath), contract.CrashReportModel);
                var logs = Array.Empty<LogSourceModel>();

                crashReportImGui.ShowAndWait(crashReport!, logs, new CrashReportRendererUtilities(url, tenant, crashReport!, logs));
                break;
            }
            case ".html":
            {
                var document = new HtmlDocument();
                document.Load(File.OpenRead(crashReportPath));
                var gzipBase64Json = document.GetElementbyId("json-model-data").InnerText.Trim('\u200B', '\r', '\n', '\r', '\t', ' ');
                if (string.IsNullOrEmpty(gzipBase64Json)) return;

                // Ideally, don't load the HTML at all, just read the file and extract the JSON from it.
                await using var gzipBase64Stream = gzipBase64Json.AsStream();
                using var base64Transform = new FromBase64Transform();
                await using var gzipStream = new CryptoStream(gzipBase64Stream, base64Transform, CryptoStreamMode.Read, false);
                await using var jsonStream = new GZipStream(gzipStream, CompressionMode.Decompress, false);

                var crashReport = await TryDeserializeAsync(jsonStream, contract.CrashReportModel);
                var logs = Array.Empty<LogSourceModel>();

                if (crashReport is null) return;

                crashReportImGui.ShowAndWait(crashReport, logs, new CrashReportRendererUtilities(url, tenant, crashReport, logs));
                break;
            }
            default:
                Console.WriteLine("Invalid file extension.");
                break;
        }
        Console.CancelKeyPress -= OnConsoleOnCancelKeyPress;
    }
}