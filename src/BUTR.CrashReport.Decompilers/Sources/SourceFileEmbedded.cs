using System;
using System.Collections.Generic;

namespace BUTR.CrashReport.Decompilers.Sources;

internal sealed record SourceFileEmbedded : SourceFile
{
    public string FilePath { get; init; }
    public string SourceCode { get; init; }

    private readonly List<KeyValuePair<int, int>> _linesIndices;

    public SourceFileEmbedded(string filePath, string sourceCode) : base(SourceKind.Embedded)
    {
        FilePath = filePath;
        SourceCode = sourceCode;

        _linesIndices = new();
        var previousIdx = 0;
        while (SourceCode.IndexOf(Environment.NewLine, previousIdx, StringComparison.InvariantCulture) is var idx and not -1)
        {
            _linesIndices.Add(new(previousIdx, idx));
            previousIdx = idx + Environment.NewLine.Length;
        }
        _linesIndices.Add(new(previousIdx, SourceCode.Length));
    }

    public override string GetMethodLine(int startLine, int endLine)
    {
        if (startLine < 0 || endLine < 0 || startLine > endLine) return string.Empty;

        var (startIdx, _) = _linesIndices[startLine - 1];
        var (_, endIdx) = _linesIndices[endLine - 1];

        return SourceCode.Substring(startIdx, endIdx - startIdx);
    }

    public override string[] GetMethodLines(IList<SourceSequencePoint> sequencePoints)
    {
        return SourceCode.Split([Environment.NewLine], StringSplitOptions.None);
    }

    public void Deconstruct(out string buildTimeFilePath, out string srcCode)
    {
        buildTimeFilePath = FilePath;
        srcCode = SourceCode;
    }
}