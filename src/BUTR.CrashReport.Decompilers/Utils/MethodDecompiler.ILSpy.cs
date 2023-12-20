using BUTR.CrashReport.ILSpy;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.Resolver;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Threading;

namespace BUTR.CrashReport.Utils;

public static partial class MethodDecompiler
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
    
    public static string[] DecompileILWithCSharpCode(MethodBase? method)
    {
        if (method is null) return Array.Empty<string>();
        
        try
        {
            if (!TryCopyMethod(method, out var stream, out var methodHandle)) return Array.Empty<string>();
            
            using var _ = stream;
            using var peFile = new PEFile("Assembly", stream);

            var output = new PlainTextOutput2();
            var disassembler = CSharpILMixedLanguage.CreateDisassembler(output, CancellationToken.None);
            disassembler.DisassembleMethod(peFile, methodHandle);

            return output.ToString()!.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
        }
        catch (Exception e)
        {
            return new[] { e.ToString() };
        }

        return Array.Empty<string>();
    }
    
    public static string[] DecompileCSharpCode(MethodBase? method)
    {
        if (method is null) return Array.Empty<string>();

        try
        {
            if (!TryCopyMethod(method, out var stream, out var methodHandle)) return Array.Empty<string>();

            using var _ = stream;
            using var peFile = new PEFile("Assembly", stream);

            var compilation = new SimpleCompilation(peFile.WithOptions(TypeSystemOptions.Default | TypeSystemOptions.Uncached), MinimalCorlib.Instance);
            var resolver = new CSharpResolver(compilation);

            IModuleReference moduleReference = peFile;
            var module = moduleReference.Resolve(resolver)!;
            var method2 = module.TypeDefinitions.SelectMany(x => x.Methods).First(x => x.MetadataToken == methodHandle);

            var output = new PlainTextOutput2();
            var language = new CSharpLanguage();
            language.DecompileMethod(method2, output, new DecompilerSettings(LanguageVersion.Preview));

            return output.ToString()!.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
        }
        catch (Exception e)
        {
            return new[] { e.ToString() };
        }

        return Array.Empty<string>();
    }
}