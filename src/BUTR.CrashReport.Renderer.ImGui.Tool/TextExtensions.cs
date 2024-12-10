using System;
using System.IO;
using System.Text;

namespace BUTR.CrashReport.Renderer.ImGui.Tool;

internal static class TextExtensions
{
    public static Encoding PlatformCompatibleUnicode => BitConverter.IsLittleEndian ? Encoding.Unicode : Encoding.BigEndianUnicode;
    private static bool IsPlatformCompatibleUnicode(this Encoding encoding) => BitConverter.IsLittleEndian ? encoding.CodePage == 1200 : encoding.CodePage == 1201;

    public static Stream AsStream(this string @string, Encoding? encoding = null) =>
        (@string ?? throw new ArgumentNullException(nameof(@string))).AsMemory().AsStream(encoding);
    public static Stream AsStream(this ReadOnlyMemory<char> charBuffer, Encoding? encoding = null) => (encoding ??= Encoding.UTF8).IsPlatformCompatibleUnicode()
            ? new UnicodeStream(charBuffer)
            : Encoding.CreateTranscodingStream(new UnicodeStream(charBuffer), PlatformCompatibleUnicode, encoding, false);
}