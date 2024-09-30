using BUTR.CrashReport.Interfaces;
using BUTR.CrashReport.Models;

using ELFSharp.ELF;
using ELFSharp.MachO;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;

namespace BUTR.CrashReport.Utils;

internal static class NativeModuleUtils
{
    // TODO: Endianess?
    private static readonly byte[] PEHeader = [0x4D, 0x5A];
    private static readonly byte[] ElfMagic = [0x7F, 0x45, 0x4C, 0x46];
    private static readonly byte[] MachOMagic = [0xFE, 0xED, 0xFA, 0xCE];
    private static readonly byte[] MachOMagic64 = [0xFE, 0xED, 0xFA, 0xCF];

    public static List<NativeModule> CollectModules(Process process, IPathAnonymizer pathAnonymizer) => process.Modules.OfType<ProcessModule>().Select(x =>
    {
        try
        {
            using var fs = File.OpenRead(x.FileName);

            var signature = new byte[4];
            _ = fs.Read(signature, 0, signature.Length);
            fs.Seek(0, SeekOrigin.Begin);

            if (signature.AsSpan(0, PEHeader.Length).SequenceEqual(PEHeader))
            {
                using var reader = new PEReader(fs, PEStreamOptions.LeaveOpen);
                if (reader.HasMetadata) return null;
                fs.Seek(0, SeekOrigin.Begin);
            }

            var hash = CrashReportUtils.CalculateMD5(fs);
            fs.Seek(0, SeekOrigin.Begin);

            var arch = GetArchitecture(signature, fs);
            fs.Seek(0, SeekOrigin.Begin);

            var version = x.FileVersionInfo.FileVersion ?? x.FileVersionInfo.ProductVersion;

            if (!pathAnonymizer.TryHandlePath(x.FileName, out var anonymizedPath))
                anonymizedPath = Anonymizer.AnonymizePath(x.FileName);

            return new NativeModule(x.ModuleName, anonymizedPath, version, arch, (uint) fs.Length, x.BaseAddress, (uint) x.ModuleMemorySize, hash);
        }
        catch
        {
            return null;
        }
    }).OfType<NativeModule>().ToList();

    private static NativeAssemblyArchitectureType GetArchitecture(byte[] signature, Stream stream)
    {
        if (signature.AsSpan(0, PEHeader.Length).SequenceEqual(PEHeader))
        {
            using var reader = new PEReader(stream, PEStreamOptions.LeaveOpen);
            return reader.PEHeaders.CoffHeader.Machine switch
            {
                System.Reflection.PortableExecutable.Machine.I386 => NativeAssemblyArchitectureType.x86,
                System.Reflection.PortableExecutable.Machine.Amd64 => NativeAssemblyArchitectureType.x86_64,
                System.Reflection.PortableExecutable.Machine.Arm => NativeAssemblyArchitectureType.Arm,
                System.Reflection.PortableExecutable.Machine.Arm64 => NativeAssemblyArchitectureType.Arm64,
                _ => NativeAssemblyArchitectureType.Unknown,
            };
        }
        if (signature.AsSpan(0, ElfMagic.Length).SequenceEqual(ElfMagic))
        {
            using var elf = ELFReader.Load(stream, false);
            return elf.Machine switch
            {
                ELFSharp.ELF.Machine.Intel386 => NativeAssemblyArchitectureType.x86,
                ELFSharp.ELF.Machine.AMD64 => NativeAssemblyArchitectureType.x86_64,
                ELFSharp.ELF.Machine.ARM => NativeAssemblyArchitectureType.Arm,
                ELFSharp.ELF.Machine.AArch64 => NativeAssemblyArchitectureType.Arm64,
                _ => NativeAssemblyArchitectureType.Unknown,
            };
        }
        if (signature.AsSpan(0, MachOMagic.Length).SequenceEqual(MachOMagic) || signature.AsSpan(0, MachOMagic64.Length).SequenceEqual(MachOMagic64))
        {
            var reader = MachOReader.Load(stream, false);
            return reader.Machine switch
            {
                ELFSharp.MachO.Machine.X86 => NativeAssemblyArchitectureType.x86,
                ELFSharp.MachO.Machine.X86_64 => NativeAssemblyArchitectureType.x86_64,
                ELFSharp.MachO.Machine.Arm => NativeAssemblyArchitectureType.Arm,
                ELFSharp.MachO.Machine.Arm64 => NativeAssemblyArchitectureType.Arm64,
                _ => NativeAssemblyArchitectureType.Unknown,
            };
        }
        return NativeAssemblyArchitectureType.Unknown;
    }
}