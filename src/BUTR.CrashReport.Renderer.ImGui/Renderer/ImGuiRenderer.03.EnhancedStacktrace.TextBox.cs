#if !TEXT_EDITOR
using BUTR.CrashReport.Models;

namespace BUTR.CrashReport.Renderer.ImGui.Renderer;

partial class ImGuiRenderer<TImGuiIORef, TImGuiViewportRef, TImDrawListRef, TImGuiStyleRef, TColorsRangeAccessorRef, TImGuiListClipperRef>
{
    private readonly Dictionary<MethodModel, byte[][]> _methodCodeLinesUtf8 = new(MethodEqualityComparer.Instance);

    private static void SetCodeDictionary<TValue>(IDictionary<MethodModel, TValue[]> methodDict, MethodModel key, CodeType codeType, TValue value)
    {
        if (!methodDict.TryGetValue(key, out var codeArray))
            methodDict[key] = codeArray = new TValue[(int) CodeType.Native + 1];

        codeArray[(int) codeType] = value;
    }
}
#endif