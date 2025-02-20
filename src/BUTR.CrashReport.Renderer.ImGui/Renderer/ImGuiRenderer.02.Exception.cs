using BUTR.CrashReport.ImGui.Extensions;
using BUTR.CrashReport.Models;

using Cysharp.Text;

using Utf8StringInterpolation;

namespace BUTR.CrashReport.Renderer.ImGui.Renderer;

partial class ImGuiRenderer
{
    protected class ExceptionModelEqualityComparer : IEqualityComparer<ExceptionModel>
    {
        public static ExceptionModelEqualityComparer Instance { get; } = new();
        public bool Equals(ExceptionModel? x, ExceptionModel? y) => ReferenceEquals(x, y);
        public int GetHashCode(ExceptionModel obj) => obj.GetHashCode();
    }

    protected static readonly string[] NewLine = [Environment.NewLine];
}

partial class ImGuiRenderer<TImGuiIORef, TImGuiViewportRef, TImDrawListRef, TImGuiStyleRef, TColorsRangeAccessorRef, TImGuiListClipperRef>
{
    private readonly Dictionary<ExceptionModel, List<Utf8KeyValueList>> _exceptionAdditionalDisplayKeyMetadata = new(ExceptionModelEqualityComparer.Instance);

    private byte[][] _exceptionsUtf8 = [];
    private EnhancedStacktraceFrameModel?[] _stacktracesUtf8 = [];
    private int[] _callstackLineCount = [];

    private void InitializeExceptionRecursively()
    {
        var level = 0;
        var curr = _crashReport.Exception;
        while (curr is not null)
        {
            level++;
            InitializeAdditionalMetadata(_exceptionAdditionalDisplayKeyMetadata, curr, curr.AdditionalMetadata);
            curr = curr.InnerException;
        }
        _exceptionsUtf8 = new byte[level][];
        _stacktracesUtf8 = new EnhancedStacktraceFrameModel?[level];
        _callstackLineCount = new int[level];

        level = 0;
        curr = _crashReport.Exception;
        while (curr is not null)
        {
            var callStackLines = curr.CallStack.Split(NewLine, StringSplitOptions.RemoveEmptyEntries).Select(x => x).ToArray().AsSpan();

            var sb = ZString.CreateUtf8StringBuilder();
            for (var i = 0; i < callStackLines.Length; i++)
            {
                sb.AppendLiteral(Utf8String.Format($"{" ".PadLeft(i > 98 ? 1 : i > 8 ? 2 : 3)}{i + 1}.{callStackLines[i].Trim()}"));
                if (i < callStackLines.Length - 1) sb.AppendLine();
            }

            _exceptionsUtf8[level] = sb.AsSpan().ToArray();

            var fistCallstackLine = callStackLines.Length > 0 ? callStackLines[0].Trim() : string.Empty;
            _stacktracesUtf8[level] = _crashReport.EnhancedStacktrace.FirstOrDefault(x => fistCallstackLine == $"at {x.FrameDescription}");

            _callstackLineCount[level] = callStackLines.Length;

            level++;
            curr = curr.InnerException;
        }
    }

    private void RenderExceptionRecursively(ExceptionModel? ex, int level)
    {
        if (ex is null) return;

        _imgui.PushId(level);

        var moduleId = _stacktracesUtf8[level]?.ExecutingMethod.ModuleId ?? "UNKNOWN";
        var sourceModuleId = ex.SourceModuleId ?? "UNKNOWN";

        var pluginId = _stacktracesUtf8[level]?.ExecutingMethod.LoaderPluginId ?? "UNKNOWN";
        var sourcePluginId = ex.SourceLoaderPluginId ?? "UNKNOWN";

        _imgui.Text("Exception Information:\0"u8);

        if (moduleId != "UNKNOWN") _imgui.RenderId("Potential Module Id:\0"u8, moduleId);
        if (sourceModuleId != "UNKNOWN") _imgui.RenderId("Potential Source Module Id:\0"u8, sourceModuleId);
        if (pluginId != "UNKNOWN") _imgui.RenderId("Potential Plugin Id:\0"u8, pluginId);
        if (sourcePluginId != "UNKNOWN") _imgui.RenderId("Potential Source Plugin Id:\0"u8, sourcePluginId);

        _imgui.Text("Type: \0"u8);
        _imgui.SameLine();
        _imgui.Text(ex.Type);

        if (!string.IsNullOrWhiteSpace(ex.Source))
        {
            _imgui.Text("Source: \0"u8);
            _imgui.SameLine();
            _imgui.Text(ex.Source);
        }

        if (ex.HResult != 0)
        {
            _imgui.Text("HResult: \0"u8);
            _imgui.SameLine();
            _imgui.Hex(ex.HResult);
        }

        if (!string.IsNullOrWhiteSpace(ex.Message))
        {
            _imgui.Text("Message: \0"u8);
            _imgui.SameLine();
            _imgui.Text(ex.Message);
        }

        if (!string.IsNullOrWhiteSpace(ex.CallStack))
        {
            _imgui.Text("Stacktrace:\0"u8);
            RenderInputTextWithIO("##stacktrace\0"u8, _exceptionsUtf8[level], _callstackLineCount[level]);
        }

        RenderAdditionalMetadata(_exceptionAdditionalDisplayKeyMetadata, ex);

        if (ex.InnerException is not null)
        {
            _imgui.Text("Inner Exception:\0"u8);
            _imgui.Indent();
            RenderExceptionRecursively(ex.InnerException, level + 1);
            _imgui.Unindent();
        }

        _imgui.PopId();
    }
}