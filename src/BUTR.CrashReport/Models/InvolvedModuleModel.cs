using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

public record InvolvedModuleModel
{
    public required string Id { get; set; }
    public required string EnhancedStacktraceFrameName { get; set; }
    public required IReadOnlyList<MetadataModel> AdditionalMetadata { get; set; } = new List<MetadataModel>();
}