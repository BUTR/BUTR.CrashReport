using BUTR.CrashReport.Models;

using System;

namespace BUTR.CrashReport.Utils;

/// <summary>
/// Represents a native module loaded in the process.
/// </summary>
public class NativeModule
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NativeModule"/> class.
    /// </summary>
    public NativeModule(string moduleName, string anonymizedPath, string version, NativeAssemblyArchitectureType architecture, uint size, IntPtr inProcessAddress, uint inProcessSize, string hash)
    {
        ModuleName = moduleName;
        AnonymizedPath = anonymizedPath;
        Version = version;
        Architecture = architecture;
        Hash = hash;
        Size = size;
        InProcessAddress = inProcessAddress;
        InProcessSize = inProcessSize;
    }

    /// <summary>
    /// The name of the module.
    /// </summary>
    public string ModuleName { get; set; }

    /// <summary>
    /// The anonymized path of the module.
    /// </summary>
    public string AnonymizedPath { get; set; }

    /// <summary>
    /// The version of the module.
    /// </summary>
    public string Version { get; set; }

    /// <summary>
    /// The architecture of the module.
    /// </summary>
    public NativeAssemblyArchitectureType Architecture { get; set; }

    /// <summary>
    /// The hash of the module.
    /// </summary>
    public string Hash { get; set; }

    /// <summary>
    /// The size of the module.
    /// </summary>
    public uint Size { get; set; }

    /// <summary>
    /// The in-process address of the module.
    /// </summary>
    public IntPtr InProcessAddress { get; set; }

    /// <summary>
    /// The in-process size of the module.
    /// </summary>
    public uint InProcessSize { get; set; }
}