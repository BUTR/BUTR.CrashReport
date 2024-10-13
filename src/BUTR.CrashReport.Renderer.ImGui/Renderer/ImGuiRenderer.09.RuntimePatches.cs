using BUTR.CrashReport.Models;
using BUTR.CrashReport.Renderer.ImGui.Extensions;
using BUTR.CrashReport.Renderer.ImGui.UnsafeUtils;
using BUTR.CrashReport.Renderer.ImGui.Utils;

using HonkPerf.NET.Core;
using HonkPerf.NET.RefLinq;
using HonkPerf.NET.RefLinq.Enumerators;

using ImGuiNET;

using System;
using System.Collections.Generic;
using System.Linq;

namespace BUTR.CrashReport.Renderer.ImGui.Renderer;

partial class ImGuiRenderer
{
    private class RuntimePatchesModelEqualityComparer : IEqualityComparer<RuntimePatchesModel>
    {
        public static RuntimePatchesModelEqualityComparer Instance { get; } = new();
        public bool Equals(RuntimePatchesModel? x, RuntimePatchesModel? y) => ReferenceEquals(x, y); // We can just reference compare here
        public int GetHashCode(RuntimePatchesModel obj) => obj.GetHashCode();
    }

    private class RuntimePatchModelEqualityComparer : IEqualityComparer<RuntimePatchModel>
    {
        public static RuntimePatchModelEqualityComparer Instance { get; } = new();
        public bool Equals(RuntimePatchModel? x, RuntimePatchModel? y) => ReferenceEquals(x, y); // We can just reference compare here
        public int GetHashCode(RuntimePatchModel obj) => obj.GetHashCode();
    }

    private readonly List<string> _runtimePatchProviders = new();
    private readonly List<string> _runtimePatchTypes = new();
    private readonly Dictionary<RuntimePatchesModel, byte[]> _runtimeMethodNameFullUtf8 = new(RuntimePatchesModelEqualityComparer.Instance);

    private void InitializeRuntimePatches()
    {
        for (var i = 0; i < _crashReport.RuntimePatches.Count; i++)
        {
            var runtimePatch = _crashReport.RuntimePatches[i];
            var methodNameFull = !string.IsNullOrEmpty(runtimePatch.OriginalMethodDeclaredTypeName)
                ? $"{runtimePatch.OriginalMethodDeclaredTypeName}.{runtimePatch.OriginalMethodName}"
                : runtimePatch.OriginalMethodName ?? string.Empty;
            _runtimeMethodNameFullUtf8[runtimePatch] = UnsafeHelper.ToUtf8Array(methodNameFull);
        }
        
        _runtimePatchProviders.AddRange(_crashReport.RuntimePatches.SelectMany(x => x.Patches).Select(x => x.Provider).Distinct());
        
        _runtimePatchTypes.AddRange(_crashReport.RuntimePatches.SelectMany(x => x.Patches).Select(x => x.Type).Distinct());
    }

    private void RenderRuntimePatches(RefLinqEnumerable<RuntimePatchModel, Where<RuntimePatchModel, PureValueDelegate<RuntimePatchModel, bool>, IListEnumerator<RuntimePatchModel>>> patches)
    {
        foreach (var patch in patches)
        {
            var moduleId = patch.ModuleId ?? "UNKNOWN";
            var pluginId = patch.LoaderPluginId ?? "UNKNOWN";

            _imgui.Bullet();
            _imgui.TextSameLine(patch.Provider);
            _imgui.TextSameLine(" \0"u8);
            _imgui.TextSameLine(patch.Type);
            _imgui.NewLine();
            _imgui.Indent();

            if (moduleId != "UNKNOWN") { _imgui.RenderId("Module Id:\0"u8, moduleId); _imgui.SameLine(0, 0); }
            if (pluginId != "UNKNOWN") { _imgui.RenderId("Plugin Id:\0"u8, pluginId); _imgui.SameLine(0, 0); }
            
            _imgui.TextSameLine(" Full Name: \0"u8);
            _imgui.TextSameLine(patch.FullName);
            
            for (var i = 0; i < patch.AdditionalMetadata.Count; i++)
            {
                var metadata = patch.AdditionalMetadata[i];
                if (string.IsNullOrEmpty(metadata.Key) || string.IsNullOrEmpty(metadata.Value)) continue;
                _imgui.TextSameLine(" \0"u8);
                _imgui.TextSameLine(metadata.Key);
                _imgui.TextSameLine(": \0"u8);
                _imgui.TextSameLine(metadata.Value);
            }
            _imgui.NewLine();

            _imgui.Unindent();
        }
    }

    private void RenderRuntimePatches()
    {
        for (var i = 0; i < _crashReport.RuntimePatches.Count; i++)
        {
            var runtimePatch = _crashReport.RuntimePatches[i];
            var methodNameFull = _runtimeMethodNameFullUtf8[runtimePatch];

            if (_imgui.TreeNode(methodNameFull, ImGuiTreeNodeFlags.DefaultOpen))
            {
                for (var j = 0; j < _runtimePatchTypes.Count; j++)
                {
                    var type = _runtimePatchTypes[j];
                    RenderRuntimePatches(runtimePatch.Patches.ToRefLinq().Where(x => x.Type == type));
                }
                _imgui.NewLine();

                _imgui.TreePop();
            }
        }
    }
}