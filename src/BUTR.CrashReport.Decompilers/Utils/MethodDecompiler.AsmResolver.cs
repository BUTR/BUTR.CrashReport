using AsmResolver.DotNet;
using AsmResolver.DotNet.Cloning;
using AsmResolver.DotNet.Code.Cil;
using AsmResolver.DotNet.Dynamic;
using AsmResolver.PE.DotNet.Metadata.Tables.Rows;

using Microsoft.IO;

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace BUTR.CrashReport.Decompilers.Utils;

partial class MethodDecompiler
{
    private static bool TryCopyMethod(System.Reflection.MethodBase method, Func<System.Reflection.Assembly, Stream?> getAssemblyStream, [NotNullWhen(true)] out Stream? stream, out int methodToken)
    {
        var assembly = method.Module.Assembly;

        // Prefer assembly copy since it means no File IO
        try
        {
            stream = MethodCopier.GetAssemblyCopy(method, out var metadataToken);
            if (stream is not null)
            {
                methodToken = metadataToken;
                return true;
            }
        }
        catch (Exception e)
        {
            Trace.TraceError(e.ToString());
        }

        stream = getAssemblyStream(assembly);
        if (stream is not null)
        {
            methodToken = method.MetadataToken;
            return true;
        }

        stream = null;
        methodToken = 0;
        return false;
    }

    private static MethodDecompilerCode ToLines(CilInstructionCollection? instructions, int? ilOffset)
    {
        var lines = new string[instructions?.Count ?? 0];
        var lineOffset = default(int?);
        for (var line = 0; line < lines.Length; line++)
        {
            var instruction = instructions![line];
            lines[line] = instruction.ToString();
            if (instruction.Offset == ilOffset)
                lineOffset = line + 1;
        }
        return new MethodDecompilerCode(lines, lineOffset is null ? null : new(lineOffset.Value, -1, lineOffset.Value, -1));
    }

    private static bool TryGetMethodDefinition(System.Reflection.MethodBase method, [NotNullWhen(true)] out ModuleDefinition? module, [NotNullWhen(true)] out MethodDefinition? methodDefinition)
    {
        try
        {
            module = ModuleDefinition.FromModule(typeof(MethodDecompiler).Module);
            methodDefinition = new DynamicMethodDefinition(module, method);
            return true;
        }
        catch (Exception e)
        {
            Trace.TraceError(e.ToString());
        }

        try
        {
            module = ModuleDefinition.FromModule(method.Module);
            methodDefinition = module.LookupMember(method.MetadataToken) as MethodDefinition;
            return methodDefinition is not null;
        }
        catch (Exception e)
        {
            Trace.TraceError(e.ToString());
        }

        module = null;
        methodDefinition = null;
        return false;
    }


    private static MethodDecompilerCode DecompileILCodeInternal(Stream stream, int metadataToken, int? ilOffset)
    {
        try
        {
            var moduleDefinition = ModuleDefinition.FromDataSource(new StreamDataSource(stream));
            if (moduleDefinition.LookupMember(metadataToken) is MethodDefinition methodDefinition)
            {
                return ToLines(methodDefinition.CilMethodBody?.Instructions, ilOffset);
            }
        }
        catch (Exception e)
        {
            Trace.TraceError(e.ToString());
        }

        return EmptyCode;
    }

    private static MethodDecompilerCode DecompileILCodeInternal(MethodDefinition methodDefinition, int? ilOffset)
    {
        return ToLines(methodDefinition.CilMethodBody?.Instructions, ilOffset);
    }
}

file static class MethodCopier
{
    private static readonly RecyclableMemoryStreamManager manager = new();

    public static MemoryStream? GetAssemblyCopy(System.Reflection.MethodBase method, out int metadataToken)
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
        var type = methodDefinition.DeclaringType;
        while (type is not null)
        {
            cloner.Include(type, recursive: false);
            type = type.DeclaringType;
        }
        var result = cloner.Clone();

        var clonedMethodDefinition = result.GetClonedMember(methodDefinition);
        metadataToken = clonedMethodDefinition.MetadataToken.ToInt32();

        if (methodDefinition.DeclaringType is null)
        {
            var typeDefinition = new TypeDefinition("BUTR.CrashReport", "MethodHolder", TypeAttributes.Public | TypeAttributes.Class);
            typeDefinition.Methods.Add(clonedMethodDefinition);
            destinationModule.TopLevelTypes.Add(typeDefinition);
        }

        var ms = manager.GetStream();
        destinationModule.Write(ms);
        ms.Seek(0, SeekOrigin.Begin);

        return ms;
    }
}