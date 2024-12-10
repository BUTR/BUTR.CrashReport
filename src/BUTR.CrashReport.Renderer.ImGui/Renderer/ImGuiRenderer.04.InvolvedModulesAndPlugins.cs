using BUTR.CrashReport.ImGui.Enums;
using BUTR.CrashReport.ImGui.Extensions;
using BUTR.CrashReport.ImGui.Utils;
using BUTR.CrashReport.Models;

namespace BUTR.CrashReport.Renderer.ImGui.Renderer;

partial class ImGuiRenderer<TImGuiIORef, TImGuiViewportRef, TImDrawListRef, TImGuiStyleRef, TColorsRangeAccessorRef, TImGuiListClipperRef>
{
    private KeyValuePair<string, InvolvedModuleOrPluginModel[]>[] _enhancedStacktraceGroupedByModuleId = [];
    private KeyValuePair<string, InvolvedModuleOrPluginModel[]>[] _enhancedStacktraceGroupedByLoaderPluginIdId = [];

    private void InitializeInvolved()
    {
        _enhancedStacktraceGroupedByModuleId = _crashReport.InvolvedModules
            .GroupBy(x => x.ModuleOrLoaderPluginId)
            .Select(x => new KeyValuePair<string, InvolvedModuleOrPluginModel[]>(x.Key, x.ToArray()))
            .ToArray();

        _enhancedStacktraceGroupedByLoaderPluginIdId = _crashReport.InvolvedLoaderPlugins
            .GroupBy(x => x.ModuleOrLoaderPluginId)
            .Select(x => new KeyValuePair<string, InvolvedModuleOrPluginModel[]>(x.Key, x.ToArray()))
            .ToArray();
    }

    private void RenderInvolvedModules()
    {
        var enhancedStacktraceGroupedByModuleIdSpan = CollectionsMarshal<KeyValuePair<string, InvolvedModuleOrPluginModel[]>>.AsSpan(_enhancedStacktraceGroupedByModuleId);
        for (var i = 0; i < enhancedStacktraceGroupedByModuleIdSpan.Length; i++)
        {
            var (moduleId, value) = enhancedStacktraceGroupedByModuleIdSpan[i];
            var involvedModules = CollectionsMarshal<InvolvedModuleOrPluginModel>.AsSpan(value);

            if (_imgui.TreeNode(moduleId, ImGuiTreeNodeFlags.DefaultOpen))
            {
                _imgui.RenderId("Module Id:\0"u8, moduleId);

                var didDirect = false;
                for (var j = 0; j < involvedModules.Length; j++)
                {
                    var involved = involvedModules[j];
                    if (involved.Type != InvolvedModuleOrPluginType.Direct)
                        continue;

                    if (!didDirect)
                    {
                        _imgui.Text("Directly Involved:\0"u8);
                        didDirect = true;
                    }

                    if (_imgui.TreeNode(involved.EnhancedStacktraceFrameName, ImGuiTreeNodeFlags.Bullet | ImGuiTreeNodeFlags.DefaultOpen))
                    {
                        _imgui.TreePop();
                    }
                }

                var didPatch = false;
                for (var j = 0; j < involvedModules.Length; j++)
                {
                    var involved = involvedModules[j];
                    if (involved.Type != InvolvedModuleOrPluginType.Patch)
                        continue;

                    if (!didPatch)
                    {
                        _imgui.Text("Patches Involved:\0"u8);
                        didPatch = true;
                    }

                    if (_imgui.TreeNode(involved.EnhancedStacktraceFrameName, ImGuiTreeNodeFlags.Bullet | ImGuiTreeNodeFlags.DefaultOpen))
                    {
                        _imgui.TreePop();
                    }
                }

                _imgui.TreePop();
            }
        }
    }

    private void RenderInvolvedPlugins()
    {
        var enhancedStacktraceGroupedByLoaderPluginIdIdSpan = CollectionsMarshal<KeyValuePair<string, InvolvedModuleOrPluginModel[]>>.AsSpan(_enhancedStacktraceGroupedByLoaderPluginIdId);
        for (var i = 0; i < enhancedStacktraceGroupedByLoaderPluginIdIdSpan.Length; i++)
        {
            var (loaderPluginId, value) = enhancedStacktraceGroupedByLoaderPluginIdIdSpan[i];
            var involvedLoaderPlugins = CollectionsMarshal<InvolvedModuleOrPluginModel>.AsSpan(value);

            if (_imgui.TreeNode(loaderPluginId, ImGuiTreeNodeFlags.DefaultOpen))
            {
                _imgui.RenderId("Plugin Id:\0"u8, loaderPluginId);

                var didDirect = false;
                for (var j = 0; j < involvedLoaderPlugins.Length; j++)
                {
                    var involved = involvedLoaderPlugins[j];
                    if (involved.Type != InvolvedModuleOrPluginType.Direct)
                        continue;

                    if (!didDirect)
                    {
                        _imgui.Text("Directly Involved:\0"u8);
                        didDirect = true;
                    }

                    if (_imgui.TreeNode(involved.EnhancedStacktraceFrameName, ImGuiTreeNodeFlags.Bullet | ImGuiTreeNodeFlags.DefaultOpen))
                    {
                        _imgui.TreePop();
                    }
                }

                var didPatch = false;
                for (var j = 0; j < involvedLoaderPlugins.Length; j++)
                {
                    var involved = involvedLoaderPlugins[j];
                    if (involved.Type != InvolvedModuleOrPluginType.Patch)
                        continue;

                    if (!didPatch)
                    {
                        _imgui.Text("Patches Involved:\0"u8);
                        didPatch = true;
                    }

                    if (_imgui.TreeNode(involved.EnhancedStacktraceFrameName, ImGuiTreeNodeFlags.Bullet | ImGuiTreeNodeFlags.DefaultOpen))
                    {
                        _imgui.TreePop();
                    }
                }

                _imgui.TreePop();
            }
        }
    }

    private void RenderInvolvedModulesAndPlugins()
    {
        if (_enhancedStacktraceGroupedByModuleId.Length <= 0 && _enhancedStacktraceGroupedByLoaderPluginIdId.Length <= 0) return;

        _imgui.Text("From Highest Probability to Lowest:\0"u8);
        RenderInvolvedModules();
        RenderInvolvedPlugins();
    }
}