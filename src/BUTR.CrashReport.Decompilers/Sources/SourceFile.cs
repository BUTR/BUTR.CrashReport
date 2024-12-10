using System.Collections.Generic;
using System.Linq;

namespace BUTR.CrashReport.Decompilers.Sources;

internal abstract record SourceFile(SourceKind Kind)
{
    public abstract string GetMethodLine(int startLine, int endLine);

    public virtual string[] GetMethodLines(IList<SourceSequencePoint> sequencePoints)
    {
        var startLine = sequencePoints.First().StartLine;
        var endLine = sequencePoints.Last().EndLine;

        var lines = new string[endLine - startLine + 1];
        for (var i = 0; i < lines.Length; i++)
            lines[i] = GetMethodLine(startLine + i, startLine + i);
        return lines;
    }
}