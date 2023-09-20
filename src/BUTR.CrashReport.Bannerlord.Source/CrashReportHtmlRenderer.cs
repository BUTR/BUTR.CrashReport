// <auto-generated>
//   This code file has automatically been added by the "BUTR.CrashReport.Bannerlord.Source" NuGet package (https://www.nuget.org/packages/BUTR.CrashReport.Bannerlord.Source).
//   Please see https://github.com/BUTR/BUTR.CrashReport for more information.
//
//   IMPORTANT:
//   DO NOT DELETE THIS FILE if you are using a "packages.config" file to manage your NuGet references.
//   Consider migrating to PackageReferences instead:
//   https://docs.microsoft.com/en-us/nuget/consume-packages/migrate-packages-config-to-package-reference
//   Migrating brings the following benefits:
//   * The "BUTR.CrashReport.Bannerlord.Source" folder and the "CrashReportHtmlRenderer.cs" file don't appear in your project.
//   * The added file is immutable and can therefore not be modified by coincidence.
//   * Updating/Uninstalling the package will work flawlessly.
// </auto-generated>

#region License
// MIT License
//
// Copyright (c) Bannerlord's Unofficial Tools & Resources
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion

#if !BUTRCRASHREPORT_DISABLE
#nullable enable
#if !BUTRCRASHREPORT_ENABLEWARNINGS
#pragma warning disable
#endif

namespace BUTR.CrashReport.Bannerlord
{
    using global::Bannerlord.BUTR.Shared.Extensions;
    using global::Bannerlord.BUTR.Shared.Helpers;
    using global::Bannerlord.ModuleManager;

    using global::BUTR.CrashReport.Models;

    using global::System;
    using global::System.Collections.Generic;
    using global::System.IO;
    using global::System.Linq;
    using global::System.Text;

    internal static class CrashReportHtmlRenderer
    {
        private static readonly string NL = Environment.NewLine;

        public static readonly string MiniDumpTag = "<!-- MINI DUMP -->";
        public static readonly string MiniDumpButtonTag = "<!-- MINI DUMP BUTTON -->";
        public static readonly string SaveFileTag = "<!-- SAVE FILE -->";
        public static readonly string SaveFileButtonTag = "<!-- SAVE FILE BUTTON -->";
        public static readonly string ScreenshotTag = "<!-- SCREENSHOT -->";
        public static readonly string ScreenshotButtonTag = "<!-- SCREENSHOT BUTTON -->";
        public static readonly string DecompressScriptTag = "<!-- DECOMPRESS SCRIPT -->";

        public static string Build(CrashReportInfo crashReportInfo, IEnumerable<LogSource> files)
        {
            var crashReportModel = CrashReportCreator.Create(crashReportInfo);

            var runtime = crashReportModel.Metadata.Runtime;

            var launcherType = crashReportModel.Metadata.LauncherType;
            var launcherVersion = crashReportModel.Metadata.LauncherVersion;

            var butrLoaderVersion = crashReportModel.Metadata.AdditionalMetadata.FirstOrDefault(x => x.Key == "BUTRLoaderVersion").Value is { } butrLoaderVersionVal ? butrLoaderVersionVal : string.Empty;
            var blseVersion = crashReportModel.Metadata.AdditionalMetadata.FirstOrDefault(x => x.Key == "BLSEVersion").Value is { } blseVersionVal ? blseVersionVal : string.Empty;
            var launcherExVersion = crashReportModel.Metadata.AdditionalMetadata.FirstOrDefault(x => x.Key == "LauncherExVersion").Value is { } launcherExVersionVal ? launcherExVersionVal : string.Empty;

#pragma warning disable format // @formatter:off
            return $$"""
<html>
<head>
<title>Bannerlord Crash Report</title>
<meta charset='utf-8'>
<game version='{{ApplicationVersionHelper.GameVersionStr()}}'>
<launcher type='{{launcherType}}' version='{{launcherVersion}}'>
<runtime value='{{runtime}}'>
{{(string.IsNullOrEmpty(butrLoaderVersion) ? "" : $"<butrloader version='{butrLoaderVersion}'>")}}
{{(string.IsNullOrEmpty(blseVersion) ? "" : $"<blse version='{blseVersion}'>")}}
{{(string.IsNullOrEmpty(launcherExVersion) ? "" : $"<launcherex version='{launcherExVersion}'>")}}
<report id='{{crashReportModel.Id}}' version='{{crashReportModel.Version}}'>
<style>
    .headers {
        font-family: "Consolas", monospace;
    }
    .root-container {
        font-family: "Consolas", monospace;
        font-size: small;

        margin: 5px;
        background-color: white;
        border: 1px solid grey;
        padding: 5px;
    }
    .headers-container {
        display: none;
    }
    .modules-container {
        margin: 5px;
        background-color: #ffffe0;
        border: 1px solid grey;
        padding: 5px;
    }
    .submodules-container {
        margin: 5px;
        border: 1px solid grey;
        background-color: #f8f8e7;
        padding: 5px;
    }
    .modules-official-container {
        margin: 5px;
        background-color: #f4fcdc;
        border: 1px solid grey;
        padding: 5px;
    }
    .modules-external-container {
        margin: 5px;
        background-color: #ede9e0;
        border: 1px solid grey;
        padding: 5px;
    }
    .submodules-official-container {
        margin: 5px;
        border: 1px solid grey;
        background-color: #f0f4e4;
        padding: 5px;
    }
    .modules-invalid-container {
        margin: 5px;
        background-color: #ffefd5;
        border: 1px solid grey;
        padding: 5px;
    }
    .submodules-invalid-container {
        margin: 5px;
        border: 1px solid grey;
        background-color: #f5ecdf;
        padding: 5px;
    }
</style>
</head>
<body style='background-color: #ececec;'>
<table style='width: 100%;'>
  <tbody>
    <tr>
      <td style='width: 80%;'>
        <div>
          <b>Bannerlord has encountered a problem and will close itself.</b>
          <br/>
          This is a community Crash Report. Please save it and use it for reporting the error. Do not provide screenshots, provide the report!
          <br/>
          Most likely this error was caused by a custom installed module.
          <br/>
          <br/>
          If you were in the middle of something, the progress might be lost.
          <br/>
          <br/>
          Launcher: {{launcherType}} ({{launcherVersion}})
          <br/>
          Runtime: {{runtime}}
          {{(string.IsNullOrEmpty(blseVersion) ? "" : $"<br/>BLSE Version: {blseVersion}")}}
          {{(string.IsNullOrEmpty(launcherExVersion) ? "" : $"<br/>LauncherEx Version: {launcherExVersion}")}}
          <br/>
        </div>
      </td>
      <td>
          <div style='float:right; margin-left:10px;'>
          <label>Without Color:</label>
          <input type='checkbox' onclick='changeBackgroundColor(this)'>
          <br/>
          <br/>
          <label>Font Size:</label>
          <select class='input' onchange='changeFontSize(this);'>
            <option value='1.0em' selected='selected'>Standard</option>
            <option value='0.9em'>Medium</option>
            <option value='0.8em'>Small</option>
          </select>
{{MiniDumpButtonTag}}
{{SaveFileButtonTag}}
{{ScreenshotButtonTag}}
        </div>
      </td>
    </tr>
  </tbody>
</table>
<div class='root-container'>
  <h2><a href='javascript:;' class='headers' onclick='showHideById(this, "exception")'>+ Exception</a></h2>
  <div id='exception' class='headers-container'>
  {{GetRecursiveExceptionHtml(crashReportModel.Exception)}}
  </div>
</div>
<div class='root-container'>
  <h2><a href='javascript:;' class='headers' onclick='showHideById(this, "enhanced-stacktrace")'>+ Enhanced Stacktrace</a></h2>
  <div id='enhanced-stacktrace' class='headers-container'>
  {{GetEnhancedStacktraceHtml(crashReportModel)}}
  </div>
</div>
<div class='root-container'>
  <h2><a href='javascript:;' class='headers' onclick='showHideById(this, "involved-modules")'>+ Involved Modules</a></h2>
  <div id='involved-modules' class='headers-container'>
  {{GetInvolvedModuleListHtml(crashReportModel)}}
  </div>
</div>
<div class='root-container'>
  <h2><a href='javascript:;' class='headers' onclick='showHideById(this, "installed-modules")'>+ Installed Modules</a></h2>
  <div id='installed-modules' class='headers-container'>
  {{GetModuleListHtml(crashReportModel)}}
  </div>
</div>
<div class='root-container'>
  <h2><a href='javascript:;' class='headers' onclick='showHideById(this, "assemblies")'>+ Assemblies</a></h2>
  <div id='assemblies' class='headers-container'>
  <label>Hide: </label>
  <label><input type='checkbox' onclick='showHideByClassName(this, "tw_assembly")'> Game Core</label>
  <label><input type='checkbox' onclick='showHideByClassName(this, "sys_assembly")'> System</label>
  <label><input type='checkbox' onclick='showHideByClassName(this, "module_assembly")'> Modules</label>
  <label><input type='checkbox' onclick='showHideByClassName(this, "unclas_assembly")'> Unclassified</label>
  {{GetAssemblyListHtml(crashReportModel)}}
  </div>
</div>
<div class='root-container'>
  <h2><a href='javascript:;' class='headers' onclick='showHideById(this, "harmony-patches")'>+ Harmony Patches</a></h2>
  <div id='harmony-patches' class='headers-container'>
  {{GetHarmonyPatchesListHtml(crashReportModel)}}
  </div>
</div>
<div class='root-container'>
  <h2><a href='javascript:;' class='headers' onclick='showHideById(this, "log-files")'>+ Log Files</a></h2>
  <div id='log-files' class='headers-container'>
  {{GetLogFilesListHtml(files)}}
  </div>
</div>
<div class='root-container' style='display:none;'>
  <h2><a href='javascript:;' class='headers' onclick='showHideById(this, "mini-dump")'>+ Mini Dump</a></h2>
  <div id='mini-dump' class='headers-container'>
  {{MiniDumpTag}}
  </div>
</div>
<div class='root-container' style='display:none;'>
  <h2><a href='javascript:;' class='headers' onclick='showHideById(this, "save-file")'>+ Save File</a></h2>
  <div id='save-file' class='headers-container'>
  {{SaveFileTag}}
  </div>
</div>
<div class='root-container' style='display:none;'>
  <h2><a href='javascript:;' class='headers' onclick='showHideById(this, "screenshot")'>+ Screenshot</a></h2>
  <img id='screenshot' alt='Screenshot' />
</div>
<div class='root-container' style='display:none;'>
  <h2><a href='javascript:;' class='headers' onclick='showHideById(this, "screenshot-data")'>+ Screenshot Data</a></h2>
  <div id='screenshot-data' class='headers-container'>
  {{ScreenshotTag}}
  </div>
</div>
{{DecompressScriptTag}}
<script>
  function showHideById(element, id) {
      if (document.getElementById(id).style.display === 'block') {
          document.getElementById(id).style.display = 'none';
          element.innerHTML = element.innerHTML.replace('-', '+');
      } else {
          document.getElementById(id).style.display = 'block';
          element.innerHTML = element.innerHTML.replace('+', '-');
      }
  }
  function showHideByClassName(element, className) {
      var list = document.getElementsByClassName(className)
      for (var i = 0; i < list.length; i++) {
          list[i].style.display = (element.checked) ? 'none' : 'list-item';
      }
  }
  function setBackgroundColorByClassName(className, color) {
      var list = document.getElementsByClassName(className);
      for (var i = 0; i < list.length; i++) {
        list[i].style.backgroundColor = color;
     }
  }
  function changeFontSize(fontSize) {
      document.getElementById('exception').style.fontSize = fontSize.value;
      document.getElementById('involved-modules').style.fontSize = fontSize.value;
      document.getElementById('installed-modules').style.fontSize = fontSize.value;
      document.getElementById('assemblies').style.fontSize = fontSize.value;
      document.getElementById('harmony-patches').style.fontSize = fontSize.value;
  }
  function changeBackgroundColor(element) {
      document.body.style.backgroundColor = (!element.checked) ? '#ececec' : 'white';
      setBackgroundColorByClassName('headers-container', (!element.checked) ? 'white' : 'white');
      setBackgroundColorByClassName('modules-container', (!element.checked) ? '#ffffe0' : 'white');
      setBackgroundColorByClassName('submodules-container', (!element.checked) ? '#f8f8e7' : 'white');
      setBackgroundColorByClassName('modules-official-container', (!element.checked) ? '#f4fcdc' : 'white');
      setBackgroundColorByClassName('modules-external-container', (!element.checked) ? '#ede9e0' : 'white');
      setBackgroundColorByClassName('submodules-official-container', (!element.checked) ? '#f0f4e4' : 'white');
      setBackgroundColorByClassName('modules-invalid-container', (!element.checked) ? '#ffefd5' : 'white');
      setBackgroundColorByClassName('submodules-invalid-container', (!element.checked) ? '#f5ecdf' : 'white');
  }
  function minidump(element) {
      var base64 = document.getElementById('mini-dump').innerText.trim();
      //var binData = Uint8Array.from(atob(base64), c => c.charCodeAt(0));
      var binData = new Uint8Array(atob(base64).split('').map(function(x){return x.charCodeAt(0);}));
      var result = window.pako.inflate(binData);

      var a = document.createElement('a');
      var blob = new Blob([result]);
      a.href = window.URL.createObjectURL(blob);
      a.download = "crashdump.dmp";
      a.click();
    }
  function savefile(element) {
      var base64 = document.getElementById('save-file').innerText.trim();
      //var binData = Uint8Array.from(atob(base64), c => c.charCodeAt(0));
      var binData = new Uint8Array(atob(base64).split('').map(function(x){return x.charCodeAt(0);}));
      var result = window.pako.inflate(binData);

      var a = document.createElement('a');
      var blob = new Blob([result]);
      a.href = window.URL.createObjectURL(blob);
      a.download = "savefile.sav";
      a.click();
    }
  function screenshot(element) {
      var base64 = document.getElementById('screenshot-data').innerText.trim();
      document.getElementById('screenshot').src = 'data:image/jpeg;charset=utf-8;base64,' + base64;
      document.getElementById('screenshot').parentElement.style.display = 'block';
    }
</script>
</body>
</html>
""";
#pragma warning disable format // @formatter:on
        }

        private static string GetRecursiveExceptionHtml(ExceptionModel ex)
        {
            return new StringBuilder()
                .AppendLine("Exception information")
                .AppendLine($"<br/>{NL}Type: {ex.Type}")
                .AppendLine(!string.IsNullOrWhiteSpace(ex.Message) ? $"</br>{NL}Message: {ex.Message}" : string.Empty)
                .AppendLine(!string.IsNullOrWhiteSpace(ex.Source) ? $"</br>{NL}Source: {ex.Source}" : string.Empty)
                .AppendLine(!string.IsNullOrWhiteSpace(ex.CallStack)
                    ? $"</br>{NL}CallStack:{NL}</br>{NL}<ol>{NL}<li>{string.Join($"</li>{NL}<li>", ex.CallStack.Split(new[] {NL}, StringSplitOptions.RemoveEmptyEntries))}</li>{NL}</ol>"
                    : string.Empty)
                .AppendLine(ex.InnerException != null ? $"<br/>{NL}<br/>{NL}Inner {GetRecursiveExceptionHtml(ex.InnerException)}" : string.Empty)
                .ToString();
        }

        private static string GetEnhancedStacktraceHtml(CrashReportModel crashReport)
        {
            var sb = new StringBuilder();
            var sbCil = new StringBuilder();
            sb.AppendLine("<ul>");
            foreach (var stacktrace in crashReport.EnhancedStacktrace)
            {
                sb.Append("<li>")
                    .Append($"Frame: {stacktrace.Name}</br>")
                    .Append($"Approximate IL Offset: {(stacktrace.ILOffset is null ? "UNKNOWN" : $"{stacktrace.ILOffset:X4}")}</br>")
                    .Append("<ul>");

                foreach (var method in stacktrace.PatchMethods)
                {
                    sb.Append("<li>")
                        .Append($"Module: {method.Module}</br>")
                        .Append($"Method: {method.MethodFullName}</br>")
                        .AppendLine("CIL:").Append("<pre>").AppendJoin(NL, method.CilInstructions).Append("</pre>")
                        .Append("</li>");
                }

                sb.Append("<li>")
                    .Append($"Module: {stacktrace.OriginalMethod.Module}</br>")
                    .Append($"Method: {stacktrace.OriginalMethod.MethodFullName}</br>")
                    .Append($"Method From Stackframe Issue: {stacktrace.MethodFromStackframeIssue}</br>")
                    .AppendLine("CIL:").Append("<pre>").AppendJoin(NL, stacktrace.OriginalMethod.CilInstructions).Append("</pre></br>")
                    .Append("</li>");

                sb.Append("</ul>");
                sb.AppendLine("</li>");
                sbCil.Clear();
            }
            sb.AppendLine("</ul>");
            return sb.ToString();
        }

        private static string GetInvolvedModuleListHtml(CrashReportModel crashReport)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<ul>");
            foreach (var stacktrace in crashReport.EnhancedStacktrace)
            {
                var moduleId = stacktrace.OriginalMethod.Module;
                if (moduleId == "UNKNOWN") continue;

                sb.Append("<li>")
                    .Append($"<a href='javascript:;' onclick='document.getElementById(\"{moduleId}\").scrollIntoView(false)'>").Append(moduleId).Append("</a></br>")
                    .Append($"Method: {stacktrace.OriginalMethod.MethodFullName}</br>")
                    .Append($"Frame: {stacktrace.FrameDescription}</br>")
                    .Append($"Method From Stackframe Issue: {stacktrace.MethodFromStackframeIssue}</br>")
                    .Append("</li>");

                if (stacktrace.PatchMethods.Count > 0)
                {
                    sb.Append("Patches:</br>")
                        .Append("<ul>");
                    foreach (var method in stacktrace.PatchMethods)
                    {
                        // Ignore blank transpilers used to force the jitter to skip inlining
                        if (method.Method == "BlankTranspiler") continue;
                        sb.Append("<li>")
                            .Append($"Module: {method.Module ?? "UNKNOWN"}</br>")
                            .Append($"Method: {method.MethodFullName}</br>")
                            .Append("</li>");
                    }
                    sb.Append("</ul>");
                }

                sb.Append("</br>");

                sb.AppendLine("</li>");
            }
            sb.AppendLine("</ul>");
            return sb.ToString();
        }

        private static string GetModuleListHtml(CrashReportModel crashReport)
        {
            var moduleBuilder = new StringBuilder();
            var subModulesBuilder = new StringBuilder();
            var assembliesBuilder = new StringBuilder();
            var tagsBuilder = new StringBuilder();
            var additionalAssembliesBuilder = new StringBuilder();
            var dependenciesBuilder = new StringBuilder();


            void AppendDependencies(ModuleModel module)
            {
                var deps = new Dictionary<string, string>();
                var tmp = new StringBuilder();
                foreach (var dependentModule in module.DependencyMetadatas)
                {
                    if (dependentModule.IsIncompatible)
                    {
                        deps[dependentModule.ModuleId] = tmp.Clear()
                            .Append("Incompatible ")
                            .Append($"<a href='javascript:;' onclick='document.getElementById(\"{dependentModule.ModuleId}\").scrollIntoView(false)'>")
                            .Append(dependentModule.ModuleId)
                            .Append("</a>")
                            .Append(dependentModule.IsOptional ? " (optional)" : string.Empty)
                            .ToString();
                    }
                    else if (dependentModule.Type == ModuleDependencyMetadataModelType.LoadAfter)
                    {
                        deps[dependentModule.ModuleId] = tmp.Clear()
                            .Append("Load ").Append(DependentModuleMetadata.GetLoadType(LoadType.LoadAfterThis))
                            .Append($"<a href='javascript:;' onclick='document.getElementById(\"{dependentModule.ModuleId}\").scrollIntoView(false)'>")
                            .Append(dependentModule.ModuleId)
                            .Append("</a>")
                            .Append(dependentModule.IsOptional ? " (optional)" : string.Empty)
                            .ToString();
                    }
                    else if (dependentModule.Type == ModuleDependencyMetadataModelType.LoadBefore)
                    {
                        deps[dependentModule.ModuleId] = tmp.Clear()
                            .Append("Load ").Append(DependentModuleMetadata.GetLoadType(LoadType.LoadBeforeThis))
                            .Append($"<a href='javascript:;' onclick='document.getElementById(\"{dependentModule.ModuleId}\").scrollIntoView(false)'>")
                            .Append(dependentModule.ModuleId)
                            .Append("</a>")
                            .Append(dependentModule.IsOptional ? " (optional)" : string.Empty)
                            .ToString();
                    }
                }

                dependenciesBuilder.Clear();
                foreach (var (_, line) in deps)
                {
                    dependenciesBuilder.Append("<li>").Append(line).AppendLine("</li>");
                }
            }

            void AppendSubModules(ModuleModel module)
            {
                subModulesBuilder.Clear();
                foreach (var subModule in module.SubModules)
                {
                    assembliesBuilder.Clear();
                    foreach (var (_, assembly) in subModule.AdditionalMetadata.Where(x => x.Key == "METADATA:Assembly"))
                    {
                        assembliesBuilder.Append("<li>").Append(assembly).AppendLine("</li>");
                    }

                    tagsBuilder.Clear();
                    foreach (var (tag, value) in subModule.AdditionalMetadata.Where(x => !x.Key.StartsWith("METADATA:")))
                    {
                        tagsBuilder.Append("<li>").Append(tag).Append(": ").Append(string.Join(", ", value)).AppendLine("</li>");
                    }

                    subModulesBuilder.AppendLine("<li>")
                        .AppendLine(module.IsOfficial ? "<div class=\"submodules-official-container\">" : "<div class=\"submodules-container\">")
                        .Append("<b>").Append(subModule.Name).AppendLine("</b></br>")
                        .Append("Name: ").Append(subModule.Name).AppendLine("</br>")
                        .Append("DLLName: ").Append(subModule.AssemblyName).AppendLine("</br>")
                        .Append("SubModuleClassType: ").Append(subModule.Entrypoint).AppendLine("</br>")
                        .Append(tagsBuilder.Length == 0 ? "" : $"Tags:</br>{NL}")
                        .Append(tagsBuilder.Length == 0 ? "" : $"<ul>{NL}")
                        .Append(tagsBuilder.Length == 0 ? "" : $"{tagsBuilder}{NL}")
                        .Append(tagsBuilder.Length == 0 ? "" : $"</ul>{NL}")
                        .Append(assembliesBuilder.Length == 0 ? "" : $"Assemblies:</br>{NL}")
                        .Append(assembliesBuilder.Length == 0 ? "" : $"<ul>{NL}")
                        .Append(assembliesBuilder.Length == 0 ? "" : $"{assembliesBuilder}{NL}")
                        .Append(assembliesBuilder.Length == 0 ? "" : $"</ul>{NL}")
                        .AppendLine("</div>")
                        .AppendLine("</li>");
                }
            }

            void AppendAdditionalAssemblies(ModuleModel module)
            {
                additionalAssembliesBuilder.Clear();
                foreach (var externalLoadedAssembly in crashReport.Assemblies)
                {
                    if (externalLoadedAssembly.ModuleId == module.Id)
                    {
                        additionalAssembliesBuilder.Append("<li>")
                            .Append(Path.GetFileName(externalLoadedAssembly.Path))
                            .Append(" (").Append(externalLoadedAssembly.FullName).Append(")")
                            .AppendLine("</li>");
                    }
                }
            }

            moduleBuilder.AppendLine("<ul>");
            foreach (var module in crashReport.Modules)
            {
                AppendDependencies(module);
                AppendSubModules(module);
                AppendAdditionalAssemblies(module);

                var isVortexManaged = module.AdditionalMetadata.FirstOrDefault(x => x.Key == "METADATA:MANAGED_BY_VORTEX").Value is { } str && bool.TryParse(str, out var val) && val;

                moduleBuilder.AppendLine("<li>")
                    .AppendLine(module.IsOfficial
                        ? "<div class=\"modules-official-container\">"
                        : module.IsExternal
                            ? "<div class=\"modules-external-container\">"
                            : "<div class=\"modules-container\">")
                    .Append($"<b><a href='javascript:;' onclick='showHideById(this, \"{module.Id}\")'>").Append("+ ").Append(module.Name).Append(" (").Append(module.Id).Append(", ").Append(module.Version).Append(")").AppendLine("</a></b>")
                    .AppendLine($"<div id='{module.Id}' style='display: none'>")
                    .Append("Id: ").Append(module.Id).AppendLine("</br>")
                    .Append("Name: ").Append(module.Name).AppendLine("</br>")
                    .Append("Version: ").Append(module.Version).AppendLine("</br>")
                    .Append("External: ").Append(module.IsExternal).AppendLine("</br>")
                    .Append("Vortex: ").Append(isVortexManaged).AppendLine("</br>")
                    .Append("Official: ").Append(module.IsOfficial).AppendLine("</br>")
                    .Append("Singleplayer: ").Append(module.IsSingleplayer).AppendLine("</br>")
                    .Append("Multiplayer: ").Append(module.IsMultiplayer).AppendLine("</br>")
                    .Append(dependenciesBuilder.Length == 0 ? "" : $"Dependencies:</br>{NL}")
                    .Append(dependenciesBuilder.Length == 0 ? "" : $"<ul>{NL}")
                    .Append(dependenciesBuilder.Length == 0 ? "" : $"{dependenciesBuilder}{NL}")
                    .Append(dependenciesBuilder.Length == 0 ? "" : $"</ul>{NL}")
                    .Append(string.IsNullOrWhiteSpace(module.Url) ? "" : $"Url: <a href='{module.Url}'>{module.Url}</a></br>{NL}")
                    .Append(subModulesBuilder.Length == 0 ? "" : $"SubModules:</br>{NL}")
                    .Append(subModulesBuilder.Length == 0 ? "" : $"<ul>{NL}")
                    .Append(subModulesBuilder.Length == 0 ? "" : $"{subModulesBuilder}{NL}")
                    .Append(subModulesBuilder.Length == 0 ? "" : $"</ul>{NL}")
                    .Append(additionalAssembliesBuilder.Length == 0 ? "" : $"Assemblies Present:</br>{NL}")
                    .Append(additionalAssembliesBuilder.Length == 0 ? "" : $"<ul>{NL}")
                    .Append(additionalAssembliesBuilder.Length == 0 ? "" : $"{additionalAssembliesBuilder}{NL}")
                    .Append(additionalAssembliesBuilder.Length == 0 ? "" : $"</ul>{NL}")
                    .AppendLine("</div>")
                    .AppendLine("</div>")
                    .AppendLine("</li>");
            }
            moduleBuilder.AppendLine("</ul>");

            return moduleBuilder.ToString();
        }

        private static string GetAssemblyListHtml(CrashReportModel crashReport)
        {
            var sb0 = new StringBuilder();

            void AppendAssembly(AssemblyModel assembly)
            {
                var isModule = assembly.ModuleId is not null;

                var isTW = assembly.AdditionalMetadata.FirstOrDefault(x => x.Key == "METADATA:IS_TW").Value is { } isTWStr && bool.TryParse(isTWStr, out var isTWVal) && isTWVal;
                var isSystem = assembly.AdditionalMetadata.FirstOrDefault(x => x.Key == "METADATA:IS_TW").Value is { } isSystemStr && bool.TryParse(isSystemStr, out var isSystemVal) && isSystemVal;

                sb0.Append(isModule ? "<li class='module_assembly'>" : isTW ? "<li class='tw_assembly'>" : isSystem ? "<li class='sys_assembly'>" : "<li class='unclas_assembly'>")
                    .Append(assembly.Name).Append(", ")
                    .Append(assembly.Version).Append(", ")
                    .Append(assembly.Architecture).Append(", ")
                    .Append(assembly.IsDynamic ? "" : $"{assembly.Hash}, ")
                    .Append(assembly.IsDynamic ? "DYNAMIC" : string.IsNullOrWhiteSpace(assembly.Path) ? "EMPTY" : $"<a href=\"{assembly.Path}\">{assembly.Path}</a>")
                    .AppendLine("</li>");
            }

            sb0.AppendLine("<ul>");
            foreach (var assembly in crashReport.Assemblies)
            {
                AppendAssembly(assembly);
            }
            sb0.AppendLine("</ul>");

            return sb0.ToString();
        }

        private static string GetHarmonyPatchesListHtml(CrashReportModel crashReport)
        {
            var harmonyPatchesListBuilder = new StringBuilder();
            var patchesBuilder = new StringBuilder();
            var patchBuilder = new StringBuilder();

            void AppendPatches(string name, IEnumerable<HarmonyPatchModel> patches)
            {
                patchBuilder.Clear();
                foreach (var patch in patches)
                {
                    //if (string.Equals(patch.owner, ExceptionHandlerSubSystem.Instance?.Harmony.Id, StringComparison.InvariantCultureIgnoreCase))
                    //    continue;

                    patchBuilder.Append("<li>")
                        .Append("Owner: ").Append(patch.Owner).Append("; ")
                        .Append("Namespace: ").Append(patch.Namespace).Append("; ")
                        .Append(patch.Index != 0 ? $"Index: {patch.Index}; " : "")
                        .Append(patch.Priority != 400 ? $"Priority: {patch.Priority}; " : "")
                        .Append(patch.Before.Count > 0 ? $"Before: {string.Join(", ", patch.Before)}; " : "")
                        .Append(patch.After.Count > 0 ? $"After: {string.Join(", ", patch.After)}; " : "")
                        .AppendLine("</li>");
                }

                if (patchBuilder.Length > 0)
                {
                    patchesBuilder.AppendLine("<li>")
                        .AppendLine(name)
                        .AppendLine("<ul>")
                        .AppendLine(patchBuilder.ToString())
                        .AppendLine("</ul>")
                        .AppendLine("</li>");
                }
            }

            harmonyPatchesListBuilder.AppendLine("<ul>");
            foreach (var harmonyPatch in crashReport.HarmonyPatches)
            {
                patchesBuilder.Clear();

                AppendPatches(nameof(harmonyPatch.Prefixes), harmonyPatch.Prefixes);
                AppendPatches(nameof(harmonyPatch.Postfixes), harmonyPatch.Postfixes);
                AppendPatches(nameof(harmonyPatch.Finalizers), harmonyPatch.Finalizers);
                AppendPatches(nameof(harmonyPatch.Transpilers), harmonyPatch.Transpilers);

                if (patchesBuilder.Length > 0)
                {
                    harmonyPatchesListBuilder.AppendLine("<li>")
                        .Append(harmonyPatch.OriginalMethodFullName)
                        .AppendLine("<ul>")
                        .AppendLine(patchesBuilder.ToString())
                        .AppendLine("</ul>")
                        .AppendLine("</li>")
                        .AppendLine("<br/>");
                }
            }
            harmonyPatchesListBuilder.AppendLine("</ul>");

            return harmonyPatchesListBuilder.ToString();
        }

        private static string GetLogFilesListHtml(IEnumerable<LogSource> files)
        {
            var sb = new StringBuilder();

            sb.AppendLine("<ul>");
            foreach (var logSource in files)
            {
                sb.Append("<li>").Append("<a>").Append(logSource.Name).Append("</a></br>").Append("<ul>");

                var sbSource = new StringBuilder();
                foreach (var logEntry in logSource.Logs)
                    sbSource.Append("<li>").Append(logEntry.Date).Append(": ").Append(logEntry.Message).AppendLine("</li>");

                sb.Append("<ul>").Append(sbSource).AppendLine("</ul>").AppendLine("</ul></li>");
            }
            sb.AppendLine("</ul>");
            return sb.ToString();
        }
    }
}

#pragma warning restore
#nullable restore
#endif // BUTRCRASHREPORT_DISABLE