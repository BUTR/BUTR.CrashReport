namespace BUTR.CrashReport.Renderer.ImGui.Tool.Models;

internal enum CrashUploaderStatus
{
    Success,
    ResponseIsNotHttpWebResponse,
    WrongStatusCode,
    ResponseStreamIsNull,
    FailedWithException,
    UrlIsNullOrEmpty,
}