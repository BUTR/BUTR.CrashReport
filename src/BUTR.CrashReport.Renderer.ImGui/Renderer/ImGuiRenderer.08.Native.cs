using BUTR.CrashReport.Extensions;
using BUTR.CrashReport.Models;
using BUTR.CrashReport.Renderer.ImGui.Extensions;
using BUTR.CrashReport.Renderer.ImGui.UnsafeUtils;

using ImGuiNET;

using System.Collections.Generic;
using System.IO;

namespace BUTR.CrashReport.Renderer.ImGui.Renderer;

partial class ImGuiRenderer
{
    private class NativeAssemblyModelEqualityComparer : IEqualityComparer<NativeAssemblyModel>
    {
        public static NativeAssemblyModelEqualityComparer Instance { get; } = new();
        public bool Equals(NativeAssemblyModel? x, NativeAssemblyModel? y) => ReferenceEquals(x, y); // We can just reference compare here
        public int GetHashCode(NativeAssemblyModel obj) => obj.GetHashCode();
    }

    private static readonly byte[][] _nativeArchitectureTypeNames =
    [
        "Unknown"u8.ToArray(),  // Unknown
        "x86"u8.ToArray(),  // x86
        "x64"u8.ToArray(),  // x86_64
        "Arm"u8.ToArray(),  // Arm
        "Arm64"u8.ToArray(),  // Arm64
    ];
    
    private void InitializeNatives() { }

    private void RenderNatives()
    {
        for (var i = 0; i < _crashReport.NativeModules.Count; i++)
        {
            var assembly = _crashReport.NativeModules[i];

            _imgui.Bullet();
            _imgui.TextSameLine(assembly.Id.Name);
            _imgui.TextSameLine(", \0"u8);
            _imgui.TextSameLine(assembly.Id.Version ?? string.Empty);
            _imgui.TextSameLine(", \0"u8);
            _imgui.TextSameLine(_nativeArchitectureTypeNames[(int) assembly.Architecture]);
            _imgui.TextSameLine(", \0"u8);
            _imgui.TextSameLine(assembly.Hash);
            _imgui.TextSameLine(", \0"u8);
            _imgui.SmallButton(assembly.AnonymizedPath);
        }
    }
}