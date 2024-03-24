using BUTR.CrashReport.Decompilers.ILSpy;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.Resolver;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace BUTR.CrashReport.Decompilers.Utils;

partial class MethodDecompiler
{
    /// <summary>
    /// Gets the extended IL representation of the methods
    /// </summary>
    public static string[] DecompileILCodeExtended(MethodBase? method)
    {
        if (method is null) return Array.Empty<string>();

        try
        {
            if (!TryCopyMethod(method, out var stream, out var methodHandle)) return Array.Empty<string>();

            using var _ = stream;
            using var peFile = new PEFile("Assembly", stream);

            var output = new PlainTextOutput2();
            var disassembler = ILLanguage.CreateDisassembler(output, CancellationToken.None);
            disassembler.DisassembleMethod(peFile, methodHandle);

            return output.ToString()!.Split([Environment.NewLine], StringSplitOptions.None);
        }
        catch (Exception e)
        {
            Trace.TraceError(e.ToString());
        }

        return Array.Empty<string>();
    }

    /// <summary>
    /// Gets the C# + IL representation of the methods
    /// </summary>
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

            return output.ToString()!.Split([Environment.NewLine], StringSplitOptions.None);
        }
        catch (Exception e)
        {
            Trace.TraceError(e.ToString());
        }

        return Array.Empty<string>();
    }

    /// <summary>
    /// Gets the C# representation of the methods
    /// </summary>
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
            language.DecompileMethod(method2, output, new DecompilerSettings(LanguageVersion.Preview)
            {
                AggressiveInlining = true,
            });

            return output.ToString()!.Split([Environment.NewLine], StringSplitOptions.None);
        }
        catch (Exception e)
        {
            Trace.TraceError(e.ToString());
        }

        return Array.Empty<string>();
    }
}