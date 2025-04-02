using BUTR.CrashReport.ImGui.Enums;
using BUTR.CrashReport.ImGui.Extensions;
using BUTR.CrashReport.Memory;
using BUTR.CrashReport.Memory.Utils;
using BUTR.CrashReport.Models;
using BUTR.CrashReport.Renderer.ImGui.Syntax;

using System.Numerics;

using Utf8StringInterpolation;

namespace BUTR.CrashReport.Renderer.ImGui.Renderer;

partial class ImGuiRenderer
{
    protected class IntEqualityComparer : IEqualityComparer<int>
    {
        public static IntEqualityComparer Instance { get; } = new();
        public bool Equals(int x, int y) => x == y;
        public int GetHashCode(int obj) => obj;
    }
    protected class MethodEqualityComparer : IEqualityComparer<MethodModel>
    {
        public static MethodEqualityComparer Instance { get; } = new();
        public bool Equals(MethodModel? x, MethodModel? y) => ReferenceEquals(x, y); // We can just reference compare here
        public int GetHashCode(MethodModel obj) => obj.GetHashCode();
    }

    // ReSharper disable once HeapView.ObjectAllocation
    protected static readonly LiteralSpan<byte>[] _codeTypeNamesUtf8 =
    [
        "IL:\0"u8,         // CodeType.IL
        "IL with C#:\0"u8, // CodeType.CSharpILMixed
        "C#:\0"u8,         // CodeType.CSharp
        "Native:\0"u8,     // CodeType.Native
    ];
}

partial class ImGuiRenderer<TImGuiIORef, TImGuiViewportRef, TImDrawListRef, TImGuiStyleRef, TColorsRangeAccessorRef, TImGuiListClipperRef>
{
    private enum CodeType { IL = 0, ILMixed = 1, CSharp = 2, Native = 3 }
    private readonly Dictionary<int, byte[]> _offsetsUtf8 = new(IntEqualityComparer.Instance);

    private void InitializeCodeLines()
    {
        void SetupCodeExecuting(MethodExecutingModel? method)
        {
            if (method is null) return;

            SetupCode(method);

#if TEXT_EDITOR
            SetCodeDictionary(_methodCodeLinesEditor, method, CodeType.Native, method.NativeInstructions);
#else
            SetCodeDictionary(_methodCodeLinesUtf8, method, CodeType.Native, Utf8Utils2.Join("\n"u8, method.NativeInstructions.Instructions));
#endif
        }
        void SetupCode(MethodModel? method)
        {
            if (method is null) return;

#if TEXT_EDITOR
            SetCodeDictionary(_methodCodeLinesEditor, method, CodeType.IL, method.ILInstructions);
            SetCodeDictionary(_methodCodeLinesEditor, method, CodeType.ILMixed, method.ILMixedInstructions);
            SetCodeDictionary(_methodCodeLinesEditor, method, CodeType.CSharp, method.CSharpInstructions);
#else
            SetCodeDictionary(_methodCodeLinesUtf8, method, CodeType.IL, Utf8Utils2.Join("\n"u8, method.ILInstructions?.Instructions ?? []));
            SetCodeDictionary(_methodCodeLinesUtf8, method, CodeType.ILMixed, Utf8Utils2.Join("\n"u8, method.ILMixedInstructions?.Instructions ?? []));
            SetCodeDictionary(_methodCodeLinesUtf8, method, CodeType.CSharp, Utf8Utils2.Join("\n"u8, method.CSharpInstructions?.Instructions ?? []));
#endif
        }

        for (var i = 0; i < _crashReport.EnhancedStacktrace.Count; i++)
        {
            var stacktrace = _crashReport.EnhancedStacktrace[i];
            if (stacktrace.ILOffset is not null)
                _offsetsUtf8[stacktrace.ILOffset.Value] = Utf8String.Format($"{stacktrace.ILOffset:X4}\0");

            if (stacktrace.NativeOffset is not null)
                _offsetsUtf8[stacktrace.NativeOffset.Value] = Utf8String.Format($"{stacktrace.NativeOffset:X4}\0");

            SetupCodeExecuting(stacktrace.ExecutingMethod);
            SetupCode(stacktrace.OriginalMethod);
            for (var j = 0; j < stacktrace.PatchMethods.Count; j++)
                SetupCode(stacktrace.PatchMethods[j]);
        }

#if TEXT_EDITOR
        _onDarkModeChanged += isDarkMode =>
        {
            _imGuiWithImGuiStyle.GetStyle(out var style);
            style.GetColors(out var colors);
            foreach (var editors in _methodCodeLinesEditor.Values)
            {
                foreach (var editor in editors)
                {
                    editor.SetPalette(PrismColorPalette.DefaultOrTomorrowNight(isDarkMode, colors));
                }
            }
        };
#endif
    }

    private void RenderMethodLines(MethodModel method, CodeType codeType)
    {
        var codeTypeUtf8 = _codeTypeNamesUtf8[(int) codeType];
        var lineCount = codeType switch
        {
            CodeType.IL => method.ILInstructions?.Instructions.Count ?? 0,
            CodeType.ILMixed => method.ILMixedInstructions?.Instructions.Count ?? 0,
            CodeType.CSharp => method.CSharpInstructions?.Instructions.Count ?? 0,
            CodeType.Native when method is MethodExecutingModel methodExecuting => methodExecuting.NativeInstructions.Instructions.Count,
            _ => throw new ArgumentOutOfRangeException(nameof(codeType), codeType, null),
        };

        if (lineCount == 0) return;

        if (_imgui.TreeNode(codeTypeUtf8, ImGuiTreeNodeFlags.None))
        {
            _imGuiWithImGuiStyle.GetStyle(out var currentStyle);
            currentStyle.GetColors(out var colors);
            _imgui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 1);
            if (_imgui.BeginChild(codeTypeUtf8, in Zero2, in colors[ImGuiCol.WindowBg], ImGuiChildFlags.Border | ImGuiChildFlags.AutoResizeY, ImGuiWindowFlags.None))
            {
                _imgui.PopStyleVar();
#if TEXT_EDITOR
                var editor = _methodCodeLinesEditor[method][(int) codeType];
                editor.Render(codeTypeUtf8);
#else
                var codeLinesUtf8 = _methodCodeLinesUtf8[method][(int) codeType];
                RenderInputTextWithIO(codeTypeUtf8, codeLinesUtf8, lineCount);
#endif
            }
            else
            {
                _imgui.PopStyleVar();
            }
            _imgui.EndChild();
            _imgui.TreePop();
        }
    }
    private void RenderCodeExecuting(MethodExecutingModel? method)
    {
        if (method is null) return;

        RenderCode(method);
        RenderMethodLines(method, CodeType.Native);
    }
    private void RenderCode(MethodModel? method)
    {
        if (method is null) return;

        RenderMethodLines(method, CodeType.IL);
        RenderMethodLines(method, CodeType.ILMixed);
        RenderMethodLines(method, CodeType.CSharp);
    }

    private void RenderEnhancedStacktrace()
    {
        for (var i = 0; i < _crashReport.EnhancedStacktrace.Count; i++)
        {
            var stacktrace = _crashReport.EnhancedStacktrace[i];
            var moduleId1 = stacktrace.ExecutingMethod.ModuleId ?? "UNKNOWN";
            var pluginId1 = stacktrace.ExecutingMethod.LoaderPluginId ?? "UNKNOWN";

            _imgui.PushId(i);

            if (_imgui.TreeNode(stacktrace.FrameDescription))
            {
                _imgui.Text("Executing Method:\0"u8);
                if (_imgui.TreeNode(stacktrace.ExecutingMethod.MethodFullDescription, ImGuiTreeNodeFlags.Bullet | ImGuiTreeNodeFlags.DefaultOpen))
                {
                    if (moduleId1 != "UNKNOWN") _imgui.RenderId("Module Id:\0"u8, moduleId1);
                    if (pluginId1 != "UNKNOWN") _imgui.RenderId("Plugin Id:\0"u8, pluginId1);

                    _imgui.Text("Approximate IL Offset: \0"u8);
                    _imgui.SameLine();
                    _imgui.Text(stacktrace.ILOffset is not null ? _offsetsUtf8[stacktrace.ILOffset.Value] : "UNKNOWN\0"u8);
                    _imgui.Text("Native Offset: \0"u8);
                    _imgui.SameLine();
                    _imgui.Text(stacktrace.NativeOffset is not null ? _offsetsUtf8[stacktrace.NativeOffset.Value] : "UNKNOWN\0"u8);

                    RenderCodeExecuting(stacktrace.ExecutingMethod);

                    _imgui.TreePop();
                }

                if (stacktrace.PatchMethods.Count > 0)
                {
                    _imgui.Text("Patch Methods:\0"u8);

                    for (var j = 0; j < stacktrace.PatchMethods.Count; j++)
                    {
                        _imgui.PushId(j);

                        var method = stacktrace.PatchMethods[j];

                        if (_imgui.TreeNode(method.MethodFullDescription, ImGuiTreeNodeFlags.Bullet | ImGuiTreeNodeFlags.DefaultOpen))
                        {
                            var moduleId2 = method.ModuleId ?? "UNKNOWN";
                            var pluginId2 = method.LoaderPluginId ?? "UNKNOWN";

                            if (moduleId2 != "UNKNOWN") _imgui.RenderId("Module Id:\0"u8, moduleId2);
                            if (pluginId2 != "UNKNOWN") _imgui.RenderId("Plugin Id:\0"u8, pluginId2);

                            _imgui.Text("Type: \0"u8);
                            _imgui.SameLine();
                            _imgui.Text(method.Provider);
                            _imgui.SameLine();
                            _imgui.Text(" \0"u8);
                            _imgui.SameLine();
                            _imgui.Text(method.Type);

                            RenderCode(method);

                            _imgui.TreePop();
                        }

                        _imgui.PopId();
                    }
                }

                if (stacktrace.OriginalMethod is not null)
                {
                    _imgui.Text("Original Method:\0"u8);

                    if (_imgui.TreeNode(stacktrace.OriginalMethod.MethodFullDescription, ImGuiTreeNodeFlags.Bullet | ImGuiTreeNodeFlags.DefaultOpen))
                    {
                        var moduleId3 = stacktrace.OriginalMethod.ModuleId ?? "UNKNOWN";
                        var pluginId3 = stacktrace.OriginalMethod.LoaderPluginId ?? "UNKNOWN";

                        if (moduleId3 != "UNKNOWN") _imgui.RenderId("Module Id:\0"u8, moduleId3);
                        if (pluginId3 != "UNKNOWN") _imgui.RenderId("Plugin Id:\0"u8, pluginId3);

                        RenderCode(stacktrace.OriginalMethod);

                        _imgui.TreePop();
                    }
                }

                _imgui.TreePop();
            }

            _imgui.PopId();
        }
    }
}