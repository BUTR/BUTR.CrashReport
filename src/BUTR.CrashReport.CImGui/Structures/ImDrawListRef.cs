namespace ImGui.Structures;

public readonly unsafe ref struct ImDrawListRef
{
    public readonly ImGuiNET.ImDrawList* NativePtr;

    public ImDrawListRef(ImGuiNET.ImDrawList* nativePtr) => NativePtr = nativePtr;

    public void GetVtxBuffer(out ImVectorRefImDrawVert vtxBuffer) => vtxBuffer = new(NativePtr->VtxBuffer);
    public void GetIdxBuffer(out ImVectorRefUInt16 idxBuffer) => idxBuffer = new(NativePtr->IdxBuffer);
    public void GetCmdBuffer(out ImVectorRefImDrawCmd cmdBuffer) => cmdBuffer = new(NativePtr->CmdBuffer);
}