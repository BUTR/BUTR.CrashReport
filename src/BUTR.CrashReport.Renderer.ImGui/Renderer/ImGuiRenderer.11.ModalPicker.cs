using BUTR.CrashReport.ImGui.Enums;
using BUTR.CrashReport.ImGui.Utils;
using BUTR.CrashReport.Memory;
using BUTR.CrashReport.Renderer.ImGui.Components;

namespace BUTR.CrashReport.Renderer.ImGui.Renderer;

partial class ImGuiRenderer
{
    protected delegate void FilePickerOnSelected(string selectedPath);
}

partial class ImGuiRenderer<TImGuiIORef, TImGuiViewportRef, TImDrawListRef, TImGuiStyleRef, TColorsRangeAccessorRef, TImGuiListClipperRef>
{
    private void InitializeModalPicker()
    {

    }

    private void RenderModalPicker(LiteralSpan<byte> pickerId, FilePickerMode mode, FilePickerOnSelected onSelected)
    {
        if (_imgui.BeginPopupModal(pickerId, ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.AlwaysAutoResize))
        {
            var picker = FilePicker.GetPicker(pickerId, _imgui, mode, Environment.CurrentDirectory);
            if (picker.Draw())
            {
                onSelected(picker.SelectedPath!);
                // TODO: Doesn't work
                FilePicker.RemovePicker(pickerId);
            }

            _imgui.EndPopup();
        }
    }
}