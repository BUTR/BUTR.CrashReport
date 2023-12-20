using System.IO;
using System.Text;

namespace BUTR.CrashReport.ILSpy;

internal static class TextWriterExtensions
{
    public static void Write(this TextWriter writer, StringBuilder sb, int offset, int length)
    {
        var buffer = length > 512 ? new char[length] : stackalloc char[length];
        sb.CopyTo(offset, buffer, length);
        writer.Write(buffer);
    }
}