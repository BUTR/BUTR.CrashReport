using BUTR.CrashReport.ImGui.Enums;
using BUTR.CrashReport.ImGui.Structures;

using System.Numerics;
using System.Runtime.CompilerServices;

namespace ImGui.Structures;

public readonly unsafe struct ImGuiStyleWrapper : IImGuiStyle<RangeAccessorRef<Vector4, ImGuiCol>>
{
    private CmGui ImGui { get; }

    public ImGuiNET.ImGuiStyle* NativePtr { get; }

    public ImGuiStyleWrapper(CmGui imGui, ImGuiNET.ImGuiStyle* nativePtr)
    {
        ImGui = imGui;
        NativePtr = nativePtr;
    }

    public ref float Alpha => ref Unsafe.AsRef<float>(&NativePtr->Alpha);
    public ref float DisabledAlpha => ref Unsafe.AsRef<float>(&NativePtr->DisabledAlpha);
    public ref Vector2 WindowPadding => ref Unsafe.AsRef<Vector2>(&NativePtr->WindowPadding);
    public ref float WindowRounding => ref Unsafe.AsRef<float>(&NativePtr->WindowRounding);
    public ref float WindowBorderSize => ref Unsafe.AsRef<float>(&NativePtr->WindowBorderSize);
    public ref Vector2 WindowMinSize => ref Unsafe.AsRef<Vector2>(&NativePtr->WindowMinSize);
    public ref Vector2 WindowTitleAlign => ref Unsafe.AsRef<Vector2>(&NativePtr->WindowTitleAlign);
    public ref ImGuiDir WindowMenuButtonPosition => ref Unsafe.AsRef<ImGuiDir>(&NativePtr->WindowMenuButtonPosition);
    public ref float ChildRounding => ref Unsafe.AsRef<float>(&NativePtr->ChildRounding);
    public ref float ChildBorderSize => ref Unsafe.AsRef<float>(&NativePtr->ChildBorderSize);
    public ref float PopupRounding => ref Unsafe.AsRef<float>(&NativePtr->PopupRounding);
    public ref float PopupBorderSize => ref Unsafe.AsRef<float>(&NativePtr->PopupBorderSize);
    public ref Vector2 FramePadding => ref Unsafe.AsRef<Vector2>(&NativePtr->FramePadding);
    public ref float FrameRounding => ref Unsafe.AsRef<float>(&NativePtr->FrameRounding);
    public ref float FrameBorderSize => ref Unsafe.AsRef<float>(&NativePtr->FrameBorderSize);
    public ref Vector2 ItemSpacing => ref Unsafe.AsRef<Vector2>(&NativePtr->ItemSpacing);
    public ref Vector2 ItemInnerSpacing => ref Unsafe.AsRef<Vector2>(&NativePtr->ItemInnerSpacing);
    public ref Vector2 CellPadding => ref Unsafe.AsRef<Vector2>(&NativePtr->CellPadding);
    public ref Vector2 TouchExtraPadding => ref Unsafe.AsRef<Vector2>(&NativePtr->TouchExtraPadding);
    public ref float IndentSpacing => ref Unsafe.AsRef<float>(&NativePtr->IndentSpacing);
    public ref float ColumnsMinSpacing => ref Unsafe.AsRef<float>(&NativePtr->ColumnsMinSpacing);
    public ref float ScrollbarSize => ref Unsafe.AsRef<float>(&NativePtr->ScrollbarSize);
    public ref float ScrollbarRounding => ref Unsafe.AsRef<float>(&NativePtr->ScrollbarRounding);
    public ref float GrabMinSize => ref Unsafe.AsRef<float>(&NativePtr->GrabMinSize);
    public ref float GrabRounding => ref Unsafe.AsRef<float>(&NativePtr->GrabRounding);
    public ref float LogSliderDeadzone => ref Unsafe.AsRef<float>(&NativePtr->LogSliderDeadzone);
    public ref float TabRounding => ref Unsafe.AsRef<float>(&NativePtr->TabRounding);
    public ref float TabBorderSize => ref Unsafe.AsRef<float>(&NativePtr->TabBorderSize);
    public ref float TabMinWidthForCloseButton => ref Unsafe.AsRef<float>(&NativePtr->TabMinWidthForCloseButton);
    public ref float TabBarBorderSize => ref Unsafe.AsRef<float>(&NativePtr->TabBarBorderSize);
    public ref float TabBarOverlineSize => ref Unsafe.AsRef<float>(&NativePtr->TabBarOverlineSize);
    public ref float TableAngledHeadersAngle => ref Unsafe.AsRef<float>(&NativePtr->TableAngledHeadersAngle);
    public ref Vector2 TableAngledHeadersTextAlign => ref Unsafe.AsRef<Vector2>(&NativePtr->TableAngledHeadersTextAlign);
    public ref ImGuiDir ColorButtonPosition => ref Unsafe.AsRef<ImGuiDir>(&NativePtr->ColorButtonPosition);
    public ref Vector2 ButtonTextAlign => ref Unsafe.AsRef<Vector2>(&NativePtr->ButtonTextAlign);
    public ref Vector2 SelectableTextAlign => ref Unsafe.AsRef<Vector2>(&NativePtr->SelectableTextAlign);
    public ref float SeparatorTextBorderSize => ref Unsafe.AsRef<float>(&NativePtr->SeparatorTextBorderSize);
    public ref Vector2 SeparatorTextAlign => ref Unsafe.AsRef<Vector2>(&NativePtr->SeparatorTextAlign);
    public ref Vector2 SeparatorTextPadding => ref Unsafe.AsRef<Vector2>(&NativePtr->SeparatorTextPadding);
    public ref Vector2 DisplayWindowPadding => ref Unsafe.AsRef<Vector2>(&NativePtr->DisplayWindowPadding);
    public ref Vector2 DisplaySafeAreaPadding => ref Unsafe.AsRef<Vector2>(&NativePtr->DisplaySafeAreaPadding);
    public ref float DockingSeparatorSize => ref Unsafe.AsRef<float>(&NativePtr->DockingSeparatorSize);
    public ref float MouseCursorScale => ref Unsafe.AsRef<float>(&NativePtr->MouseCursorScale);
    public ref bool AntiAliasedLines => ref Unsafe.AsRef<bool>(&NativePtr->AntiAliasedLines);
    public ref bool AntiAliasedLinesUseTex => ref Unsafe.AsRef<bool>(&NativePtr->AntiAliasedLinesUseTex);
    public ref bool AntiAliasedFill => ref Unsafe.AsRef<bool>(&NativePtr->AntiAliasedFill);
    public ref float CurveTessellationTol => ref Unsafe.AsRef<float>(&NativePtr->CurveTessellationTol);
    public ref float CircleTessellationMaxError => ref Unsafe.AsRef<float>(&NativePtr->CircleTessellationMaxError);
    public ref float HoverStationaryDelay => ref Unsafe.AsRef<float>(&NativePtr->HoverStationaryDelay);
    public ref float HoverDelayShort => ref Unsafe.AsRef<float>(&NativePtr->HoverDelayShort);
    public ref float HoverDelayNormal => ref Unsafe.AsRef<float>(&NativePtr->HoverDelayNormal);
    public ref ImGuiHoveredFlags HoverFlagsForTooltipMouse => ref Unsafe.AsRef<ImGuiHoveredFlags>(&NativePtr->HoverFlagsForTooltipMouse);
    public ref ImGuiHoveredFlags HoverFlagsForTooltipNav => ref Unsafe.AsRef<ImGuiHoveredFlags>(&NativePtr->HoverFlagsForTooltipNav);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void GetColors(out RangeAccessorRef<Vector4, ImGuiCol> colors) => colors = new(&NativePtr->Colors_0, ImGuiCol.COUNT);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ScaleAllSizes(float scaleFactor) => ImGui.ImGuiStyle_ScaleAllSizes(NativePtr, scaleFactor);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Destroy() => ImGui.ImGuiStyle_destroy(NativePtr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose() => Destroy();
}