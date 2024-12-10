using BUTR.CrashReport.ImGui.Enums;
using BUTR.CrashReport.ImGui.Extensions;
using BUTR.CrashReport.ImGui.Utils;
using BUTR.CrashReport.Models;

using Cysharp.Text;

namespace BUTR.CrashReport.Renderer.ImGui.Renderer;

partial class ImGuiRenderer
{
    protected class RuntimePatchModelEqualityComparer : IEqualityComparer<RuntimePatchModel>
    {
        public static RuntimePatchModelEqualityComparer Instance { get; } = new();
        public bool Equals(RuntimePatchModel? x, RuntimePatchModel? y) => ReferenceEquals(x, y); // We can just reference compare here
        public int GetHashCode(RuntimePatchModel obj) => obj.GetHashCode();
    }
}

partial class ImGuiRenderer<TImGuiIORef, TImGuiViewportRef, TImDrawListRef, TImGuiStyleRef, TColorsRangeAccessorRef, TImGuiListClipperRef>
{
    private readonly List<string> _runtimePatchTypes = new();
    private readonly Dictionary<RuntimePatchModel, List<Utf8KeyValueList>> _runtimePatchAdditionalDisplayKeyMetadata = new(RuntimePatchModelEqualityComparer.Instance);

    private List<KeyValuePair<string, List<RuntimePatchModel>>> _groupedRuntimePatches = new();

    private static string GetFullName(RuntimePatchesModel patches) => !string.IsNullOrEmpty(patches.OriginalMethodDeclaredTypeName)
        ? ZString.Format("{0}.{1}", patches.OriginalMethodDeclaredTypeName, patches.OriginalMethodName)
        : patches.OriginalMethodName ?? string.Empty;

    private void InitializeRuntimePatches()
    {
        for (var i = 0; i < _crashReport.RuntimePatches.Count; i++)
        {
            var runtimePatch = _crashReport.RuntimePatches[i];

            for (var j = 0; j < runtimePatch.Patches.Count; j++)
            {
                var patch = runtimePatch.Patches[j];
                InitializeAdditionalMetadata(_runtimePatchAdditionalDisplayKeyMetadata, patch, patch.AdditionalMetadata);
            }
        }

        _runtimePatchTypes.AddRange(_crashReport.RuntimePatches.SelectMany(x => x.Patches).Select(x => x.Type).Distinct());

        _groupedRuntimePatches = _crashReport.RuntimePatches
            .GroupBy(GetFullName)
            .Select(x => new KeyValuePair<string, List<RuntimePatchModel>>(x.Key, x.SelectMany(y => y.Patches).ToList()))
            .ToList();
    }

    private void RenderRuntimePatches(string type, ReadOnlySpan<RuntimePatchModel> patches)
    {
        for (var i = 0; i < patches.Length; i++)
        {
            var patch = patches[i];

            if (patch.Type != type) continue;

            if (_imgui.TreeNode(patch.FullName, ImGuiTreeNodeFlags.Bullet | ImGuiTreeNodeFlags.DefaultOpen))
            {
                var moduleId = patch.ModuleId ?? "UNKNOWN";
                var pluginId = patch.LoaderPluginId ?? "UNKNOWN";

                if (moduleId != "UNKNOWN") _imgui.RenderId("Module Id:\0"u8, moduleId);
                if (pluginId != "UNKNOWN") _imgui.RenderId("Plugin Id:\0"u8, pluginId);

                _imgui.Text("Type: \0"u8);
                _imgui.SameLine();
                _imgui.Text(patch.Provider);
                _imgui.SameLine();
                _imgui.Text(" \0"u8);
                _imgui.SameLine();
                _imgui.Text(patch.Type);

                RenderAdditionalMetadata(_runtimePatchAdditionalDisplayKeyMetadata, patch);

                _imgui.TreePop();
            }
        }
    }

    private void RenderRuntimePatches()
    {
        var groupedRuntimePatches = _groupedRuntimePatches.AsSpan();
        var runtimePatchTypes = _runtimePatchTypes.AsSpan();

        for (var i = 0; i < groupedRuntimePatches.Length; i++)
        {
            var (methodNameFull, value) = groupedRuntimePatches[i];
            var patches = value.AsSpan();

            if (_imgui.TreeNode(methodNameFull, ImGuiTreeNodeFlags.DefaultOpen))
            {
                for (var j = 0; j < runtimePatchTypes.Length; j++)
                {
                    _imgui.PushId(j);
                    RenderRuntimePatches(runtimePatchTypes[j], patches);
                    _imgui.PopId();
                }
                _imgui.NewLine();

                _imgui.TreePop();
            }
        }
    }
}