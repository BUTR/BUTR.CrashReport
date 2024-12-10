using BUTR.CrashReport.Models;

using System.Text.Json.Serialization;

namespace BUTR.CrashReport.Renderer.Html.Tool;

[JsonSourceGenerationOptions(UseStringEnumConverter = true)]
[JsonSerializable(typeof(CrashReportModel))]
[JsonSerializable(typeof(LogSourceModel[]))]
internal partial class CustomJsonSerializerContext : JsonSerializerContext;