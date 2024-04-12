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

The library is already used in Bannerlord. Check [ButterLib](https://www.nexusmods.com/mountandblade2bannerlord/mods/2018)

## What Data is Provided
We analyze the stacktrace of the exception - we find the original method and any Harmony patches that were applied to the original.  
The methods then are decompiled. We expose the Native code (NASM syntax), the IL code, IL+C# and C# code where possible. We have a ways to exclude an assemsbly from any C# decompilation due to possible copyright infringements.  
We also gather all currently loaded mods and their capabilities (like using Shell, FileSystem, etc).  
Optionally, if a plugin loader like BepInEx exiss, it's plugins will also be exposed.  

## Formats
* `HTML` - has the ability to create a 'lite' and 'fat' versions. The 'lite' version includes the crash report data and the logs. The 'fat' can include a minidump, a save file and a screenshot.
* `ZIP` - stores the JSON version of the crash report, a log file, a minidump, a save file and a screenshot.

## Examples
* HTML Lite - https://report.butr.link/05C876
* JSON of that crash report https://report.butr.link/05C876.json

## Backends
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

[`BUTR.CrashReport.Bannerlord.Source`](https://github.com/BUTR/BUTR.CrashReport/tree/master/src/BUTR.CrashReport.Bannerlord.Source) can be used as a reference for implementing most of the interfaces

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
`BUTR.CrashReport.Bannerlord.Tool` - provides the ability to conver a ZIP crash report format to the HTML crash report format
`BUTR.CrashReport.Bannerlord.Parser` - parses the old crash reports for Bannerlord that didn't use the new Json format (pre version 13)
