using Silk.NET.Core.Contexts;

using System.ComponentModel;
using System.Runtime.InteropServices;

namespace BUTR.CrashReport.Renderer.ImGui.Implementation.CImGui.Utils;

internal static class DarkThemeUtils
{
    private const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;
    private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
    private const int S_OK = 0;

    [DllImport("dwmapi", SetLastError = true)]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

    [DllImport("uxtheme.dll", EntryPoint = "#132")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ShouldAppsUseDarkMode();

    private static bool IsWindows10OrGreater(int build = -1) => Environment.OSVersion.Version.Major >= 10 && Environment.OSVersion.Version.Build >= build;
    private static bool IsDarkModeSupported { get; } = IsWindows10OrGreater(17763);

    public static void SetDarkModeTitleBar(INativeWindow nativeWindow)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && nativeWindow.Win32 is { Hwnd: var windowHandle })
        {
            if (IsDarkModeSupported && ShouldAppsUseDarkMode())
            {
                var attr = IsWindows10OrGreater(18985) ? DWMWA_USE_IMMERSIVE_DARK_MODE : DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1;
                var attrValue = 1;
                if (DwmSetWindowAttribute(windowHandle, attr, ref attrValue, sizeof(int)) is var hResult and not S_OK)
                    throw new Win32Exception(hResult);
            }
        }

    }
}