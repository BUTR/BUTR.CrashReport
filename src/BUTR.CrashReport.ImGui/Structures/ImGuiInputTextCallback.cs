namespace BUTR.CrashReport.ImGui.Structures;

public delegate int ImGuiInputTextCallback<TData>(ImGuiInputTextCallbackData<TData> data) where TData : struct;

public delegate int ImGuiInputTextInt64Callback(ImGuiInputTextCallbackInt64Data data);