using BUTR.CrashReport.ImGui.Extensions;
using BUTR.CrashReport.ImGui.Utils;
using BUTR.CrashReport.Memory.Utils;
using BUTR.CrashReport.Models;

namespace BUTR.CrashReport.Renderer.ImGui.Renderer;

partial class ImGuiRenderer
{
    protected sealed record Utf8KeyValue(byte[] Key, byte[] Value);
    protected sealed record Utf8KeyValueList(string Key, List<Utf8KeyValue> Values);
}

partial class ImGuiRenderer<TImGuiIORef, TImGuiViewportRef, TImDrawListRef, TImGuiStyleRef, TColorsRangeAccessorRef, TImGuiListClipperRef>
{
    private static void InitializeAdditionalMetadata<TKey>(Dictionary<TKey, List<Utf8KeyValueList>> dict, TKey key, IList<MetadataModel> metadatas) where TKey : notnull
    {
        if (!dict.TryGetValue(key, out var metadataDict))
            dict[key] = metadataDict = new();

        for (var i = 0; i < metadatas.Count; i++)
        {
            var metadata = metadatas[i];
            if (!metadata.Key.StartsWith("DISPLAY:") || string.IsNullOrEmpty(metadata.Value))
                continue;

            var keyValue = metadata.Key.AsSpan(8);
            var groupIdx = keyValue.IndexOf(':');

            if (keyValue.Length == 0)
                continue;

            var groupKey = groupIdx != -1 ? keyValue.Slice(0, groupIdx).ToString() : string.Empty;
            var keyUtf8 = groupIdx != -1 ? Utf8Utils.ToUtf8Array(keyValue.Slice(groupIdx + 1)) : Utf8Utils.ToUtf8Array(keyValue);
            var valueUtf8 = Utf8Utils.ToUtf8Array(metadata.Value);

            var entry = metadataDict.FirstOrDefault(x => x.Key == groupKey);
            if (entry is null)
                metadataDict.Add(entry = new(groupKey, new()));
            entry.Values.Add(new(keyUtf8, valueUtf8));
        }
    }

    private void RenderAdditionalMetadata<TKey>(Dictionary<TKey, List<Utf8KeyValueList>> dict, TKey key) where TKey : notnull
    {
        if (!dict.TryGetValue(key, out var groups))
            return;

        var groupsSpan = groups.AsSpan();
        for (var i = 0; i < groupsSpan.Length; i++)
        {
            var (groupKey, utf8KeyValues) = groupsSpan[i];
            var values = utf8KeyValues.AsSpan();

            if (groupKey.Length > 0)
            {
                _imgui.Text(groupKey);
                _imgui.SameLine();
                _imgui.Text(":\0"u8);

                for (var j = 0; j < values.Length; j++)
                {
                    var (_, valueUtf8) = values[j];
                    _imgui.Bullet();
                    _imgui.TextWrapped(valueUtf8);
                }
            }
            else
            {
                for (var j = 0; j < values.Length; j++)
                {
                    var (keyUtf8, valueUtf8) = values[j];
                    _imgui.Text(keyUtf8);
                    _imgui.SameLine();
                    _imgui.Text(": \0"u8);
                    _imgui.SameLine();
                    _imgui.TextWrapped(valueUtf8);
                }
            }
        }
    }

    private void RenderAdditionalMetadataSameLine<TKey>(Dictionary<TKey, List<Utf8KeyValueList>> dict, TKey key) where TKey : notnull
    {
        if (!dict.TryGetValue(key, out var groups))
            return;

        var groupsSpan = groups.AsSpan();
        for (var i = 0; i < groupsSpan.Length; i++)
        {
            var (groupKey, utf8KeyValues) = groupsSpan[i];
            var values = utf8KeyValues.AsSpan();

            if (groupKey.Length > 0)
            {
                _imgui.Text(groupKey);
                _imgui.SameLine();
                _imgui.Text(": \0"u8);
                _imgui.SameLine();

                for (var j = 0; j < values.Length; j++)
                {
                    var (_, valueUtf8) = values[j];
                    _imgui.Text(", \0"u8);
                    _imgui.SameLine();
                    _imgui.TextWrapped(valueUtf8);
                    _imgui.SameLine();
                }
            }

            for (var j = 0; j < values.Length; j++)
            {
                var (keyUtf8, valueUtf8) = values[j];
                _imgui.Text(", \0"u8);
                _imgui.SameLine();
                _imgui.Text(keyUtf8);
                _imgui.SameLine();
                _imgui.Text(" - \0"u8);
                _imgui.SameLine();
                _imgui.TextWrapped(valueUtf8);
                _imgui.SameLine();
            }
        }
    }
}