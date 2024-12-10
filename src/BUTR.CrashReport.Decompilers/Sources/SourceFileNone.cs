namespace BUTR.CrashReport.Decompilers.Sources;

internal sealed record SourceFileNone() : SourceFile(SourceKind.None)
{
    public override string GetMethodLine(int startLine, int endLine) => string.Empty;
}