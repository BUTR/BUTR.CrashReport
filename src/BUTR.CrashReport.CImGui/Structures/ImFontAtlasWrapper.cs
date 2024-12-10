using System.Runtime.CompilerServices;

namespace ImGui.Structures;

public readonly unsafe ref struct ImFontAtlasWrapper
{
    private CmGui ImGui { get; }

    public ImGuiNET.ImFontAtlas* NativePtr { get; }

    public ImFontAtlasWrapper(CmGui imGui, ImGuiNET.ImFontAtlas* nativePtr)
    {
        ImGui = imGui;
        NativePtr = nativePtr;
    }

    public ref IntPtr TexID => ref Unsafe.AsRef<IntPtr>(&NativePtr->TexID);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddFontDefault(out ImGuiNET.ImFontPtr imFontPtr)
    {
        ImGuiNET.ImFontConfig* font_cfg = null;
        imFontPtr = new ImGuiNET.ImFontPtr((ImGuiNET.ImFont*) ImGui.ImFontAtlas_AddFontDefault(NativePtr, font_cfg));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddFontDefault(ImFontConfigWrapper config, out ImGuiNET.ImFontPtr imFontPtr) => imFontPtr = new((ImGuiNET.ImFont*) ImGui.ImFontAtlas_AddFontDefault(NativePtr, config.NativePtr));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void GetTexDataAsRGBA32(out IntPtr out_pixels, out int out_width, out int out_height, out int out_bytes_per_pixel)
    {
        Unsafe.SkipInit(out out_pixels);
        Unsafe.SkipInit(out out_width);
        Unsafe.SkipInit(out out_height);
        Unsafe.SkipInit(out out_bytes_per_pixel);
        ImGui.ImFontAtlas_GetTexDataAsRGBA32(
            NativePtr,
            (IntPtr*) Unsafe.AsPointer(ref out_pixels),
            (int*) Unsafe.AsPointer(ref out_width),
            (int*) Unsafe.AsPointer(ref out_height),
            (int*) Unsafe.AsPointer(ref out_bytes_per_pixel));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetTexID(IntPtr id) => ImGui.ImFontAtlas_SetTexID(NativePtr, id);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ClearTexData() => ImGui.ImFontAtlas_ClearTexData(NativePtr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Destroy() => ImGui.ImFontAtlas_destroy(NativePtr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose() => Destroy();
}