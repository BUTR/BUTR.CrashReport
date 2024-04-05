using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BUTR.CrashReport.Extensions;
using BUTR.CrashReport.Models;

namespace BUTR.CrashReport.Renderer.Html;
    
public static partial class CrashReportHtml
{
    private static readonly string MiniDumpTag = "<!-- MINI DUMP -->";
    private static readonly string MiniDumpButtonTag = "<!-- MINI DUMP BUTTON -->";
    private static readonly string SaveFileTag = "<!-- SAVE FILE -->";
    private static readonly string SaveFileButtonTag = "<!-- SAVE FILE BUTTON -->";
    private static readonly string ScreenshotTag = "<!-- SCREENSHOT -->";
    private static readonly string ScreenshotButtonTag = "<!-- SCREENSHOT BUTTON -->";
    private static readonly string JsonModelTag = "<!-- JSON MODEL -->";
    private static readonly string JsonModelButtonTag = "<!-- JSON MODEL BUTTON -->";

    public static string AddData(string htmlReport, string gzipBase64CrashReportJson, string? gZipBase64MiniDump = null, string? gZipBase64SaveFile = null, string? base64Screenshot = null)
    {
        var IncludeMiniDump = !string.IsNullOrEmpty(gZipBase64MiniDump);
        var IncludeSaveFile = !string.IsNullOrEmpty(gZipBase64SaveFile);
        var IncludeScreenshot = !string.IsNullOrEmpty(base64Screenshot);

        if (IncludeMiniDump)
        {
            htmlReport = htmlReport
                .Replace(MiniDumpTag, gZipBase64MiniDump)
                .Replace(MiniDumpButtonTag, """

                                            <![if !IE]>
                                                          <br/>
                                                          <br/>
                                                          <button onclick='minidump(this)'>Get MiniDump</button>
                                            <![endif]>
                                            """);
        }

        if (IncludeSaveFile)
        {
            htmlReport = htmlReport
                .Replace(SaveFileTag, gZipBase64SaveFile)
                .Replace(SaveFileButtonTag, """

                                            <![if !IE]>
                                                          <br/>
                                                          <br/>
                                                          <button onclick='savefile(this)'>Get Save File</button>
                                            <![endif]>
                                            """);
        }

        if (IncludeScreenshot)
        {
            htmlReport = htmlReport
                .Replace(ScreenshotTag, base64Screenshot)
                .Replace(ScreenshotButtonTag, """

                                              <![if !IE]>
                                                          <br/>
                                                          <br/>
                                                          <button onclick='screenshot(this)'>Show Screenshot</button>
                                              <![endif]>
                                              """);
        }
            
        htmlReport = htmlReport
            .Replace(JsonModelTag, gzipBase64CrashReportJson)
            .Replace(JsonModelButtonTag, """

                                         <![if !IE]>
                                                       <br/>
                                                       <br/>
                                                       <button onclick='jsonmodel(this)'>Get as Json</button>
                                         <![endif]>
                                         """);

        return htmlReport;
    }
     
    public static string Build(CrashReportModel crashReportModel, IEnumerable<LogSource> files) => GetBase(crashReportModel, files);

    private static string GetRecursiveExceptionHtml(CrashReportModel crashReport, ExceptionModel? ex)
    {
        if (ex is null) return string.Empty;

        var callStackLines = ex.CallStack.Split([Environment.NewLine], StringSplitOptions.RemoveEmptyEntries).Select(x => x.EscapeGenerics()).ToArray();
        var firstCallStackLine = callStackLines[0].Trim();
        var stacktrace = crashReport.EnhancedStacktrace.FirstOrDefault(x => firstCallStackLine == $"at {x.FrameDescription}");

        var moduleId = stacktrace?.ExecutingMethod.ModuleId ?? "UNKNOWN";
        var sourceModuleId = ex.SourceModuleId ?? "UNKNOWN";
            
        var pluginId = stacktrace?.ExecutingMethod.LoaderPluginId ?? "UNKNOWN";
        var sourcePluginId = ex.SourceLoaderPluginId ?? "UNKNOWN";

        var hasMessage = !string.IsNullOrWhiteSpace(ex.Message);
        var hasCallStack = !string.IsNullOrWhiteSpace(ex.CallStack);
        var hasInner = ex.InnerException is not null;
        return new StringBuilder()
            .Append("Exception Information:").Append("<br/>")
            .AppendIf(moduleId != "UNKNOWN", sb =>  sb.Append("Potential Module Id: ").Append("<b><a href='javascript:;' onclick='scrollToElement(\"").Append(moduleId).Append("\")'>").Append(moduleId).Append("</a></b>").Append("<br/>"))
            .AppendIf(sourceModuleId != "UNKNOWN", sb =>  sb.Append("Potential Source Module Id: ").Append("<b><a href='javascript:;' onclick='scrollToElement(\"").Append(sourceModuleId).Append("\")'>").Append(sourceModuleId).Append("</a></b>").Append("<br/>"))
            .AppendIf(pluginId != "UNKNOWN", sb =>  sb.Append("Potential Plugin Id: ").Append("<b><a href='javascript:;' onclick='scrollToElement(\"").Append(pluginId).Append("\")'>").Append(pluginId).Append("</a></b>").Append("<br/>"))
            .AppendIf(sourcePluginId != "UNKNOWN", sb =>  sb.Append("Potential Source Plugin Id: ").Append("<b><a href='javascript:;' onclick='scrollToElement(\"").Append(sourcePluginId).Append("\")'>").Append(sourcePluginId).Append("</a></b>").Append("<br/>"))
            .Append("Type: ").Append(ex.Type.EscapeGenerics()).Append("<br/>")
            .AppendIf(hasMessage, sb => sb.Append("Message: ").Append(ex.Message.EscapeGenerics()).Append("<br/>"))
            .AppendIf(hasCallStack, sb => sb.Append("Stacktrace:").Append("<br/>"))
            .AppendIf(hasCallStack, "<ol>")
            .AppendIf(hasCallStack, "<li>")
            .AppendJoinIf(hasCallStack, "</li><li>", callStackLines)
            .AppendIf(hasCallStack, "</li>")
            .AppendIf(hasCallStack, "</ol>")
            .AppendIf(hasInner, "<br/>")
            .AppendIf(hasInner, "<br/>")
            .AppendIf(hasInner, sb => sb.Append("Inner ").Append(GetRecursiveExceptionHtml(crashReport, ex.InnerException!)))
            .ToString();
    }

    private static string GetEnhancedStacktraceHtml(CrashReportModel crashReport)
    {
        var random = new Random();
        var sbMain = new StringBuilder();
        var sbCil = new StringBuilder();
        sbMain.Append("<ul>");
        foreach (var stacktrace in crashReport.EnhancedStacktrace)
        {
            var id1 = random.Next();
            var id2 = random.Next();
            var id3 = random.Next();
            var id4 = random.Next();
            var moduleId2 = stacktrace.ExecutingMethod.ModuleId ?? "UNKNOWN";
            var pluginId2 = stacktrace.ExecutingMethod.LoaderPluginId ?? "UNKNOWN";
            sbMain.Append("<li>")
                .Append("Frame: ").Append(stacktrace.FrameDescription.EscapeGenerics()).Append("<br/>")
                .Append("Executing Method:")
                .Append("<ul>")
                .Append("<li>")
                .AppendIf(moduleId2 != "UNKNOWN", sb =>  sb.Append("Module Id: ").Append("<b><a href='javascript:;' onclick='scrollToElement(\"").Append(moduleId2).Append("\")'>").Append(moduleId2).Append("</a></b>").Append("<br/>"))
                .AppendIf(pluginId2 != "UNKNOWN", sb =>  sb.Append("Plugin Id: ").Append("<b><a href='javascript:;' onclick='scrollToElement(\"").Append(pluginId2).Append("\")'>").Append(pluginId2).Append("</a></b>").Append("<br/>"))
                .Append("Method: ").Append(stacktrace.ExecutingMethod.MethodFullDescription.EscapeGenerics()).Append("<br/>")
                .Append("Method From Stackframe Issue: ").Append(stacktrace.MethodFromStackframeIssue).Append("<br/>")
                .Append("Approximate IL Offset: ").Append(stacktrace.ILOffset is not null ? $"{stacktrace.ILOffset:X4}" : "UNKNOWN").Append("<br/>")
                .Append("Native Offset: ").Append(stacktrace.NativeOffset is not null ? $"{stacktrace.NativeOffset:X4}" : "UNKNOWN").Append("<br/>")
                .AppendIf(stacktrace.ExecutingMethod.ILInstructions.Count > 0, sp => sp
                    .Append(ContainerCode($"{id1}", "IL:", string.Join(Environment.NewLine, stacktrace.ExecutingMethod.ILInstructions.Select(x => x.EscapeGenerics())))))
                .AppendIf(stacktrace.ExecutingMethod.CSharpILMixedInstructions.Count > 0, sp => sp
                    .Append(ContainerCode($"{id2}", "IL with C#:", string.Join(Environment.NewLine, stacktrace.ExecutingMethod.CSharpILMixedInstructions.Select(x => x.EscapeGenerics())))))
                .AppendIf(stacktrace.ExecutingMethod.CSharpInstructions.Count > 0, sp => sp
                    .Append(ContainerCode($"{id3}", "C#:", string.Join(Environment.NewLine, stacktrace.ExecutingMethod.CSharpILMixedInstructions.Select(x => x.EscapeGenerics())))))
                .AppendIf(stacktrace.ExecutingMethod.NativeInstructions.Count > 0, sp => sp
                    .Append(ContainerCode($"{id4}", "Native:", string.Join(Environment.NewLine, stacktrace.ExecutingMethod.NativeInstructions.Select(x => x.EscapeGenerics())))))
                .Append("</li>")
                .Append("</ul>");

            if (stacktrace.PatchMethods.Count > 0)
            {
                sbMain.Append("Patch Methods:")
                    .Append("<ul>");
                foreach (var method in stacktrace.PatchMethods)
                {
                    var id01 = random.Next();
                    var id02 = random.Next();
                    var id03 = random.Next();
                    var moduleId = method.ModuleId ?? "UNKNOWN";
                    var pluginId = method.LoaderPluginId ?? "UNKNOWN";
                    var harmonyPatch = method as MethodHarmonyPatch;
                    sbMain.Append("<li>")
                        .Append("Type: ").Append(harmonyPatch is not null ? "Harmony" : "UNKNOWN").Append("<br/>")
                        .AppendIf(harmonyPatch is not null, sb => sb.Append("Patch Type: ").Append(harmonyPatch!.PatchType.ToString()).Append("<br/>"))
                        .AppendIf(moduleId != "UNKNOWN", sb => sb.Append("Module Id: ").Append("<b><a href='javascript:;' onclick='scrollToElement(\"").Append(moduleId).Append("\")'>").Append(moduleId).Append("</a></b>").Append("<br/>"))
                        .AppendIf(pluginId != "UNKNOWN", sb => sb.Append("Plugin Id: ").Append("<b><a href='javascript:;' onclick='scrollToElement(\"").Append(pluginId).Append("\")'>").Append(pluginId).Append("</a></b>").Append("<br/>"))
                        .Append("Method: ").Append(method.MethodFullDescription.EscapeGenerics()).Append("<br/>")
                        .AppendIf(method.ILInstructions.Count > 0, sp => sp
                            .Append(ContainerCode($"{id01}", "IL:", string.Join(Environment.NewLine, method.ILInstructions.Select(x => x.EscapeGenerics())))))
                        .AppendIf(method.CSharpILMixedInstructions.Count > 0, sp => sp
                            .Append(ContainerCode($"{id02}", "IL with C#:", string.Join(Environment.NewLine, method.CSharpILMixedInstructions.Select(x => x.EscapeGenerics())))))
                        .AppendIf(method.CSharpInstructions.Count > 0, sp => sp
                            .Append(ContainerCode($"{id03}", "C#:", string.Join(Environment.NewLine, method.CSharpILMixedInstructions.Select(x => x.EscapeGenerics())))))
                        .Append("</li>");
                }
                sbMain.Append("</ul>");
            }

            if (stacktrace.OriginalMethod is not null)
            {
                var moduleId3 = stacktrace.OriginalMethod.ModuleId ?? "UNKNOWN";
                var pluginId3 = stacktrace.OriginalMethod.LoaderPluginId ?? "UNKNOWN";
                    
                var id01 = random.Next();
                var id02 = random.Next();
                var id03 = random.Next();
                sbMain.Append("Original Method:")
                    .Append("<ul>")
                    .Append("<li>")
                    .AppendIf(moduleId3 != "UNKNOWN", sb =>  sb.Append("Module Id: ").Append("<b><a href='javascript:;' onclick='scrollToElement(\"").Append(moduleId3).Append("\")'>").Append(moduleId3).Append("</a></b>").Append("<br/>"))
                    .AppendIf(pluginId3 != "UNKNOWN", sb =>  sb.Append("Plugin Id: ").Append("<b><a href='javascript:;' onclick='scrollToElement(\"").Append(pluginId3).Append("\")'>").Append(pluginId3).Append("</a></b>").Append("<br/>"))
                    .Append("Method: ").Append(stacktrace.OriginalMethod.MethodFullDescription.EscapeGenerics()).Append("<br/>")
                    .AppendIf(stacktrace.OriginalMethod.ILInstructions.Count > 0, sb => sb
                        .Append(ContainerCode($"{id01}", "IL:", string.Join(Environment.NewLine, stacktrace.OriginalMethod.ILInstructions.Select(x => x.EscapeGenerics())))))
                    .AppendIf(stacktrace.OriginalMethod.CSharpILMixedInstructions.Count > 0, sb => sb
                        .Append(ContainerCode($"{id02}", "IL with C#:", string.Join(Environment.NewLine, stacktrace.OriginalMethod.CSharpILMixedInstructions.Select(x => x.EscapeGenerics())))))
                    .AppendIf(stacktrace.OriginalMethod.CSharpInstructions.Count > 0, sb => sb
                        .Append(ContainerCode($"{id03}", "C#:", string.Join(Environment.NewLine, stacktrace.OriginalMethod.CSharpILMixedInstructions.Select(x => x.EscapeGenerics())))))
                    .Append("</li>")
                    .Append("</ul>");
            }

            sbMain.Append("</br>");
            sbMain.Append("</li>");
            sbCil.Clear();
        }
        sbMain.Append("</ul>");
        return sbMain.ToString();
    }

    private static void AddInvolvedModules(CrashReportModel crashReport, StringBuilder sbMain)
    {
        foreach (var grouping in crashReport.EnhancedStacktrace.GroupBy(x => x.ExecutingMethod.ModuleId ?? "UNKNOWN"))
        {
            var moduleId = grouping.Key;
            if (moduleId == "UNKNOWN") continue;

            sbMain.Append("<li>")
                .Append("Module Id: ").Append("<a href='javascript:;' onclick='scrollToElement(\"").Append(moduleId).Append("\")'>").Append(moduleId).Append("</a>").Append("<br/>");

            foreach (var stacktrace in grouping)
            {
                sbMain.Append("Method: ").Append(stacktrace.ExecutingMethod.MethodFullDescription.EscapeGenerics()).Append("<br/>")
                    .Append("Frame: ").Append(stacktrace.FrameDescription.EscapeGenerics()).Append("<br/>");

                if (stacktrace.PatchMethods.Count > 0)
                {
                    sbMain.Append("Patches:").Append("<br/>")
                        .Append("<ul>");
                    foreach (var method in stacktrace.PatchMethods)
                    {
                        var harmonyPatch = method as MethodHarmonyPatch;
                            
                        // Ignore blank transpilers used to force the jitter to skip inlining
                        if (method.MethodName == "BlankTranspiler") continue;
                        var moduleId2 = method.ModuleId ?? "UNKNOWN";
                        sbMain.Append("<li>")
                            .AppendIf(moduleId2 == "UNKNOWN", sb =>  sb.Append("Module Id: ").Append(moduleId2).Append("<br/>"))
                            .AppendIf(moduleId2 != "UNKNOWN", sb =>  sb.Append("Module Id: ").Append("<b><a href='javascript:;' onclick='scrollToElement(\"").Append(moduleId2).Append("\")'>").Append(moduleId2).Append("</a></b>").Append("<br/>"))
                            .Append("Method: ").Append(method.MethodFullDescription.EscapeGenerics()).Append("<br/>")
                            .AppendIf(harmonyPatch is not null, sb => sb.Append("Harmony Patch Type: ").Append(harmonyPatch!.PatchType).Append("<br/>"))
                            .Append("</li>");
                    }
                    sbMain.Append("</ul>");
                }

                sbMain.Append("</br>");

                sbMain.Append("</li>");
            }

            sbMain.Append("</li>");
        }
    }
    private static void AddInvolvedPlugins(CrashReportModel crashReport, StringBuilder sbMain)
    {
        foreach (var grouping in crashReport.EnhancedStacktrace.GroupBy(x => x.ExecutingMethod.LoaderPluginId ?? "UNKNOWN"))
        {
            var pluginId = grouping.Key;
            if (pluginId == "UNKNOWN") continue;

            sbMain.Append("<li>")
                .Append("Plugin Id: ").Append("<a href='javascript:;' onclick='scrollToElement(\"").Append(pluginId).Append("\")'>").Append(pluginId).Append("</a>").Append("<br/>");

            foreach (var stacktrace in grouping)
            {
                sbMain.Append("Method: ").Append(stacktrace.ExecutingMethod.MethodFullDescription.EscapeGenerics()).Append("<br/>")
                    .Append("Frame: ").Append(stacktrace.FrameDescription.EscapeGenerics()).Append("<br/>");

                if (stacktrace.PatchMethods.Count > 0)
                {
                    sbMain.Append("Patches:").Append("<br/>")
                        .Append("<ul>");
                    foreach (var method in stacktrace.PatchMethods)
                    {
                        var harmonyPatch = method as MethodHarmonyPatch;
                            
                        // Ignore blank transpilers used to force the jitter to skip inlining
                        if (method.MethodName == "BlankTranspiler") continue;
                        var pluginId2 = method.LoaderPluginId ?? "UNKNOWN";
                        sbMain.Append("<li>")
                            .AppendIf(pluginId2 == "UNKNOWN", sb =>  sb.Append("Plugin Id: ").Append(pluginId2).Append("<br/>"))
                            .AppendIf(pluginId2 != "UNKNOWN", sb =>  sb.Append("Plugin Id: ").Append("<b><a href='javascript:;' onclick='scrollToElement(\"").Append(pluginId2).Append("\")'>").Append(pluginId2).Append("</a></b>").Append("<br/>"))
                            .Append("Method: ").Append(method.MethodFullDescription.EscapeGenerics()).Append("<br/>")
                            .AppendIf(harmonyPatch is not null, sb => sb.Append("Harmony Patch Type: ").Append(harmonyPatch!.PatchType).Append("<br/>"))
                            .Append("</li>");
                    }
                    sbMain.Append("</ul>");
                }

                sbMain.Append("</br>");

                sbMain.Append("</li>");
            }

            sbMain.Append("</li>");
        }
    }
    private static string GetInvolvedHtml(CrashReportModel crashReport)
    {
        var sb = new StringBuilder();
        sb.Append("Based on Stacktrace:")
            .Append("<ul>");
        AddInvolvedModules(crashReport, sb);
        AddInvolvedPlugins(crashReport, sb);
        sb.Append("</ul>");
        return sb.ToString();
    }

    private static string GetInstalledModulesHtml(CrashReportModel crashReport)
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
                if (dependentModule.Type == DependencyMetadataModelType.Incompatible)
                {
                    deps[dependentModule.ModuleOrPluginId] = tmp.Clear()
                        .Append("Incompatible ")
                        .Append("<a href='javascript:;' onclick='scrollToElement(\"").Append(dependentModule.ModuleOrPluginId).Append("\")'>")
                        .Append(dependentModule.ModuleOrPluginId)
                        .Append("</a>")
                        .AppendIf(dependentModule.IsOptional, " (optional)")
                        .AppendIf(hasVersion, sb => sb.Append(" >= ").Append(dependentModule.Version))
                        .AppendIf(hasVersionRange, dependentModule.VersionRange)
                        .ToString();
                }
                else if (dependentModule.Type == DependencyMetadataModelType.LoadAfter)
                {
                    deps[dependentModule.ModuleOrPluginId] = tmp.Clear()
                        .Append("Load ").Append("After ")
                        .Append("<a href='javascript:;' onclick='scrollToElement(\"").Append(dependentModule.ModuleOrPluginId).Append("\")'>")
                        .Append(dependentModule.ModuleOrPluginId)
                        .Append("</a>")
                        .AppendIf(dependentModule.IsOptional, " (optional)")
                        .AppendIf(hasVersion, sb => sb.Append(" >= ").Append(dependentModule.Version))
                        .AppendIf(hasVersionRange, dependentModule.VersionRange)
                        .ToString();
                }
                else if (dependentModule.Type == DependencyMetadataModelType.LoadBefore)
                {
                    deps[dependentModule.ModuleOrPluginId] = tmp.Clear()
                        .Append("Load ").Append("Before ")
                        .Append("<a href='javascript:;' onclick='scrollToElement(\"").Append(dependentModule.ModuleOrPluginId).Append("\")'>")
                        .Append(dependentModule.ModuleOrPluginId)
                        .Append("</a>")
                        .AppendIf(dependentModule.IsOptional, " (optional)")
                        .AppendIf(hasVersion, sb => sb.Append(" >= ").Append(dependentModule.Version))
                        .AppendIf(hasVersionRange, dependentModule.VersionRange)
                        .ToString();
                }
            }

            dependenciesBuilder.Clear();
            foreach (var dep in deps)
                dependenciesBuilder.Append("<li>").Append(dep.Value).Append("</li>");
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
                    .Append("DLLName: ").Append(subModule.AssemblyId?.Name).Append("</br>")
                    .Append("SubModuleClassType: ").Append(subModule.Entrypoint).Append("</br>")
                    .AppendIf(hasTags, sb => sb.Append("Tags:").Append("</br>"))
                    .AppendIf(hasTags, "<ul>")
                    .AppendIf(hasTags, tagsBuilder)
                    .AppendIf(hasTags, "</ul>")
                    .AppendIf(hasAssemblies, sb => sb.Append("Assemblies:").Append("</br>"))
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
            foreach (var assembly in crashReport.Assemblies.Where(y => y.ModuleId == module.Id))
                additionalAssembliesBuilder.Append("<li>").Append(assembly.Id.Name).Append(" (").Append(assembly.GetFullName()).Append(")").Append("</li>");
        }

        moduleBuilder.Append("<ul>");
        foreach (var module in crashReport.Modules)
        {
            AppendDependencies(module);
            AppendSubModules(module);
            AppendAdditionalAssemblies(module);

            var isVortexManaged = module.AdditionalMetadata.FirstOrDefault(x => x.Key == "METADATA:MANAGED_BY_VORTEX")?.Value is { } str && bool.TryParse(str, out var val) && val;

            var container = module switch
            {
                { IsOfficial: true } => "modules-official-container",
                { IsExternal: true } => "modules-external-container",
                _ => "modules-container",
            };
            var hasDependencies = dependenciesBuilder.Length != 0;
            var hasUrl = !string.IsNullOrWhiteSpace(module.Url);
            var hasUpdateInfo = module.UpdateInfo is not null;
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
                .AppendIf(hasDependencies, sb => sb.Append("Dependencies:").Append("</br>"))
                .AppendIf(hasDependencies, "<ul>")
                .AppendIf(hasDependencies, dependenciesBuilder)
                .AppendIf(hasDependencies, "</ul>")
                .Append("Capabilities:").Append("</br>")
                .Append("<ul>")
                .Append((StringBuilder sb) =>
                {
                    if (module.Capabilities.Count == 0)
                        sb.Append("<li>").Append("None").Append("</li>");
                        
                    foreach (var capability in module.Capabilities)
                        sb.Append("<li>").Append(capability).Append("</li>");
                        
                    return sb;
                })
                .Append("</ul>")
                .AppendIf(hasUrl, sb => sb.Append("Url: <a href='").Append(module.Url).Append("'>").Append(module.Url).Append("</a>").Append("</br>"))
                .AppendIf(hasUpdateInfo, sb => sb.Append("Update Info: ").Append(module.UpdateInfo).Append("</br>"))
                .AppendIf(hasSubModules, sb => sb.Append("SubModules:").Append("</br>"))
                .AppendIf(hasSubModules, "<ul>")
                .AppendIf(hasSubModules, subModulesBuilder)
                .AppendIf(hasSubModules, "</ul>")
                .AppendIf(hasAssemblies, sb => sb.Append("Assemblies Present:").Append("</br>"))
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
        
    private static string GetLoadedBLSEPluginsHtml(CrashReportModel crashReport)
    {
        var moduleBuilder = new StringBuilder();

        moduleBuilder.Append("<ul>");
        foreach (var plugin in crashReport.LoaderPlugins)
        {
            var container = "modules-container";
            var hasUpdateInfo = plugin.UpdateInfo is not null;
            moduleBuilder.Append("<li>")
                .Append("<div class='").Append(container).Append("'>")
                .Append("<b><a href='javascript:;' onclick='showHideById(this, \"").Append(plugin.Id).Append("\")'>").Append("+ ").Append(plugin.Name).Append(" (").Append(plugin.Id).AppendIf(!string.IsNullOrEmpty(plugin.Version), sb => sb.Append(", ").Append(plugin.Version)).Append(")").Append("</a></b>")
                .Append("<div id='").Append(plugin.Id).Append("' style='display: none'>")
                .Append("Id: ").Append(plugin.Id).Append("</br>")
                .Append("Name: ").Append(plugin.Name).Append("</br>")
                .AppendIf(!string.IsNullOrEmpty(plugin.Version), sb => sb.Append("Version: ").Append(plugin.Version).Append("</br>"))
                .AppendIf(hasUpdateInfo, sb => sb.Append("Update Info: ").Append(plugin.UpdateInfo).Append("</br>"))
                .Append("</div>")
                .Append("</div>")
                .Append("</li>");
        }
        moduleBuilder.Append("</ul>");

        return moduleBuilder.ToString();
    }

    private static string GetAssembliesHtml(CrashReportModel crashReport)
    {
        var sb0 = new StringBuilder();

        void AppendAssembly(AssemblyModel assembly)
        {
            var @class = string.Join(" ", assembly.Type.GetFlags().Select(x => x switch
            {
                AssemblyModelType.Dynamic => "dynamic_assembly",
                AssemblyModelType.GAC => "gac_assembly",
                AssemblyModelType.System => "sys_assembly",
                AssemblyModelType.GameCore => "game_assembly",
                AssemblyModelType.GameModule => "game_module_assembly",
                AssemblyModelType.Module => "module_assembly",
                AssemblyModelType.Loader => "loader_assembly",
                AssemblyModelType.LoaderPlugin => "loader_plugin_assembly",
                _ => string.Empty,
            }));
            var isDynamic = assembly.Type.HasFlag(AssemblyModelType.Dynamic);
            var hasPath = assembly.AnonymizedPath != "EMPTY" && !string.IsNullOrWhiteSpace(assembly.AnonymizedPath);
            sb0.Append("<li class='").Append(@class).Append("'>")
                .Append(assembly.Id.Name).Append(", ")
                .Append(assembly.Id.Version).Append(", ")
                .Append(assembly.Architecture).Append(", ")
                .AppendIf(!isDynamic, sb => sb.Append(assembly.Hash).Append(", "))
                .AppendIf(isDynamic && !hasPath, "DYNAMIC")
                .AppendIf(!isDynamic && !hasPath, "EMPTY")
                .AppendIf(!isDynamic && hasPath, sb => sb.Append("<a href='javascript:;'>...").Append(Path.DirectorySeparatorChar).Append(assembly.AnonymizedPath).Append("</a>"))
                .Append("</li>");
        }

        sb0.Append("<ul>");
        foreach (var assembly in crashReport.Assemblies)
            AppendAssembly(assembly);
        sb0.Append("</ul>");

        return sb0.ToString();
    }

    private static string GetHarmonyPatchesHtml(CrashReportModel crashReport)
    {
        var harmonyPatchesListBuilder = new StringBuilder();
        var patchesBuilder = new StringBuilder();
        var patchBuilder = new StringBuilder();

        void AppendPatches(string name, IEnumerable<HarmonyPatchModel> patches)
        {
            patchBuilder.Clear();
            foreach (var patch in patches)
            {
                var moduleId = patch.ModuleId ?? "UNKNOWN";
                var pluginId = patch.LoaderPluginId ?? "UNKNOWN";
                var hasIndex = patch.Index != 0;
                var hasPriority = patch.Priority != 400;
                var hasBefore = patch.Before.Count > 0;
                var hasAfter = patch.After.Count > 0;
                patchBuilder.Append("<li>")
                    .AppendIf(moduleId != "UNKNOWN", sb =>  sb.Append("Module Id: ").Append("<b><a href='javascript:;' onclick='scrollToElement(\"").Append(moduleId).Append("\")'>").Append(moduleId).Append("</a></b>").Append("; "))
                    .AppendIf(pluginId != "UNKNOWN", sb =>  sb.Append("Plugin Id: ").Append("<b><a href='javascript:;' onclick='scrollToElement(\"").Append(pluginId).Append("\")'>").Append(pluginId).Append("</a></b>").Append("; "))
                    .Append("Owner: ").Append(patch.Owner).Append("; ")
                    .Append("Namespace: ").Append(patch.Namespace).Append("; ")
                    .AppendIf(hasIndex, sb => sb.Append("Index: ").Append(patch.Index).Append("; "))
                    .AppendIf(hasPriority, sb => sb.Append("Priority: ").Append(patch.Priority).Append("; "))
                    .AppendIf(hasBefore, sb => sb.Append("Before: ").AppendJoin(", ", patch.Before).Append("; "))
                    .AppendIf(hasAfter, sb => sb.Append("After: ").AppendJoin(", ", patch.After).Append(";"))
                    .Append("</li>");
            }

            if (patchBuilder.Length > 0)
            {
                patchesBuilder.Append("<li>").Append(name).Append("<ul>").Append(patchBuilder).Append("</ul>").Append("</li>");
            }
        }

        harmonyPatchesListBuilder.Append("<ul>");
        foreach (var harmonyPatch in crashReport.HarmonyPatches)
        {
            patchesBuilder.Clear();

            AppendPatches("Prefixes", harmonyPatch.Patches.Where(x => x.Type == HarmonyPatchType.Prefix));
            AppendPatches("Postfixes", harmonyPatch.Patches.Where(x => x.Type == HarmonyPatchType.Postfix));
            AppendPatches("Finalizers", harmonyPatch.Patches.Where(x => x.Type == HarmonyPatchType.Finalizer));
            AppendPatches("Transpilers", harmonyPatch.Patches.Where(x => x.Type == HarmonyPatchType.Transpiler));

            if (patchesBuilder.Length > 0)
            {
                var methodNameFull = !string.IsNullOrEmpty(harmonyPatch.OriginalMethodDeclaredTypeName)
                    ? $"{harmonyPatch.OriginalMethodDeclaredTypeName}.{harmonyPatch.OriginalMethodName}"
                    : harmonyPatch.OriginalMethodName;
                harmonyPatchesListBuilder.Append("<li>")
                    .Append(methodNameFull).Append("<ul>").Append(patchesBuilder).Append("</ul>")
                    .Append("</li>")
                    .Append("<br/>");
            }
        }
        harmonyPatchesListBuilder.Append("</ul>");

        return harmonyPatchesListBuilder.ToString();
    }

    private static string GetLogFilesHtml(IEnumerable<LogSource> files)
    {
        var sb = new StringBuilder();

        sb.Append("<ul>");
        foreach (var logSource in files)
        {
            if (logSource.Logs.Count == 0) continue;

            sb.Append("<li>").Append("<a>").Append(logSource.Name).Append("</a>").Append("<pre>");
            var longestType = logSource.Logs.Max(x => x.Type.Length);
            foreach (var logEntry in logSource.Logs)
            {
                var toAppend = longestType - logEntry.Type.Length + 1;
                var style = logEntry.Level switch
                {
                    LogLevel.Error or LogLevel.Fatal => "color:red",
                    LogLevel.Warning => "color:orange",
                    _ => ""
                };
                var level = logEntry.Level switch
                {
                    LogLevel.Fatal => "FTL",
                    LogLevel.Error => "ERR",
                    LogLevel.Warning => "WRN",
                    LogLevel.Information => "INF",
                    LogLevel.Debug => "DBG",
                    LogLevel.Verbose => "VRB",
                    _ => "   "
                };
                sb.Append(logEntry.Date.ToString("u")).Append(" [").Append(logEntry.Type).Append(']').Append(' ', toAppend).Append('[').Append("<span style ='").Append(style).Append("'>").Append(level).Append("</span>").Append("]: ").Append(logEntry.Message).AppendLine();
            }
            sb.Append("</pre>").Append("</ul></li>");
        }
        sb.Append("</ul>");
        return sb.ToString();
    }
}