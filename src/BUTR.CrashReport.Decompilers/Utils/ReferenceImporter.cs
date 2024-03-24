using AsmResolver.DotNet;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace BUTR.CrashReport.Decompilers.Utils;

/// <summary>
/// <inheritdoc cref="BUTR.CrashReport.Decompilers.Utils.AssemblyTypeReferenceInternal"/>
/// </summary>
public record AssemblyTypeReferenceInternal
{
    /// <summary>
    /// <inheritdoc cref="BUTR.CrashReport.Decompilers.Utils.AssemblyTypeReferenceInternal.Name"/>
    /// </summary>
    /// <returns><inheritdoc cref="BUTR.CrashReport.Decompilers.Utils.AssemblyTypeReferenceInternal.Name"/></returns>
    public required string Name { get; set; }

    /// <summary>
    /// <inheritdoc cref="BUTR.CrashReport.Decompilers.Utils.AssemblyTypeReferenceInternal.Namespace"/>
    /// </summary>
    /// <returns><inheritdoc cref="BUTR.CrashReport.Decompilers.Utils.AssemblyTypeReferenceInternal.Namespace"/></returns>
    public required string Namespace { get; set; }

    /// <summary>
    /// <inheritdoc cref="BUTR.CrashReport.Decompilers.Utils.AssemblyTypeReferenceInternal.FullName"/>
    /// </summary>
    /// <returns><inheritdoc cref="BUTR.CrashReport.Decompilers.Utils.AssemblyTypeReferenceInternal.FullName"/></returns>
    public required string FullName { get; set; }
}

/// <summary>
/// Helper method to create import models from an assembly
/// </summary>
public static class ReferenceImporter
{
    /// <summary>
    /// Helper method to create import models from an assembly
    /// </summary>
    public static Dictionary<AssemblyName, AssemblyTypeReferenceInternal[]> GetImportedTypeReferences(Dictionary<AssemblyName, Assembly> AvailableAssemblies) => AvailableAssemblies.ToDictionary(x => x.Key, x =>
    {
        // TODO: Can we do that with the built-in Reflection API?
        foreach (var assemblyModule in x.Value.Modules)
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
                Trace.TraceError(x.Key.ToString());
                Trace.TraceError(e.ToString());
            }
        }
        
        try
        {
            var assembly = AssemblyDefinition.FromFile(x.Value.Location);
            foreach (var module in assembly.Modules)
            {
                return module.GetImportedTypeReferences().Select(y => new AssemblyTypeReferenceInternal
                {
                    Name = y.Name ?? string.Empty,
                    Namespace = y.Namespace ?? string.Empty,
                    FullName = y.FullName,
                }).ToArray();
            }
        }
        catch (Exception e)
        {
            Trace.TraceError(x.Key.ToString());
            Trace.TraceError(e.ToString());
        }
        
        return Array.Empty<AssemblyTypeReferenceInternal>();
    });
}