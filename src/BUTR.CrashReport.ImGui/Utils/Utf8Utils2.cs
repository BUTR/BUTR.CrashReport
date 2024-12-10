using Cysharp.Text;

public static class Utf8Utils2
{
    public static byte[] Join(ReadOnlySpan<byte> separator, IList<string> nativeInstructionsInstructions)
    {
        using var sb = new Utf8ValueStringBuilder(false);
        for (var i = 0; i < nativeInstructionsInstructions.Count; i++)
        {
            if (i > 0)
                sb.AppendLiteral(separator);
            sb.Append(nativeInstructionsInstructions[i]);
        }
        return sb.AsSpan().ToArray();
    }
}