using BUTR.CrashReport.Models;

using System.Collections.Generic;

namespace BUTR.CrashReport.Renderer.ImGui.Tool.Models;

internal sealed record CrashReportUploadBody(CrashReportModel CrashReport, IEnumerable<LogSourceModel> LogSources);