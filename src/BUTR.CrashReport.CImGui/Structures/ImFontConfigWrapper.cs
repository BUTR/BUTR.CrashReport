using System.Numerics;
using System.Runtime.CompilerServices;

namespace ImGui.Structures;

public readonly unsafe ref struct ImFontConfigWrapper
{
    private CmGui ImGui { get; }

    public ImGuiNET.ImFontConfig* NativePtr { get; }

    public ImFontConfigWrapper(CmGui imGui, ImGuiNET.ImFontConfig* nativePtr)
    {
        ImGui = imGui;
        NativePtr = nativePtr;
    }

    public IntPtr FontData { get => (IntPtr) NativePtr->FontData; set => NativePtr->FontData = (void*) value; }
    public ref int FontDataSize => ref Unsafe.AsRef<int>(&NativePtr->FontDataSize);
    public ref bool FontDataOwnedByAtlas => ref Unsafe.AsRef<bool>(&NativePtr->FontDataOwnedByAtlas);
    public ref int FontNo => ref Unsafe.AsRef<int>(&NativePtr->FontNo);
    public ref float SizePixels => ref Unsafe.AsRef<float>(&NativePtr->SizePixels);
    public ref int OversampleH => ref Unsafe.AsRef<int>(&NativePtr->OversampleH);
    public ref int OversampleV => ref Unsafe.AsRef<int>(&NativePtr->OversampleV);
    public ref bool PixelSnapH => ref Unsafe.AsRef<bool>(&NativePtr->PixelSnapH);
    public ref Vector2 GlyphExtraSpacing => ref Unsafe.AsRef<Vector2>(&NativePtr->GlyphExtraSpacing);
    public ref Vector2 GlyphOffset => ref Unsafe.AsRef<Vector2>(&NativePtr->GlyphOffset);
    public ref IntPtr GlyphRanges => ref Unsafe.AsRef<IntPtr>(&NativePtr->GlyphRanges);
    public ref float GlyphMinAdvanceX => ref Unsafe.AsRef<float>(&NativePtr->GlyphMinAdvanceX);
    public ref float GlyphMaxAdvanceX => ref Unsafe.AsRef<float>(&NativePtr->GlyphMaxAdvanceX);
    public ref bool MergeMode => ref Unsafe.AsRef<bool>(&NativePtr->MergeMode);
    public ref uint FontBuilderFlags => ref Unsafe.AsRef<uint>(&NativePtr->FontBuilderFlags);
    public ref float RasterizerMultiply => ref Unsafe.AsRef<float>(&NativePtr->RasterizerMultiply);
    public ref float RasterizerDensity => ref Unsafe.AsRef<float>(&NativePtr->RasterizerDensity);
    public ref ushort EllipsisChar => ref Unsafe.AsRef<ushort>(&NativePtr->EllipsisChar);
    public RangeAccessorRef<byte> Name => new(NativePtr->Name, 40);
    public ImGuiNET.ImFontPtr DstFont => new(NativePtr->DstFont);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Destroy() => ImGui.ImFontConfig_destroy(NativePtr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose() => Destroy();
}