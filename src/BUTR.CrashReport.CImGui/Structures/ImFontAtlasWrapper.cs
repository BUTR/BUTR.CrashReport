using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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
    public void AddFontFromMemoryTTF<T>(Span<T> fontData, float size_pixels, ImFontConfigWrapper config, out ImGuiNET.ImFontPtr imFontPtr) where T : unmanaged
    {
        var font_data_size = fontData.Length;
        ushort* glyph_ranges = null;
        fixed (T* font_data = fontData)
        {
            imFontPtr = new ImGuiNET.ImFontPtr((ImGuiNET.ImFont*) ImGui.ImFontAtlas_AddFontFromMemoryTTF(NativePtr, font_data, font_data_size, size_pixels, config.NativePtr, glyph_ranges));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddFontFromMemoryCompressedTTF<T>(Span<T> fontData, float size_pixels, ImFontConfigWrapper config, out ImGuiNET.ImFontPtr imFontPtr) where T : unmanaged
    {
        var font_data_size = fontData.Length;
        ushort* glyph_ranges = null;
        fixed (T* font_data = fontData)
        {
            imFontPtr = new ImGuiNET.ImFontPtr((ImGuiNET.ImFont*) ImGui.ImFontAtlas_AddFontFromMemoryCompressedTTF(NativePtr, font_data, font_data_size, size_pixels, config.NativePtr, glyph_ranges));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddFontDefault(ImFontConfigWrapper config, out ImGuiNET.ImFontPtr imFontPtr) => imFontPtr = new((ImGuiNET.ImFont*) ImGui.ImFontAtlas_AddFontDefault(NativePtr, config.NativePtr));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void GetTexDataAsRGBA32(out IntPtr out_pixels, out int out_width, out int out_height, out int out_bytes_per_pixel)
    {
        fixed (IntPtr* out_pixelsPtr = &out_pixels)
        fixed (int* out_widthPtr = &out_width)
        fixed (int* out_heightPtr = &out_height)
        fixed (int* out_bytes_per_pixelPtr = &out_bytes_per_pixel)
        {
            ImGui.ImFontAtlas_GetTexDataAsRGBA32(NativePtr, out_pixelsPtr, out_widthPtr, out_heightPtr, out_bytes_per_pixelPtr);
        }
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