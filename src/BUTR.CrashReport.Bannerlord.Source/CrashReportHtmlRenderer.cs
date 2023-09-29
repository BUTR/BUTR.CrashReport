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

#if !BUTRCRASHREPORT_DISABLE || BUTRCRASHREPORT_ENABLEHTMLRENDERER
#nullable enable
#if !BUTRCRASHREPORT_ENABLEWARNINGS
#pragma warning disable
#endif

namespace BUTR.CrashReport.Bannerlord
{
    using global::BUTR.CrashReport.Models;

    using global::System;
    using global::System.Collections.Generic;
    using global::System.IO;
    using global::System.Linq;
    using global::System.Text;

    internal static class CrashReportHtmlRenderer
    {
        public static readonly string MiniDumpTag = "<!-- MINI DUMP -->";
        public static readonly string MiniDumpButtonTag = "<!-- MINI DUMP BUTTON -->";
        public static readonly string SaveFileTag = "<!-- SAVE FILE -->";
        public static readonly string SaveFileButtonTag = "<!-- SAVE FILE BUTTON -->";
        public static readonly string ScreenshotTag = "<!-- SCREENSHOT -->";
        public static readonly string ScreenshotButtonTag = "<!-- SCREENSHOT BUTTON -->";
        public static readonly string DecompressScriptTag = "<!-- DECOMPRESS SCRIPT -->";
        public static readonly string JsonModelDataTag = "<!-- JSON MODEL -->";

#pragma warning disable format // @formatter:off
        private static readonly string Scripts = """
<script>
   function showHideById(element, id) {
     if (document.getElementById(id).style.display === "block") {
       document.getElementById(id).style.display = "none";
       element.innerHTML = element.innerHTML.replace("-", "+");
     } else {
       document.getElementById(id).style.display = "block";
       element.innerHTML = element.innerHTML.replace("+", "-");
     }
   }
   function showHideByClassName(element, className) {
     var list = document.getElementsByClassName(className);
     for (var i = 0; i < list.length; i++) {
       list[i].style.display = element.checked ? "none" : "list-item";
     }
   }
   function setBackgroundColorByClassName(className, color) {
     var list = document.getElementsByClassName(className);
     for (var i = 0; i < list.length; i++) {
       list[i].style.backgroundColor = color;
     }
   }
   function changeFontSize(fontSize) {
     document.getElementById("exception").style.fontSize = fontSize.value;
     document.getElementById("involved-modules").style.fontSize = fontSize.value;
     document.getElementById("installed-modules").style.fontSize = fontSize.value;
     document.getElementById("assemblies").style.fontSize = fontSize.value;
     document.getElementById("harmony-patches").style.fontSize = fontSize.value;
   }
   function changeBackgroundColor(element) {
     document.body.style.backgroundColor = !element.checked ? "#ececec" : "white";
     setBackgroundColorByClassName("headers-container", !element.checked ? "white" : "white");
     setBackgroundColorByClassName("modules-container", !element.checked ? "#ffffe0" : "white");
     setBackgroundColorByClassName("submodules-container", !element.checked ? "#f8f8e7" : "white");
     setBackgroundColorByClassName("modules-official-container", !element.checked ? "#f4fcdc" : "white");
     setBackgroundColorByClassName("modules-external-container", !element.checked ? "#ede9e0" : "white");
     setBackgroundColorByClassName("submodules-official-container", !element.checked ? "#f0f4e4" : "white");
     setBackgroundColorByClassName("modules-invalid-container", !element.checked ? "#ffefd5" : "white");
     setBackgroundColorByClassName("submodules-invalid-container", !element.checked ? "#f5ecdf" : "white");
   }
   function minidump(element) {
     var base64 = document.getElementById("mini-dump").innerText.trim();
     //var binData = Uint8Array.from(atob(base64), c => c.charCodeAt(0));
     var binData = new Uint8Array(
       atob(base64)
         .split("")
         .map(function (x) {
           return x.charCodeAt(0);
         })
     );
     var result = window.pako.inflate(binData);

     var a = document.createElement("a");
     var blob = new Blob([result]);
     a.href = window.URL.createObjectURL(blob);
     a.download = "crashdump.dmp";
     a.click();
   }
   function savefile(element) {
     var base64 = document.getElementById("save-file").innerText.trim();
     //var binData = Uint8Array.from(atob(base64), c => c.charCodeAt(0));
     var binData = new Uint8Array(
       atob(base64)
         .split("")
         .map(function (x) {
           return x.charCodeAt(0);
         })
     );
     var result = window.pako.inflate(binData);

     var a = document.createElement("a");
     var blob = new Blob([result]);
     a.href = window.URL.createObjectURL(blob);
     a.download = "savefile.sav";
     a.click();
   }
   function screenshot(element) {
     var base64 = document.getElementById("screenshot-data").innerText.trim();
     document.getElementById("screenshot").src = "data:image/jpeg;charset=utf-8;base64," + base64;
     document.getElementById("screenshot").parentElement.style.display = "block";
   }
 </script>                                    
""";
#pragma warning disable format // @formatter:on

        public static string AddData(string htmlReport, string crashReportJson, string? gZipBase64MiniDump = null, string? gZipBase64SaveFile = null, string? base64Screenshot = null)
        {
            var IncludeMiniDump = !string.IsNullOrEmpty(gZipBase64MiniDump);
            var IncludeSaveFile = !string.IsNullOrEmpty(gZipBase64SaveFile);
            var IncludeScreenshot = !string.IsNullOrEmpty(base64Screenshot);

            if (IncludeMiniDump)
            {
                htmlReport = htmlReport
                    .Replace(CrashReportHtmlRenderer.MiniDumpTag, gZipBase64MiniDump)
                    .Replace(CrashReportHtmlRenderer.MiniDumpButtonTag, @"
<![if !IE]>
              <br/>
              <br/>
              <button onclick='minidump(this)'>Get MiniDump</button>
<![endif]>");
            }

            if (IncludeSaveFile)
            {
                htmlReport = htmlReport
                    .Replace(CrashReportHtmlRenderer.SaveFileTag, gZipBase64SaveFile)
                    .Replace(CrashReportHtmlRenderer.SaveFileButtonTag, @"
<![if !IE]>
              <br/>
              <br/>
              <button onclick='savefile(this)'>Get Save File</button>
<![endif]>");
            }

            if (IncludeScreenshot)
            {
                htmlReport = htmlReport
                    .Replace(CrashReportHtmlRenderer.ScreenshotTag, base64Screenshot)
                    .Replace(CrashReportHtmlRenderer.ScreenshotButtonTag, @"
<![if !IE]>
              <br/>
              <br/>
              <button onclick='screenshot(this)'>Show Screenshot</button>
<![endif]>");
            }

            if (IncludeMiniDump || IncludeSaveFile)
            {
                htmlReport = htmlReport.Replace(CrashReportHtmlRenderer.DecompressScriptTag, @"
<![if !IE]>
    <script src=""https://cdn.jsdelivr.net/pako/1.0.3/pako_inflate.min.js""></script>
<![endif]>");
            }

            htmlReport = htmlReport.Replace(CrashReportHtmlRenderer.JsonModelDataTag, crashReportJson);

            return htmlReport;
        }

        public static string Build(CrashReportModel crashReportModel, IEnumerable<LogSource> files)
        {
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
    <meta charset='utf-8' />
    <game version='{{crashReportModel.GameVersion}}' />
    <launcher type='{{launcherType}}' version='{{launcherVersion}}' />
    <runtime value='{{runtime}}' />
    {{(!string.IsNullOrEmpty(butrLoaderVersion) ? $"<butrloader version='{butrLoaderVersion}' />" : string.Empty)}}
    {{(!string.IsNullOrEmpty(blseVersion) ? $"<blse version='{blseVersion}' />" : string.Empty)}}
    {{(!string.IsNullOrEmpty(launcherExVersion) ? $"<launcherex version='{launcherExVersion}' />" : string.Empty)}}
    <report id='{{crashReportModel.Id}}' version='{{crashReportModel.Version}}' />
    <style>
      .headers {
        font-family: 'Consolas', monospace;
      }
      .root-container {
        font-family: 'Consolas', monospace;
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
              <br />
              This is a community Crash Report. Please save it and use it for reporting the error. Do not provide screenshots, provide the report!
              <br />
              Most likely this error was caused by a custom installed module.
              <br />
              <br />
              If you were in the middle of something, the progress might be lost.
              <br />
              <br />
              Launcher: {{launcherType}} ({{launcherVersion}})
              <br />
              Runtime: {{runtime}}
              {{(!string.IsNullOrEmpty(blseVersion) ? $"<br />BLSE Version: {blseVersion}" : string.Empty)}}
              {{(!string.IsNullOrEmpty(launcherExVersion) ? $"<br />LauncherEx Version: {launcherExVersion}" : string.Empty)}}
              <br />
            </div>
          </td>
          <td>
            <div style='float: right; margin-left: 10px;'>
              <label>Without Color:</label>
              <input type='checkbox' onclick='changeBackgroundColor(this)' />
              <br />
              <br />
              <label>Font Size:</label>
              <select class='input' onchange='changeFontSize(this);'>
                <option value='1.0em' selected='selected'>Standard</option>
                <option value='0.9em'>Medium</option>
                <option value='0.8em'>Small</option>
              </select>
              {{MiniDumpButtonTag}} {{SaveFileButtonTag}} {{ScreenshotButtonTag}}
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
        <label><input type='checkbox' onclick='showHideByClassName(this, "sys_assembly")' /> System</label>
        <label><input type='checkbox' onclick='showHideByClassName(this, "gac_assembly")' /> GAC</label>
        <label><input type='checkbox' onclick='showHideByClassName(this, "tw_assembly")' /> Game</label>
        <label><input type='checkbox' onclick='showHideByClassName(this, "tw_module_assembly")' /> Game Modules</label>
        <label><input type='checkbox' onclick='showHideByClassName(this, "module_assembly")' /> Modules</label>
        <label><input type='checkbox' onclick='showHideByClassName(this, "dynamic_assembly")' /> Dynamic</label>
        <label><input type='checkbox' onclick='showHideByClassName(this, "unclas_assembly")' /> Unclassified</label>
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
    <div class='root-container' style='display: none;'>
      <h2><a href='javascript:;' class='headers' onclick='showHideById(this, "mini-dump")'>+ Mini Dump</a></h2>
      <div id='mini-dump' class='headers-container'>
        {{MiniDumpTag}}
      </div>
    </div>
    <div class='root-container' style='display: none;'>
      <h2><a href='javascript:;' class='headers' onclick='showHideById(this, "save-file")'>+ Save File</a></h2>
      <div id='save-file' class='headers-container'>
        {{SaveFileTag}}
      </div>
    </div>
    <div class='root-container' style='display: none;'>
      <h2><a href='javascript:;' class='headers' onclick='showHideById(this, "screenshot")'>+ Screenshot</a></h2>
      <img id='screenshot' alt='Screenshot' />
    </div>
    <div class='root-container' style='display: none;'>
      <h2><a href='javascript:;' class='headers' onclick='showHideById(this, "screenshot-data")'>+ Screenshot Data</a></h2>
      <div id='screenshot-data' class='headers-container'>
        {{ScreenshotTag}}
      </div>
    </div>
    <div class='root-container' style='display: none;'>
      <h2><a href='javascript:;' class="headers" onclick='showHideById(this, "json-model-data")'>+ Json Model Data</a></h2>
      <div id='json-model-data' class='headers-container'>
        {{JsonModelDataTag}}
      </div>
    </div>
    {{DecompressScriptTag}}
    {{Scripts}}
  </body>
</html>
""";
#pragma warning disable format // @formatter:on
        }

        private static string GetRecursiveExceptionHtml(ExceptionModel? ex)
        {
            if (ex is null) return string.Empty;

            var hasMessage = !string.IsNullOrWhiteSpace(ex.Message);
            var hasCallStack = !string.IsNullOrWhiteSpace(ex.CallStack);
            var hasInner = ex.InnerException is not null;
            return new StringBuilder()
                .Append("Exception information").Append("<br/>")
                .Append("Type: ").Append(ex.Type).Append("<br/>")
                .AppendIf(hasMessage, "Message: ").AppendIf(hasMessage, ex.Message).AppendIf(hasMessage, "<br/>")
                .AppendIf(hasCallStack, "CallStack:").AppendIf(hasCallStack, "<br/>")
                .AppendIf(hasMessage, "<ol>")
                .AppendIf(hasMessage, "<li>")
                .AppendJoinIf(hasMessage, $"</li><li>", ex.CallStack.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
                .AppendIf(hasMessage, "</li>")
                .AppendIf(hasMessage, "</ol>")
                .AppendIf(hasInner, "<br/>")
                .AppendIf(hasInner, "<br/>")
                .AppendIf(hasInner, "Inner ").AppendIf(hasInner, GetRecursiveExceptionHtml(ex.InnerException!))
                .ToString();
        }

        private static string GetEnhancedStacktraceHtml(CrashReportModel crashReport)
        {
            var random = new Random();
            var sb = new StringBuilder();
            var sbCil = new StringBuilder();
            sb.Append("<ul>");
            foreach (var stacktrace in crashReport.EnhancedStacktrace)
            {
                sb.Append("<li>")
                    .Append("Frame: ").Append(stacktrace.Name).Append("<br/>")
                    .Append("Approximate IL Offset: ").Append(stacktrace.ILOffset is not null ? $"{stacktrace.ILOffset:X4}" : "UNKNOWN").Append("<br/>")
                    .Append("Native Offset: ").Append(stacktrace.NativeOffset is not null ? $"{stacktrace.NativeOffset:X4}" : "UNKNOWN")
                    .Append("<ul>");

                foreach (var method in stacktrace.PatchMethods)
                {
                    var id = random.Next();
                    sb.Append("<li>")
                        .Append($"Module: {method.Module}").Append("<br/>")
                        .Append($"Method: {method.MethodFullName}").Append("<br/>")
                        .Append($"<div><a href='javascript:;' class='headers' onclick='showHideById(this, \"{id}\")'>+ CIL:</a><div id='{id}' class='headers-container'><pre>")
                        .AppendJoin('\n', method.CilInstructions).Append("</pre></div></div>")
                        .Append("</li>");
                }

                var id2 = random.Next();
                var id3 = random.Next();
                sb.Append("<li>")
                    .Append($"Module: ").Append(stacktrace.OriginalMethod.Module).Append("<br/>")
                    .Append($"Method: ").Append(stacktrace.OriginalMethod.MethodFullName).Append("<br/>")
                    .Append($"Method From Stackframe Issue: ").Append(stacktrace.MethodFromStackframeIssue).Append("<br/>")
                    .Append($"<div><a href='javascript:;' class='headers' onclick='showHideById(this, \"{id2}\")'>+ CIL:</a><div id='{id2}' class='headers-container'><pre>")
                    .AppendJoin(Environment.NewLine, stacktrace.OriginalMethod.CilInstructions).Append("</pre></div></div>")
                    .Append($"<div><a href='javascript:;' class='headers' onclick='showHideById(this, \"{id3}\")'>+ Native:</a><div id='{id3}' class='headers-container'><pre>")
                    .AppendJoin('\n', stacktrace.OriginalMethod.NativeInstructions).Append("</pre></div></div>")
                    .Append("</br>")
                    .Append("</li>");

                sb.Append("</ul>");
                sb.Append("</li>");
                sbCil.Clear();
            }
            sb.Append("</ul>");
            return sb.ToString();
        }

        private static string GetInvolvedModuleListHtml(CrashReportModel crashReport)
        {
            var sb = new StringBuilder();
            sb.Append("<ul>");
            foreach (var stacktrace in crashReport.EnhancedStacktrace)
            {
                var moduleId = stacktrace.OriginalMethod.Module;
                if (moduleId == "UNKNOWN") continue;

                sb.Append("<li>")
                    .Append($"<a href='javascript:;' onclick='document.getElementById(\"{moduleId}\").scrollIntoView(false)'>").Append(moduleId).Append("</a>").Append("<br/>")
                    .Append("Method: ").Append(stacktrace.OriginalMethod.MethodFullName).Append("<br/>")
                    .Append("Frame: ").Append(stacktrace.FrameDescription).Append("<br/>")
                    .Append("Method From Stackframe Issue: ").Append(stacktrace.MethodFromStackframeIssue).Append("<br/>")
                    .Append("</li>");

                if (stacktrace.PatchMethods.Count > 0)
                {
                    sb.Append("Patches:").Append("<br/>")
                        .Append("<ul>");
                    foreach (var method in stacktrace.PatchMethods)
                    {
                        // Ignore blank transpilers used to force the jitter to skip inlining
                        if (method.Method == "BlankTranspiler") continue;
                        sb.Append("<li>")
                            .Append($"Module: ").Append(method.Module ?? "UNKNOWN").Append("<br/>")
                            .Append($"Method: ").Append(method.MethodFullName).Append("<br/>")
                            .Append("</li>");
                    }
                    sb.Append("</ul>");
                }

                sb.Append("</br>");

                sb.Append("</li>");
            }
            sb.Append("</ul>");
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
                    var hasVersion = !string.IsNullOrEmpty(dependentModule.Version);
                    var hasVersionRange = !string.IsNullOrEmpty(dependentModule.VersionRange);
                    if (dependentModule.IsIncompatible)
                    {
                        deps[dependentModule.ModuleId] = tmp.Clear()
                            .Append("Incompatible ")
                            .Append($"<a href='javascript:;' onclick='document.getElementById(\"{dependentModule.ModuleId}\").scrollIntoView(false)'>")
                            .Append(dependentModule.ModuleId)
                            .Append("</a>")
                            .AppendIf(dependentModule.IsOptional, " (optional)")
                            .AppendIf(hasVersion, $" >= ").AppendIf(hasVersion, dependentModule.Version)
                            .AppendIf(hasVersionRange, dependentModule.VersionRange)
                            .ToString();
                    }
                    else if (dependentModule.Type == ModuleDependencyMetadataModelType.LoadAfter)
                    {
                        deps[dependentModule.ModuleId] = tmp.Clear()
                            .Append("Load ").Append("Before ")
                            .Append($"<a href='javascript:;' onclick='document.getElementById(\"{dependentModule.ModuleId}\").scrollIntoView(false)'>")
                            .Append(dependentModule.ModuleId)
                            .Append("</a>")
                            .AppendIf(dependentModule.IsOptional, " (optional)")
                            .AppendIf(hasVersion, $" >= ").AppendIf(hasVersion, dependentModule.Version)
                            .AppendIf(hasVersionRange, dependentModule.VersionRange)
                            .ToString();
                    }
                    else if (dependentModule.Type == ModuleDependencyMetadataModelType.LoadBefore)
                    {
                        deps[dependentModule.ModuleId] = tmp.Clear()
                            .Append("Load ").Append("After ")
                            .Append($"<a href='javascript:;' onclick='document.getElementById(\"{dependentModule.ModuleId}\").scrollIntoView(false)'>")
                            .Append(dependentModule.ModuleId)
                            .Append("</a>")
                            .AppendIf(dependentModule.IsOptional, " (optional)")
                            .AppendIf(hasVersion, $" >= ").AppendIf(hasVersion, dependentModule.Version)
                            .AppendIf(hasVersionRange, dependentModule.VersionRange)
                            .ToString();
                    }
                }

                dependenciesBuilder.Clear();
                foreach (var (_, line) in deps)
                {
                    dependenciesBuilder.Append("<li>")
                        .Append(line)
                        .Append("</li>");
                }
            }

            void AppendSubModules(ModuleModel module)
            {
                subModulesBuilder.Clear();
                foreach (var subModule in module.SubModules)
                {
                    assembliesBuilder.Clear();
                    foreach (var metadata in subModule.AdditionalMetadata.Where(x => x.Key == "METADATA:Assembly"))
                    {
                        assembliesBuilder.Append("<li>").Append(metadata.Value).Append("</li>");
                    }

                    tagsBuilder.Clear();
                    foreach (var metadata in subModule.AdditionalMetadata.Where(x => !x.Key.StartsWith("METADATA:")))
                    {
                        tagsBuilder.Append("<li>").Append(metadata.Key).Append(": ").Append(metadata.Value).Append("</li>");
                    }

                    var hasTags = tagsBuilder.Length != 0;
                    var hasAssemblies = assembliesBuilder.Length != 0;
                    subModulesBuilder.Append("<li>")
                        .Append(module.IsOfficial ? "<div class=\"submodules-official-container\">" : "<div class=\"submodules-container\">")
                        .Append("<b>").Append(subModule.Name).Append("</b>").Append("</br>")
                        .Append("Name: ").Append(subModule.Name).Append("</br>")
                        .Append("DLLName: ").Append(subModule.AssemblyName).Append("</br>")
                        .Append("SubModuleClassType: ").Append(subModule.Entrypoint).Append("</br>")
                        .AppendIf(hasTags, "Tags:").AppendIf(hasTags, "</br>")
                        .AppendIf(hasTags, "<ul>")
                        .AppendIf(hasTags, tagsBuilder)
                        .AppendIf(hasTags, "</ul>")
                        .AppendIf(hasAssemblies, "Assemblies:").AppendIf(hasAssemblies, "</br>")
                        .AppendIf(hasAssemblies, "<ul>")
                        .AppendIf(hasAssemblies, assembliesBuilder)
                        .AppendIf(hasAssemblies, "</ul>")
                        .Append("</div>")
                        .Append("</li>");
                }
            }

            void AppendAdditionalAssemblies(ModuleModel module)
            {
                additionalAssembliesBuilder.Clear();
                foreach (var externalLoadedAssembly in crashReport.Assemblies)
                {
                    if (externalLoadedAssembly.ModuleId == module.Id)
                    {
                        additionalAssembliesBuilder.Append("<li>").Append(Path.GetFileName(externalLoadedAssembly.Path)).Append(" (").Append(externalLoadedAssembly.FullName).Append(")").Append("</li>");
                    }
                }
            }

            moduleBuilder.Append("<ul>");
            foreach (var module in crashReport.Modules)
            {
                AppendDependencies(module);
                AppendSubModules(module);
                AppendAdditionalAssemblies(module);

                var isVortexManaged = module.AdditionalMetadata.FirstOrDefault(x => x.Key == "METADATA:MANAGED_BY_VORTEX").Value is { } str && bool.TryParse(str, out var val) && val;

                var container = module switch
                {
                    { IsOfficial: true } => "modules-official-container",
                    { IsExternal: true } => "modules-official-container",
                    _ => "modules-container",
                };
                var hasDependencies = dependenciesBuilder.Length != 0;
                var hasUrl = !string.IsNullOrWhiteSpace(module.Url);
                var hasSubModules = subModulesBuilder.Length != 0;
                var hasAssemblies = additionalAssembliesBuilder.Length != 0;
                moduleBuilder.Append("<li>")
                    .Append("<div class='").Append(container).Append("'>")
                    .Append("<b><a href='javascript:;' onclick='showHideById(this, \"").Append(module.Id).Append("\")'>").Append("+ ").Append(module.Name).Append(" (").Append(module.Id).Append(", ").Append(module.Version).Append(")").Append("</a></b>")
                    .Append("<div id='").Append(module.Id).Append("' style='display: none'>")
                    .Append("Id: ").Append(module.Id).Append("</br>")
                    .Append("Name: ").Append(module.Name).Append("</br>")
                    .Append("Version: ").Append(module.Version).Append("</br>")
                    .Append("External: ").Append(module.IsExternal).Append("</br>")
                    .Append("Vortex: ").Append(isVortexManaged).Append("</br>")
                    .Append("Official: ").Append(module.IsOfficial).Append("</br>")
                    .Append("Singleplayer: ").Append(module.IsSingleplayer).Append("</br>")
                    .Append("Multiplayer: ").Append(module.IsMultiplayer).Append("</br>")
                    .AppendIf(hasDependencies, "Dependencies:").AppendIf(hasDependencies, "</br>")
                    .AppendIf(hasDependencies, "<ul>")
                    .AppendIf(hasDependencies, dependenciesBuilder)
                    .AppendIf(hasDependencies, "</ul>")
                    .AppendIf(hasUrl, "Url: <a href='").AppendIf(hasUrl, module.Url).AppendIf(hasUrl, "'>").AppendIf(hasUrl, module.Url).AppendIf(hasUrl, "</a>").AppendIf(hasUrl, "</br>")
                    .AppendIf(hasSubModules, "SubModules:").AppendIf(hasSubModules, "</br>")
                    .AppendIf(hasSubModules, "<ul>")
                    .AppendIf(hasSubModules, subModulesBuilder)
                    .AppendIf(hasSubModules, "</ul>")
                    .AppendIf(hasAssemblies, "Assemblies Present:").AppendIf(hasAssemblies, "</br>")
                    .AppendIf(hasAssemblies, "<ul>")
                    .AppendIf(hasAssemblies, additionalAssembliesBuilder)
                    .AppendIf(hasAssemblies, "</ul>")
                    .Append("</div>")
                    .Append("</div>")
                    .Append("</li>");
            }
            moduleBuilder.Append("</ul>");

            return moduleBuilder.ToString();
        }

        private static string GetAssemblyListHtml(CrashReportModel crashReport)
        {
            var sb0 = new StringBuilder();

            void AppendAssembly(AssemblyModel assembly)
            {
                var isModule = assembly.ModuleId is not null;

                var @class = string.Join(" ", assembly.Type.GetFlags().Select(x => x switch
                {
                    AssemblyModelType.Dynamic => "dynamic_assembly",
                    AssemblyModelType.GAC => "gac_assembly",
                    AssemblyModelType.System => "sys_assembly",
                    AssemblyModelType.GameCore => "tw_assembly",
                    AssemblyModelType.GameModule => "tw_module_assembly",
                    AssemblyModelType.Module => "module_assembly",
                    _ => string.Empty,
                }));
                var isDynamic = assembly.Type.HasFlag(AssemblyModelType.Dynamic);
                var hasPath = !string.IsNullOrWhiteSpace(assembly.Path);
                sb0.Append($"<li class='{@class}'>")
                    .Append(assembly.Name).Append(", ")
                    .Append(assembly.Version).Append(", ")
                    .Append(assembly.Architecture).Append(", ")
                    .AppendIf(!isDynamic, assembly.Hash).AppendIf(!isDynamic, ", ")
                    .AppendIf(isDynamic && !hasPath, "DYNAMIC")
                    .AppendIf(!isDynamic && !hasPath, "EMPTY")
                    .AppendIf(!isDynamic && hasPath, "<a href='").AppendIf(!isDynamic && hasPath, assembly.Path).AppendIf(!isDynamic && hasPath, "'>").AppendIf(!isDynamic && hasPath, assembly.Path).AppendIf(!isDynamic && hasPath, "</a>")
                    .Append("</li>");
            }

            sb0.Append("<ul>");
            foreach (var assembly in crashReport.Assemblies)
                AppendAssembly(assembly);
            sb0.Append("</ul>");

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
                    var hasIndex = patch.Index != 0;
                    var hasPriority = patch.Priority != 400;
                    var hasBefore = patch.Before.Count > 0;
                    var hasAfter = patch.After.Count > 0;
                    patchBuilder.Append("<li>")
                        .Append("Owner: ").Append(patch.Owner).Append("; ")
                        .Append("Namespace: ").Append(patch.Namespace).Append("; ")
                        .AppendIf(hasIndex, "Index: ").AppendIf(hasIndex, patch.Index).AppendIf(hasIndex, "; ")
                        .AppendIf(hasPriority, "Priority: ").AppendIf(hasPriority, patch.Priority).AppendIf(hasPriority, "; ")
                        .AppendIf(hasBefore, "Before: ").AppendJoinIf(hasBefore, ", ", patch.Before).AppendIf(hasBefore, "; ")
                        .AppendIf(hasAfter, "After: ").AppendJoinIf(hasAfter, ", ", patch.After).AppendIf(hasAfter, ";")
                        .Append("</li>");
                }

                if (patchBuilder.Length > 0)
                {
                    patchesBuilder.Append("<li>").Append(name).Append("<ul>").Append(patchBuilder.ToString()).Append("</ul>").Append("</li>");
                }
            }

            harmonyPatchesListBuilder.Append("<ul>");
            foreach (var harmonyPatch in crashReport.HarmonyPatches)
            {
                patchesBuilder.Clear();

                AppendPatches(nameof(harmonyPatch.Prefixes), harmonyPatch.Prefixes);
                AppendPatches(nameof(harmonyPatch.Postfixes), harmonyPatch.Postfixes);
                AppendPatches(nameof(harmonyPatch.Finalizers), harmonyPatch.Finalizers);
                AppendPatches(nameof(harmonyPatch.Transpilers), harmonyPatch.Transpilers);

                if (patchesBuilder.Length > 0)
                {
                    harmonyPatchesListBuilder.Append("<li>")
                        .Append(harmonyPatch.OriginalMethodFullName).Append("<ul>").Append(patchesBuilder.ToString()).Append("</ul>")
                        .Append("</li>")
                        .Append("<br/>");
                }
            }
            harmonyPatchesListBuilder.Append("</ul>");

            return harmonyPatchesListBuilder.ToString();
        }

        private static string GetLogFilesListHtml(IEnumerable<LogSource> files)
        {
            var sb = new StringBuilder();

            sb.Append("<ul>");
            foreach (var logSource in files)
            {
                sb.Append("<li>").Append("<a>").Append(logSource.Name).Append("</a>").Append("<pre>");
                var sbSource = new StringBuilder();
                var longestType = logSource.Logs.Max(x => x.Type.Length);
                foreach (var logEntry in logSource.Logs)
                {
                    var toAppend = (longestType - logEntry.Type.Length) + 1;
                    var style = logEntry.Level == "ERR" ? "color:red" : logEntry.Level == "WRN" ? "color:orange" : "";
                    sb.Append(logEntry.Date.ToString("u")).Append(" [").Append(logEntry.Type).Append(']').Append(' ', toAppend).Append('[').Append("<span style ='").Append(style).Append("'>").Append(logEntry.Level).Append("</span>").Append("]: ").Append(logEntry.Message);
                }
                sb.Append("</pre>").Append("</ul></li>");
            }
            sb.Append("</ul>");
            return sb.ToString();
        }
    }
}

#pragma warning restore
#nullable restore
#endif // BUTRCRASHREPORT_DISABLE