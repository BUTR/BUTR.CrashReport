using BUTR.CrashReport.Models;

using System.Text.Json.Serialization;

namespace BUTR.CrashReport.Renderer.ImGui.WASM;

[JsonSourceGenerationOptions(UseStringEnumConverter = true, WriteIndented = true)]
[JsonSerializable(typeof(CrashReportModel))]
[JsonSerializable(typeof(LogSourceModel[]))]
internal partial class CustomJsonSerializerContext : JsonSerializerContext;