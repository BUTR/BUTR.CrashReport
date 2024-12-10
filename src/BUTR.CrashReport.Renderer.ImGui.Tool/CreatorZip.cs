using BUTR.CrashReport.Models;
using BUTR.CrashReport.Renderer.Zip;

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace BUTR.CrashReport.Renderer.ImGui.Tool;

internal static class CreatorZip
{
    public static void Create(CrashReportModel crashReport, ICollection<LogSourceModel> logSources, Stream stream)
    {
        using var crashReportStream = new MemoryStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(crashReport, CustomJsonSerializerContext.Default.CrashReportModel)));
        using var logsStream = new MemoryStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(logSources, CustomJsonSerializerContext.Default.LogSourceModelArray)));

        using var archiveSteam = CrashReportZip.Build(crashReportStream, logsStream, Stream.Null, Stream.Null, Stream.Null);
        archiveSteam.Seek(0, SeekOrigin.Begin);
        archiveSteam.CopyTo(stream);
    }
}