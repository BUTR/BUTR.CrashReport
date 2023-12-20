using AsmResolver.DotNet;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BUTR.CrashReport.Utils;

public record AssemblyTypeReferenceInternal
{
    public required string Name { get; set; }
    
    public required string Namespace { get; set; }
    
    public required string FullName { get; set; }
}


public class ReferenceImporter
{
    public static Dictionary<AssemblyName, AssemblyTypeReferenceInternal[]> GetImportedTypeReferences(Dictionary<AssemblyName, Assembly> AvailableAssemblies) => AvailableAssemblies.ToDictionary(x => x.Key, x =>
    {
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
            catch (Exception) { /* ignore */ }

        }
        return Array.Empty<AssemblyTypeReferenceInternal>();
    });

}