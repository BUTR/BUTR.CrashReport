using BUTR.CrashReport.Models;
using BUTR.CrashReport.Renderer.Html;

using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace BUTR.CrashReport.Renderer.ImGui.WASM;

internal static class CreatorHtml
{
    private static string CompressJson(string jsonModel)
    {
        using var compressedBase64Stream = new MemoryStream();

        using (var base64Stream = new CryptoStream(compressedBase64Stream, new ToBase64Transform(), CryptoStreamMode.Write, true))
        using (var compressorStream = new GZipStream(base64Stream, CompressionLevel.Optimal, true))
        using (var streamWriter = new StreamWriter(compressorStream, Encoding.UTF8, 1024, true))
        {
            streamWriter.Write(jsonModel);
        }

        using (var streamReader = new StreamReader(compressedBase64Stream))
        {
            compressedBase64Stream.Seek(0, SeekOrigin.Begin);
            return streamReader.ReadToEnd();
        }
    }

    public static void Create(CrashReportModel crashReport, string html, Stream stream)
    {
        var json = JsonSerializer.Serialize(crashReport, CustomJsonSerializerContext.Default.CrashReportModel);

        var report = CrashReportHtml.AddData(html, CompressJson(json));

        using var streamWriter = new StreamWriter(stream);
        streamWriter.Write(report);
    }
}