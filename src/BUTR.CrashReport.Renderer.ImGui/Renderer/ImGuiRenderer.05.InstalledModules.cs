using BUTR.CrashReport.ImGui.Enums;
using BUTR.CrashReport.ImGui.Extensions;
using BUTR.CrashReport.Memory;
using BUTR.CrashReport.Memory.Utils;
using BUTR.CrashReport.Models;

using Cysharp.Text;

using Utf8StringInterpolation;

namespace BUTR.CrashReport.Renderer.ImGui.Renderer;

partial class ImGuiRenderer
{
    protected class DependencyMetadataModelEqualityComparer : IEqualityComparer<DependencyMetadataModel>
    {
        public static DependencyMetadataModelEqualityComparer Instance { get; } = new();
        public bool Equals(DependencyMetadataModel? x, DependencyMetadataModel? y) => ReferenceEquals(x, y); // We can just reference compare here
        public int GetHashCode(DependencyMetadataModel obj) => obj.GetHashCode();
    }
    protected class ModuleSubModuleModelEqualityComparer : IEqualityComparer<ModuleSubModuleModel>
    {
        public static ModuleSubModuleModelEqualityComparer Instance { get; } = new();
        public bool Equals(ModuleSubModuleModel? x, ModuleSubModuleModel? y) => ReferenceEquals(x, y); // We can just reference compare here
        public int GetHashCode(ModuleSubModuleModel obj) => obj.GetHashCode();
    }

    // ReSharper disable once HeapView.ObjectAllocation
    protected static readonly LiteralSpan<byte>[] _dependencyTypeNames =
    [
        LiteralSpan<byte>.Empty,
        "Load Before  \0"u8, // LoadBefore
        "Load After   \0"u8, // LoadAfter
        "Incompatible \0"u8, // Incompatible
    ];

    protected static readonly char[] _colonChar = [':'];
}

partial class ImGuiRenderer<TImGuiIORef, TImGuiViewportRef, TImDrawListRef, TImGuiStyleRef, TColorsRangeAccessorRef, TImGuiListClipperRef>
{
    private readonly Dictionary<string, byte[]> _moduleIdUpdateInfoUtf8 = new(StringComparer.Ordinal);
    private readonly Dictionary<string, Dictionary<string, byte[]>> _moduleDependencyTextUtf8 = new(StringComparer.Ordinal);
    private readonly Dictionary<string, byte[][]> _moduleAdditionalUpdateInfos = new(StringComparer.Ordinal);
    private readonly Dictionary<string, List<Utf8KeyValueList>> _moduleAdditionalDisplayKeyMetadata = new(StringComparer.Ordinal);
    private readonly Dictionary<DependencyMetadataModel, List<Utf8KeyValueList>> _dependencyAdditionalDisplayKeyMetadata = new(DependencyMetadataModelEqualityComparer.Instance);
    private readonly Dictionary<ModuleSubModuleModel, List<Utf8KeyValueList>> _subModuleAdditionalDisplayKeyMetadata = new(ModuleSubModuleModelEqualityComparer.Instance);

    private void InitializeInstalledModules()
    {
        for (var i = 0; i < _crashReport.Modules.Count; i++)
        {
            var module = _crashReport.Modules[i];

            if (module.UpdateInfo is not null)
            {
                _moduleIdUpdateInfoUtf8[module.Id] = Utf8Utils.ToUtf8Array(module.UpdateInfo.ToString());
            }

            var additionalUpdateInfo = module.AdditionalMetadata.FirstOrDefault(x => x.Key == "AdditionalUpdateInfos")?.Value.Split(_colonChar).Select(x => x.Split(_colonChar) is { Length: 2 } split
                ? new UpdateInfo
                {
                    Provider = split[0],
                    Value = split[1],
                }
                : null).OfType<UpdateInfo>().ToArray() ?? [];
            _moduleAdditionalUpdateInfos[module.Id] = additionalUpdateInfo.Select(x => Utf8Utils.ToUtf8Array(x.ToString())).ToArray();

            for (var j = 0; j < module.DependencyMetadatas.Count; j++)
            {
                var dependentModule = module.DependencyMetadatas[j];

                var finalBuilder = ZString.CreateUtf8StringBuilder();
                finalBuilder.AppendLiteral(dependentModule.IsOptional ? " (optional)"u8 : []);
                finalBuilder.AppendLiteral(!string.IsNullOrEmpty(dependentModule.Version) ? Utf8String.Format($" >= {dependentModule.Version!}") : []);
                finalBuilder.AppendLiteral(!string.IsNullOrEmpty(dependentModule.VersionRange) ? Utf8String.Format($" {dependentModule.VersionRange!}") : []);
                finalBuilder.AppendLiteral("\0"u8);
                SetNestedDictionary(_moduleDependencyTextUtf8, module.Id, dependentModule.ModuleOrPluginId, finalBuilder.AsSpan().ToArray());

                InitializeAdditionalMetadata(_dependencyAdditionalDisplayKeyMetadata, dependentModule, dependentModule.AdditionalMetadata);
            }

            for (var j = 0; j < module.SubModules.Count; j++)
            {
                var subModule = module.SubModules[j];
                InitializeAdditionalMetadata(_subModuleAdditionalDisplayKeyMetadata, subModule, subModule.AdditionalMetadata);
            }

            InitializeAdditionalMetadata(_moduleAdditionalDisplayKeyMetadata, module.Id, module.AdditionalMetadata);
        }
    }

    private void RenderDependencies(ModuleModel module)
    {
        if (module.DependencyMetadatas.Count == 0) return;

        _imgui.Text("Dependencies:\0"u8);
        for (var i = 0; i < module.DependencyMetadatas.Count; i++)
        {
            _imgui.PushId(i);

            var dependentModule = module.DependencyMetadatas[i];
            var type = Clamp(dependentModule.Type, DependencyMetadataType.LoadBefore, DependencyMetadataType.Incompatible);
            _imgui.Bullet();
            _imgui.Text(_dependencyTypeNames[type]);
            _imgui.SameLine();
            _imgui.SmallButtonRound(dependentModule.ModuleOrPluginId);
            _imgui.SameLine();
            _imgui.Text(_moduleDependencyTextUtf8[module.Id][dependentModule.ModuleOrPluginId]);

            RenderAdditionalMetadata(_dependencyAdditionalDisplayKeyMetadata, dependentModule);

            _imgui.PopId();
        }
    }

    private void RenderCapabilities(IList<CapabilityModel> moduleCapabilities)
    {
        if (moduleCapabilities.Count == 0) return;

        _imgui.Text("Capabilities:\0"u8);
        for (var i = 0; i < moduleCapabilities.Count; i++)
        {
            var capability = moduleCapabilities[i];
            _imgui.Bullet();
            _imgui.Text(capability.Name);
        }
    }

    private void RenderSubModules(IList<ModuleSubModuleModel> moduleSubModules)
    {
        if (moduleSubModules.Count == 0) return;

        _imgui.Text("SubModules:\0"u8);
        for (var i = 0; i < moduleSubModules.Count; i++)
        {
            var subModule = moduleSubModules[i];
            _imgui.Bullet();
            var color = _isDarkMode ? DarkSubModule : LightSubModule;
            if (_imgui.BeginChild(subModule.Name, in Zero2, in color, ImGuiChildFlags.Border | ImGuiChildFlags.AutoResizeY, ImGuiWindowFlags.None))
            {
                _imgui.Text("Name: \0"u8);
                _imgui.SameLine();
                _imgui.Text(subModule.Name);
                _imgui.Text("Assembly Entrypoint: \0"u8);
                _imgui.SameLine();
                _imgui.Text(subModule.AssemblyId?.Name ?? string.Empty);
                _imgui.Text("Entrypoint: \0"u8);
                _imgui.SameLine();
                _imgui.Text(subModule.Entrypoint);

                RenderAdditionalMetadata(_subModuleAdditionalDisplayKeyMetadata, subModule);
            }

            _imgui.EndChild();
        }
    }

    private void RenderAdditionalAssemblies(string moduleId)
    {
        // TODO: Potentially a huge iteration count
        var first = true;
        for (var i = 0; i < _crashReport.Assemblies.Count; i++)
        {
            var assembly = _crashReport.Assemblies[i];
            if (assembly.ModuleId != moduleId) continue;

            if (first)
            {
                first = false;
                _imgui.Text("Assemblies Present:\0"u8);
            }

            _imgui.Bullet();
            _imgui.Text(assembly.Id.Name);
            _imgui.SameLine();
            _imgui.Text(" (\0"u8);
            _imgui.SameLine();
            _imgui.Text(_assemblyFullNameUtf8[assembly]);
            _imgui.SameLine();
            _imgui.Text(")\0"u8);
        }
    }

    private void RenderInstalledModules()
    {
        for (var i = 0; i < _crashReport.Modules.Count; i++)
        {
            var module = _crashReport.Modules[i];

            var color = module switch
            {
                { IsOfficial: true } => _isDarkMode ? DarkOfficialModule : LightOfficialModule,
                { IsExternal: true } => _isDarkMode ? DarkExternalModule : LightExternalModule,
                _ => _isDarkMode ? DarkUnofficialModule : LightUnofficialModule,
            };

            if (_imgui.BeginChild(module.Id, in Zero2, in color, ImGuiChildFlags.Border | ImGuiChildFlags.AutoResizeY, ImGuiWindowFlags.None))
            {
                if (_imgui.TreeNode(module.Id))
                {
                    _imgui.RenderId("Id:\0"u8, module.Id);
                    _imgui.Text("Name: \0"u8);
                    _imgui.SameLine();
                    _imgui.Text(module.Name);
                    _imgui.Text("Version: \0"u8);
                    _imgui.SameLine();
                    _imgui.Text(module.Version);
                    _imgui.Text("External: \0"u8);
                    _imgui.SameLine();
                    _imgui.Text(module.IsExternal);
                    _imgui.Text("Official: \0"u8);
                    _imgui.SameLine();
                    _imgui.Text(module.IsOfficial);
                    _imgui.Text("Singleplayer: \0"u8);
                    _imgui.SameLine();
                    _imgui.Text(module.IsSingleplayer);
                    _imgui.Text("Multiplayer: \0"u8);
                    _imgui.SameLine();
                    _imgui.Text(module.IsMultiplayer);

                    if (module.Url is not null)
                    {
                        _imgui.Text("Url: \0"u8);
                        _imgui.SameLine();
                        _imgui.TextLinkOpenURL(module.Url, module.Url);
                    }

                    if (module.UpdateInfo is not null)
                    {
                        _imgui.Text("Update Info: \0"u8);
                        _imgui.SameLine();
                        _imgui.Text(_moduleIdUpdateInfoUtf8[module.Id]);
                    }

                    if (_moduleAdditionalUpdateInfos[module.Id].Length > 0)
                    {
                        for (var j = 0; j < _moduleAdditionalUpdateInfos[module.Id].Length; j++)
                        {
                            _imgui.Text("Update Info: \0"u8);
                            _imgui.SameLine();
                            _imgui.Text(_moduleAdditionalUpdateInfos[module.Id][j]);
                        }
                    }

                    RenderAdditionalMetadata(_moduleAdditionalDisplayKeyMetadata, module.Id);

                    RenderDependencies(module);

                    RenderCapabilities(module.Capabilities);

                    RenderSubModules(module.SubModules);

                    RenderAdditionalAssemblies(module.Id);

                    _imgui.TreePop();
                }
            }

            _imgui.EndChild();
        }
    }
}