using ImGui;

using ImGuiNET;

using Silk.NET.Input;
using Silk.NET.Windowing;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BUTR.CrashReport.Renderer.ImGui.Implementation.CImGui.Controller;

internal partial class ImGuiController
{
    private static readonly uint _sizeOfImDrawVert = (uint) Unsafe.SizeOf<ImDrawVert>();
    private static readonly IntPtr _offsetOfImDrawVertPos = Marshal.OffsetOf<ImDrawVert>(nameof(ImDrawVert.pos));
    private static readonly IntPtr _offsetOfImDrawVertUV = Marshal.OffsetOf<ImDrawVert>(nameof(ImDrawVert.uv));
    private static readonly IntPtr _offsetOfImDrawVertCol = Marshal.OffsetOf<ImDrawVert>(nameof(ImDrawVert.col));

    // ReSharper disable once HeapView.ObjectAllocation
    private static readonly StandardCursor[] _mouseCursors =
    [
        StandardCursor.Arrow,   // ImGuiMouseCursor.Arrow
        StandardCursor.IBeam,   // ImGuiMouseCursor.TextInput
        StandardCursor.Arrow,   // ImGuiMouseCursor.ResizeAll
        StandardCursor.VResize, // ImGuiMouseCursor.ResizeNS
        StandardCursor.HResize, // ImGuiMouseCursor.ResizeEW
        StandardCursor.Arrow,   // ImGuiMouseCursor.ResizeNESW
        StandardCursor.Arrow,   // ImGuiMouseCursor.ResizeNWSE
        StandardCursor.Hand,    // ImGuiMouseCursor.Hand
        StandardCursor.Arrow,   // ImGuiMouseCursor.NotAllowed
    ];

    private readonly CmGui _imgui;
    private IntPtr _context;

    private readonly IView _view;
    private readonly IInputContext _input;

    private readonly List<char> _pressedChars = new(32);

    private uint _windowsWidth, _windowsHeight;

    private bool _frameBegun;

    private IKeyboard Keyboard => _input.Keyboards[0];
    private IMouse Mouse => _input.Mice[0];
}