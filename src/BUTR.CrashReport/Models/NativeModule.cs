using BUTR.CrashReport.Models;

using System;

namespace BUTR.CrashReport.Utils;

public class NativeModule
{
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

    public string ModuleName { get; set; }
    public string AnonymizedPath { get; set; }
    public string Version { get; set; }
    public NativeAssemblyArchitectureType Architecture { get; set; }
    public string Hash { get; set; }
    public uint Size { get; set; }
    public IntPtr InProcessAddress { get; set; }
    public uint InProcessSize { get; set; }
}