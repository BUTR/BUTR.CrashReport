namespace BUTR.CrashReport.Models;

public enum AssemblyModelType
{
    /// <summary>
    /// Unknown assembly origin
    /// </summary>
    Unclassified,

    /// <summary>
    /// Game core assembly
    /// </summary>
    GameCore,

    /// <summary>
    /// Game module assembly
    /// </summary>
    GameModule,

    /// <summary>
    /// System assembly
    /// </summary>
    System,

    /// <summary>
    /// Custom module assembly
    /// </summary>
    Module,
}