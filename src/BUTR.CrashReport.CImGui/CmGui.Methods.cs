using BUTR.CrashReport.ImGui.Enums;
using BUTR.CrashReport.ImGui.Structures;
using BUTR.CrashReport.Native;

using ImGui.Structures;

using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ImGui;

unsafe partial class CmGui
{
    private readonly struct UserDataGeneric
    {
        public readonly Func<Pointer<ImGuiNET.ImGuiInputTextCallbackData>, int> GenericTDataConverter;
        public readonly Delegate Callback;
        public readonly void* DataPtr;

        public UserDataGeneric(Func<Pointer<ImGuiNET.ImGuiInputTextCallbackData>, int> genericTDataConverter, Delegate callback, void* dataPtr)
        {
            GenericTDataConverter = genericTDataConverter;
            Callback = callback;
            DataPtr = dataPtr;
        }
    }
    private readonly struct UserDataInt64
    {
        public readonly Delegate Callback;
        public readonly Int64 Data;

        public UserDataInt64(Delegate callback, Int64 data)
        {
            Callback = callback;
            Data = data;
        }
    }

    private readonly Dictionary<Type, Func<Pointer<ImGuiNET.ImGuiInputTextCallbackData>, int>> _callbacks = new();

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static int ImGuiInputTextCallbackGeneric(ImGuiNET.ImGuiInputTextCallbackData* callbackData)
    {
        if (callbackData->UserData == null)
            return 0;

        ref var userData = ref Unsafe.AsRef<UserDataGeneric>(callbackData->UserData);
        var genericTDataConverter = userData.GenericTDataConverter;

        return genericTDataConverter((IntPtr) callbackData);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static int ImGuiInputTextCallbackInt64(ImGuiNET.ImGuiInputTextCallbackData* callbackData)
    {
        if (callbackData->UserData == null)
            return 0;

        ref var userData = ref Unsafe.AsRef<UserDataInt64>(callbackData->UserData);
        var data = userData.Data;
        var managedCallback = (ImGuiInputTextInt64Callback) userData.Callback;
        return managedCallback(new ImGuiInputTextCallbackInt64Data
        {
            EventFlag = (ImGuiInputTextFlags) callbackData->EventFlag,
            Flags = (ImGuiInputTextFlags) callbackData->Flags,
            EventChar = callbackData->EventChar,
            EventKey = (ImGuiKey) callbackData->EventKey,
            CursorPos = callbackData->CursorPos,
            SelectionStart = callbackData->SelectionStart,
            SelectionEnd = callbackData->SelectionEnd,
            Data = data,
        });
    }

    private static int GenericTDataCallback<TData>(Pointer<ImGuiNET.ImGuiInputTextCallbackData> callbackDataPtr) where TData : struct
    {
        var callbackData = (ImGuiNET.ImGuiInputTextCallbackData*) callbackDataPtr;
        ref var userData = ref Unsafe.AsRef<UserDataGeneric>(callbackData->UserData);
        ref var managedData = ref Unsafe.AsRef<TData>(userData.DataPtr);
        var managedCallback = (ImGuiInputTextCallback<TData>) userData.Callback;
        return managedCallback(new ImGuiInputTextCallbackData<TData>
        {
            EventFlag = (ImGuiInputTextFlags) callbackData->EventFlag,
            Flags = (ImGuiInputTextFlags) callbackData->Flags,
            EventChar = callbackData->EventChar,
            EventKey = (ImGuiKey) callbackData->EventKey,
            CursorPos = callbackData->CursorPos,
            SelectionStart = callbackData->SelectionStart,
            SelectionEnd = callbackData->SelectionEnd,
            Data = managedData,
        });
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void PushId(int id) => igPushID_Int(id);

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void PushId(ReadOnlySpan<byte> utf8Label) => igPushID_Str((byte*) Unsafe.AsPointer(ref MemoryMarshal.GetReference(utf8Label)));

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void PopId() => igPopID();

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool InputTextMultilineInt64(ReadOnlySpan<byte> utf8Label, Span<byte> input, int lineCount, ImGuiInputTextInt64Callback callback, Int64 data)
    {
        var size = new Vector2(-1, GetTextLineHeight() * (lineCount + 1));
        var flags = ImGuiInputTextFlags.ReadOnly | ImGuiInputTextFlags.CallbackAlways;

        PushStyleColor(ImGuiCol.FrameBg, in Zero4);

        delegate* unmanaged[Cdecl]<ImGuiNET.ImGuiInputTextCallbackData*, int> nativeCallbackPtr = &ImGuiInputTextCallbackInt64;

        var userData = new UserDataInt64(callback, data);

        // Lifetime of objects is bound to the function scope
        var result = igInputTextMultiline(
            (byte*) Unsafe.AsPointer(ref MemoryMarshal.GetReference(utf8Label)),
            (byte*) Unsafe.AsPointer(ref MemoryMarshal.GetReference(input)),
            (uint) input.Length,
            size,
            (ImGuiNET.ImGuiInputTextFlags) flags,
            (IntPtr) nativeCallbackPtr,
            (IntPtr) Unsafe.AsPointer(ref userData)) > 0;

        PopStyleColor();

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool InputTextMultiline<TData>(ReadOnlySpan<byte> utf8Label, Span<byte> input, int lineCount, ImGuiInputTextCallback<TData> callback, TData data)
        where TData : struct
    {
        if (!_callbacks.TryGetValue(typeof(TData), out var genericTDataCallback))
            _callbacks.Add(typeof(TData), genericTDataCallback = GenericTDataCallback<TData>);

        var size = new Vector2(-1, GetTextLineHeight() * (lineCount + 1));
        var flags = ImGuiInputTextFlags.ReadOnly | ImGuiInputTextFlags.CallbackAlways;

        PushStyleColor(ImGuiCol.FrameBg, in Zero4);

        delegate* unmanaged[Cdecl]<ImGuiNET.ImGuiInputTextCallbackData*, int> nativeCallbackPtr = &ImGuiInputTextCallbackGeneric;

        var userData = new UserDataGeneric(genericTDataCallback, callback, Unsafe.AsPointer(ref data));

        // Lifetime of objects is bound to the function scope
        var result = igInputTextMultiline(
            (byte*) Unsafe.AsPointer(ref MemoryMarshal.GetReference(utf8Label)),
            (byte*) Unsafe.AsPointer(ref MemoryMarshal.GetReference(input)),
            (uint) input.Length,
            size,
            (ImGuiNET.ImGuiInputTextFlags) flags,
            (IntPtr) nativeCallbackPtr,
            (IntPtr) Unsafe.AsPointer(ref userData)) > 0;

        PopStyleColor();

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool InputText(ReadOnlySpan<byte> utf8Label, Span<byte> input, ImGuiInputTextFlags flags)
    {
        var buf_size = input.Length;
        var user_data = IntPtr.Zero;

        PushStyleColor(ImGuiCol.FrameBg, in Zero4);

        var result = igInputText(
            (byte*) Unsafe.AsPointer(ref MemoryMarshal.GetReference(utf8Label)),
            (byte*) Unsafe.AsPointer(ref MemoryMarshal.GetReference(input)),
            (uint) buf_size,
            (ImGuiNET.ImGuiInputTextFlags) flags,
            IntPtr.Zero,
            user_data) > 0;

        PopStyleColor();

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void Text(ReadOnlySpan<byte> utf8Label) => igTextV((byte*) Unsafe.AsPointer(ref MemoryMarshal.GetReference(utf8Label)), null);

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void TextWrapped(ReadOnlySpan<byte> utf8Label) => igTextWrappedV((byte*) Unsafe.AsPointer(ref MemoryMarshal.GetReference(utf8Label)), null);

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void TextColored(ReadOnlySpan<byte> utf8Label, ref readonly Vector4 color) => igTextColoredV(color, (byte*) Unsafe.AsPointer(ref MemoryMarshal.GetReference(utf8Label)), null);

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void TextLinkOpenURL(ReadOnlySpan<byte> utf8Label, ReadOnlySpan<byte> utf8Url) => igTextLinkOpenURL((byte*) Unsafe.AsPointer(ref MemoryMarshal.GetReference(utf8Label)), (byte*) Unsafe.AsPointer(ref MemoryMarshal.GetReference(utf8Url)));

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool Checkbox(ReadOnlySpan<byte> utf8Label, ref bool isSelected) => igCheckbox((byte*) Unsafe.AsPointer(ref MemoryMarshal.GetReference(utf8Label)), (byte*) Unsafe.AsPointer(ref Unsafe.As<bool, byte>(ref isSelected))) > 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void OpenPopup(ReadOnlySpan<byte> utf8Label, ImGuiPopupFlags flags) => igOpenPopup_Str((byte*) Unsafe.AsPointer(ref MemoryMarshal.GetReference(utf8Label)), (ImGuiNET.ImGuiPopupFlags) flags);

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void CloseCurrentPopup() => igCloseCurrentPopup();

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool BeginPopup(ReadOnlySpan<byte> utf8Label, ImGuiWindowFlags flags) => igBeginPopup((byte*) Unsafe.AsPointer(ref MemoryMarshal.GetReference(utf8Label)), (ImGuiNET.ImGuiWindowFlags) flags) > 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool BeginPopupModal(ReadOnlySpan<byte> utf8Label, ImGuiWindowFlags flags) => igBeginPopupModal((byte*) Unsafe.AsPointer(ref MemoryMarshal.GetReference(utf8Label)), null, (ImGuiNET.ImGuiWindowFlags) flags) > 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool BeginPopupContextWindow(ReadOnlySpan<byte> utf8Label)
    {
        var popup_flags = ImGuiPopupFlags.MouseButtonRight;
        return igBeginPopupContextWindow((byte*) Unsafe.AsPointer(ref MemoryMarshal.GetReference(utf8Label)), (ImGuiNET.ImGuiPopupFlags) popup_flags) > 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void EndPopup() => igEndPopup();

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool Selectable(ReadOnlySpan<byte> utf8Label, ref bool isSelected, ImGuiSelectableFlags flags)
    {
        ref var selectedNum = ref Unsafe.As<bool, byte>(ref isSelected);
        return igSelectable_Bool((byte*) Unsafe.AsPointer(ref MemoryMarshal.GetReference(utf8Label)), selectedNum, (ImGuiNET.ImGuiSelectableFlags) flags, Zero2) > 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool IsMouseDoubleClicked(ImGuiMouseButton mouseButton) => igIsMouseDoubleClicked_Nil((ImGuiNET.ImGuiMouseButton) mouseButton) > 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool Button(ReadOnlySpan<byte> utf8Label) => igButton((byte*) Unsafe.AsPointer(ref MemoryMarshal.GetReference(utf8Label)), Zero2) > 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool SmallButton(ReadOnlySpan<byte> utf8Label) => igSmallButton((byte*) Unsafe.AsPointer(ref MemoryMarshal.GetReference(utf8Label))) > 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool Begin(ReadOnlySpan<byte> utf8Label, ImGuiWindowFlags flags)
    {
        var p_open = (byte*) null;
        return igBegin((byte*) Unsafe.AsPointer(ref MemoryMarshal.GetReference(utf8Label)), p_open, (ImGuiNET.ImGuiWindowFlags) flags) > 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool BeginTable(ReadOnlySpan<byte> utf8Label, int column)
    {
        var flags = ImGuiNET.ImGuiTableFlags.None;
        const float inner_width = 0.0f;
        return igBeginTable((byte*) Unsafe.AsPointer(ref MemoryMarshal.GetReference(utf8Label)), column, flags, Zero2, inner_width) > 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool BeginChild(ReadOnlySpan<byte> utf8Label, ref readonly Vector2 size, ImGuiChildFlags child_flags, ImGuiWindowFlags window_flags) => igBeginChild_Str((byte*) Unsafe.AsPointer(ref MemoryMarshal.GetReference(utf8Label)), size, (ImGuiNET.ImGuiChildFlags) child_flags, (ImGuiNET.ImGuiWindowFlags) window_flags) > 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool TreeNode(ReadOnlySpan<byte> utf8Label, ImGuiTreeNodeFlags flags) => igTreeNodeExV_Str(
        (byte*) Unsafe.AsPointer(ref MemoryMarshal.GetReference(utf8Label)),
        (ImGuiNET.ImGuiTreeNodeFlags) flags,
        (byte*) Unsafe.AsPointer(ref MemoryMarshal.GetReference(utf8Label)),
        null) > 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void PushStyleColor(ImGuiCol idx, ref readonly Vector4 color) => igPushStyleColor_Vec4((ImGuiNET.ImGuiCol) idx, color);

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void SetNextWindowPos(ref readonly Vector2 position) => igSetNextWindowPos(position, ImGuiNET.ImGuiCond.None, Zero2);

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void SetNextWindowSize(ref readonly Vector2 size) => igSetNextWindowSize(size, ImGuiNET.ImGuiCond.None);

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void SetNextWindowViewport(uint viewportId) => igSetNextWindowViewport(viewportId);


    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void TreePop() => igTreePop();

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void NewLine() => igNewLine();

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void StyleColorsLight() => igStyleColorsLight(IntPtr.Zero);

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void StyleColorsDark() => igStyleColorsDark(IntPtr.Zero);

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void SetWindowFontScale(float scale) => igSetWindowFontScale(scale);

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void EndChild() => igEndChild();

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void End() => igEnd();

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool TableNextColumn() => igTableNextColumn() > 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void SameLine() => igSameLine(0.0f, 0.0f);

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void EndTable() => igEndTable();

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void Separator() => igSeparator();

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void PushStyleVar(ImGuiStyleVar idx, float value) => igPushStyleVar_Float((ImGuiNET.ImGuiStyleVar) idx, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void PushStyleVar(ImGuiStyleVar idx, ref readonly Vector2 value) => igPushStyleVar_Vec2((ImGuiNET.ImGuiStyleVar) idx, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void Bullet() => igBullet();

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void Indent() => igIndent(0.0f);

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void Unindent() => igUnindent(0.0f);

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void SameLine(float offsetFromStartX, float spacing) => igSameLine(offsetFromStartX, spacing);

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void PopStyleVar() => igPopStyleVar(1);

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void PopStyleVar(int count) => igPopStyleVar(count);

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void PopStyleColor() => igPopStyleColor(1);

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void PopStyleColor(int count) => igPopStyleColor(count);

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public float GetTextLineHeight() => igGetTextLineHeight();

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public float GetTextLineHeightWithSpacing() => igGetTextLineHeightWithSpacing();


    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public IntPtr CreateContext() => (IntPtr) igCreateContext(null);

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public IntPtr GetCurrentContext() => (IntPtr) igGetCurrentContext();

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void SetCurrentContext(IntPtr ctx) => igSetCurrentContext(ctx);

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void Render() => igRender();

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void NewFrame() => igNewFrame();

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public ImGuiMouseCursor GetMouseCursor() => (ImGuiMouseCursor) igGetMouseCursor();

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void DestroyContext(IntPtr ctx) => igDestroyContext(ctx);


    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool IsWindowAppearing() => igIsWindowAppearing() > 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public IntPtr GetCurrentWindow() => (IntPtr) igGetCurrentWindow();

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void BringWindowToDisplayFront(IntPtr window) => igBringWindowToDisplayFront(window);

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool MenuItem(ReadOnlySpan<byte> utf8Label)
    {
        byte* shortcut = null;
        byte selected = 0;
        byte enabled = 1;
        return igMenuItem_Bool((byte*) Unsafe.AsPointer(ref MemoryMarshal.GetReference(utf8Label)), shortcut, selected, enabled) > 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool IsItemClicked(ImGuiMouseButton mouseButton) => igIsItemClicked((ImGuiNET.ImGuiMouseButton) mouseButton) > 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool IsKeyDown(ImGuiKey key) => igIsKeyDown_Nil((ImGuiNET.ImGuiKey) key) > 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool IsKeyPressed(ImGuiKey key) => igIsKeyPressed_Bool((ImGuiNET.ImGuiKey) key, 1) > 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void SetClipboardText(ReadOnlySpan<byte> utf8Data) => igSetClipboardText((byte*) Unsafe.AsPointer(ref MemoryMarshal.GetReference(utf8Data)));

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void CalcTextSize(ReadOnlySpan<byte> utf8Data, out Vector2 size)
    {
        Unsafe.SkipInit(out size);

        float wrap_width = -1f;
        byte hide_text_after_double_hash = 0;
        var ptrStart = (byte*) Unsafe.AsPointer(ref MemoryMarshal.GetReference(utf8Data));
        var ptrEnd = (byte*) Unsafe.Add<byte>(ptrStart, utf8Data.Length - 1);
        igCalcTextSize((Vector2*) Unsafe.AsPointer(ref size), ptrStart, ptrEnd, hide_text_after_double_hash, wrap_width);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool IsWindowFocused() => igIsWindowFocused(ImGuiNET.ImGuiFocusedFlags.None) > 0;
    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void SetWindowFocus() => igSetWindowFocus_Nil();
    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool IsWindowHovered() => igIsWindowHovered(ImGuiNET.ImGuiHoveredFlags.None) > 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public float GetWindowWidth() => igGetWindowWidth();
    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public float GetWindowHeight() => igGetWindowHeight();


    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void ColorConvertU32ToFloat4(uint u, out Vector4 color)
    {
        Unsafe.SkipInit(out color);
        igColorConvertU32ToFloat4((Vector4*) Unsafe.AsPointer(ref color), u);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public uint ColorConvertFloat4ToU32(ref readonly Vector4 color) => igColorConvertFloat4ToU32(color);

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public float GetScrollX() => igGetScrollX();
    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public float GetScrollY() => igGetScrollY();
    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void SetScrollX(float value) => igSetScrollX_Float(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void SetScrollY(float value) => igSetScrollY_Float(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void GetCursorScreenPos(out Vector2 GetCursorScreenPos)
    {
        Unsafe.SkipInit(out GetCursorScreenPos);
        igGetCursorScreenPos((Vector2*) Unsafe.AsPointer(ref GetCursorScreenPos));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void SetMouseCursor(ImGuiMouseCursor textInput) => igSetMouseCursor((ImGuiNET.ImGuiMouseCursor) textInput);

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool IsMouseClicked(ImGuiMouseButton button)
    {
        byte repeat = 0;
        return igIsMouseClicked_Bool((ImGuiNET.ImGuiMouseButton) button, repeat) > 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void GetMousePos(out Vector2 mousePos)
    {
        Unsafe.SkipInit(out mousePos);
        igGetMousePos((Vector2*) Unsafe.AsPointer(ref mousePos));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool IsMouseDragging(ImGuiMouseButton button)
    {
        float lock_threshold = -1f;
        return igIsMouseDragging((ImGuiNET.ImGuiMouseButton) button, lock_threshold) > 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool IsMouseDown(ImGuiMouseButton button) => igIsMouseDown_Nil((ImGuiNET.ImGuiMouseButton) button) > 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void Dummy(ref readonly Vector2 vector2) => igDummy(vector2);

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool IsMouseHoveringRect(ref readonly Vector2 lineStartScreenPos, ref readonly Vector2 end)
    {
        byte clip = 1;
        return igIsMouseHoveringRect(lineStartScreenPos, end, clip) > 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void BeginTooltip() => igBeginTooltip();
    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void EndTooltip() => igEndTooltip();


    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public Span<T> MemAlloc<T>(uint length) => new(igMemAlloc((uint) (length * Unsafe.SizeOf<T>())), (int) length);


    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void GetDrawData(out ImDrawDataWrapper drawDataWrapper) => drawDataWrapper = new(this, (ImGuiNET.ImDrawData*) igGetDrawData());

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void GetIO(out ImGuiIOWrapper ioWrapper) => ioWrapper = new(this, (ImGuiNET.ImGuiIO*) igGetIO());

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void ImFontConfig(out ImFontConfigWrapper fontConfigWrapper) => fontConfigWrapper = new(this, (ImGuiNET.ImFontConfig*) ImFontConfig_ImFontConfig());

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void GetMainViewport(out ImGuiViewportWrapper viewportWrapper) => viewportWrapper = new(this, (ImGuiNET.ImGuiViewport*) igGetMainViewport());

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void GetStyle(out ImGuiStyleWrapper styleWrapper) => styleWrapper = new(this, (ImGuiNET.ImGuiStyle*) igGetStyle());

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void CreateImGuiListClipper(out ImGuiListClipperWrapper listClipperWrapper) => listClipperWrapper = new(this, (ImGuiNET.ImGuiListClipper*) ImGuiListClipper_ImGuiListClipper());
    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void GetWindowDrawList(out ImDrawListWrapper drawListWrapper) => drawListWrapper = new(this, (ImGuiNET.ImDrawList*) igGetWindowDrawList());
}