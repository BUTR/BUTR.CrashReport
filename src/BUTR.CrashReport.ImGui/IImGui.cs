using BUTR.CrashReport.ImGui.Enums;
using BUTR.CrashReport.ImGui.Structures;

using System.Numerics;

namespace BUTR.CrashReport.ImGui;

public interface IImGui : IDisposable
{
    void PushId(int id);
    void PushId(ReadOnlySpan<byte> utf8Label);
    void PopId();

    bool InputTextMultilineInt64(ReadOnlySpan<byte> utf8Label, Span<byte> input, int lineCount, ImGuiInputTextInt64Callback callback, Int64 data);

    // BUG: Generic method doesn't work with NativeAOT
    bool InputTextMultiline<TData>(ReadOnlySpan<byte> utf8Label, Span<byte> input, int lineCount, ImGuiInputTextCallback<TData> callback, TData data)
        where TData : struct;

    bool InputText(ReadOnlySpan<byte> utf8Label, Span<byte> input, ImGuiInputTextFlags flags);

    void Text(ReadOnlySpan<byte> utf8Label);

    void TextWrapped(ReadOnlySpan<byte> utf8Label);

    void TextColored(ReadOnlySpan<byte> utf8Label, ref readonly Vector4 color);

    void TextLinkOpenURL(ReadOnlySpan<byte> utf8Label, ReadOnlySpan<byte> utf8Url);

    bool Checkbox(ReadOnlySpan<byte> utf8Label, ref bool isSelected);

    void OpenPopup(ReadOnlySpan<byte> utf8Label, ImGuiPopupFlags flags);
    bool BeginPopup(ReadOnlySpan<byte> utf8Label, ImGuiWindowFlags flags);
    bool BeginPopupModal(ReadOnlySpan<byte> utf8Label, ImGuiWindowFlags flags);
    bool BeginPopupContextWindow(ReadOnlySpan<byte> utf8Label);
    void EndPopup();
    void CloseCurrentPopup();

    bool Selectable(ReadOnlySpan<byte> utf8Label, ref bool isSelected, ImGuiSelectableFlags flags);

    bool Button(ReadOnlySpan<byte> utf8Label);

    bool SmallButton(ReadOnlySpan<byte> utf8Label);

    bool Begin(ReadOnlySpan<byte> utf8Label, ImGuiWindowFlags flags);
    void End();

    bool BeginTable(ReadOnlySpan<byte> utf8Label, int column);
    bool TableNextColumn();
    void EndTable();

    bool BeginChild(ReadOnlySpan<byte> utf8Label, ref readonly Vector2 size, ImGuiChildFlags child_flags, ImGuiWindowFlags window_flags);
    void EndChild();

    bool TreeNode(ReadOnlySpan<byte> utf8Label, ImGuiTreeNodeFlags flags);
    void TreePop();

    void PushStyleColor(ImGuiCol idx, ref readonly Vector4 color);
    void PopStyleColor();
    void PopStyleColor(int count);

    void PushStyleVar(ImGuiStyleVar idx, float value);
    void PushStyleVar(ImGuiStyleVar idx, ref readonly Vector2 value);
    void PopStyleVar();
    void PopStyleVar(int count);

    void SetNextWindowPos(ref readonly Vector2 position);
    void SetNextWindowSize(ref readonly Vector2 size);
    void SetNextWindowViewport(uint viewportId);
    void StyleColorsLight();
    void StyleColorsDark();
    void SetWindowFontScale(float scale);

    void SameLine();
    void SameLine(float offsetFromStartX, float spacing);

    void NewLine();

    void Separator();

    void Bullet();

    void Indent();
    void Unindent();

    float GetTextLineHeight();
    float GetTextLineHeightWithSpacing();

    bool IsWindowAppearing();
    bool IsWindowFocused();
    bool IsWindowHovered();
    IntPtr GetCurrentWindow();

    void BringWindowToDisplayFront(IntPtr window);

    bool MenuItem(ReadOnlySpan<byte> utf8Label);

    bool IsItemClicked(ImGuiMouseButton mouseButton);

    bool IsMouseDoubleClicked(ImGuiMouseButton mouseButton);
    bool IsKeyDown(ImGuiKey key);
    bool IsKeyPressed(ImGuiKey key);

    void SetClipboardText(ReadOnlySpan<byte> utf8Data);

    void CalcTextSize(ReadOnlySpan<byte> utf8Data, out Vector2 size);

    float GetWindowHeight();
    float GetWindowWidth();

    void SetWindowFocus();

    void ColorConvertU32ToFloat4(uint u, out Vector4 color);
    uint ColorConvertFloat4ToU32(ref readonly Vector4 color);

    float GetScrollX();
    float GetScrollY();
    void SetScrollX(float value);
    void SetScrollY(float value);


    void GetCursorScreenPos(out Vector2 cursorScreenPos);
    void SetMouseCursor(ImGuiMouseCursor textInput);
    bool IsMouseClicked(ImGuiMouseButton button);
    void GetMousePos(out Vector2 mousePos);
    bool IsMouseDragging(ImGuiMouseButton button);
    bool IsMouseDown(ImGuiMouseButton button);
    bool IsMouseHoveringRect(ref readonly Vector2 lineStartScreenPos, ref readonly Vector2 end);


    void Dummy(ref readonly Vector2 vector2);

    void BeginTooltip();
    void EndTooltip();
    
    void Markdown(ReadOnlySpan<byte> utf8Markdown);
}