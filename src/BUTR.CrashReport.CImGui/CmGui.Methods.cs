using BUTR.CrashReport.ImGui.Enums;
using BUTR.CrashReport.ImGui.Structures;
using BUTR.CrashReport.Memory;
using BUTR.CrashReport.Native;

using ImGui.Structures;

using System.Diagnostics;
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

    
    private static readonly LiteralSpan<byte> LinkIcon = ""u8;

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void MDLinkCallback2(MarkdownLinkCallbackData data)
    {
        if (data.UserData is null || data.LinkLength == 0)
            return;

        var handle = GCHandle.FromIntPtr((IntPtr) data.UserData);
        var cmgui = (CmGui) handle.Target!;
        cmgui.GetIO(out var io);
        
        if (io.PlatformOpenInShell is not { } platformOpenInShell)
            return;
        
        Span<byte> utf8Link = new Span<byte>(data.Link, data.LinkLength);
        Span<byte> buffer = stackalloc byte[data.LinkLength + 1];
        utf8Link.CopyTo(buffer);
        buffer[data.LinkLength] = 0;
        
        var ctx = cmgui.GetCurrentContext();

        fixed (byte* bufferPtr = buffer)
            platformOpenInShell(ctx, bufferPtr);
    }

    
    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void PushId(int id) => igPushID_Int(id);

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void PushId(ReadOnlySpan<byte> utf8Label)
    {
        fixed (byte* utf8LabelPtr = utf8Label)
            igPushID_Str(utf8LabelPtr);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void PopId() => igPopID();

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool InputTextMultilineInt64(ReadOnlySpan<byte> utf8Label, Span<byte> input, int lineCount, ImGuiInputTextInt64Callback callback, Int64 data)
    {
        var size = new Vector2(-1, GetTextLineHeight() * (lineCount + 1));
        var flags = ImGuiInputTextFlags.ReadOnly | ImGuiInputTextFlags.CallbackEdit;


        delegate* unmanaged[Cdecl]<ImGuiNET.ImGuiInputTextCallbackData*, int> nativeCallbackPtr = &ImGuiInputTextCallbackInt64;

        var userData = new UserDataInt64(callback, data);

        //GetContentRegionAvail(out var contentRegionAvail);

        // Lifetime of objects is bound to the function scope
        fixed (byte* utf8LabelPtr = utf8Label)
        fixed (byte* inputPtr = input)
        {
            PushStyleColor(ImGuiCol.FrameBg, in Zero4);
            //igPushTextWrapPos(contentRegionAvail.X);
            var result = igInputTextMultiline(
                utf8LabelPtr,
                inputPtr,
                (uint) input.Length,
                size,
                (ImGuiNET.ImGuiInputTextFlags) flags,
                (IntPtr) nativeCallbackPtr,
                (IntPtr) (&userData)) > 0;

            PopStyleColor();
            //igPopTextWrapPos();

            return result;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool InputTextMultiline<TData>(ReadOnlySpan<byte> utf8Label, Span<byte> input, int lineCount, ImGuiInputTextCallback<TData> callback, TData data)
        where TData : struct
    {
        if (!_callbacks.TryGetValue(typeof(TData), out var genericTDataCallback))
            _callbacks.Add(typeof(TData), genericTDataCallback = GenericTDataCallback<TData>);

        var size = new Vector2(-1, GetTextLineHeight() * (lineCount + 1));
        var flags = ImGuiInputTextFlags.ReadOnly | ImGuiInputTextFlags.CallbackEdit;

        PushStyleColor(ImGuiCol.FrameBg, in Zero4);

        delegate* unmanaged[Cdecl]<ImGuiNET.ImGuiInputTextCallbackData*, int> nativeCallbackPtr = &ImGuiInputTextCallbackGeneric;

        var userData = new UserDataGeneric(genericTDataCallback, callback, Unsafe.AsPointer(ref data));

        // Lifetime of objects is bound to the function scope
        fixed (byte* utf8LabelPtr = utf8Label)
        fixed (byte* inputPtr = input)
        {
            var result = igInputTextMultiline(
                utf8LabelPtr,
                inputPtr,
                (uint) input.Length,
                size,
                (ImGuiNET.ImGuiInputTextFlags) flags,
                (IntPtr) nativeCallbackPtr,
                (IntPtr) (&userData)) > 0;

            PopStyleColor();

            return result;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool InputText(ReadOnlySpan<byte> utf8Label, Span<byte> input, ImGuiInputTextFlags flags)
    {
        var buf_size = input.Length;
        var user_data = IntPtr.Zero;

        PushStyleColor(ImGuiCol.FrameBg, in Zero4);

        fixed (byte* inputPtr = input)
        fixed (byte* utf8LabelPtr = utf8Label)
        {
            var result = igInputText(
                utf8LabelPtr,
                inputPtr,
                (uint) buf_size,
                (ImGuiNET.ImGuiInputTextFlags) flags,
                IntPtr.Zero,
                user_data) > 0;

            PopStyleColor();

            return result;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void Text(ReadOnlySpan<byte> utf8Label)
    {
        fixed (byte* utf8LabelPtr = utf8Label)
        {
            var ptrStart = utf8LabelPtr;
            var ptrEnd = (byte*) Unsafe.Add<byte>(ptrStart, utf8Label.Length - 1);
            Debug.Assert(*ptrEnd == 0, "string must be null-terminated");
            igTextUnformatted(ptrStart, ptrEnd);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void TextWrapped(ReadOnlySpan<byte> utf8Label)
    {
        GetContentRegionAvail(out var contentRegionAvail);

        igPushTextWrapPos(contentRegionAvail.X);
        fixed (byte* utf8LabelPtr = utf8Label)
        {
            var ptrStart = utf8LabelPtr;
            var ptrEnd = (byte*) Unsafe.Add<byte>(ptrStart, utf8Label.Length - 1);
            Debug.Assert(*ptrEnd == 0, "string must be null-terminated");
            igTextUnformatted(ptrStart, ptrEnd);
        }
        igPopTextWrapPos();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void TextColored(ReadOnlySpan<byte> utf8Label, ref readonly Vector4 color)
    {
        PushStyleColor(ImGuiCol.Text, in color);
        fixed (byte* utf8LabelPtr = utf8Label)
        {
            var ptrStart = utf8LabelPtr;
            var ptrEnd = (byte*) Unsafe.Add<byte>(ptrStart, utf8Label.Length - 1);
            Debug.Assert(*ptrEnd == 0, "string must be null-terminated");
            igTextUnformatted(ptrStart, ptrEnd);
        }
        PopStyleColor();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void TextLinkOpenURL(ReadOnlySpan<byte> utf8Label, ReadOnlySpan<byte> utf8Url)
    {
        fixed (byte* utf8LabelPtr = utf8Label)
        fixed (byte* utf8UrlPtr = utf8Url)
            igTextLinkOpenURL(utf8LabelPtr, utf8UrlPtr);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool Checkbox(ReadOnlySpan<byte> utf8Label, ref bool isSelected)
    {
        ref var isSelectedRef = ref Unsafe.As<bool, byte>(ref isSelected);
        fixed (byte* utf8LabelPtr = utf8Label)
        fixed (byte* isSelectedRefPtr = &isSelectedRef)
            return igCheckbox(utf8LabelPtr, isSelectedRefPtr) > 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void OpenPopup(ReadOnlySpan<byte> utf8Label, ImGuiPopupFlags flags)
    {
        fixed (byte* utf8LabelPtr = utf8Label)
            igOpenPopup_Str(utf8LabelPtr, (ImGuiNET.ImGuiPopupFlags) flags);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void CloseCurrentPopup() => igCloseCurrentPopup();

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool BeginPopup(ReadOnlySpan<byte> utf8Label, ImGuiWindowFlags flags)
    {
        fixed (byte* utf8LabelPtr = utf8Label)
            return igBeginPopup(utf8LabelPtr, (ImGuiNET.ImGuiWindowFlags) flags) > 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool BeginPopupModal(ReadOnlySpan<byte> utf8Label, ImGuiWindowFlags flags)
    {
        fixed (byte* utf8LabelPtr = utf8Label)
            return igBeginPopupModal(utf8LabelPtr, null, (ImGuiNET.ImGuiWindowFlags) flags) > 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool BeginPopupContextWindow(ReadOnlySpan<byte> utf8Label)
    {
        const ImGuiPopupFlags popup_flags = ImGuiPopupFlags.MouseButtonRight;
        fixed (byte* utf8LabelPtr = utf8Label)
            return igBeginPopupContextWindow(utf8LabelPtr, (ImGuiNET.ImGuiPopupFlags) popup_flags) > 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void EndPopup() => igEndPopup();

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool Selectable(ReadOnlySpan<byte> utf8Label, ref bool isSelected, ImGuiSelectableFlags flags)
    {
        ref var selectedNum = ref Unsafe.As<bool, byte>(ref isSelected);
        fixed (byte* utf8LabelPtr = utf8Label)
            return igSelectable_Bool(utf8LabelPtr, selectedNum, (ImGuiNET.ImGuiSelectableFlags) flags, Zero2) > 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool IsMouseDoubleClicked(ImGuiMouseButton mouseButton) => igIsMouseDoubleClicked_Nil((ImGuiNET.ImGuiMouseButton) mouseButton) > 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool Button(ReadOnlySpan<byte> utf8Label)
    {
        fixed (byte* utf8LabelPtr = utf8Label)
            return igButton(utf8LabelPtr, Zero2) > 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool SmallButton(ReadOnlySpan<byte> utf8Label)
    {
        fixed (byte* utf8LabelPtr = utf8Label)
            return igSmallButton(utf8LabelPtr) > 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool Begin(ReadOnlySpan<byte> utf8Label, ImGuiWindowFlags flags)
    {
        const nint p_open = 0;
        fixed (byte* utf8LabelPtr = utf8Label)
            return igBegin(utf8LabelPtr, p_open, (ImGuiNET.ImGuiWindowFlags) flags) > 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool BeginTable(ReadOnlySpan<byte> utf8Label, int column)
    {
        const ImGuiNET.ImGuiTableFlags flags = ImGuiNET.ImGuiTableFlags.None;
        const float inner_width = 0.0f;
        fixed (byte* utf8LabelPtr = utf8Label)
        {
            return igBeginTable(utf8LabelPtr, column, flags, Zero2, inner_width) > 0;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool BeginChild(ReadOnlySpan<byte> utf8Label, ref readonly Vector2 size, ImGuiChildFlags child_flags, ImGuiWindowFlags window_flags)
    {
        fixed (byte* utf8LabelPtr = utf8Label)
            return igBeginChild_Str(utf8LabelPtr, size, (ImGuiNET.ImGuiChildFlags) child_flags, (ImGuiNET.ImGuiWindowFlags) window_flags) > 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool TreeNode(ReadOnlySpan<byte> utf8Label, ImGuiTreeNodeFlags flags)
    {
        fixed (byte* utf8LabelPtr = utf8Label)
            return igTreeNodeEx_Str(utf8LabelPtr, (ImGuiNET.ImGuiTreeNodeFlags) flags) > 0;
    }

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
    public IntPtr CreateContext() => igCreateContext(null);

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public IntPtr GetCurrentContext() => igGetCurrentContext();

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
    public IntPtr GetCurrentWindow() => igGetCurrentWindow();

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void BringWindowToDisplayFront(IntPtr window) => igBringWindowToDisplayFront(window);

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool MenuItem(ReadOnlySpan<byte> utf8Label)
    {
        const nint shortcut = 0;
        const byte selected = 0;
        const byte enabled = 1;
        fixed (byte* utf8LabelPtr = utf8Label)
            return igMenuItem_Bool(utf8LabelPtr, shortcut, selected, enabled) > 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool IsItemClicked(ImGuiMouseButton mouseButton) => igIsItemClicked((ImGuiNET.ImGuiMouseButton) mouseButton) > 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool IsKeyDown(ImGuiKey key) => igIsKeyDown_Nil((ImGuiNET.ImGuiKey) key) > 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool IsKeyPressed(ImGuiKey key) => igIsKeyPressed_Bool((ImGuiNET.ImGuiKey) key, 1) > 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void SetClipboardText(ReadOnlySpan<byte> utf8Data)
    {
        fixed (byte* utf8DataPtr = utf8Data)
            igSetClipboardText(utf8DataPtr);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void CalcTextSize(ReadOnlySpan<byte> utf8Data, out Vector2 size)
    {
        const float wrap_width = -1f;
        const byte hide_text_after_double_hash = 0;
        fixed (Vector2* sizePtr = &size)
        fixed (byte* utf8DataPtr = utf8Data)
        {
            var ptrStart = utf8DataPtr;
            var ptrEnd = (byte*) Unsafe.Add<byte>(ptrStart, utf8Data.Length - 1);
            igCalcTextSize(sizePtr, ptrStart, ptrEnd, hide_text_after_double_hash, wrap_width);
        }
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
        fixed (Vector4* colorPtr = &color)
            igColorConvertU32ToFloat4(colorPtr, u);
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
    public void GetCursorScreenPos(out Vector2 cursorScreenPos)
    {
        fixed (Vector2* cursorScreenPosPtr = &cursorScreenPos)
            igGetCursorScreenPos(cursorScreenPosPtr);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void GetContentRegionAvail(out Vector2 contentRegionAvail)
    {
        fixed (Vector2* contentRegionAvailPtr = &contentRegionAvail)
            igGetContentRegionAvail(contentRegionAvailPtr);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void GetWindowPos(out Vector2 windowPos)
    {
        fixed (Vector2* windowPosPtr = &windowPos)
            igGetContentRegionAvail(windowPosPtr);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void SetMouseCursor(ImGuiMouseCursor textInput) => igSetMouseCursor((ImGuiNET.ImGuiMouseCursor) textInput);

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool IsMouseClicked(ImGuiMouseButton button)
    {
        const byte repeat = 0;
        return igIsMouseClicked_Bool((ImGuiNET.ImGuiMouseButton) button, repeat) > 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void GetMousePos(out Vector2 mousePos)
    {
        fixed (Vector2* mousePosPtr = &mousePos)
            igGetMousePos(mousePosPtr);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool IsMouseDragging(ImGuiMouseButton button)
    {
        const float lock_threshold = -1f;
        return igIsMouseDragging((ImGuiNET.ImGuiMouseButton) button, lock_threshold) > 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool IsMouseDown(ImGuiMouseButton button) => igIsMouseDown_Nil((ImGuiNET.ImGuiMouseButton) button) > 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void Dummy(ref readonly Vector2 vector2) => igDummy(vector2);

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public bool IsMouseHoveringRect(ref readonly Vector2 lineStartScreenPos, ref readonly Vector2 end)
    {
        const byte clip = 1;
        return igIsMouseHoveringRect(lineStartScreenPos, end, clip) > 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void BeginTooltip() => igBeginTooltip();

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void EndTooltip() => igEndTooltip();
    
    public void Markdown(ReadOnlySpan<byte> utf8Markdown)
    {
        var cmGui = this;
        var config = default(MarkdownConfig);
        var configRef = new MarkdownConfigRef(&config)
        {
            LinkCallback = mdDefaultMarkdownLinkCallback,
            TooltipCallback = mdDefaultMarkdownTooltipCallback,
            ImageCallback = null,
            LinkIcon = LinkIcon,
            Heading1 = new() { Font = null, IsSeparator = 1 },
            Heading2 = new() { Font = null, IsSeparator = 1 },
            Heading3 = new() { Font = null, IsSeparator = 1 },
            FormatCallback = mdDefaultMarkdownFormatCallback,
        };
         
        fixed (byte* utf8MarkdownPtr = utf8Markdown)
            mdMarkdown(utf8MarkdownPtr, (IntPtr) utf8Markdown.Length, configRef.NativePtr);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public Span<T> MemAlloc<T>(uint length)
    {
        Debug.Assert(length > 0, "Length must be greater than 0");
        var ptr = igMemAlloc((uint) (length * Unsafe.SizeOf<T>()));
        return new Span<T>(ptr, (int) length);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void GetDrawData(out ImDrawDataWrapper drawDataWrapper)
    {
        var ptr = igGetDrawData();
        Debug.Assert(ptr != null, "DrawData is null");
        drawDataWrapper = new(this, (ImGuiNET.ImDrawData*) ptr);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void GetIO(out ImGuiIOWrapper ioWrapper)
    {
        var ptr = igGetIO();
        Debug.Assert(ptr != null, "IO is null");
        ioWrapper = new(this, (ImGuiNET.ImGuiIO*) ptr);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void ImFontConfig(out ImFontConfigWrapper fontConfigWrapper)
    {
        var ptr = ImFontConfig_ImFontConfig();
        Debug.Assert(ptr != null, "FontConfig is null");
        fontConfigWrapper = new(this, (ImGuiNET.ImFontConfig*) ptr);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void GetMainViewport(out ImGuiViewportWrapper viewportWrapper)
    {
        var ptr = igGetMainViewport();
        Debug.Assert(ptr != null, "Viewport is null");
        viewportWrapper = new(this, (ImGuiNET.ImGuiViewport*) ptr);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void GetStyle(out ImGuiStyleWrapper styleWrapper)
    {
        var ptr = igGetStyle();
        Debug.Assert(ptr != null, "Style is null");
        styleWrapper = new(this, (ImGuiNET.ImGuiStyle*) ptr);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void CreateImGuiListClipper(out ImGuiListClipperWrapper listClipperWrapper)
    {
        var ptr = ImGuiListClipper_ImGuiListClipper();
        Debug.Assert(ptr != null, "ListClipper is null");
        listClipperWrapper = new(this, (ImGuiNET.ImGuiListClipper*) ptr);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | AggressiveOptimization)]
    public void GetWindowDrawList(out ImDrawListWrapper drawListWrapper)
    {
        var ptr = igGetWindowDrawList();
        Debug.Assert(ptr != null, "DrawList is null");
        drawListWrapper = new(this, (ImGuiNET.ImDrawList*) ptr);
    }
}