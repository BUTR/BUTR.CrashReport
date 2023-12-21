using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace BUTR.CrashReport.Utils;

partial class MethodDecompiler
{
    private static bool TryCopyMethod(MethodBase method, [NotNullWhen(true)] out Stream? stream, out MethodDefinitionHandle methodHandle)
    {
        var assembly = method.Module.Assembly;

        if (!assembly.IsDynamic && !string.IsNullOrEmpty(assembly.Location))
        {
            try
            {
                methodHandle = MetadataTokens.MethodDefinitionHandle(method.MetadataToken);
                stream = File.OpenRead(assembly.Location);
                return true;
            }
            catch (Exception) { /* ignore */ }
        }

        try
        {
            stream = MethodCopier.GetAssemblyCopy(method, out var metadataToken);
            if (stream is not null)
            {
                methodHandle = MetadataTokens.MethodDefinitionHandle(metadataToken);
                return true;
            }
        }
        catch (Exception) { /* ignore */ }

        stream = null;
        methodHandle = default;
        return false;
    }
}