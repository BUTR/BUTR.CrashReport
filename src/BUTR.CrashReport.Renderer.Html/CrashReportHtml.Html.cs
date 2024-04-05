using BUTR.CrashReport.Models;

using System.Collections.Generic;
using System.Text;

namespace BUTR.CrashReport.Renderer.Html;

partial class CrashReportHtml
{
#pragma warning disable format // @formatter:off
    private static string GetBase(CrashReportModel crashReport, IEnumerable<LogSource> files)
    {
        var sbMetadata = new StringBuilder();
        foreach (var metadata in crashReport.Metadata.AdditionalMetadata)
            sbMetadata.Append($"<br />").Append(metadata.Key).Append(": ").AppendLine(metadata.Value);

        var pluginLoaderVersion = crashReport.Metadata.LoaderPluginProviderVersion;
        
        return $$"""
<html>  
  <head>
    <title>{{crashReport.Metadata.GameName}} Crash Report</title>
    <meta charset='utf-8' />
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
              <b>{{crashReport.Metadata.GameName}} has encountered a problem and will close itself.</b>
              <br />
              This is a community Crash Report. Please save it and use it for reporting the error. Do not provide screenshots, provide the report!
              <br />
              Most likely this error was caused by a custom installed module.
              <br />
              <br />
              If you were in the middle of something, the progress might be lost.
              <br />
              <br />
              Launcher: {{crashReport.Metadata.LauncherType}} ({{crashReport.Metadata.LauncherVersion}})
              <br />
              Runtime: {{crashReport.Metadata.Runtime}}
              {{(!string.IsNullOrEmpty(pluginLoaderVersion) ? $"<br />{crashReport.Metadata.LoaderPluginProviderName} Version: {pluginLoaderVersion}" : string.Empty)}}
              {{sbMetadata}}
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
              {{JsonModelButtonTag}} {{MiniDumpButtonTag}} {{SaveFileButtonTag}} {{ScreenshotButtonTag}}
            </div>
          </td>
        </tr>
      </tbody>
    </table>
{{Container("exception", "Exception", GetRecursiveExceptionHtml(crashReport, crashReport.Exception))}}
{{Container("enhanced-stacktrace", "Enhanced Stacktrace", GetEnhancedStacktraceHtml(crashReport))}}
{{Container("involved", "Involved Modules and Plugins", GetInvolvedHtml(crashReport))}}
{{Container("installed-modules", "Installed Modules", GetInstalledModulesHtml(crashReport))}}
{{Container("installed-plugins", $"Loaded {crashReport.Metadata.LoaderPluginProviderName} Plugins", GetLoadedBLSEPluginsHtml(crashReport))}}
{{Container("assemblies", "Assemblies", $"""
        <label>Hide: </label>
        <label><input type='checkbox' onclick='showHideByClassName(this, "sys_assembly")' /> System</label>
        <label><input type='checkbox' onclick='showHideByClassName(this, "gac_assembly")' /> GAC</label>
        <label><input type='checkbox' onclick='showHideByClassName(this, "game_assembly")' /> Game</label>
        <label><input type='checkbox' onclick='showHideByClassName(this, "game_module_assembly")' /> Game Modules</label>
        <label><input type='checkbox' onclick='showHideByClassName(this, "module_assembly")' /> Modules</label>
        <label><input type='checkbox' onclick='showHideByClassName(this, "loader_assembly")' /> Loader</label>
        <label><input type='checkbox' onclick='showHideByClassName(this, "loader_plugin_assembly")' /> Loader Plugin</label>
        <label><input type='checkbox' onclick='showHideByClassName(this, "dynamic_assembly")' /> Dynamic</label>
        <label><input type='checkbox' onclick='showHideByClassName(this, "unclas_assembly")' /> Unclassified</label>
        {GetAssembliesHtml(crashReport)}
""")}}
{{Container("harmony-patches", "Harmony Patches", GetHarmonyPatchesHtml(crashReport))}}
{{Container("log-files", "Log Files", GetLogFilesHtml(files))}}

{{Container("mini-dump", "Mini Dump", MiniDumpTag, true)}}
{{Container("save-file", "Save File", SaveFileTag, true)}}
{{Container("save-file", "Screenshot", "<img id='screenshot' alt='Screenshot' />", true)}}
{{Container("screenshot-data", "Screenshot Data", ScreenshotTag, true)}}
{{Container("json-model-data", "Json Model Data", JsonModelTag, true)}}

<![if !IE]>
    <script src="https://cdn.jsdelivr.net/pako/1.0.3/pako_inflate.min.js"></script>
<![endif]>
    {{Scripts}}
  </body>
</html>
""";
    }
                
                
    private static readonly string Scripts = $$"""
<script>
    function scrollToElement(className) {
      var element = document.getElementById(className);
      
	  var iterElement = element;
	  iterElement.style.display = "block";
      while (iterElement.parentNode && iterElement.parentNode.style){
	    iterElement.parentNode.style.display = "block";
        iterElement = iterElement.parentNode;
      }
      
      element.scrollIntoView(false);
   }
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
     document.getElementById("enhanced-stacktrace").style.fontSize = fontSize.value;
     document.getElementById("involved").style.fontSize = fontSize.value;
     document.getElementById("installed-modules").style.fontSize = fontSize.value;
     document.getElementById("installed-plugins").style.fontSize = fontSize.value;
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
   function jsonmodel(element) {
     var base64 = document.getElementById("json-model-data").innerText.trim();
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
     a.download = "crashreport.json";
     a.click();
   }
 </script>                                    
""";

    private static string ContainerNew(string id, string name, string content, bool hide = false) => $"""
    <details {(hide ? "style='display: none;'" : string.Empty)}>
      <summary>{name}</summary>
      <div id='{id}'>
        {content}
      </div>
    </details>
""";
        
    private static string ContainerCodeNew(string id, string name, string content, bool hide = false) => $"""
    <details {(hide ? "style='display: none;'" : string.Empty)}>
      <summary>{name}</summary>
      <div>
        <pre>
          {content}
        </pre>
      </div>
    </details>
""";
        
    private static string Container(string id, string name, string content, bool hide = false) => $"""
    <div class='root-container' {(hide ? "style='display: none;'" : string.Empty)}>
      <h2><a href='javascript:;' class='headers' onclick='showHideById(this, "{id}")'>+ {name}</a></h2>
      <div id='{id}' class='headers-container'>
        {content}
      </div>
    </div>
""";
        
    private static string ContainerCode(string id, string name, string content, bool hide = false) => $"""
    <div>
      <a href="javascript:;" class="headers" onclick="showHideById(this, "{id}")">+ {name}</a>
      <div id="{id}" class="headers-container" style="display: none;">
        <pre>
          {content}
        </pre>
      </div>
    </div>
""";
   #pragma warning disable format // @formatter:on
}