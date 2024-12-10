using BUTR.CrashReport.Decompilers.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BUTR.CrashReport.Decompilers.Sources;

internal sealed record SourceFileDecompiled : SourceFile
{
    private static readonly char[] NewLine = Environment.NewLine.ToArray();

    public StringBuilder SourceCode { get; init; }

    private readonly List<KeyValuePair<int, int>> _linesIndices;

    public SourceFileDecompiled(StringBuilder sourceCode) : base(SourceKind.Decompiled)
    {
        SourceCode = sourceCode;

        _linesIndices = new();
        var previousIdx = 0;
        while (SourceCode.IndexOf(NewLine, previousIdx) is var idx and not -1)
        {
            _linesIndices.Add(new(previousIdx, idx));
            previousIdx = idx + NewLine.Length;
        }
        _linesIndices.Add(new(previousIdx, SourceCode.Length));
    }

    public override string GetMethodLine(int startLine, int endLine)
    {
        if (startLine < 0 || endLine < 0 || startLine > endLine) return string.Empty;

        var (startIdx, _) = _linesIndices[startLine - 1];
        var (_, endIdx) = _linesIndices[endLine - 1];

        return SourceCode.ToString(startIdx, endIdx - startIdx);
    }

    public override string[] GetMethodLines(IList<SourceSequencePoint> sequencePoints)
    {
        return SourceCode.ToString().Split([Environment.NewLine], StringSplitOptions.None);
    }
}