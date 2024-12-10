using ImGuiNET;

namespace BUTR.CrashReport.Renderer.ImGui.Implementation.CImGui.Controller;

partial class ImGuiController
{
    public void Initialize()
    {
        _context = _imgui.CreateContext();
        _imgui.SetCurrentContext(_context);

        _imgui.GetIO(out var io);
        io.BackendFlags |= ImGuiBackendFlags.HasMouseCursors;
        io.BackendFlags |= ImGuiBackendFlags.HasSetMousePos;
        io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;
        io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard | ImGuiConfigFlags.DpiEnableScaleFonts;

        _windowsWidth = (uint) _view.Size.X;
        _windowsHeight = (uint) _view.Size.Y;

        CreateDeviceObjects();

        SetPerFrameImGuiData(1f / 60f);

        _view.Resize += WindowResized;

        Keyboard.KeyChar += OnKeyChar;
        Keyboard.KeyDown += (keyboard, keycode, scancode) => OnKeyEvent(keyboard, keycode, scancode, down: true);
        Keyboard.KeyUp += (keyboard, keycode, scancode) => OnKeyEvent(keyboard, keycode, scancode, down: false);

        Mouse.Scroll += MouseOnScroll;
        Mouse.MouseMove += MouseOnMouseMove;
        Mouse.MouseDown += MouseOnMouseDown;
        Mouse.MouseUp += MouseOnMouseUp;

        _frameBegun = true;
        _imgui.NewFrame();
    }

    partial void CreateDeviceObjects();
}