using BUTR.CrashReport.Models;
using BUTR.CrashReport.Renderer.ImGui.UnsafeUtils;

using ImGuiNET;

using System;
using System.Collections.Generic;
using System.Linq;

namespace BUTR.CrashReport.Renderer.ImGui.Renderer;

partial class ImGuiRenderer
{
    private readonly Dictionary<string, byte[]> _loaderPluginIdUpdateInfoUtf8 = new(StringComparer.Ordinal);
    private readonly Dictionary<string, byte[][]> _loaderPluginIdAdditionalUpdateInfos = new(StringComparer.Ordinal);

    private void InitializeInstalledLoaderPlugins()
    {
        for (var i = 0; i < _crashReport.LoaderPlugins.Count; i++)
        {
            var loaderPlugin = _crashReport.LoaderPlugins[i];
            if (loaderPlugin.UpdateInfo is not null)
            {
                _loaderPluginIdUpdateInfoUtf8[loaderPlugin.Id] = UnsafeHelper.ToUtf8Array(loaderPlugin.UpdateInfo.ToString());
            }

            var additionalUpdateInfo = loaderPlugin.AdditionalMetadata.FirstOrDefault(x => x.Key == "AdditionalUpdateInfos")?.Value.Split(';').Select(x => x.Split(':') is { Length: 2 } split
                ? new UpdateInfoModuleOrLoaderPlugin
                {
                    Provider = split[0],
                    Value = split[1],
                }
                : null).OfType<UpdateInfoModuleOrLoaderPlugin>().ToArray() ?? [];
            _loaderPluginIdAdditionalUpdateInfos[loaderPlugin.Id] = additionalUpdateInfo.Select(x => UnsafeHelper.ToUtf8Array(x.ToString())).ToArray();
        }
    }


    private void RenderLoadedLoaderPlugins()
    {
        if (_crashReport.LoaderPlugins.Count == 0) return;

        for (var i = 0; i < _crashReport.LoaderPlugins.Count; i++)
        {
            var loaderPlugin = _crashReport.LoaderPlugins[i];
            if (_imgui.BeginChild(loaderPlugin.Id, in Zero2, in Plugin, ImGuiChildFlags.Border | ImGuiChildFlags.AutoResizeY, ImGuiWindowFlags.None))
            {
                if (_imgui.TreeNode(loaderPlugin.Id))
                {
                    _imgui.RenderId("Id:\0"u8, loaderPlugin.Id);

                    _imgui.TextSameLine("Name: \0"u8);
                    _imgui.Text(loaderPlugin.Name);

                    if (!string.IsNullOrEmpty(loaderPlugin.Version))
                    {
                        _imgui.TextSameLine("Version: \0"u8);
                        _imgui.Text(loaderPlugin.Version!);
                    }

                    if (loaderPlugin.UpdateInfo is not null)
                    {
                        _imgui.TextSameLine("Update Info: \0"u8);
                        _imgui.Text(_loaderPluginIdUpdateInfoUtf8[loaderPlugin.Id]);
                    }

                    if (_loaderPluginIdAdditionalUpdateInfos[loaderPlugin.Id].Length > 0)
                    {
                        for (var j = 0; j < _loaderPluginIdAdditionalUpdateInfos[loaderPlugin.Id].Length; j++)
                        {
                            _imgui.TextSameLine("Update Info: \0"u8);
                            _imgui.Text(_loaderPluginIdAdditionalUpdateInfos[loaderPlugin.Id][j]);
                        }
                    }

                    RenderCapabilities(loaderPlugin.Capabilities);

                    _imgui.TreePop();
                }
            }

            _imgui.EndChild();
        }
    }
}