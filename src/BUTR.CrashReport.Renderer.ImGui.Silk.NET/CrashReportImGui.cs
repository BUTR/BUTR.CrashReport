using BUTR.CrashReport.ImGui.Enums;
using BUTR.CrashReport.Models;
using BUTR.CrashReport.Native;
using BUTR.CrashReport.Renderer.ImGui.Implementation.CImGui.Controller;
using BUTR.CrashReport.Renderer.ImGui.Implementation.CImGui.Utils;
using BUTR.CrashReport.Renderer.ImGui.Renderer;

using ImGui;
using ImGui.Structures;

using Silk.NET.Core.Loader;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

using System.Numerics;
using System.Runtime.InteropServices;

[assembly: DelegateLoader(typeof(CmGui), false)]

namespace BUTR.CrashReport.Renderer.ImGui.Implementation.CImGui;

internal class CmGuiLoader : IDisposable
{
    private const string LibWindows = "cimgui.dll";
    private const string LibLinux = "libcimgui.so";
    private const string LibOSX = "libcimgui.dylib";

    private LibraryLoader? Loader;
    private IntPtr? Library { get; set; }
    public IntPtr LoadFunctionPointer(string funcionName)
    {
        Loader ??= LibraryLoader.GetPlatformDefaultLoader();
        Library ??= Loader.LoadNativeLibrary([LibWindows, LibLinux, LibOSX]);

        return Loader.LoadFunctionPointer(Library.GetValueOrDefault(), funcionName);
    }

    public void Dispose()
    {
        Loader?.FreeNativeLibrary(Library.GetValueOrDefault());
    }
}

public sealed class CrashReportImGui : IDisposable
{
    private static bool IsGlfwWindow(IView window) => window.GetType().Name.Contains("Glfw");
    private static bool IsSdlWindow(IView window) => window.GetType().Name.Contains("Sdl");

    private readonly WindowProvider _windowProvider;
    private readonly GlfwWindowOptions? _glfwWindowOptions;
    private readonly SdlWindowOptions? _sdlWindowOptions;
    private readonly CmGuiLoader _imguiLoader = new();
    private readonly CmGui _imgui;

    private Action? _onClose;

    public CrashReportImGui(INativeLoaderUtilities nativeLoaderUtilities, WindowProvider windowProvider = WindowProvider.None, params IWindowOptions[] windowOptions)
    {
        if (windowProvider == WindowProvider.None)
            windowProvider = WindowProvider.SDL;

        _windowProvider = windowProvider;

        if (PathResolver.Default is DefaultPathResolver pr)
            pr.Resolvers.Add(path => nativeLoaderUtilities.GetNativeLibrariesFolderPath().Select(x => Path.Combine(x, path)));

        _glfwWindowOptions = windowOptions.OfType<GlfwWindowOptions>().FirstOrDefault();
        _sdlWindowOptions = windowOptions.OfType<SdlWindowOptions>().FirstOrDefault();

        _imgui = new CmGui();
        _imgui.LoadFrom(_imguiLoader.LoadFunctionPointer);

        if (_windowProvider.IsSet(WindowProvider.Glfw))
            GlfwUtils.Setup(_glfwWindowOptions);

        if (_windowProvider.IsSet(WindowProvider.SDL))
            SdlUtils.Setup(_sdlWindowOptions);
    }

    public void Close()
    {
        _onClose?.Invoke();
    }

    public void ShowAndWait(CrashReportModel crashReportModel, IList<LogSourceModel> logSources, ICrashReportRendererUtilities utilities)
    {
        // ReSharper disable once AccessToModifiedClosure
        var imGuiRenderer = new ImGuiRenderer<ImGuiIOWrapper, ImGuiViewportWrapper, ImDrawListWrapper, ImGuiStyleWrapper, RangeAccessorRef<Vector4, ImGuiCol>, ImGuiListClipperWrapper>(
            _imgui, _imgui, _imgui, _imgui, _imgui, _imgui,
            crashReportModel, logSources, utilities, () => _onClose?.Invoke());

        // Looks like the compatibility profile uses less CPU cycles compared to core
        // But only core is supported on macOS
        var profile = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? ContextProfile.Core : ContextProfile.Compatability;
        var window = Window.Create(WindowOptions.Default with
        {
            Title = $"{crashReportModel.Metadata.GameName} Crash Report",
            VSync = true,
            IsEventDriven = false,
            TransparentFramebuffer = false,
            WindowBorder = WindowBorder.Resizable,
            API = GraphicsAPI.Default with
            {
                Profile = profile,
            },
        });
        //window.Title = $"{crashReportModel.Metadata.GameName} Crash Report ({window.GetType().Name})";
        _onClose = window.Close;

        window.Initialize();

        var gl = window.CreateOpenGL();
        var inputContext = window.CreateInput();
        var controller = new ImGuiController(_imgui, gl, window, inputContext);

        controller.Initialize();

        // ReSharper disable AccessToDisposedClosure
        void OnWindowOnResize(Vector2D<int> newSize)
        {
            gl.Viewport(newSize);
        }
        // ReSharper restore AccessToDisposedClosure
        window.Resize += OnWindowOnResize;

        // ReSharper disable AccessToDisposedClosure
        void OnWindowOnFramebufferResize(Vector2D<int> newSize)
        {
            gl.Viewport(newSize);
        }
        // ReSharper restore AccessToDisposedClosure
        window.FramebufferResize += OnWindowOnFramebufferResize;

        // ReSharper disable AccessToDisposedClosure
        void OnWindowOnUpdate(double delta)
        {
            controller.Update(delta);
        }
        // ReSharper restore AccessToDisposedClosure
        window.Update += OnWindowOnUpdate;

        // ReSharper disable AccessToDisposedClosure
        void OnWindowOnRender(double delta)
        {
            gl.Clear(ClearBufferMask.ColorBufferBit);

            imGuiRenderer.Render();

            controller.Render();
        }
        // ReSharper restore AccessToDisposedClosure
        window.Render += OnWindowOnRender;

        if (window.Native is { } nativeWindow)
            DarkThemeUtils.SetDarkModeTitleBar(nativeWindow);

        if (IsGlfwWindow(window))
            GlfwUtils.Init(window, _glfwWindowOptions);

        if (IsSdlWindow(window))
            SdlUtils.Init(window, _sdlWindowOptions);

        //window.GLContext?.SwapInterval(0);

        static void DoLoop(IWindow window)
        {
            window.Run(() =>
            {
                if (!window.IsInitialized || !window.IsVisible || window.IsClosing)
                    return;

                window.DoEvents();

                window.DoUpdate();

                window.DoRender();
            });
            window.DoEvents();
        }
        DoLoop(window);

        if (!window.IsClosing)
            window.Close();

        window.Resize -= OnWindowOnResize;
        window.FramebufferResize -= OnWindowOnFramebufferResize;
        window.Update -= OnWindowOnUpdate;
        window.Render -= OnWindowOnRender;

        controller.Reset();

        inputContext.Dispose();
        gl.PurgeEntryPoints();
        gl.Dispose();

        window.Dispose(); // Calls Reset
    }

    public void Dispose()
    {
        _imgui.Dispose();
        _imguiLoader.Dispose();

        if (_windowProvider.IsSet(WindowProvider.Glfw))
            GlfwUtils.Reset(_glfwWindowOptions);

        if (_windowProvider.IsSet(WindowProvider.SDL))
            SdlUtils.Reset(_sdlWindowOptions);
    }
}