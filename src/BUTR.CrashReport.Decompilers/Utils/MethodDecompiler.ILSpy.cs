using BUTR.CrashReport.Decompilers.ILSpy;
using BUTR.CrashReport.Decompilers.Sources;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Threading;

namespace BUTR.CrashReport.Decompilers.Utils;

partial class MethodDecompiler
{
    private static MethodDecompilerCode DecompileILWithCSharpCodeInternal(Stream stream, int methodToken, int? ilOffset, SourceLocation? csharpSource)
    {
        try
        {
            using var peFile = new PEFile("Assembly", stream, PEStreamOptions.LeaveOpen);

            var output = new PlainTextOutput();
            var methodBodyDisassembler = CSharpILMixedLanguage.CreateMethodBodyDisassembler(output, ilOffset, csharpSource, CancellationToken.None);
            var disassembler = CSharpILMixedLanguage.CreateDisassembler(output, methodBodyDisassembler, CancellationToken.None);
            disassembler.DisassembleMethod(peFile, (MethodDefinitionHandle) MetadataTokens.Handle(methodToken));

            return new MethodDecompilerCode(
                output.ToString()!.Split([Environment.NewLine], StringSplitOptions.None),
                methodBodyDisassembler.LineStart is null ? null : new(methodBodyDisassembler.LineStart.Value, -1, methodBodyDisassembler.LineEnd ?? -1, -1)
            );
        }
        catch (Exception e)
        {
            Trace.TraceError(e.ToString());
        }

        return EmptyCode;
    }
}