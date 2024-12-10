using AsmResolver.DotNet;
using AsmResolver.IO;

using BUTR.CrashReport.Decompilers.Models;

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BUTR.CrashReport.Decompilers.Utils;

/// <summary>
/// Helper method to create import models from an assembly
/// </summary>
public static class ReferenceImporter
{
    /// <summary>
    /// Helper method to create import models from an assembly
    /// </summary>
    public static AssemblyTypeReferenceInternal[] GetImportedTypeReferences(Assembly assembly, Func<Assembly, Stream?> getAssemblyStream)
    {
        // TODO: Can we do that with the built-in Reflection API?
        foreach (var assemblyModule in assembly.Modules)
        {
            try
            {
                var module = ModuleDefinition.FromModule(assemblyModule);
                return module.GetImportedTypeReferences().Select(y => new AssemblyTypeReferenceInternal
                {
                    Name = y.Name ?? string.Empty,
                    Namespace = y.Namespace ?? string.Empty,
                    FullName = y.FullName,
                }).ToArray();
            }
            catch (Exception e)
            {
                Trace.TraceError(assembly.FullName);
                Trace.TraceError(e.ToString());
            }
        }

        try
        {
            if (getAssemblyStream(assembly) is { } stream)
            {
                var assemblyDefinition = AssemblyDefinition.FromReader(new BinaryStreamReader(new StreamDataSource(stream)));
                foreach (var module in assemblyDefinition.Modules)
                {
                    return module.GetImportedTypeReferences().Select(y => new AssemblyTypeReferenceInternal
                    {
                        Name = y.Name ?? string.Empty,
                        Namespace = y.Namespace ?? string.Empty,
                        FullName = y.FullName,
                    }).ToArray();
                }
            }

        }
        catch (Exception e)
        {
            Trace.TraceError(assembly.FullName);
            Trace.TraceError(e.ToString());
        }

        return [];
    }
}