using ImGui.Structures;

namespace BUTR.CrashReport.Renderer.ImGui.Implementation.CImGui.Controller;

partial class ImGuiController
{
    /// <summary>
    /// Renders the ImGui draw list data.
    /// </summary>
    public void Render()
    {
        if (!_frameBegun)
            return;

        var oldCtx = _imgui.GetCurrentContext();
        if (oldCtx != _context)
        {
            _imgui.SetCurrentContext(_context);
        }

        //_frameBegun = false;
        _imgui.Render();

        _imgui.GetDrawData(out var imDrawData);
        RenderImDrawData(ref imDrawData);

        if (oldCtx != _context)
        {
            _imgui.SetCurrentContext(oldCtx);
        }
    }

    partial void RenderImDrawData(ref readonly ImDrawDataWrapper drawData);
}