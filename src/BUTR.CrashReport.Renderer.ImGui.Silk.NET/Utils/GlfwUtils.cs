using Silk.NET.Core;
using Silk.NET.GLFW;
using Silk.NET.Input.Glfw;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Glfw;

namespace BUTR.CrashReport.Renderer.ImGui.Implementation.CImGui.Utils;

internal static class GlfwUtils
{
    public static void Setup(GlfwWindowOptions? glfwWindowOptions)
    {
        GlfwWindowing.RegisterPlatform();
        GlfwInput.RegisterPlatform();
        GlfwWindowing.Use();
        _ = GlfwProvider.GLFW.Value;
    }

    private static void SetWindowIcon(IWindow window)
    {
        var iconPaths = typeof(CrashReportImGui).Assembly.GetManifestResourceNames().Where(x => x.StartsWith("resources\\icon_") && x.EndsWith(".bin")).ToArray().AsSpan();
        var icons = new RawImage[iconPaths.Length];

        for (var i = 0; i < iconPaths.Length; i++)
        {
            var iconPath = iconPaths[i];
            using var stream = typeof(CrashReportImGui).Assembly.GetManifestResourceStream(iconPath)!;
            using var reader = new BinaryReader(stream);

            var size = int.Parse(iconPath.Split('_')[1].Split('.')[0]);
            icons[i] = new(size, size, new Memory<byte>(reader.ReadBytes((int) stream.Length)));
        }

        window.SetWindowIcon(icons);
    }

    public static void Init(IWindow window, GlfwWindowOptions? glfwWindowOptions)
    {
        SetWindowIcon(window);
    }

    public static void Reset(GlfwWindowOptions? glfwWindowOptions)
    {
        if (glfwWindowOptions is null || glfwWindowOptions.ResetProvider)
        {
            GlfwProvider.GLFW.Value.Terminate();
            GlfwProvider.GLFW.Value.PurgeEntryPoints();
            GlfwProvider.GLFW.Value.Dispose();
            // When Silk.NET checks whether Glfw is applicable, it creates the context (and loads the native lib)
            // without unloading it. So we need to dispose it twice, since one dispose means one free call
            try
            {
                GlfwProvider.GLFW.Value.Dispose();
            }
            catch { /* ignore */ }
        }
    }
}