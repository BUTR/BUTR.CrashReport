namespace BUTR.CrashReport.Renderer.ImGui.Implementation.CImGui;

[Flags]
public enum WindowProvider
{
    None = 0,
    Glfw = 1,
    SDL = 2,
}

internal static class WindowProviderExtensions
{
    public static bool IsSet(this WindowProvider value, WindowProvider flag) => (value & flag) != 0;
}