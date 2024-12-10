using System.Collections.Generic;

namespace BUTR.CrashReport.Decompilers.Sources;

internal sealed record SourceLocation(SourceFile SourceFile, IList<SourceSequencePoint> SequencePoints);