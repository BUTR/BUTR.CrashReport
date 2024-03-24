namespace BUTR.CrashReport.Models;

/// <summary>
/// Represents the functionality that is used by the module or plugin.
/// </summary>
public record CapabilityModuleOrPluginModel
{
    /// <summary>
    /// The name of the capability.
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// An optional description of the capability.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Creates a new instance of <see cref="CapabilityModuleOrPluginModel"/>.
    /// </summary>
    public CapabilityModuleOrPluginModel(string name) => Name = name;
}