using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;

using System.Threading;

namespace BUTR.CrashReport.Decompilers.ILSpy;

internal class ILLanguage
{
    public static ReflectionDisassembler CreateDisassembler(ITextOutput output, CancellationToken ct)
    {
        return new(output, ct)
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