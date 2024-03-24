using AsmResolver.DotNet;
using AsmResolver.DotNet.Cloning;
using AsmResolver.DotNet.Dynamic;

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

using TypeAttributes = AsmResolver.PE.DotNet.Metadata.Tables.Rows.TypeAttributes;

namespace BUTR.CrashReport.Decompilers.Utils;

internal static class MethodCopier
{
    public static MemoryStream? GetAssemblyCopy(MethodBase method, out int metadataToken)
    {
        metadataToken = 0;
        MethodDefinition? methodDefinition = null;
        ModuleDefinition? module = null;

        try
        {
            module = ModuleDefinition.FromModule(typeof(MethodDecompiler).Module);
            methodDefinition = new DynamicMethodDefinition(module, method);
        }
        catch (Exception e)
        {
            Trace.TraceError(e.ToString());
        }

        try
        {
            module = ModuleDefinition.FromModule(method.Module);
            methodDefinition = module.LookupMember(method.MetadataToken) as MethodDefinition;
        }
        catch (Exception e)
        {
            Trace.TraceError(e.ToString());
        }

        if (module is null || methodDefinition is null) return null;

        var destinationModule = new ModuleDefinition(method.Module.Name);

        var cloner = new MemberCloner(destinationModule);
        if (methodDefinition.DeclaringType is not null) cloner.Include(methodDefinition.DeclaringType, recursive: true);
        cloner.Include(methodDefinition, recursive: true);
        cloner.AddListener(new InjectTypeClonerListener(destinationModule));
        cloner.AddListener(new AssignTokensClonerListener(destinationModule));
        var result = cloner.Clone();

        var clonedMethodDefinition = result.GetClonedMember(methodDefinition);
        metadataToken = clonedMethodDefinition.MetadataToken.ToInt32();

        if (methodDefinition.DeclaringType is null)
        {
            var typeDefinition = new TypeDefinition("BUTR.CrashReport", "MethodHolder", TypeAttributes.Public | TypeAttributes.Class);
            typeDefinition.Methods.Add(clonedMethodDefinition);
            destinationModule.TopLevelTypes.Add(typeDefinition);
        }

        var ms = new MemoryStream();
        destinationModule.Write(ms);
        ms.Seek(0, SeekOrigin.Begin);

        return ms;
    }
}