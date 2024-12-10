using BUTR.CrashReport.Models;
using BUTR.CrashReport.Renderer.ImGui.Tool.Models;

using System.Text.Json.Serialization;

namespace BUTR.CrashReport.Renderer.ImGui.Tool;

[JsonSourceGenerationOptions(UseStringEnumConverter = true, WriteIndented = true)]
[JsonSerializable(typeof(CrashReportUploadBody))]
[JsonSerializable(typeof(CrashReportModel))]
[JsonSerializable(typeof(LogSourceModel[]))]
internal partial class CustomJsonSerializerContext : JsonSerializerContext;