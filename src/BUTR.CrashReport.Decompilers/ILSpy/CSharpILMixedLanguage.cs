using BUTR.CrashReport.Decompilers.Extensions;
using BUTR.CrashReport.Decompilers.Sources;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading;

namespace BUTR.CrashReport.Decompilers.ILSpy;

internal static class CSharpILMixedLanguage
{
    internal class MixedMethodBodyDisassembler : MethodBodyDisassembler
    {
        public int? LineStart { get; private set; }
        public int? LineEnd { get; private set; }

        private readonly int? _ilOffset;
        private readonly SourceFile _csharpSource;
        private readonly Dictionary<int, SourceSequencePoint> _sequencePoints;

        public MixedMethodBodyDisassembler(PlainTextOutput output, int? ilOffset, SourceLocation? csharpSource, CancellationToken ct) : base(output, ct)
        {
            _ilOffset = ilOffset;
            _csharpSource = csharpSource?.SourceFile ?? new SourceFileNone();
            _sequencePoints = csharpSource?.SequencePoints.ToDictionary(x => x.Offset, x => x) ?? [];
        }

        protected override void WriteInstruction(ITextOutput output, MetadataFile metadataFile, MethodDefinitionHandle methodHandle, ref BlobReader blob, int methodRva)
        {
            var lineOffset = 0;

            if (_sequencePoints.TryGetValue(blob.Offset, out var info))
            {
                if (_csharpSource.Kind is SourceKind.None or SourceKind.SourceLink)
                {
                    output.WriteLine("// (C# source code not found)");
                }
                else if (info.IsHidden)
                {
                    output.WriteLine("// (no C# code)");
                    lineOffset++;
                }
                else
                {
                    for (var lineNumber = info.StartLine; lineNumber <= info.EndLine; lineNumber++)
                    {
                        var text = _csharpSource.GetMethodLine(lineNumber, lineNumber);
                        var startColumn = 1;
                        var endColumn = text.Length + 1;
                        if (lineNumber == info.StartLine)
                            startColumn = info.StartColumn;
                        if (lineNumber == info.EndLine)
                            endColumn = info.EndColumn;
                        WriteHighlightedCommentLine(output, text, startColumn - 1, endColumn - 1, info.StartLine == info.EndLine);

                        /*
                        if (_previousLineNumber == lineNumber) continue;
                            
                        output.Write("// ");
                        output.Write(_csharpSource.GetMethodLine(lineNumber, lineNumber));
                        lineOffset++;

                        _previousLineNumber = lineNumber;
                        */
                    }
                }
            }

            base.WriteInstruction(output, metadataFile, methodHandle, ref blob, methodRva);

            if (blob.Offset == _ilOffset)
            {
                var pto = (PlainTextOutput) output;
                LineStart = pto.Location.Line + lineOffset;
                LineEnd = pto.Location.Line + lineOffset;
            }
        }

        private void WriteHighlightedCommentLine(ITextOutput output, string text, int startColumn, int endColumn, bool isSingleLine)
        {
            if (startColumn > text.Length)
            {
                Debug.Fail("startColumn is invalid");
                startColumn = text.Length;
            }
            if (endColumn > text.Length)
            {
                Debug.Fail("endColumn is invalid");
                endColumn = text.Length;
            }
            output.Write("// ");
            output.Write(isSingleLine ? text.AsSpan(0, startColumn).TrimStart() : text.AsSpan(0, startColumn));
            output.Write(text.AsSpan(startColumn, endColumn - startColumn));
            output.Write(text.AsSpan(endColumn));
            output.WriteLine();
        }
    }

    public static MixedMethodBodyDisassembler CreateMethodBodyDisassembler(PlainTextOutput output, int? ilOffset, SourceLocation? csharpSource, CancellationToken ct)
    {
        return new MixedMethodBodyDisassembler(output, ilOffset, csharpSource, ct)
        {
            ShowMetadataTokens = false,
            ShowMetadataTokensInBase10 = false,
            ShowRawRVAOffsetAndBytes = false,
            ShowSequencePoints = false,
            DetectControlStructure = true,
        };
    }
    public static ReflectionDisassembler CreateDisassembler(PlainTextOutput output, MethodBodyDisassembler methodBodyDisassembler, CancellationToken ct)
    {
        return new(output, methodBodyDisassembler, ct)
        {
            ShowMetadataTokens = false,
            ShowMetadataTokensInBase10 = false,
            ShowRawRVAOffsetAndBytes = false,
            ShowSequencePoints = false,
            DetectControlStructure = true,
            ExpandMemberDefinitions = false,
        };
    }
}