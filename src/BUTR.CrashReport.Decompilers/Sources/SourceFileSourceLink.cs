namespace BUTR.CrashReport.Decompilers.Sources;

internal sealed record SourceFileSourceLink(string FilePath, string SourceUrl) : SourceFile(SourceKind.SourceLink)
{
    public override string GetMethodLine(int startLine, int endLine) => string.Empty;
}