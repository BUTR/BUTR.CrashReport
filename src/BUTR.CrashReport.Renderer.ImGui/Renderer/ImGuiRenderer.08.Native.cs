using BUTR.CrashReport.ImGui.Enums;
using BUTR.CrashReport.ImGui.Extensions;
using BUTR.CrashReport.ImGui.Utils;
using BUTR.CrashReport.Memory;
using BUTR.CrashReport.Models;

namespace BUTR.CrashReport.Renderer.ImGui.Renderer;

partial class ImGuiRenderer
{
    protected class NativeAssemblyModelEqualityComparer : IEqualityComparer<NativeAssemblyModel>
    {
        public static NativeAssemblyModelEqualityComparer Instance { get; } = new();
        public bool Equals(NativeAssemblyModel? x, NativeAssemblyModel? y) => ReferenceEquals(x, y); // We can just reference compare here
        public int GetHashCode(NativeAssemblyModel obj) => obj.GetHashCode();
    }

    // ReSharper disable once HeapView.ObjectAllocation
    protected static readonly LiteralSpan<byte>[] _nativeArchitectureTypeNames =
    [
        "Unknown\0"u8, // Unknown
        "x86\0"u8,     // x86
        "x64\0"u8,     // x86_64
        "Arm\0"u8,     // Arm
        "Arm64\0"u8,   // Arm64
    ];
}

partial class ImGuiRenderer<TImGuiIORef, TImGuiViewportRef, TImDrawListRef, TImGuiStyleRef, TColorsRangeAccessorRef, TImGuiListClipperRef>
{
    private readonly Dictionary<NativeAssemblyModel, List<Utf8KeyValueList>> _nativeAdditionalDisplayKeyMetadata = new(NativeAssemblyModelEqualityComparer.Instance);

    private void InitializeNatives()
    {
        for (var i = 0; i < _crashReport.NativeModules.Count; i++)
        {
            var assembly = _crashReport.NativeModules[i];
            InitializeAdditionalMetadata(_nativeAdditionalDisplayKeyMetadata, assembly, assembly.AdditionalMetadata);
        }
    }

    private void RenderNatives()
    {
        _imGuiWithImGuiListClipper.CreateImGuiListClipper(out var clipper);
        using var _ = clipper;

        clipper.Begin(_crashReport.NativeModules.Count, _imgui.GetTextLineHeightWithSpacing());

        while (clipper.Step())
        {
            for (var i = clipper.DisplayStart; i < clipper.DisplayEnd; ++i)
            {
                var assembly = _crashReport.NativeModules[i];

                if (_imgui.TreeNode(assembly.Id.Name, ImGuiTreeNodeFlags.Bullet | ImGuiTreeNodeFlags.DefaultOpen))
                {
                    _imgui.SameLine();
                    _imgui.Text(", \0"u8);
                    _imgui.SameLine();
                    _imgui.Text(assembly.Id.Version ?? string.Empty);
                    _imgui.SameLine();
                    _imgui.Text(", \0"u8);
                    _imgui.SameLine();
                    _imgui.Text(_nativeArchitectureTypeNames[(int) assembly.Architecture]);
                    _imgui.SameLine();
                    _imgui.Text(", \0"u8);
                    _imgui.SameLine();
                    _imgui.Text(assembly.Hash);
                    _imgui.SameLine();
                    _imgui.Text(", \0"u8);
                    _imgui.SameLine();
                    _imgui.SmallButtonRound(assembly.AnonymizedPath);
                    _imgui.SameLine();

                    RenderAdditionalMetadataSameLine(_nativeAdditionalDisplayKeyMetadata, assembly);

                    _imgui.NewLine();

                    _imgui.TreePop();
                }
            }
        }

        clipper.End();
    }
}