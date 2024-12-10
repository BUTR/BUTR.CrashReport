using AsmResolver.IO;

using System.IO;

namespace BUTR.CrashReport.Decompilers.Utils;

internal class StreamDataSource : IDataSource
{
    public ulong BaseAddress { get; }

    public byte this[ulong address]
    {
        get
        {
            _stream.Seek((long) (address - BaseAddress), SeekOrigin.Begin);
            return (byte) _stream.ReadByte();
        }
    }
    public ulong Length => (ulong) _stream.Length;

    private readonly Stream _stream;

    public StreamDataSource(Stream stream)
    {
        _stream = stream;
        BaseAddress = 0;
    }

    public bool IsValidAddress(ulong address) => address - BaseAddress < (ulong) _stream.Length;

    public int ReadBytes(ulong address, byte[] buffer, int index, int count)
    {
        _stream.Seek((long) (address - BaseAddress), SeekOrigin.Begin);
        return _stream.Read(buffer, index, count);
    }
}