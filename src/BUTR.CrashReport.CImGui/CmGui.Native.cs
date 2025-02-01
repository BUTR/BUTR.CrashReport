using System.Numerics;
using System.Runtime.InteropServices;

namespace ImGui;

partial class CmGui
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte igBeginDel(IntPtrByte name, IntPtrByte p_open, ImGuiNET.ImGuiWindowFlags flags);
    public igBeginDel igBegin = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte igBeginChild_StrDel(IntPtrByte str_id, Vector2 size, ImGuiNET.ImGuiChildFlags flags, ImGuiNET.ImGuiWindowFlags flags_ex);
    public igBeginChild_StrDel igBeginChild_Str = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte igBeginTableDel(IntPtrByte str_id, int column, ImGuiNET.ImGuiTableFlags flags, Vector2 outer_size, float inner_width);
    public igBeginTableDel igBeginTable = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igBulletDel();
    public igBulletDel igBullet = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte igButtonDel(IntPtrByte label, Vector2 size);
    public igButtonDel igButton = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte igCheckboxDel(IntPtrByte label, IntPtrByte v);
    public igCheckboxDel igCheckbox = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtrVoid igCreateContextDel(IntPtrImFontAtlas shared_font_atlas);
    public igCreateContextDel igCreateContext = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igDestroyContextDel(IntPtrVoid ctx);
    public igDestroyContextDel igDestroyContext = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igEndDel();
    public igEndDel igEnd = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igEndChildDel();
    public igEndChildDel igEndChild = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igEndTableDel();
    public igEndTableDel igEndTable = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtrVoid igGetCurrentContextDel();
    public igGetCurrentContextDel igGetCurrentContext = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtrImDrawData igGetDrawDataDel();
    public igGetDrawDataDel igGetDrawData = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtrImGuiIO igGetIODel();
    public igGetIODel igGetIO = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtrImGuiViewport igGetMainViewportDel();
    public igGetMainViewportDel igGetMainViewport = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate ImGuiNET.ImGuiMouseCursor igGetMouseCursorDel();
    public igGetMouseCursorDel igGetMouseCursor = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtrImGuiStyle igGetStyleDel();
    public igGetStyleDel igGetStyle = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate float igGetTextLineHeightDel();
    public igGetTextLineHeightDel igGetTextLineHeight = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igIndentDel(float indent_w);
    public igIndentDel igIndent = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte igInputTextDel(IntPtrByte label, IntPtrByte buf, uint buf_size, ImGuiNET.ImGuiInputTextFlags flags, IntPtrVoid callback, IntPtrVoid user_data);
    public igInputTextDel igInputText = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte igInputTextMultilineDel(IntPtrByte label, IntPtrByte buf, uint buf_size, Vector2 size, ImGuiNET.ImGuiInputTextFlags flags, IntPtrVoid callback, IntPtrVoid user_data);
    public igInputTextMultilineDel igInputTextMultiline = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igNewFrameDel();
    public igNewFrameDel igNewFrame = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igNewLineDel();
    public igNewLineDel igNewLine = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igPopStyleColorDel(int count);
    public igPopStyleColorDel igPopStyleColor = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igPopStyleVarDel(int count);
    public igPopStyleVarDel igPopStyleVar = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igPushStyleColor_Vec4Del(ImGuiNET.ImGuiCol idx, Vector4 col);
    public igPushStyleColor_Vec4Del igPushStyleColor_Vec4 = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igPushStyleVar_FloatDel(ImGuiNET.ImGuiStyleVar idx, float val);
    public igPushStyleVar_FloatDel igPushStyleVar_Float = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igPushStyleVar_Vec2Del(ImGuiNET.ImGuiStyleVar idx, Vector2 val);
    public igPushStyleVar_Vec2Del igPushStyleVar_Vec2 = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igRenderDel();
    public igRenderDel igRender = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igSameLineDel(float offset_from_start_x, float spacing);
    public igSameLineDel igSameLine = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igSeparatorDel();
    public igSeparatorDel igSeparator = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igSetCurrentContextDel(IntPtrVoid ctx);
    public igSetCurrentContextDel igSetCurrentContext = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igSetNextWindowPosDel(Vector2 pos, ImGuiNET.ImGuiCond cond, Vector2 pivot);
    public igSetNextWindowPosDel igSetNextWindowPos = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igSetNextWindowSizeDel(Vector2 size, ImGuiNET.ImGuiCond cond);
    public igSetNextWindowSizeDel igSetNextWindowSize = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igSetNextWindowViewportDel(uint viewport_id);
    public igSetNextWindowViewportDel igSetNextWindowViewport = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igSetWindowFontScaleDel(float scale);
    public igSetWindowFontScaleDel igSetWindowFontScale = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte igSmallButtonDel(IntPtrByte label);
    public igSmallButtonDel igSmallButton = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igStyleColorsDarkDel(IntPtrImGuiStyle dst);
    public igStyleColorsDarkDel igStyleColorsDark = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igStyleColorsLightDel(IntPtrImGuiStyle dst);
    public igStyleColorsLightDel igStyleColorsLight = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte igTableNextColumnDel();
    public igTableNextColumnDel igTableNextColumn = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igTextUnformattedDel(IntPtrByte text, IntPtrByte text_end);
    public igTextUnformattedDel igTextUnformatted = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igPushTextWrapPosDel(float wrap_local_pos_x);
    public igPushTextWrapPosDel igPushTextWrapPos = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igGetContentRegionAvailDel(IntPtrVector2 pOut);
    public igGetContentRegionAvailDel igGetContentRegionAvail = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igGetWindowPosDel(IntPtrVector2 pOut);
    public igGetWindowPosDel igGetWindowPos = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igPopTextWrapPosDel();
    public igPopTextWrapPosDel igPopTextWrapPos = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igTextLinkOpenURLDel(IntPtrByte text, IntPtrByte url);
    public igTextLinkOpenURLDel igTextLinkOpenURL = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte igTreeNodeEx_StrDel(IntPtrByte label, ImGuiNET.ImGuiTreeNodeFlags flags);
    public igTreeNodeEx_StrDel igTreeNodeEx_Str = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igTreePopDel();
    public igTreePopDel igTreePop = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igUnindentDel(float indent_w);
    public igUnindentDel igUnindent = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ImGuiIO_AddInputCharacterDel(IntPtrImGuiIO io, uint c);
    public ImGuiIO_AddInputCharacterDel ImGuiIO_AddInputCharacter = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ImGuiIO_AddKeyEventDel(IntPtrImGuiIO io, ImGuiNET.ImGuiKey key, byte down);
    public ImGuiIO_AddKeyEventDel ImGuiIO_AddKeyEvent = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ImGuiIO_SetKeyEventNativeDataDel(IntPtrImGuiIO io, ImGuiNET.ImGuiKey key, int native_keycode, int native_scancode, int native_state);
    public ImGuiIO_SetKeyEventNativeDataDel ImGuiIO_SetKeyEventNativeData = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ImGuiIO_AddMouseButtonEventDel(IntPtrImGuiIO io, ImGuiNET.ImGuiMouseButton button, byte down);
    public ImGuiIO_AddMouseButtonEventDel ImGuiIO_AddMouseButtonEvent = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ImGuiIO_AddMouseWheelEventDel(IntPtrImGuiIO io, float wheel_x, float wheel_y);
    public ImGuiIO_AddMouseWheelEventDel ImGuiIO_AddMouseWheelEvent = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ImGuiIO_AddMousePosEventDel(IntPtrImGuiIO io, float x, float y);
    public ImGuiIO_AddMousePosEventDel ImGuiIO_AddMousePosEvent = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ImGuiIO_AddFocusEventDel(IntPtrImGuiIO io, byte focused);
    public ImGuiIO_AddFocusEventDel ImGuiIO_AddFocusEvent = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ImGuiIO_destroyDel(IntPtrImGuiIO io);
    public ImGuiIO_destroyDel ImGuiIO_destroy = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ImFontAtlas_GetTexDataAsRGBA32Del(IntPtrImFontAtlas atlas, IntPtrIntPtr out_pixels, IntPtrInt32 out_width, IntPtrInt32 out_height, IntPtrInt32 out_bytes_per_pixel);
    public ImFontAtlas_GetTexDataAsRGBA32Del ImFontAtlas_GetTexDataAsRGBA32 = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ImFontAtlas_SetTexIDDel(IntPtrImFontAtlas atlas, IntPtrVoid id);
    public ImFontAtlas_SetTexIDDel ImFontAtlas_SetTexID = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ImFontAtlas_ClearTexDataDel(IntPtrImFontAtlas atlas);
    public ImFontAtlas_ClearTexDataDel ImFontAtlas_ClearTexData = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtrImFont ImFontAtlas_AddFontDefaultDel(IntPtrImFontAtlas atlas, IntPtrImFontConfig font_cfg);
    public ImFontAtlas_AddFontDefaultDel ImFontAtlas_AddFontDefault = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtrImFont ImFontAtlas_AddFontFromMemoryTTFDel(IntPtrImFontAtlas atlas, IntPtrVoid font_data, int font_data_size, float size_pixels, IntPtrImFontConfig font_cfg, IntPtrUInt16 glyph_ranges);
    public ImFontAtlas_AddFontFromMemoryTTFDel ImFontAtlas_AddFontFromMemoryTTF = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtrImFont ImFontAtlas_AddFontFromMemoryCompressedTTFDel(IntPtrImFontAtlas atlas, IntPtrVoid compressed_font_data, int compressed_font_data_size, float size_pixels, IntPtrImFontConfig font_cfg, IntPtrUInt16 glyph_ranges);
    public ImFontAtlas_AddFontFromMemoryCompressedTTFDel ImFontAtlas_AddFontFromMemoryCompressedTTF = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ImFontAtlas_destroyDel(IntPtrImFontAtlas atlas);
    public ImFontAtlas_destroyDel ImFontAtlas_destroy = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtrImFontConfig ImFontConfig_ImFontConfigDel();
    public ImFontConfig_ImFontConfigDel ImFontConfig_ImFontConfig = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ImFontConfig_destroyDel(IntPtrImFontConfig font_cfg);
    public ImFontConfig_destroyDel ImFontConfig_destroy = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtrImGuiListClipper ImGuiListClipper_ImGuiListClipperDel();
    public ImGuiListClipper_ImGuiListClipperDel ImGuiListClipper_ImGuiListClipper = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ImGuiListClipper_BeginDel(IntPtrImGuiListClipper clipper, int items_count, float items_height);
    public ImGuiListClipper_BeginDel ImGuiListClipper_Begin = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ImGuiListClipper_EndDel(IntPtrImGuiListClipper clipper);
    public ImGuiListClipper_EndDel ImGuiListClipper_End = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte ImGuiListClipper_StepDel(IntPtrImGuiListClipper clipper);
    public ImGuiListClipper_StepDel ImGuiListClipper_Step = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ImGuiListClipper_destroyDel(IntPtrImGuiListClipper clipper);
    public ImGuiListClipper_destroyDel ImGuiListClipper_destroy = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ImGuiStyle_ScaleAllSizesDel(IntPtrImGuiStyle style, float scale_factor);
    public ImGuiStyle_ScaleAllSizesDel ImGuiStyle_ScaleAllSizes = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ImGuiStyle_destroyDel(IntPtrImGuiStyle style);
    public ImGuiStyle_destroyDel ImGuiStyle_destroy = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ImDrawData_AddDrawListDel(IntPtrImDrawData draw_data, IntPtrImDrawList draw_list);
    public ImDrawData_AddDrawListDel ImDrawData_AddDrawList = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ImDrawData_ClearDel(IntPtrImDrawData draw_data);
    public ImDrawData_ClearDel ImDrawData_Clear = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ImDrawData_DeIndexAllBuffersDel(IntPtrImDrawData draw_data);
    public ImDrawData_DeIndexAllBuffersDel ImDrawData_DeIndexAllBuffers = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ImDrawData_ScaleClipRectsDel(IntPtrImDrawData draw_data, Vector2 scale);
    public ImDrawData_ScaleClipRectsDel ImDrawData_ScaleClipRects = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ImDrawData_destroyDel(IntPtrImDrawData draw_data);
    public ImDrawData_destroyDel ImDrawData_destroy = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ImDrawList_AddText_Vec2Del(IntPtrImDrawList draw_list, Vector2 pos, uint col, IntPtrByte text_begin, IntPtrByte text_end);
    public ImDrawList_AddText_Vec2Del ImDrawList_AddText_Vec2 = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ImDrawList_AddRectDel(IntPtrImDrawList draw_list, Vector2 p_min, Vector2 p_max, uint col, float rounding, ImGuiNET.ImDrawFlags flags, float thickness);
    public ImDrawList_AddRectDel ImDrawList_AddRect = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ImDrawList_AddRectFilledDel(IntPtrImDrawList draw_list, Vector2 p_min, Vector2 p_max, uint col, float rounding, ImGuiNET.ImDrawFlags flags);
    public ImDrawList_AddRectFilledDel ImDrawList_AddRectFilled = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igOpenPopup_StrDel(IntPtrByte str_id, ImGuiNET.ImGuiPopupFlags flags);
    public igOpenPopup_StrDel igOpenPopup_Str = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igCloseCurrentPopupDel();
    public igCloseCurrentPopupDel igCloseCurrentPopup = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte igSelectable_BoolDel(IntPtrByte label, byte selected, ImGuiNET.ImGuiSelectableFlags flags, Vector2 size);
    public igSelectable_BoolDel igSelectable_Bool = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte igIsMouseDoubleClicked_NilDel(ImGuiNET.ImGuiMouseButton button);
    public igIsMouseDoubleClicked_NilDel igIsMouseDoubleClicked_Nil = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte igBeginPopupDel(IntPtrByte str_id, ImGuiNET.ImGuiWindowFlags flags);
    public igBeginPopupDel igBeginPopup = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte igBeginPopupModalDel(IntPtrByte name, IntPtrByte p_open, ImGuiNET.ImGuiWindowFlags flags);
    public igBeginPopupModalDel igBeginPopupModal = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte igBeginPopupContextWindowDel(IntPtrByte str_id, ImGuiNET.ImGuiPopupFlags flags);
    public igBeginPopupContextWindowDel igBeginPopupContextWindow = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igEndPopupDel();
    public igEndPopupDel igEndPopup = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte igIsWindowAppearingDel();
    public igIsWindowAppearingDel igIsWindowAppearing = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtrVoid igGetCurrentWindowDel();
    public igGetCurrentWindowDel igGetCurrentWindow = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igBringWindowToDisplayFrontDel(IntPtrVoid window);
    public igBringWindowToDisplayFrontDel igBringWindowToDisplayFront = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte igMenuItem_BoolDel(IntPtrByte label, IntPtrByte shortcut, byte selected, byte enabled);
    public igMenuItem_BoolDel igMenuItem_Bool = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte igIsItemClickedDel(ImGuiNET.ImGuiMouseButton mouse_button);
    public igIsItemClickedDel igIsItemClicked = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte igIsKeyDown_NilDel(ImGuiNET.ImGuiKey key);
    public igIsKeyDown_NilDel igIsKeyDown_Nil = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte igIsKeyPressed_BoolDel(ImGuiNET.ImGuiKey key, byte repeat);
    public igIsKeyPressed_BoolDel igIsKeyPressed_Bool = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igSetClipboardTextDel(IntPtrByte text);
    public igSetClipboardTextDel igSetClipboardText = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igPushID_StrDel(IntPtrByte str_id);
    public igPushID_StrDel igPushID_Str = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igPushID_IntDel(int int_id);
    public igPushID_IntDel igPushID_Int = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igPopIDDel();
    public igPopIDDel igPopID = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate float igGetTextLineHeightWithSpacingDel();
    public igGetTextLineHeightWithSpacingDel igGetTextLineHeightWithSpacing = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igCalcTextSizeDel(IntPtrVector2 out_size, IntPtrByte text_begin, IntPtrByte text_end, byte hide_text_after_double_hash, float wrap_width);
    public igCalcTextSizeDel igCalcTextSize = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte igBeginTooltipDel();
    public igBeginTooltipDel igBeginTooltip = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igEndTooltipDel();
    public igEndTooltipDel igEndTooltip = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte igIsMouseHoveringRectDel(Vector2 r_min, Vector2 r_max, byte clip);
    public igIsMouseHoveringRectDel igIsMouseHoveringRect = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igDummyDel(Vector2 size);
    public igDummyDel igDummy = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte igIsMouseDown_NilDel(ImGuiNET.ImGuiMouseButton button);
    public igIsMouseDown_NilDel igIsMouseDown_Nil = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte igIsMouseDraggingDel(ImGuiNET.ImGuiMouseButton button, float lock_threshold);
    public igIsMouseDraggingDel igIsMouseDragging = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igGetMousePosDel(IntPtrVector2 out_pos);
    public igGetMousePosDel igGetMousePos = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte igIsMouseClicked_BoolDel(ImGuiNET.ImGuiMouseButton button, byte repeat);
    public igIsMouseClicked_BoolDel igIsMouseClicked_Bool = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igSetMouseCursorDel(ImGuiNET.ImGuiMouseCursor cursor_type);
    public igSetMouseCursorDel igSetMouseCursor = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igGetCursorScreenPosDel(IntPtrVector2 out_pos);
    public igGetCursorScreenPosDel igGetCursorScreenPos = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate float igGetScrollXDel();
    public igGetScrollXDel igGetScrollX = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate float igGetScrollYDel();
    public igGetScrollYDel igGetScrollY = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igSetScrollX_FloatDel(float scroll_x);
    public igSetScrollX_FloatDel igSetScrollX_Float = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igSetScrollY_FloatDel(float scroll_y);
    public igSetScrollY_FloatDel igSetScrollY_Float = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate uint igColorConvertFloat4ToU32Del(Vector4 col);
    public igColorConvertFloat4ToU32Del igColorConvertFloat4ToU32 = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igColorConvertU32ToFloat4Del(IntPtrVector4 out_col, uint in_col);
    public igColorConvertU32ToFloat4Del igColorConvertU32ToFloat4 = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void igSetWindowFocus_NilDel();
    public igSetWindowFocus_NilDel igSetWindowFocus_Nil = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate float igGetWindowWidthDel();
    public igGetWindowWidthDel igGetWindowWidth = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate float igGetWindowHeightDel();
    public igGetWindowHeightDel igGetWindowHeight = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte igIsWindowHoveredDel(ImGuiNET.ImGuiHoveredFlags flags);
    public igIsWindowHoveredDel igIsWindowHovered = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte igIsWindowFocusedDel(ImGuiNET.ImGuiFocusedFlags flags);
    public igIsWindowFocusedDel igIsWindowFocused = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtrImDrawList igGetWindowDrawListDel();
    public igGetWindowDrawListDel igGetWindowDrawList = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtrVoid igMemAllocDel(uint size);
    public igMemAllocDel igMemAlloc = null!;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ImGuiIO_AddInputCharactersUTF8Del(IntPtrImGuiIO self, IntPtrByte str);
    public ImGuiIO_AddInputCharactersUTF8Del ImGuiIO_AddInputCharactersUTF8 = null!;
}