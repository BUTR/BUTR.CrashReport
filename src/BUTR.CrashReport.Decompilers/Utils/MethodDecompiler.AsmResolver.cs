using AsmResolver.DotNet;
using AsmResolver.DotNet.Code.Cil;
using AsmResolver.DotNet.Dynamic;

using System;
using System.Linq;
using System.Reflection;

namespace BUTR.CrashReport.Utils;

public static partial class MethodDecompiler
{
    public static string[] DecompileILCode(MethodBase? method)
    {
        static string[] ToLines(CilInstructionCollection? instructions) => instructions?.Select(x => x.ToString()).ToArray() ?? Array.Empty<string>();

        if (method is null) return Array.Empty<string>();

        try
        {
            try
            {
                var module = ModuleDefinition.FromModule(typeof(MethodDecompiler).Module);
                var dynamicMethodDefinition = new DynamicMethodDefinition(module, method);
                return ToLines(dynamicMethodDefinition.CilMethodBody?.Instructions);
            }
            catch (Exception) { /* ignore */ }

            try
            {
                var module = ModuleDefinition.FromModule(method.Module);
                var cilMethodBody = module.LookupMember(method.MetadataToken) is MethodDefinition methodDefinition ? methodDefinition.CilMethodBody : null;
                return ToLines(cilMethodBody?.Instructions);
            }
            catch (Exception) { /* ignore */ }
        }
        catch (Exception) { /* ignore */ }

        return Array.Empty<string>();
    }
}