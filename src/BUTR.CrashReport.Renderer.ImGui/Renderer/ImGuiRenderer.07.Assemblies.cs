﻿using BUTR.CrashReport.Extensions;
using BUTR.CrashReport.Models;
using BUTR.CrashReport.Renderer.ImGui.Extensions;
using BUTR.CrashReport.Renderer.ImGui.UnsafeUtils;

using ImGuiNET;

using System.Collections.Generic;

namespace BUTR.CrashReport.Renderer.ImGui.Renderer;

partial class ImGuiRenderer
{
    private class AssemblyModelEqualityComparer : IEqualityComparer<AssemblyModel>
    {
        public static AssemblyModelEqualityComparer Instance { get; } = new();
        public bool Equals(AssemblyModel? x, AssemblyModel? y) => ReferenceEquals(x, y); // We can just reference compare here
        public int GetHashCode(AssemblyModel obj) => obj.GetHashCode();
    }

    private static bool _hideSystemAssemblies;
    private static bool _hideGACAssemblies;
    private static bool _hideGameAssemblies;
    private static bool _hideGameModulesAssemblies;
    private static bool _hideModulesAssemblies;
    private static bool _hideLoaderAssemblies;
    private static bool _hideLoaderPluginsAssemblies;
    private static bool _hideDynamicAssemblies;
    private static bool _hideUnclassifiedAssemblies;

    private readonly Dictionary<AssemblyModel, byte[]> _assemblyFullNameUtf8 = new(AssemblyModelEqualityComparer.Instance);

    private static readonly byte[][] _architectureTypeNames =
    [
        "Unknown"u8.ToArray(),  // Unknown
        "MSIL"u8.ToArray(),  // MSIL
        "x86"u8.ToArray(),  // X86
        "IA64"u8.ToArray(),  // IA64
        "x64"u8.ToArray(),  // Amd64
        "Arm"u8.ToArray(),  // Arm
    ];

    private void InitializeAssemblies()
    {
        for (var i = 0; i < _crashReport.Assemblies.Count; i++)
        {
            var assembly = _crashReport.Assemblies[i];
            _assemblyFullNameUtf8[assembly] = UnsafeHelper.ToUtf8Array(assembly.GetFullName());
        }
    }

    private void RenderAssemblies()
    {
        _imgui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 1);
        _imgui.TextSameLine("Hide: \0"u8);
        _imgui.CheckboxSameLine(" System | \0"u8, ref _hideSystemAssemblies);
        _imgui.CheckboxSameLine(" GAC | \0"u8, ref _hideGACAssemblies);
        _imgui.CheckboxSameLine(" Game | \0"u8, ref _hideGameAssemblies);
        _imgui.CheckboxSameLine(" Game Modules | \0"u8, ref _hideGameModulesAssemblies);
        _imgui.CheckboxSameLine(" Modules | \0"u8, ref _hideModulesAssemblies);
        _imgui.CheckboxSameLine(" Loader | \0"u8, ref _hideLoaderAssemblies);
        _imgui.CheckboxSameLine(" Loader Plugins | \0"u8, ref _hideLoaderPluginsAssemblies);
        _imgui.CheckboxSameLine(" Dynamic | \0"u8, ref _hideDynamicAssemblies);
        _imgui.Checkbox(" Unclassified \0"u8, ref _hideUnclassifiedAssemblies);
        _imgui.PopStyleVar();

        for (var i = 0; i < _crashReport.Assemblies.Count; i++)
        {
            var assembly = _crashReport.Assemblies[i];
            if (_hideSystemAssemblies && assembly.Type.IsSet(AssemblyModelType.System)) continue;
            if (_hideGACAssemblies && assembly.Type.IsSet(AssemblyModelType.GAC)) continue;
            if (_hideGameAssemblies && assembly.Type.IsSet(AssemblyModelType.GameCore)) continue;
            if (_hideGameModulesAssemblies && assembly.Type.IsSet(AssemblyModelType.GameModule)) continue;
            if (_hideModulesAssemblies && assembly.Type.IsSet(AssemblyModelType.Module)) continue;
            if (_hideLoaderAssemblies && assembly.Type.IsSet(AssemblyModelType.Loader)) continue;
            if (_hideLoaderPluginsAssemblies && assembly.Type.IsSet(AssemblyModelType.LoaderPlugin)) continue;
            if (_hideDynamicAssemblies && assembly.Type.IsSet(AssemblyModelType.Dynamic)) continue;
            if (_hideUnclassifiedAssemblies && assembly.Type == AssemblyModelType.Unclassified) continue;

            var isDynamic = assembly.Type.IsSet(AssemblyModelType.Dynamic);
            var hasPath = assembly.AnonymizedPath != "EMPTY" && assembly.AnonymizedPath != "DYNAMIC" && !string.IsNullOrWhiteSpace(assembly.AnonymizedPath);

            _imgui.Bullet();
            _imgui.TextSameLine(assembly.Id.Name);
            _imgui.TextSameLine(", \0"u8);
            _imgui.TextSameLine(assembly.Id.Version ?? string.Empty);
            _imgui.TextSameLine(", \0"u8);
            _imgui.TextSameLine(_architectureTypeNames[(int) assembly.Architecture]);
            if (!isDynamic)
            {
                _imgui.TextSameLine(", \0"u8);
                _imgui.TextSameLine(assembly.Hash);
            }
            if (hasPath)
            {
                _imgui.TextSameLine(", \0"u8);
                _imgui.SmallButton(assembly.AnonymizedPath);
            }
            else
            {
                _imgui.Text(isDynamic ? ", DYNAMIC\0"u8 : ", EMPTY\0"u8);
            }
        }
    }
}