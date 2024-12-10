using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.Metadata;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Threading;

namespace BUTR.CrashReport.Decompilers.Sources;

internal static class ILSpyCodeProvider
{
    private static readonly List<ICSharpCode.Decompiler.DebugInfo.SequencePoint> Empty = new();

    private static CSharpDecompiler CreateDecompiler(MetadataFile module, DecompilerSettings settings, CancellationToken ct)
    {
        var resolver = new UniversalAssemblyResolver(null, false, module.DetectTargetFrameworkId(), module.DetectRuntimePack());
        return new CSharpDecompiler(module, resolver, settings) { CancellationToken = ct };
    }

    private static void WriteCode(TextWriter output, DecompilerSettings settings, SyntaxTree syntaxTree)
    {
        syntaxTree.AcceptVisitor(new InsertParenthesesVisitor { InsertParenthesesForReadability = true });
        TokenWriter tokenWriter = new TextWriterTokenWriter(output) { IndentationString = settings.CSharpFormattingOptions.IndentationString };
        tokenWriter = TokenWriter.WrapInWriterThatSetsLocationsInAST(tokenWriter);
        syntaxTree.AcceptVisitor(new CSharpOutputVisitor(tokenWriter, settings.CSharpFormattingOptions));
    }

    private static SourceLocation GetCSharpSource(MetadataFile module, MethodDefinitionHandle handle)
    {
        var settings = new DecompilerSettings(LanguageVersion.Latest);
        var decompiler = CreateDecompiler(module, settings, CancellationToken.None);
        var syntaxTree = decompiler.Decompile(handle);

        using var csharpOutput = new StringWriter();
        WriteCode(csharpOutput, settings, syntaxTree);

        var sequencePointsMap = decompiler.CreateSequencePoints(syntaxTree);
        var ilFunction = sequencePointsMap.Keys.FirstOrDefault(x => (x.MoveNextMethod ?? x.Method)?.MetadataToken == handle);
        var sequencePoints = ilFunction is not null ? sequencePointsMap[ilFunction] : Empty;

        return new SourceLocation(
            new SourceFileDecompiled(csharpOutput.GetStringBuilder()),
            sequencePoints.Select(x => new SourceSequencePoint(x.Offset, x.EndOffset, x.StartLine, x.StartColumn, x.EndLine, x.EndColumn, x.IsHidden)).ToArray());
    }

    [return: NotNullIfNotNull("method")]
    public static SourceLocation? GetCSharpSourceFromAssembly(MethodBase? method, Stream assemblyStream, bool disposeStream)
    {
        if (method is null) return null;

        using var peFile = new PEFile("Assembly", assemblyStream, disposeStream ? PEStreamOptions.Default : PEStreamOptions.LeaveOpen);

        var methodHandle = (MethodDefinitionHandle) MetadataTokens.Handle(method.MetadataToken);
        return GetCSharpSource(peFile, methodHandle);
    }
}