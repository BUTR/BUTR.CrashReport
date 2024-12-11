using BUTR.CrashReport.Renderer.ImGui.Silk.NET.Extensions;

using Silk.NET.Input.Sdl;
using Silk.NET.SDL;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Sdl;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BUTR.CrashReport.Renderer.ImGui.Implementation.CImGui.Utils;

internal static class SdlUtils
{
    internal static void Setup(SdlWindowOptions? sdlWindowOptions)
    {
        SdlWindowing.RegisterPlatform();
        SdlInput.RegisterPlatform();
        SdlWindowing.Use();
        _ = SdlProvider.SDL.Value;
    }

    private static unsafe void SetWindowIcon(IWindow window)
    {
        var sdl = Sdl.GetApi();
        var sdlWindow = (global::Silk.NET.SDL.Window*) (window.Native?.Sdl ?? IntPtr.Zero);

        var iconSpan = typeof(CrashReportImGui).Assembly.GetManifestResourceStreamAsSpan("resources\\icon_128.bin");
        var surface = sdl.CreateRGBSurfaceFrom
        (
            Unsafe.AsPointer(ref MemoryMarshal.GetReference(iconSpan)),
            128,
            128,
            32, 32 / 8 * 128,
            0x000000FF, 0x0000FF00, 0x00FF0000, 0xFF000000
        );
        sdl.SetWindowIcon(sdlWindow, surface);
        sdl.FreeSurface(surface);
    }

    internal static void Init(IWindow window, SdlWindowOptions? sdlWindowOptions)
    {
        SetWindowIcon(window);
    }

    internal static void Reset(SdlWindowOptions? sdlWindowOptions)
    {
        if (sdlWindowOptions == null || sdlWindowOptions.ResetProvider)
        {
            SdlProvider.SDL.Value.Quit();
            SdlProvider.SDL.Value.PurgeEntryPoints();
            SdlProvider.SDL.Value.Dispose();
            // When Silk.NET checks whether Sdl is applicable, it creates the context (and loads the native lib)
            // without unloading it. So we need to dispose it twice, since one dispose means one free call
            try
            {
                SdlProvider.SDL.Value.Dispose();
            }
            catch { /* ignore */ }
        }
    }

    public static unsafe float GetScale()
    {
        var sdl = Sdl.GetApi();
        var window = sdl.GLGetCurrentWindow();
        var displayIndex = sdl.GetWindowDisplayIndex(window);

        float dpi;
        sdl.GetDisplayDPI(displayIndex, &dpi, null, null);
        return dpi / 96.0f;
    }
}