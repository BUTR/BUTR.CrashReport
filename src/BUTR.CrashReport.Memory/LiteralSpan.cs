using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BUTR.CrashReport.Memory;

public readonly unsafe struct LiteralSpan<T> : IEquatable<LiteralSpan<T>> where T : unmanaged
{
    public static LiteralSpan<byte> Empty => default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator LiteralSpan<T>(ReadOnlySpan<T> span) => new(span);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlySpan<T>(LiteralSpan<T> span) => new(span.Ptr, span.Length);

    public readonly T* Ptr;
    public readonly int Length;

    public LiteralSpan(ReadOnlySpan<T> span)
    {
        Ptr = (T*) Unsafe.AsPointer(ref MemoryMarshal.GetReference(span));
        Length = span.Length;
    }

    public LiteralSpan(T* ptr, int length)
    {
        Ptr = ptr;
        Length = length;
    }

    public ref readonly T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref Unsafe.AsRef<T>(Unsafe.Add<T>(Ptr, index));
    }

    public Enumerator GetEnumerator() => new(this);

    public bool Equals(LiteralSpan<T> other) => Ptr == other.Ptr && Length == other.Length;

    public override bool Equals(object? obj) => obj is LiteralSpan<T> other && Equals(other);

    public override int GetHashCode()
    {
        unchecked
        {
            return (unchecked((int) (long) Ptr) * 397) ^ Length;
        }
    }

    public ref struct Enumerator
    {
        private readonly LiteralSpan<T> _literalSpan;
        private int _index;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Enumerator(LiteralSpan<T> literalSpan)
        {
            _literalSpan = literalSpan;
            _index = -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            var index = _index + 1;
            if (index >= _literalSpan.Length) return false;

            _index = index;
            return true;

        }

        public ref readonly T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _literalSpan[_index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset() => _index = -1;

        public void Dispose() { }
    }
}