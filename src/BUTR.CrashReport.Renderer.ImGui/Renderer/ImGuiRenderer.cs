using BUTR.CrashReport.ImGui;
using BUTR.CrashReport.ImGui.Enums;
using BUTR.CrashReport.ImGui.Extensions;
using BUTR.CrashReport.ImGui.Structures;
using BUTR.CrashReport.Memory;
using BUTR.CrashReport.Models;
using BUTR.CrashReport.Renderer.ImGui.Extensions;

using System.Numerics;

using Utf8StringInterpolation;

namespace BUTR.CrashReport.Renderer.ImGui.Renderer;

// Generic rules to avoid allocations:
// 1. Split any interpolated string into a hardcoded ("()\0"u8) and dynamic (entry.Text) parts and render them separately
// 2. Use custom equality comparer if a key doesn't implement IEquatable<T> in dictionaries
// 3. Use a FrozenDictionary instead of a standard. Set the EqualityComparer in the Dictionary
// 4. Cache all dynamic strings as utf8 byte array
// 5. Don't forget to add a null termination to your utf8 byte array
// The only allocations left are the FrozenDictionary finds. Can't do much now.

/// <summary>
/// This is an _almost_ zero allocation Crash Report Renderer.
/// We allocate a few bytes per second because of implecit Span{} casts and FrozenDictionary finds.
/// </summary>
public partial class ImGuiRenderer<TImGuiIORef, TImGuiViewportRef, TImDrawListRef, TImGuiStyleRef, TColorsRangeAccessorRef, TImGuiListClipperRef> : ImGuiRenderer
    where TImGuiIORef : IImGuiIO
    where TImGuiViewportRef : IImGuiViewport
    where TImDrawListRef : IImDrawList
    where TImGuiStyleRef : IImGuiStyle<TColorsRangeAccessorRef>
    where TColorsRangeAccessorRef : IRangeAccessor<Vector4, ImGuiCol>
    where TImGuiListClipperRef : IImGuiListClipper
{
    private readonly IImGui _imgui;
    private readonly IImGuiWithImGuiIO<TImGuiIORef> _imGuiWithImGuiIO;
    private readonly IImGuiWithImGuiViewport<TImGuiViewportRef> _imguiWithViewport;
    private readonly IImGuiWithImDrawList<TImDrawListRef> _imGuiWithImDrawList;
    private readonly IImGuiWithImGuiStyle<TImGuiStyleRef, TColorsRangeAccessorRef> _imGuiWithImGuiStyle;
    private readonly IImGuiWithImGuiListClipper<TImGuiListClipperRef> _imGuiWithImGuiListClipper;

    private readonly CrashReportModel _crashReport;
    private readonly IList<LogSourceModel> _logSources;
    private readonly ICrashReportRendererUtilities _crashReportRendererUtilities;
    private readonly Action _onClose;

    private event Action<bool>? _onDarkModeChanged;

    private bool _isDarkMode;
    private bool _isDarkModeOld;

    private LiteralSpan<byte> _involvedModulesAndPluginsTitleUtf8 = LiteralSpan<byte>.Empty;
    private byte[] _loadedPluginsTitleUtf8 = [];

    public ImGuiRenderer(IImGui imgui,
        IImGuiWithImGuiIO<TImGuiIORef> imGuiWithImGuiIO,
        IImGuiWithImGuiViewport<TImGuiViewportRef> imguiWithViewport,
        IImGuiWithImDrawList<TImDrawListRef> imGuiWithImDrawList,
        IImGuiWithImGuiStyle<TImGuiStyleRef, TColorsRangeAccessorRef> imGuiWithImGuiStyle,
        IImGuiWithImGuiListClipper<TImGuiListClipperRef> imGuiWithImGuiListClipper,
        CrashReportModel crashReport, IList<LogSourceModel> logSources, ICrashReportRendererUtilities crashReportRendererUtilities, Action onClose)
    {
        _imgui = imgui;
        _imGuiWithImGuiIO = imGuiWithImGuiIO;
        _imguiWithViewport = imguiWithViewport;
        _imGuiWithImDrawList = imGuiWithImDrawList;
        _imGuiWithImGuiStyle = imGuiWithImGuiStyle;
        _imGuiWithImGuiListClipper = imGuiWithImGuiListClipper;
        _crashReport = crashReport;
        _logSources = logSources;
        _crashReportRendererUtilities = crashReportRendererUtilities;
        _onClose = onClose;

        _isDarkMode = _crashReportRendererUtilities.IsDefaultDarkMode;
        _isDarkModeOld = !_isDarkMode;

        InitializeInputTextWithIO();
        InitializeRender();
        InitializeSummary();
        InitializeExceptionRecursively();
        InitializeCodeLines();
        InitializeInvolved();
        InitializeInstalledModules();
        InitializeInstalledLoaderPlugins();
        InitializeAssemblies();
        InitializeNatives();
        InitializeRuntimePatches();
        InitializeLogFiles();
        InitializeModalPicker();
    }

    private void InitializeRender()
    {
        _loadedPluginsTitleUtf8 = Utf8String.Format($"Loaded {_crashReport.Metadata.LoaderPluginProviderName ?? string.Empty} Plugins\0");
        _involvedModulesAndPluginsTitleUtf8 = _crashReportRendererUtilities.Capabilities.IsSet(CrashReportRendererCapabilities.PluginLoader)
            ? "Involved Modules and Plugins\0"u8
            : "Involved Modules\0"u8;
    }

    public void SetDarkMode(bool isDarkMode)
    {
        _isDarkMode = isDarkMode;
    }

    public void Render()
    {
        _imguiWithViewport.GetMainViewport(out var viewPort);
        _imgui.SetNextWindowPos(in viewPort.WorkPos);
        _imgui.SetNextWindowSize(in viewPort.WorkSize);
        _imgui.SetNextWindowViewport(viewPort.ID);

        if (_isDarkMode && !_isDarkModeOld)
        {
            _imgui.StyleColorsDark();
            _imGuiWithImGuiStyle.GetStyle(out var style);
            style.GetColors(out var colors);
            colors[ImGuiCol.Button] = Primary;
            colors[ImGuiCol.ButtonHovered] = Primary2;
            colors[ImGuiCol.ButtonActive] = Primary3;
            colors[ImGuiCol.HeaderHovered] = Primary2;
            colors[ImGuiCol.HeaderActive] = Primary3;
            colors[ImGuiCol.FrameBgHovered] = Primary;
            colors[ImGuiCol.FrameBgActive] = Primary2;
            colors[ImGuiCol.CheckMark] = Primary3;
            colors[ImGuiCol.ChildBg] = Primary3;
            colors[ImGuiCol.WindowBg] = DarkBackground;
            colors[ImGuiCol.ChildBg] = DarkChildBackground;
            colors[ImGuiCol.FrameBg] = Black;
            _isDarkModeOld = _isDarkMode;
            _onDarkModeChanged?.Invoke(_isDarkMode);
        }
        else if (!_isDarkMode && _isDarkModeOld)
        {
            _imgui.StyleColorsLight();
            _imGuiWithImGuiStyle.GetStyle(out var style);
            style.GetColors(out var colors);
            colors[ImGuiCol.Button] = Primary;
            colors[ImGuiCol.ButtonHovered] = Primary2;
            colors[ImGuiCol.ButtonActive] = Primary3;
            colors[ImGuiCol.HeaderHovered] = Primary2;
            colors[ImGuiCol.HeaderActive] = Primary3;
            colors[ImGuiCol.FrameBgHovered] = Primary;
            colors[ImGuiCol.FrameBgActive] = Primary2;
            colors[ImGuiCol.CheckMark] = Primary3;
            colors[ImGuiCol.WindowBg] = LightBackground;
            colors[ImGuiCol.ChildBg] = LightChildBackground;
            colors[ImGuiCol.ChildBg] = LightChildBackground;
            _isDarkModeOld = _isDarkMode;
            _onDarkModeChanged?.Invoke(_isDarkMode);
        }

        if (_imgui.Begin("Crash Report\0"u8, ImGuiWindowFlags.HorizontalScrollbar | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.UnsavedDocument))
        {
            RenderSummary();

            _imgui.NewLine();
            if (_imgui.BeginChildRound("Exception\0"u8, in Zero2, ImGuiChildFlags.Border | ImGuiChildFlags.AutoResizeY, ImGuiWindowFlags.None))
            {
                _imgui.SetWindowFontScale(2);
                if (_imgui.TreeNode("Exception\0"u8, ImGuiTreeNodeFlags.None))
                {
                    _imgui.SetWindowFontScale(1);
                    RenderExceptionRecursively(_crashReport.Exception, 0);

                    _imgui.TreePop();
                }
                _imgui.SetWindowFontScale(1);
            }
            _imgui.EndChild();

            if (_imgui.BeginChildRound("Enhanced Stacktrace\0"u8, in Zero2, ImGuiChildFlags.Border | ImGuiChildFlags.AutoResizeY, ImGuiWindowFlags.None))
            {
                _imgui.SetWindowFontScale(2);
                if (_imgui.TreeNode("Enhanced Stacktrace\0"u8, ImGuiTreeNodeFlags.None))
                {
                    _imgui.SetWindowFontScale(1);
                    RenderEnhancedStacktrace();

                    _imgui.TreePop();
                }
                _imgui.SetWindowFontScale(1);
            }
            _imgui.EndChild();

            if (_imgui.BeginChildRound(_involvedModulesAndPluginsTitleUtf8, in Zero2, ImGuiChildFlags.Border | ImGuiChildFlags.AutoResizeY, ImGuiWindowFlags.None))
            {
                _imgui.SetWindowFontScale(2);
                if (_imgui.TreeNode(_involvedModulesAndPluginsTitleUtf8, ImGuiTreeNodeFlags.None))
                {
                    _imgui.SetWindowFontScale(1);
                    RenderInvolvedModulesAndPlugins();

                    _imgui.TreePop();
                }
                _imgui.SetWindowFontScale(1);
            }
            _imgui.EndChild();

            if (_imgui.BeginChildRound("Installed Modules\0"u8, in Zero2, ImGuiChildFlags.Border | ImGuiChildFlags.AutoResizeY, ImGuiWindowFlags.None))
            {
                _imgui.SetWindowFontScale(2);
                if (_imgui.TreeNode("Installed Modules\0"u8, ImGuiTreeNodeFlags.None))
                {
                    _imgui.SetWindowFontScale(1);
                    RenderInstalledModules();

                    _imgui.TreePop();
                }
                _imgui.SetWindowFontScale(1);
            }
            _imgui.EndChild();

            if (_crashReportRendererUtilities.Capabilities.IsSet(CrashReportRendererCapabilities.PluginLoader))
            {
                if (_imgui.BeginChildRound(_loadedPluginsTitleUtf8, in Zero2, ImGuiChildFlags.Border | ImGuiChildFlags.AutoResizeY, ImGuiWindowFlags.None))
                {
                    _imgui.SetWindowFontScale(2);
                    if (_imgui.TreeNode(_loadedPluginsTitleUtf8, ImGuiTreeNodeFlags.None))
                    {
                        _imgui.SetWindowFontScale(1);
                        RenderLoadedLoaderPlugins();

                        _imgui.TreePop();
                    }
                    _imgui.SetWindowFontScale(1);
                }
                _imgui.EndChild();
            }

            if (_imgui.BeginChildRound("Assemblies\0"u8, in Zero2, ImGuiChildFlags.Border | ImGuiChildFlags.AutoResizeY, ImGuiWindowFlags.None))
            {
                _imgui.SetWindowFontScale(2);
                if (_imgui.TreeNode("Assemblies\0"u8, ImGuiTreeNodeFlags.None))
                {
                    _imgui.SetWindowFontScale(1);
                    RenderAssemblies();

                    _imgui.TreePop();
                }
                _imgui.SetWindowFontScale(1);
            }
            _imgui.EndChild();

            if (_imgui.BeginChildRound("Native Assemblies\0"u8, in Zero2, ImGuiChildFlags.Border | ImGuiChildFlags.AutoResizeY, ImGuiWindowFlags.None))
            {
                _imgui.SetWindowFontScale(2);
                if (_imgui.TreeNode("Native Assemblies\0"u8, ImGuiTreeNodeFlags.None))
                {
                    _imgui.SetWindowFontScale(1);
                    RenderNatives();

                    _imgui.TreePop();
                }
                _imgui.SetWindowFontScale(1);
            }
            _imgui.EndChild();

            if (_imgui.BeginChildRound("Runtime Patches\0"u8, in Zero2, ImGuiChildFlags.Border | ImGuiChildFlags.AutoResizeY, ImGuiWindowFlags.None))
            {
                _imgui.SetWindowFontScale(2);
                if (_imgui.TreeNode("Runtime Patches\0"u8, ImGuiTreeNodeFlags.None))
                {
                    _imgui.SetWindowFontScale(1);
                    RenderRuntimePatches();

                    _imgui.TreePop();
                }
                _imgui.SetWindowFontScale(1);
            }
            _imgui.EndChild();

            if (_crashReportRendererUtilities.Capabilities.IsSet(CrashReportRendererCapabilities.Logs))
            {
                if (_imgui.BeginChildRound("Log Files\0"u8, in Zero2, ImGuiChildFlags.Border | ImGuiChildFlags.AutoResizeY, ImGuiWindowFlags.None))
                {
                    _imgui.SetWindowFontScale(2);
                    if (_imgui.TreeNode("Log Files\0"u8, ImGuiTreeNodeFlags.None))
                    {
                        _imgui.SetWindowFontScale(1);
                        RenderLogFiles();

                        _imgui.TreePop();
                    }
                    _imgui.SetWindowFontScale(1);
                }
                _imgui.EndChild();
            }

            _imgui.End();
        }
    }
}