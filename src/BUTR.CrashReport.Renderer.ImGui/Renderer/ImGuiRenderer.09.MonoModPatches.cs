/*
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

namespace BUTR.CrashReport.Renderer.ImGui.Renderer;

partial class ImGuiRenderer
{
    private class MonoModPatchesModelEqualityComparer : IEqualityComparer<MonoModPatchesModel>
    {
        public static MonoModPatchesModelEqualityComparer Instance { get; } = new();
        public bool Equals(MonoModPatchesModel? x, MonoModPatchesModel? y) => ReferenceEquals(x, y); // We can just reference compare here
        public int GetHashCode(MonoModPatchesModel obj) => obj.GetHashCode();
    }

    private class MonoModPatchModelEqualityComparer : IEqualityComparer<MonoModPatchModel>
    {
        public static MonoModPatchModelEqualityComparer Instance { get; } = new();
        public bool Equals(MonoModPatchModel? x, MonoModPatchModel? y) => ReferenceEquals(x, y); // We can just reference compare here
        public int GetHashCode(MonoModPatchModel obj) => obj.GetHashCode();
    }

    private readonly Dictionary<MonoModPatchesModel, byte[]> _monoModMethodNameFullUtf8 = new(MonoModPatchesModelEqualityComparer.Instance);
    private readonly Dictionary<MonoModPatchModel, byte[]> _monoModBeforeUtf8 = new(MonoModPatchModelEqualityComparer.Instance);
    private readonly Dictionary<MonoModPatchModel, byte[]> _monoModAfterUtf8 = new(MonoModPatchModelEqualityComparer.Instance);

    private void InitializeMonoModPatches()
    {
        for (var i = 0; i < _crashReport.MonoModPatches.Count; i++)
        {
            var monoModPatch = _crashReport.MonoModPatches[i];
            var methodNameFull = !string.IsNullOrEmpty(monoModPatch.OriginalMethodDeclaredTypeName)
                ? $"{monoModPatch.OriginalMethodDeclaredTypeName}.{monoModPatch.OriginalMethodName}"
                : monoModPatch.OriginalMethodName ?? string.Empty;
            _monoModMethodNameFullUtf8[monoModPatch] = UnsafeHelper.ToUtf8Array(methodNameFull);

            for (var j = 0; j < monoModPatch.Detours.Count; j++)
            {
                var detour = monoModPatch.Detours[j];
                for (var k = 0; k < detour.Before.Count; k++)
                {
                    var before = detour.Before[k];
                    _monoModBeforeUtf8[detour] = UnsafeHelper.ToUtf8Array(before);
                }
                for (var m = 0; m < detour.After.Count; m++)
                {
                    var after = detour.After[m];
                    _monoModAfterUtf8[detour] = UnsafeHelper.ToUtf8Array(after);
                }
            }
        }
    }

    private void RenderMonoModPatches(ReadOnlySpan<byte> name, RefLinqEnumerable<MonoModPatchModel, Where<MonoModPatchModel, PureValueDelegate<MonoModPatchModel, bool>, IListEnumerator<MonoModPatchModel>>> patches)
    {
        foreach (var patch in patches)
        {
            var moduleId = patch.ModuleId ?? "UNKNOWN";
            var pluginId = patch.LoaderPluginId ?? "UNKNOWN";

            _imgui.Bullet();
            _imgui.Text(name);
            _imgui.Indent();

            if (moduleId != "UNKNOWN") { _imgui.RenderId("Module Id:\0"u8, moduleId); _imgui.SameLine(0, 0); }
            if (pluginId != "UNKNOWN") { _imgui.RenderId("Plugin Id:\0"u8, pluginId); _imgui.SameLine(0, 0); }
            _imgui.TextSameLine(" Id: \0"u8);
            _imgui.TextSameLine(patch.Id);
            _imgui.TextSameLine(" Namespace: \0"u8);
            _imgui.TextSameLine(patch.Namespace);
            _imgui.TextSameLine(" IsActive: \0"u8);
            _imgui.TextSameLine(patch.IsActive);
            if (patch.Index is not null) { _imgui.TextSameLine(" Index: \0"u8); _imgui.TextSameLine(patch.Index.Value); }
            if (patch.MaxIndex is not null) { _imgui.TextSameLine(" MaxIndex: \0"u8); _imgui.TextSameLine(patch.MaxIndex.Value); }
            if (patch.GlobalIndex is not null) { _imgui.TextSameLine(" GlobalIndex: \0"u8); _imgui.TextSameLine(patch.GlobalIndex.Value); }
            if (patch.Priority is not null) { _imgui.TextSameLine(" Priority: \0"u8); _imgui.TextSameLine(patch.Priority.Value); }
            if (patch.SubPriority is not null) { _imgui.TextSameLine(" SubPriority: \0"u8); _imgui.TextSameLine(patch.SubPriority.Value); }
            if (patch.Before.Count > 0) { _imgui.TextSameLine(" Before: \0"u8); _imgui.TextSameLine(_monoModBeforeUtf8[patch]); }
            if (patch.After.Count > 0) { _imgui.TextSameLine(" After: \0"u8); _imgui.TextSameLine(_monoModAfterUtf8[patch]); }
            _imgui.NewLine();

            _imgui.Unindent();
        }
    }

    private void RenderMonoModPatches()
    {
        for (var i = 0; i < _crashReport.MonoModPatches.Count; i++)
        {
            var monoModPatch = _crashReport.MonoModPatches[i];
            var methodNameFull = _monoModMethodNameFullUtf8[monoModPatch];

            if (_imgui.TreeNode(methodNameFull, ImGuiTreeNodeFlags.DefaultOpen))
            {
                RenderMonoModPatches("Detours\0"u8, monoModPatch.Detours.ToRefLinq().Where(x => x.Type == MonoModPatchModelType.Detour));
                RenderMonoModPatches("ILHooks\0"u8, monoModPatch.Detours.ToRefLinq().Where(x => x.Type == MonoModPatchModelType.ILHook));
                _imgui.NewLine();

                _imgui.TreePop();
            }
        }
    }
}
*/