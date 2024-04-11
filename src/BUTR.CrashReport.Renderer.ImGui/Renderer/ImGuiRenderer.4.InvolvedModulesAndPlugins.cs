using BUTR.CrashReport.Models;

using ImGuiNET;

using System.Collections.Generic;
using System.Linq;

namespace BUTR.CrashReport.Renderer.ImGui.Renderer;

partial class ImGuiRenderer
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
        foreach (var kv in _enhancedStacktraceGroupedByModuleId)
        {
            if (_imgui.TreeNode(kv.Key, ImGuiTreeNodeFlags.DefaultOpen))
            {
                _imgui.RenderId("Module Id:\0"u8, kv.Key);

                for (var j = 0; j < kv.Value.Length; j++)
                {
                    var involved = kv.Value[j];
                    _imgui.Bullet();
                    _imgui.Indent();

                    _imgui.TextSameLine("Frame: \0"u8);
                    _imgui.Text(involved.EnhancedStacktraceFrameName);
                    
                    _imgui.Unindent();
                }

                _imgui.TreePop();
            }
        }
    }

    private void RenderInvolvedPlugins()
    {
        foreach (var kv in _enhancedStacktraceGroupedByLoaderPluginIdId)
        {
            if (_imgui.TreeNode(kv.Key, ImGuiTreeNodeFlags.DefaultOpen))
            {
                _imgui.RenderId("Plugin Id:\0"u8, kv.Key);

                for (var j = 0; j < kv.Value.Length; j++)
                {
                    var involved = kv.Value[j];
                    _imgui.Bullet();
                    _imgui.Indent();

                    _imgui.TextSameLine("Frame: \0"u8);
                    _imgui.Text(involved.EnhancedStacktraceFrameName);
                    
                    _imgui.Unindent();
                }

                _imgui.TreePop();
            }
        }
    }

    private void RenderInvolvedModulesAndPlugins()
    {
        _imgui.Text("From highest probability to lowest:\0"u8);
        _imgui.Indent();
        RenderInvolvedModules();
        RenderInvolvedPlugins();
        _imgui.Unindent();
    }
}