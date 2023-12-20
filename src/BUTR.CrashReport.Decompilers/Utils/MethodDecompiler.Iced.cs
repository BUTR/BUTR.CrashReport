extern alias iced;
using iced::Iced.Intel;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

using static BUTR.CrashReport.Utils.MonoModUtils;

using Decoder = iced::Iced.Intel.Decoder;

namespace BUTR.CrashReport.Utils;

public static partial class MethodDecompiler
{
    public static string[] DecompileNativeCode(MethodBase? method, int nativeILOffset)
    {
        static IEnumerable<string> GetLines(MethodBase method, int nativeILOffset)
        {
            var nativeCodePtr = GetNativeMethodBody!(CurrentPlatformTriple!(), method);

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
                    DigitSeparator = "`",
                    FirstOperandCharIndex = 10
                }
            };

            while (decoder.IP < length)
            {
                var instr = decoder.Decode();
                formatter.Format(instr, output); // Don't use instr.ToString(), it allocates more, uses masm syntax and default options
                sb.Append(instr.IP.ToString("X4")).Append(" ").Append(output.ToStringAndReset());
                yield return sb.ToString();
                sb.Clear();
            }
        }

        if (method is null) return Array.Empty<string>();
        if (nativeILOffset == StackFrame.OFFSET_UNKNOWN) return Array.Empty<string>();
        if (CurrentPlatformTriple is null || GetNativeMethodBody is null) return Array.Empty<string>();

        try
        {
            return GetLines(method, nativeILOffset).ToArray();
        }
        catch (Exception) { /* ignore */ }
        
        return Array.Empty<string>();
    }
}