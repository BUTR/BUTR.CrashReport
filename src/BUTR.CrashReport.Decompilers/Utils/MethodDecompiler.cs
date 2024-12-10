using BUTR.CrashReport.Decompilers.Sources;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BUTR.CrashReport.Decompilers.Utils;

public record MethodDecompilerResult(MethodDecompilerCode IL, MethodDecompilerCode ILMixed, MethodDecompilerCode CSharp);
public record MethodDecompilerCode(IList<string> Code, MethodDecompilerCodeHighlight? Highlight);
public record MethodDecompilerCodeHighlight(int StartLine, int StartColumn, int EndLine, int EndColumn);

/// <summary>
/// Can provide C#, IL and Native representation of a method.
/// </summary>
public static partial class MethodDecompiler
{
    private static MethodDecompilerCode EmptyCode { get; } = new([], null);
    private static MethodDecompilerResult Empty { get; } = new(EmptyCode, EmptyCode, EmptyCode);

    public static MethodDecompilerResult DecompileMethod(MethodBase? method, int? ilOffset, bool skipDecompilation,
        Func<Assembly, Stream?> getAssemblyStream,
        Func<Assembly, Stream?> getPdbStream,
        Func<string, string?> getUrlContent)
    {
        if (method is null) return Empty;

        var ilCode = EmptyCode;
        var ilWithCSharpCode = EmptyCode;

        var csharpSource = default(SourceLocation);

        if (getPdbStream(method.Module.Assembly) is { } pdbStream)
        {
            using var stream = pdbStream;

            var csharpSources = DebugCodeProvider.GetCSharpSourceFromPdb(method, pdbStream, false);
            csharpSource = GetSourceLocation(csharpSources, getUrlContent);
        }

        if (getAssemblyStream(method.Module.Assembly) is { } originalStream)
        {
            using var stream = originalStream;

            if (csharpSource is null)
            {
                stream.Seek(0, SeekOrigin.Begin);
                var csharpSources = DebugCodeProvider.GetCSharpSourcesFromAssembly(method, stream, false);
                csharpSource = GetSourceLocation(csharpSources, getUrlContent);
            }

            if (csharpSource is null && !skipDecompilation)
            {
                stream.Seek(0, SeekOrigin.Begin);
                csharpSource = ILSpyCodeProvider.GetCSharpSourceFromAssembly(method, stream, false);
            }

            stream.Seek(0, SeekOrigin.Begin);
            ilCode = DecompileILCodeInternal(stream, method.MetadataToken, ilOffset);

            stream.Seek(0, SeekOrigin.Begin);
            ilWithCSharpCode = DecompileILWithCSharpCodeInternal(stream, method.MetadataToken, ilOffset, csharpSource);
        }
        else if (TryCopyMethod(method, getAssemblyStream, out var methodCopyStream, out var methodToken))
        {
            using var stream = methodCopyStream;

            if (csharpSource is null && !skipDecompilation)
            {
                stream.Seek(0, SeekOrigin.Begin);
                csharpSource = ILSpyCodeProvider.GetCSharpSourceFromAssembly(method, stream, false);
            }

            stream.Seek(0, SeekOrigin.Begin);
            ilCode = DecompileILCodeInternal(stream, methodToken, ilOffset);

            stream.Seek(0, SeekOrigin.Begin);
            ilWithCSharpCode = DecompileILWithCSharpCodeInternal(stream, method.MetadataToken, ilOffset, csharpSource);

        }

        // If we didn't get any IL code, try to get it from the method definition
        if (ilCode.Code.Count == 0)
        {
            if (TryGetMethodDefinition(method, out _, out var methodDefinition))
            {
                ilCode = DecompileILCodeInternal(methodDefinition, ilOffset);
            }
        }


        var csharpSequence = csharpSource?.SequencePoints.FirstOrDefault(x => x.Offset <= ilOffset && x.EndOffset > ilOffset);
        if (csharpSource is not null && csharpSequence is not null && csharpSequence.IsHidden)
        {
            var idx = csharpSource.SequencePoints.IndexOf(csharpSequence);
            if (idx > 0)
                csharpSequence = csharpSource.SequencePoints[idx - 1];
        }

        var csharpCode = new MethodDecompilerCode(
            csharpSource?.SourceFile.GetMethodLines(csharpSource.SequencePoints) ?? [],
            csharpSequence is null ? null : new(csharpSequence.StartLine, csharpSequence.StartColumn, csharpSequence.EndLine, csharpSequence.EndColumn)
        );

        return new(ilCode, ilWithCSharpCode, csharpCode);
    }

    private static SourceLocation? GetSourceLocation(List<SourceLocation> csharpSources, Func<string, string?> getUrlContent)
    {
        var embeddedSource = csharpSources.FirstOrDefault(x => x.SourceFile is SourceFileEmbedded);
        if (embeddedSource is not null)
        {
            csharpSources.Remove(embeddedSource);
            return embeddedSource;
        }

        var sourceLink = csharpSources.FirstOrDefault(x => x.SourceFile is SourceFileSourceLink);
        if (sourceLink is not null)
        {
            csharpSources.Remove(sourceLink);
            var (filePath, sourceLinkUrl) = (SourceFileSourceLink) sourceLink.SourceFile;
            var sourceCodeString = getUrlContent(sourceLinkUrl);
            if (sourceCodeString is not null)
                return sourceLink with { SourceFile = new SourceFileSourceLinkResolved(filePath, sourceCodeString) };
        }


        return csharpSources.FirstOrDefault();
    }
}