using BUTR.CrashReport.Models;
using BUTR.CrashReport.Renderer.Html;
using BUTR.CrashReport.Renderer.ImGui.Tool.Models;

using NativeFileDialogSharp;

using Silk.NET.SDL;

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

using TextCopy;

namespace BUTR.CrashReport.Renderer.ImGui.Tool;

internal sealed class CrashReportRendererUtilities : ICrashReportRendererUtilities
{
    public bool IsDefaultDarkMode => true;

    private readonly string? _uploadUrl;
    private readonly int _tenant;

    private readonly CrashReportRendererCapabilities _capabilities =
#if WINDOWS
        CrashReportRendererCapabilities.Dialogs |
#endif
        CrashReportRendererCapabilities.CopyAsHtml |
        CrashReportRendererCapabilities.CloseAndContinue;

    public CrashReportRendererCapabilities Capabilities => _capabilities;

    public CrashReportRendererUtilities(string? uploadUrl, int tenant, CrashReportModel model, LogSourceModel[] logs)
    {
        _uploadUrl = uploadUrl;
        _tenant = tenant;

        if (!string.IsNullOrEmpty(_uploadUrl) && tenant > 0)
        {
            _capabilities |= CrashReportRendererCapabilities.Upload;
        }

        if (!string.IsNullOrEmpty(model.Metadata.LoaderPluginProviderName))
        {
            _capabilities |= CrashReportRendererCapabilities.PluginLoader;
        }

        if (logs.Length > 0)
        {
            _capabilities |= CrashReportRendererCapabilities.Logs;
        }

        /*
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            _capabilities |= CrashReportRendererCapabilities.Dialogs;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && RuntimeInformation.ProcessArchitecture == Architecture.X64)
        {
            _capabilities |= CrashReportRendererCapabilities.Dialogs;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && RuntimeInformation.ProcessArchitecture == Architecture.X64)
        {
            _capabilities |= CrashReportRendererCapabilities.Dialogs;
        }
        */
    }

    private CrashUploaderResult Upload(CrashReportModel crashReportModel, IEnumerable<LogSourceModel> logSources)
    {
        try
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", $"{crashReportModel.Metadata.GameName} CrashUploader v{typeof(CrashReportRendererUtilities).Assembly.GetName().Version}");
            httpClient.DefaultRequestHeaders.Add("Tenant", _tenant.ToString());
            httpClient.DefaultRequestHeaders.Add("CrashReportVersion", crashReportModel.Version.ToString());

            using var memoryStream = new MemoryStream();
            using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                JsonSerializer.Serialize(gZipStream, new CrashReportUploadBody(crashReportModel, logSources), CustomJsonSerializerContext.Default.CrashReportUploadBody);
                memoryStream.Seek(0, SeekOrigin.Begin);
            }

            using var content = new StreamContent(memoryStream);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            content.Headers.ContentEncoding.Add("gzip");
            content.Headers.ContentEncoding.Add("deflate");

            using var request = new HttpRequestMessage(HttpMethod.Post, _uploadUrl);
            request.Content = content;
            using var response = httpClient.Send(request);
            if (response.StatusCode is not HttpStatusCode.OK and not HttpStatusCode.Created)
                return CrashUploaderResult.WrongStatusCode(response.StatusCode);

            using var stream = response.Content.ReadAsStream();
            using var responseReader = new StreamReader(stream);
            var result = responseReader.ReadLine();
            if (string.IsNullOrEmpty(result))
                return CrashUploaderResult.ResponseUrlIsNullOrEmpty();
            return CrashUploaderResult.Success(result);
        }
        catch (Exception e)
        {
            return CrashUploaderResult.FailedWithException(e.ToString());
        }
    }

    private (bool IsSuccessful, string Result) UploadInternal(CrashReportModel crashReport, ICollection<LogSourceModel> logSources)
    {
        var result = Upload(crashReport, (IEnumerable<LogSourceModel>) logSources);
        return result.Status switch
        {
            CrashUploaderStatus.Success => (true, result.Url ?? string.Empty),
            CrashUploaderStatus.ResponseIsNotHttpWebResponse => (false, $"Status: {result.Status}"),
            CrashUploaderStatus.ResponseStreamIsNull => (false, $"Status: {result.Status}"),
            CrashUploaderStatus.WrongStatusCode => (false, $"Status: {result.Status}\nStatusCode: {result.StatusCode}"),
            CrashUploaderStatus.FailedWithException => (false, $"Status: {result.Status}\nException: {result.Exception}"),
            _ => (false, "Unknown error"),
        };
    }

    public unsafe void Upload(CrashReportModel crashReport, ICollection<LogSourceModel> logSources)
    {
        try
        {
            var sdl = SdlProvider.SDL.Value;
            var window = sdl.GLGetCurrentWindow();
            var (isSuccessful, result) = UploadInternal(crashReport, logSources);
            if (isSuccessful)
            {
                ClipboardService.SetText(result);
                sdl.ShowSimpleMessageBox(0x00000040, "Success!\0"u8, $"""
                                                                      Report available at
                                                                      {result}
                                                                      The url was copied to the clipboard!
                                                                      """, window);
            }
            else
            {
                sdl.ShowSimpleMessageBox(0x00000040, "Success!\0"u8, $"""
                                                                      The crash uploader could not upload the report!
                                                                      Please report this to the mod developers!
                                                                      {result}
                                                                      """, window);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public void CopyAsHtml(CrashReportModel crashReport, ICollection<LogSourceModel> logSources)
    {
        var reportAsHtml = CrashReportHtml.Build(crashReport, logSources);
        ClipboardService.SetText(reportAsHtml);
    }

    public void SaveAsHtml(CrashReportModel crashReport, ICollection<LogSourceModel> logSources, bool addMiniDump, bool addLatestSave, bool addScreenshots, Stream stream)
    {
        var reportAsHtml = CrashReportHtml.Build(crashReport, logSources);
        CreatorHtml.Create(crashReport, reportAsHtml, stream);
    }

    public void SaveAsZip(CrashReportModel crashReport, ICollection<LogSourceModel> logSources, Stream stream)
    {
        CreatorZip.Create(crashReport, logSources, stream);
    }

    public Stream SaveFileDialog(string filter, string defaultPath)
    {
        if (Dialog.FileSave(filter, Path.GetDirectoryName(defaultPath)) is { IsOk: true } result)
            return new FileStream(result.Path, FileMode.Create, FileAccess.Write);
        return Stream.Null;
    }
}