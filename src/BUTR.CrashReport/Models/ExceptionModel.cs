using System.Collections.Generic;

namespace BUTR.CrashReport.Models;

public record ExceptionModel
{
    public required string Type { get; set; }
    public required string Message { get; set; }
    public required string CallStack { get; set; }
    public required ExceptionModel? InnerException { get; set; }
    public required IReadOnlyList<MetadataModel> AdditionalMetadata { get; set; } = new List<MetadataModel>();
}