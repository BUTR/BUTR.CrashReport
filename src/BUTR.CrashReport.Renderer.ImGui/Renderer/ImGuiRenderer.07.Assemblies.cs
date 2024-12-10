using BUTR.CrashReport.ImGui.Enums;
using BUTR.CrashReport.ImGui.Extensions;
using BUTR.CrashReport.Memory;
using BUTR.CrashReport.Memory.Utils;
using BUTR.CrashReport.Models;
using BUTR.CrashReport.Renderer.ImGui.Extensions;

namespace BUTR.CrashReport.Renderer.ImGui.Renderer;

partial class ImGuiRenderer
{
    protected class AssemblyModelEqualityComparer : IEqualityComparer<AssemblyModel>
    {
        public static AssemblyModelEqualityComparer Instance { get; } = new();
        public bool Equals(AssemblyModel? x, AssemblyModel? y) => ReferenceEquals(x, y); // We can just reference compare here
        public int GetHashCode(AssemblyModel obj) => obj.GetHashCode();
    }

    // ReSharper disable once HeapView.ObjectAllocation
    protected static readonly LiteralSpan<byte>[] _architectureTypeNames =
    [
        "Unknown\0"u8, // Unknown
        "MSIL\0"u8,    // MSIL
        "x86\0"u8,     // X86
        "IA64\0"u8,    // IA64
        "x64\0"u8,     // Amd64
        "Arm\0"u8,     // Arm
    ];
}

partial class ImGuiRenderer<TImGuiIORef, TImGuiViewportRef, TImDrawListRef, TImGuiStyleRef, TColorsRangeAccessorRef, TImGuiListClipperRef>
{
    private readonly Dictionary<AssemblyModel, byte[]> _assemblyFullNameUtf8 = new(AssemblyModelEqualityComparer.Instance);
    private readonly Dictionary<AssemblyModel, List<Utf8KeyValueList>> _assemblyAdditionalDisplayKeyMetadata = new(AssemblyModelEqualityComparer.Instance);

    private bool _hideSystemAssemblies;
    private bool _hideGACAssemblies;
    private bool _hideGameAssemblies;
    private bool _hideGameModulesAssemblies;
    private bool _hideModulesAssemblies;
    private bool _hideLoaderAssemblies;
    private bool _hideLoaderPluginsAssemblies;
    private bool _hideDynamicAssemblies;
    private bool _hideUnclassifiedAssemblies;

    private bool _hasSystemAssemblies;
    private bool _hasGACAssemblies;
    private bool _hasGameAssemblies;
    private bool _hasGameModulesAssemblies;
    private bool _hasModulesAssemblies;
    private bool _hasLoaderAssemblies;
    private bool _hasLoaderPluginsAssemblies;
    private bool _hasDynamicAssemblies;
    private bool _hasUnclassifiedAssemblies;

    private void InitializeAssemblies()
    {
        for (var i = 0; i < _crashReport.Assemblies.Count; i++)
        {
            var assembly = _crashReport.Assemblies[i];
            _assemblyFullNameUtf8[assembly] = Utf8Utils.ToUtf8Array(assembly.GetFullName());

            InitializeAdditionalMetadata(_assemblyAdditionalDisplayKeyMetadata, assembly, assembly.AdditionalMetadata);
        }

        _hasSystemAssemblies = _crashReport.Assemblies.Any(x => x.Type.IsSet(AssemblyType.System));
        _hasGACAssemblies = _crashReport.Assemblies.Any(x => x.Type.IsSet(AssemblyType.GAC));
        _hasGameAssemblies = _crashReport.Assemblies.Any(x => x.Type.IsSet(AssemblyType.GameCore));
        _hasGameModulesAssemblies = _crashReport.Assemblies.Any(x => x.Type.IsSet(AssemblyType.GameModule));
        _hasModulesAssemblies = _crashReport.Assemblies.Any(x => x.Type.IsSet(AssemblyType.Module));
        _hasLoaderAssemblies = _crashReport.Assemblies.Any(x => x.Type.IsSet(AssemblyType.Loader));
        _hasLoaderPluginsAssemblies = _crashReport.Assemblies.Any(x => x.Type.IsSet(AssemblyType.LoaderPlugin));
        _hasDynamicAssemblies = _crashReport.Assemblies.Any(x => x.Type.IsSet(AssemblyType.Dynamic));
        _hasUnclassifiedAssemblies = _crashReport.Assemblies.Any(x => x.Type == AssemblyType.Unclassified);
    }

    private void RenderAssembliesStep(AssemblyModel assembly)
    {
        if (_hideSystemAssemblies && assembly.Type.IsSet(AssemblyType.System)) return;
        if (_hideGACAssemblies && assembly.Type.IsSet(AssemblyType.GAC)) return;
        if (_hideGameAssemblies && assembly.Type.IsSet(AssemblyType.GameCore)) return;
        if (_hideGameModulesAssemblies && assembly.Type.IsSet(AssemblyType.GameModule)) return;
        if (_hideModulesAssemblies && assembly.Type.IsSet(AssemblyType.Module)) return;
        if (_hideLoaderAssemblies && assembly.Type.IsSet(AssemblyType.Loader)) return;
        if (_hideLoaderPluginsAssemblies && assembly.Type.IsSet(AssemblyType.LoaderPlugin)) return;
        if (_hideDynamicAssemblies && assembly.Type.IsSet(AssemblyType.Dynamic)) return;
        if (_hideUnclassifiedAssemblies && assembly.Type == AssemblyType.Unclassified) return;

        if (_imgui.TreeNode(assembly.Id.Name, ImGuiTreeNodeFlags.Bullet | ImGuiTreeNodeFlags.DefaultOpen))
        {
            var isDynamic = assembly.Type.IsSet(AssemblyType.Dynamic);
            var hasPath = assembly.AnonymizedPath != "EMPTY" && assembly.AnonymizedPath != "DYNAMIC" && !string.IsNullOrWhiteSpace(assembly.AnonymizedPath);

            _imgui.SameLine();
            _imgui.Text(", \0"u8);
            _imgui.SameLine();
            _imgui.Text(assembly.Id.Version ?? string.Empty);
            _imgui.SameLine();
            _imgui.Text(", \0"u8);
            _imgui.SameLine();
            _imgui.Text(_architectureTypeNames[(int) assembly.Architecture]);
            _imgui.SameLine();
            if (!isDynamic)
            {
                _imgui.Text(", \0"u8);
                _imgui.SameLine();
                _imgui.Text(assembly.Hash);
                _imgui.SameLine();
            }
            if (hasPath)
            {
                _imgui.Text(", \0"u8);
                _imgui.SameLine();
                _imgui.SmallButtonRound(assembly.AnonymizedPath);
            }
            else
            {
                _imgui.Text(isDynamic ? ", DYNAMIC\0"u8 : ", EMPTY\0"u8);
            }

            _imgui.SameLine();

            RenderAdditionalMetadataSameLine(_assemblyAdditionalDisplayKeyMetadata, assembly);

            _imgui.NewLine();

            _imgui.TreePop();
        }
    }

    private void RenderAssembliesWithLoop()
    {
        for (var i = 0; i < _crashReport.Assemblies.Count; i++)
        {
            RenderAssembliesStep(_crashReport.Assemblies[i]);
        }
    }

    private void RenderAssembliesWithClipper()
    {
        _imGuiWithImGuiListClipper.CreateImGuiListClipper(out var clipper);
        using var _ = clipper;

        clipper.Begin(_crashReport.Assemblies.Count, _imgui.GetTextLineHeightWithSpacing());

        while (clipper.Step())
        {
            for (var i = clipper.DisplayStart; i < clipper.DisplayEnd; ++i)
            {
                RenderAssembliesStep(_crashReport.Assemblies[i]);
            }
        }

        clipper.End();
    }

    private void RenderAssemblies()
    {
        _imgui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 1);
        _imgui.Text("Hide: \0"u8);
        _imgui.SameLine();
        if (_hasSystemAssemblies) { _imgui.CheckboxRound(" System | \0"u8, ref _hideSystemAssemblies); _imgui.SameLine(); }
        if (_hasGACAssemblies) { _imgui.CheckboxRound(" GAC | \0"u8, ref _hideGACAssemblies); _imgui.SameLine(); }
        if (_hasGameAssemblies) { _imgui.CheckboxRound(" Game | \0"u8, ref _hideGameAssemblies); _imgui.SameLine(); }
        if (_hasGameModulesAssemblies) { _imgui.CheckboxRound(" Game Modules | \0"u8, ref _hideGameModulesAssemblies); _imgui.SameLine(); }
        if (_hasModulesAssemblies) { _imgui.CheckboxRound(" Modules | \0"u8, ref _hideModulesAssemblies); _imgui.SameLine(); }
        if (_hasLoaderAssemblies) { _imgui.CheckboxRound(" Loader | \0"u8, ref _hideLoaderAssemblies); _imgui.SameLine(); }
        if (_hasLoaderPluginsAssemblies) { _imgui.CheckboxRound(" Loader Plugins | \0"u8, ref _hideLoaderPluginsAssemblies); _imgui.SameLine(); }
        if (_hasDynamicAssemblies) { _imgui.CheckboxRound(" Dynamic | \0"u8, ref _hideDynamicAssemblies); _imgui.SameLine(); }
        if (_hasUnclassifiedAssemblies) { _imgui.CheckboxRound(" Unclassified\0"u8, ref _hideUnclassifiedAssemblies); }
        _imgui.PopStyleVar();

        var hasFilters = _hideSystemAssemblies ||
                         _hideGACAssemblies ||
                         _hideGameAssemblies ||
                         _hideGameModulesAssemblies ||
                         _hideModulesAssemblies ||
                         _hideLoaderAssemblies ||
                         _hideLoaderPluginsAssemblies ||
                         _hideDynamicAssemblies ||
                         _hideUnclassifiedAssemblies;

        if (hasFilters)
        {
            RenderAssembliesWithLoop();
        }
        else
        {
            RenderAssembliesWithClipper();
        }
    }
}