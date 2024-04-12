# BUTR.CrashReport

<p align="center">
  <a href="https://www.nuget.org/packages/BUTR.CrashReport" alt="NuGet BUTR.CrashReport">
    <img src="https://img.shields.io/nuget/v/BUTR.CrashReport?label=NuGet%20BUTR.CrashReport&colorB=blue" />
  </a>
  </br>
  <a href="https://www.nuget.org/packages/BUTR.CrashReport.Renderer.ImGui" alt="NuGet BUTR.CrashReport.Renderer.ImGui">
    <img src="https://img.shields.io/nuget/v/BUTR.CrashReport.Renderer.ImGui?label=NuGet%20BUTR.CrashReport.Renderer.ImGui&colorB=blue" />
  </a>
  <a href="https://www.nuget.org/packages/BUTR.CrashReport.Renderer.WinForms" alt="NuGet BUTR.CrashReport.Renderer.WinForms">
    <img src="https://img.shields.io/nuget/v/BUTR.CrashReport.Renderer.WinForms?label=NuGet%20BUTR.CrashReport.Renderer.WinForms&colorB=blue" />
  </a>
</p>

This is a library that provides detailed crash reports for .NET based games. It is currently used in the game Mount & Blade 2: Bannerlord. For more information, check out [ButterLib](https://www.nexusmods.com/mountandblade2bannerlord/mods/2018).  
Unity is supported, see the [Valheim](https://github.com/BUTR/Valheim.CrashReporter) PoC as an example.

## Table of Contents
- [Data Provided](#data-provided)
- [Export Formats](#export-formats)
- [Examples](#examples)
- [Rendering Backends](#rendering-backends)
- [Usage](#usage)
- [Tools](#tools)
- [Versioning](#versioning)

## Data Provided
BUTR.CrashReport analyzes the stacktrace of the exception to find the original method and any Harmony patches that were applied.  
It decompiles methods - Native code (NASM syntax), IL code, IL+C# and C# code where possible.  
It also gathers all currently loaded mods and their capabilities (like using Shell, FileSystem, etc).  
If a plugin loader like BepInEx exists, its plugins will also be exposed.

## Export Formats
* `HTML` - 'lite' and 'fat' versions. The 'lite' version includes the crash report data and the logs. The 'fat' version can include a minidump, a save file and a screenshot.
* `ZIP` - stores the JSON version of the crash report, a log file, a minidump, a save file and a screenshot.
```
crashreport.zip/
├── crashreport.json
├── logs.json
├── minudump.dmp
├── save.sav
└── screenshot.bmp
```

## Examples
* HTML Lite - https://report.butr.link/05C876
* JSON of that crash report https://report.butr.link/05C876.json

## Rendering Backends
* `ImGui` - uses Dear ImGui ([cimgui](https://github.com/cimgui/cimgui)) via GLFW and OpenGL
* `WinForms` - uses the HTML format and renders it via the `WebBrowser` control

## Usage
Add the `BUTR.CrashReport` NuGet package to your project
The following interfaces should be implemented:
* `IAssemblyUtilities` - Provides functionality related to assemblies.
* `ICrashReportMetadataProvider` - Provides metadata for a crash report.
* `IHarmonyProvider` - Provides information about Harmony patches, if there are any.
* `ILoaderPluginProvider` - Represents the loader plugin information.
* `IModuleProvider` - Provides the implementation for getting the module information.
* `IModelConverter` - Converts the data interfaces to models.
* `IPathAnonymizer` - Anonymizes paths.
* `IStacktraceFilter` - Represents a filter that can be used to filter out irrelevant stack trace frames from a crash report.

Refer to [`BUTR.CrashReport.Bannerlord.Source`](https://github.com/BUTR/BUTR.CrashReport/tree/master/src/BUTR.CrashReport.Bannerlord.Source) and [Valheim.CrashReporter](https://github.com/BUTR/Valheim.CrashReporter) for implementation examples.

Add a backend
* `BUTR.CrashReport.Renderer.ImGui` for the ImGui backend
```csharp
private static IEnumerable<LogSource> GetLogSources() { .. }
...
var crashReport = CrashReportInfo.Create(exception, metadata, ...);
var crashReportModel = CrashReportInfo.ToModel(crashReport, ...);
var logSources = GetLogSources().ToArray();
CrashReportImGui.ShowAndWait(crashReportModel, logSources, ...);
```
* `BUTR.CrashReport.Renderer.WinForms` for the WinForms backend
```csharp
private static IEnumerable<LogSource> GetLogSources() { .. }
...
var crashReport = CrashReportInfo.Create(exception, metadata, ...);
var crashReportModel = CrashReportInfo.ToModel(crashReport, ...);
var logSources = GetLogSources().ToArray();
var forms = new CrashReportWinForms(crashReportModel, logSources, crashReportRendererUtilities);
forms.ShowDialog();
```

## Tools
* `BUTR.CrashReport.Bannerlord.Tool` - converts a ZIP crash report format to the HTML crash report format
* `BUTR.CrashReport.Bannerlord.Parser` - parses old crash reports for Bannerlord that didn't use the new Json format (pre version 13)

## Versioning
The project follows semantic versioning. Only changes related to the JSON model will affect the versioning.
