using BUTR.CrashReport.ImGui.Enums;
using BUTR.CrashReport.ImGui.Extensions;
using BUTR.CrashReport.Memory.Utils;
using BUTR.CrashReport.Models;

namespace BUTR.CrashReport.Renderer.ImGui.Renderer;

partial class ImGuiRenderer<TImGuiIORef, TImGuiViewportRef, TImDrawListRef, TImGuiStyleRef, TColorsRangeAccessorRef, TImGuiListClipperRef>
{
    private readonly Dictionary<string, byte[]> _loaderPluginIdUpdateInfoUtf8 = new(StringComparer.Ordinal);
    private readonly Dictionary<string, byte[][]> _loaderPluginIdAdditionalUpdateInfos = new(StringComparer.Ordinal);
    private readonly Dictionary<string, List<Utf8KeyValueList>> _loaderPluginAdditionalDisplayKeyMetadata = new(StringComparer.Ordinal);

    private void InitializeInstalledLoaderPlugins()
    {
        for (var i = 0; i < _crashReport.LoaderPlugins.Count; i++)
        {
            var loaderPlugin = _crashReport.LoaderPlugins[i];
            if (loaderPlugin.UpdateInfo is not null)
            {
                _loaderPluginIdUpdateInfoUtf8[loaderPlugin.Id] = Utf8Utils.ToUtf8Array(loaderPlugin.UpdateInfo.ToString());
            }

            var additionalUpdateInfo = loaderPlugin.AdditionalMetadata.FirstOrDefault(x => x.Key == "AdditionalUpdateInfos")?.Value.Split(';').Select(x => x.Split(':') is { Length: 2 } split
                ? new UpdateInfo
                {
                    Provider = split[0],
                    Value = split[1],
                }
                : null).OfType<UpdateInfo>().ToArray() ?? [];
            _loaderPluginIdAdditionalUpdateInfos[loaderPlugin.Id] = additionalUpdateInfo.Select(x => Utf8Utils.ToUtf8Array(x.ToString())).ToArray();

            InitializeAdditionalMetadata(_loaderPluginAdditionalDisplayKeyMetadata, loaderPlugin.Id, loaderPlugin.AdditionalMetadata);
        }
    }


    private void RenderLoadedLoaderPlugins()
    {
        if (_crashReport.LoaderPlugins.Count == 0) return;

        for (var i = 0; i < _crashReport.LoaderPlugins.Count; i++)
        {
            var loaderPlugin = _crashReport.LoaderPlugins[i];
            var color = _isDarkMode ? DarkPlugin : LightPlugin;
            if (_imgui.BeginChild(loaderPlugin.Id, in Zero2, in color, ImGuiChildFlags.Border | ImGuiChildFlags.AutoResizeY, ImGuiWindowFlags.None))
            {
                if (_imgui.TreeNode(loaderPlugin.Id))
                {
                    _imgui.RenderId("Id:\0"u8, loaderPlugin.Id);

                    _imgui.Text("Name: \0"u8);
                    _imgui.SameLine();
                    _imgui.Text(loaderPlugin.Name);

                    if (!string.IsNullOrEmpty(loaderPlugin.Version))
                    {
                        _imgui.Text("Version: \0"u8);
                        _imgui.SameLine();
                        _imgui.Text(loaderPlugin.Version!);
                    }

                    if (loaderPlugin.UpdateInfo is not null)
                    {
                        _imgui.Text("Update Info: \0"u8);
                        _imgui.SameLine();
                        _imgui.Text(_loaderPluginIdUpdateInfoUtf8[loaderPlugin.Id]);
                    }

                    if (_loaderPluginIdAdditionalUpdateInfos[loaderPlugin.Id].Length > 0)
                    {
                        for (var j = 0; j < _loaderPluginIdAdditionalUpdateInfos[loaderPlugin.Id].Length; j++)
                        {
                            _imgui.Text("Update Info: \0"u8);
                            _imgui.SameLine();
                            _imgui.Text(_loaderPluginIdAdditionalUpdateInfos[loaderPlugin.Id][j]);
                        }
                    }

                    RenderAdditionalMetadata(_loaderPluginAdditionalDisplayKeyMetadata, loaderPlugin.Id);

                    RenderCapabilities(loaderPlugin.Capabilities);

                    _imgui.TreePop();
                }
            }

            _imgui.EndChild();
        }
    }
}