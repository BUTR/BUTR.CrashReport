namespace BUTR.CrashReport.Decompilers.Sources;

internal sealed record SourceSequencePoint(int Offset, int EndOffset, int StartLine, int StartColumn, int EndLine, int EndColumn, bool IsHidden);