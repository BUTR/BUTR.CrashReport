extern alias iced;

using iced::Iced.Intel;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using Decoder = iced::Iced.Intel.Decoder;

namespace BUTR.CrashReport.Decompilers.Utils;

partial class MethodDecompiler
{
    /// <summary>
    /// Gets the Native representation of the methods
    /// </summary>
    public static string[] DecompileNativeCode(IntPtr nativeCodePtr, int nativeILOffset)
    {
        static IEnumerable<string> GetLines(IntPtr nativeCodePtr, int nativeILOffset)
        {
            var length = (uint) nativeILOffset + 16;
            var bytecode = new byte[length];

            Marshal.Copy(nativeCodePtr, bytecode, 0, bytecode.Length);

            var codeReader = new ByteArrayCodeReader(bytecode);
            var decoder = Decoder.Create(IntPtr.Size == 4 ? 32 : 64, codeReader);

            var output = new StringOutput();
            var sb = new StringBuilder();

            var formatter = new NasmFormatter
            {
                Options =
                {
                    FirstOperandCharIndex = 10,
                }
            };

            while (decoder.IP < length)
            {
                var instr = decoder.Decode();
                formatter.Format(instr, output); // Don't use instr.ToString(), it allocates more, uses masm syntax and default options
                sb.Append(instr.IP.ToString("X4")).Append(' ').Append(output.ToStringAndReset());
                yield return sb.ToString();
                sb.Clear();
            }
        }

        if (nativeCodePtr == IntPtr.Zero) return [];
        if (nativeILOffset == StackFrame.OFFSET_UNKNOWN) return [];

        try
        {
            return GetLines(nativeCodePtr, nativeILOffset).ToArray();
        }
        catch (Exception e)
        {
            Trace.TraceError(e.ToString());
        }

        return [];
    }
}