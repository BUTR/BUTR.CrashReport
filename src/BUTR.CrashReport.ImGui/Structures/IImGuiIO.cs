using System.Numerics;

namespace BUTR.CrashReport.ImGui.Structures;

public interface IImGuiIO : IDisposable
{
    ref Vector2 DisplaySize { get; }

    ref bool KeyCtrl { get; }
    ref bool KeyShift { get; }
    ref bool KeyAlt { get; }
    ref bool KeySuper { get; }

    ref bool ConfigMacOSXBehaviors { get; }

    ref bool WantCaptureKeyboard { get; }
    ref bool WantCaptureMouse { get; }
    ref bool WantTextInput { get; }
}