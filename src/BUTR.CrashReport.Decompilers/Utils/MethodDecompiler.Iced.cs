extern alias iced;

using iced::Iced.Intel;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace BUTR.CrashReport.Decompilers.Utils;

partial class MethodDecompiler
{
    /// <summary>
    /// Gets the Native representation of the methods
    /// </summary>
    public static MethodDecompilerCode DecompileNativeCode(IntPtr nativeCodePtr, int nativeOffset)
    {
        static MethodDecompilerCode GetLines(IntPtr nativeCodePtr, int nativeOffset)
        {
            var codeReader = new PointerCodeReader(nativeCodePtr);
            var decoder = Decoder.Create(IntPtr.Size == 4 ? 32 : 64, codeReader);

            var output = new StringOutput();
            var sb = new System.Text.StringBuilder();

            var formatter = new NasmFormatter
            {
                Options =
                {
                    FirstOperandCharIndex = 10,
                },
            };

            const int maxInstructions = 512;
            var currentInstruction = 0;
            var lines = new List<string>(100);
            var lineHit = -1;
            while (currentInstruction++ < maxInstructions)
            {
                var instr = decoder.Decode();
                if (instr.Code == Code.INVALID) break;
                if (instr.IP < (ulong) (nativeOffset + 16)) break;
                if (instr.IP == (ulong) nativeOffset) lineHit = currentInstruction;

                formatter.Format(instr, output); // Don't use instr.ToString(), it allocates more, uses masm syntax and default options
                sb.Append(instr.IP.ToString("X4")).Append(' ').Append(output.ToStringAndReset());
                lines.Add(sb.ToString());
                sb.Clear();
            }
            return new(lines, lineHit == -1 ? null : new MethodDecompilerCodeHighlight(lineHit, 0, lineHit, 0));
        }

        if (nativeCodePtr == IntPtr.Zero) return EmptyCode;
        if (nativeOffset == StackFrame.OFFSET_UNKNOWN) return EmptyCode;

        try
        {
            return GetLines(nativeCodePtr, nativeOffset);
        }
        catch (Exception e)
        {
            Trace.TraceError(e.ToString());
        }

        return EmptyCode;
    }
}

file sealed class PointerCodeReader : CodeReader
{
    private readonly IntPtr _ptr;
    private int _currentPosition;

    public PointerCodeReader(IntPtr ptr)
    {
        _ptr = ptr;
        _currentPosition = 0;
    }

    public override int ReadByte()
    {
        try
        {
            return Marshal.ReadByte(_ptr, _currentPosition++);
        }
        catch (AccessViolationException)
        {
            return -1;
        }
    }
}