using System.Net;

namespace BUTR.CrashReport.Renderer.ImGui.Tool.Models;

internal record CrashUploaderResult
{
    public static CrashUploaderResult Success(string url) => new(CrashUploaderStatus.Success) { Url = url };
    public static CrashUploaderResult WrongStatusCode(HttpStatusCode statusCode) => new(CrashUploaderStatus.WrongStatusCode) { StatusCode = statusCode };
    public static CrashUploaderResult ResponseUrlIsNullOrEmpty() => new(CrashUploaderStatus.UrlIsNullOrEmpty);
    public static CrashUploaderResult FailedWithException(string exception) => new(CrashUploaderStatus.FailedWithException) { Exception = exception };

    public CrashUploaderStatus Status { get; }
    public string? Url { get; private set; }
    public HttpStatusCode? StatusCode { get; private set; }
    public string? Exception { get; private set; }

    private CrashUploaderResult(CrashUploaderStatus status)
    {
        Status = status;
    }
}