using AsmResolver.DotNet;
using AsmResolver.DotNet.Code.Cil;
using AsmResolver.DotNet.Dynamic;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace BUTR.CrashReport.Utils;

partial class MethodDecompiler
{
    private static bool TryGetMethodDefinition(MethodBase method, [NotNullWhen(true)] out ModuleDefinition? module, [NotNullWhen(true)] out MethodDefinition? methodDefinition)
    {
        try
        {
            module = ModuleDefinition.FromModule(typeof(MethodDecompiler).Module);
            methodDefinition = new DynamicMethodDefinition(module, method);
            return true;
        }
        catch (Exception) { /* ignore */ }

        try
        {
            module = ModuleDefinition.FromModule(method.Module);
            methodDefinition = module.LookupMember(method.MetadataToken) as MethodDefinition;
            return methodDefinition is not null;
        }
        catch (Exception) { /* ignore */ }

        module = null;
        methodDefinition = null;
        return false;
    }
    
    /// <summary>
    /// Gets the IL representation of the methods
    /// </summary>
    public static string[] DecompileILCode(MethodBase? method)
    {
        static string[] ToLines(CilInstructionCollection? instructions) => instructions?.Select(x => x.ToString()).ToArray() ?? Array.Empty<string>();

        if (method is null) return Array.Empty<string>();

        try
        {
            if (!TryGetMethodDefinition(method, out _, out var methodDefinition)) return Array.Empty<string>();

            return ToLines(methodDefinition.CilMethodBody?.Instructions);
        }
        catch (Exception) { /* ignore */ }

        return Array.Empty<string>();
    }
}