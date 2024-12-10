using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace BUTR.CrashReport.Renderer.ImGui.Tool;

internal sealed class UnicodeStream : Stream
{
    private const int BytesPerChar = 2;

    // By sealing UnicodeStream we avoid a lot of the complexity of MemoryStream.
    private ReadOnlyMemory<char> _charMemory;
    private int _position;
    private Task<int>? _cachedResultTask; // For async reads, avoid allocating a Task.FromResult<int>(nRead) every time we read.

    public UnicodeStream(string @string) : this((@string ?? throw new ArgumentNullException(nameof(@string))).AsMemory()) { }
    public UnicodeStream(ReadOnlyMemory<char> charMemory) => _charMemory = charMemory;

    public override int Read(Span<byte> buffer)
    {
        EnsureOpen();
        var charPosition = _position / BytesPerChar;
        // MemoryMarshal.AsBytes will throw on strings longer than int.MaxValue / 2, so only slice what we need. 
        var byteSlice = MemoryMarshal.AsBytes(_charMemory.Slice(charPosition, Math.Min(_charMemory.Length - charPosition, 1 + buffer.Length / BytesPerChar)).Span);
        var slicePosition = _position % BytesPerChar;
        var nRead = Math.Min(buffer.Length, byteSlice.Length - slicePosition);
        byteSlice.Slice(slicePosition, nRead).CopyTo(buffer);
        _position += nRead;
        return nRead;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        ValidateBufferArgs(buffer, offset, count);
        return Read(buffer.AsSpan(offset, count));
    }

    public override int ReadByte()
    {
        // Could be optimized.
        Span<byte> span = stackalloc byte[1];
        return Read(span) == 0 ? -1 : span[0];
    }

    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        EnsureOpen();
        if (cancellationToken.IsCancellationRequested)
            return ValueTask.FromCanceled<int>(cancellationToken);
        try
        {
            return new ValueTask<int>(Read(buffer.Span));
        }
        catch (Exception exception)
        {
            return ValueTask.FromException<int>(exception);
        }
    }

    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        ValidateBufferArgs(buffer, offset, count);
        var valueTask = ReadAsync(buffer.AsMemory(offset, count), cancellationToken);
        if (!valueTask.IsCompletedSuccessfully)
            return valueTask.AsTask();
        var lastResultTask = _cachedResultTask;
        return lastResultTask != null && lastResultTask.Result == valueTask.Result ? lastResultTask : _cachedResultTask = Task.FromResult(valueTask.Result);
    }

    private void EnsureOpen()
    {
        if (_position == -1)
            throw new ObjectDisposedException(GetType().Name);
    }

    // https://learn.microsoft.com/en-us/dotnet/api/system.io.stream.flush?view=net-5.0
    // In a class derived from Stream that doesn't support writing, Flush is typically implemented as an empty method to ensure full compatibility with other Stream types since it's valid to flush a read-only stream.
    public override void Flush() { }
    public override Task FlushAsync(CancellationToken cancellationToken) => cancellationToken.IsCancellationRequested ? Task.FromCanceled(cancellationToken) : Task.CompletedTask;
    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => false;
    public override long Length => throw new NotSupportedException();
    public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    protected override void Dispose(bool disposing)
    {
        try
        {
            if (disposing)
            {
                _cachedResultTask = null;
                _charMemory = default;
                _position = -1;
            }
        }
        finally
        {
            base.Dispose(disposing);
        }
    }

    private static void ValidateBufferArgs(byte[] buffer, int offset, int count)
    {
        if (buffer is null)
            throw new ArgumentNullException(nameof(buffer));
        if (offset < 0)
            throw new ArgumentOutOfRangeException(nameof(offset));
        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count));
        if (count > buffer.Length - offset)
            throw new ArgumentException(null, nameof(count));
    }
}