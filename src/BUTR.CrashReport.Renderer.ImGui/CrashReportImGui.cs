using BUTR.CrashReport.Interfaces;
using BUTR.CrashReport.Models;
using BUTR.CrashReport.Renderer.ImGui.Controller;
using BUTR.CrashReport.Renderer.ImGui.ImGui;
using BUTR.CrashReport.Renderer.ImGui.Renderer;

using Silk.NET.Core.Loader;
using Silk.NET.Input;
using Silk.NET.Input.Glfw;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Glfw;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BUTR.CrashReport.Renderer.ImGui;

public class CrashReportImGui
{
    static CrashReportImGui()
    {
        GlfwInput.RegisterPlatform();
        GlfwWindowing.RegisterPlatform();
    }

    public static void ShowAndWait(Exception exception, IList<LogSource> logSources, Dictionary<string, string> additionalMetadata,
        ICrashReportMetadataProvider crashReportMetadataProvider,
        IStacktraceFilter stacktraceFilter,
        IAssemblyUtilities assemblyUtilities,
        IModuleProvider moduleProvider,
        ILoaderPluginProvider loaderPluginProvider,
        IHarmonyProvider harmonyProvider,
        IModelConverter modelConverter,
        IPathAnonymizer pathAnonymizer,
        ICrashReportRendererUtilities crashReportRendererUtilities)
    {
        var crashReport = CrashReportInfo.Create(exception, additionalMetadata, stacktraceFilter, assemblyUtilities, moduleProvider, loaderPluginProvider, harmonyProvider);

        var crashReportModel = CrashReportInfo.ToModel(crashReport, crashReportMetadataProvider, modelConverter, moduleProvider, loaderPluginProvider, assemblyUtilities, pathAnonymizer);

        ShowAndWait(crashReportModel, logSources, crashReportRendererUtilities);
    }

    public static void ShowAndWait(CrashReportModel crashReportModel, IList<LogSource> logSources, ICrashReportRendererUtilities crashReportRendererUtilities)
    {
        if (PathResolver.Default is DefaultPathResolver pr)
            pr.Resolvers = [path => crashReportRendererUtilities.GetNativeLibrariesFolderPath().Select(x => Path.Combine(x, path))];

        var window = Window.Create(WindowOptions.Default with
        {
            Title = $"{crashReportModel.Metadata.GameName} Crash Report",
            VSync = true,
        });

        var gl = default(GL)!;
        var controller = default(ImGuiController)!;
        var imGuiRenderer = default(ImGuiRenderer)!;

        window.Load += () =>
        {
            window.SetDefaultIcon();

            var imgui = new CmGui();
            var inputContext = window.CreateInput();

            gl = window.CreateOpenGL();
            controller = new ImGuiController(imgui, gl, window, inputContext);
            imGuiRenderer = new ImGuiRenderer(imgui, crashReportModel, logSources, crashReportRendererUtilities, window.Close);

            controller.Init();
        };
        window.FramebufferResize += s =>
        {
            gl.Viewport(s);
        };
        window.Render += delta =>
        {
            if (window.IsClosing) return;

            controller.Update(delta);

            gl.Clear(ClearBufferMask.ColorBufferBit);

            imGuiRenderer.Render();

            controller.Render();
        };

        DoLoop(window);

        controller.Dispose();
        window.Dispose();
    }

    private static void DoLoop(IWindow window)
    {
        window.Initialize();
        window.Run(() =>
        {
            window.DoEvents();

            if (!window.IsClosing)
                window.DoUpdate();

            if (!window.IsClosing)
                window.DoRender();
        });
        window.DoEvents();
    }
}