using System;
using System.IO;
using System.Runtime.InteropServices;
using static Fp.Processor;

namespace Fp.Helpers;

/// <summary>
/// Signed 8-bit helper.
/// </summary>
public record S8Helper(Processor Parent) : BaseUnmanagedIntegerHelper<sbyte>
{
    /// <inheritdoc />
    public override Stream InputStream => Parent.InputStream ?? throw new InvalidOperationException();

    /// <inheritdoc />
    public override Stream OutputStream => Parent.InputStream ?? throw new InvalidOperationException();

    /// <inheritdoc />
    public override sbyte this[Span<byte> source]
    {
        get => (sbyte)source[0];
        set => source[0] = (byte)value;
    }

    /// <inheritdoc />
    public override sbyte this[ReadOnlySpan<byte> source] => (sbyte)source[0];
}

/// <summary>
/// Signed 8-bit array helper.
/// </summary>
public record S8ArrayHelper(Processor Parent) : BaseUnmanagedIntegerArrayHelper<sbyte>
{
    /// <inheritdoc />
    public override Stream InputStream => Parent.InputStream ?? throw new InvalidOperationException();

    /// <inheritdoc />
    public override Stream OutputStream => Parent.InputStream ?? throw new InvalidOperationException();

    /// <inheritdoc />
    public override ReadOnlySpan<sbyte> this[Span<byte> source]
    {
        get => MemoryMarshal.Cast<byte, sbyte>(source);
        set => SetS8Array(source, value);
    }

    /// <inheritdoc />
    public override ReadOnlySpan<sbyte> this[ReadOnlySpan<byte> source] => MemoryMarshal.Cast<byte, sbyte>(source);
}

/// <summary>
/// Signed 16-bit helper.
/// </summary>
public record S16Helper(Processor Parent, bool LittleEndian) : BaseUnmanagedIntegerHelper<short>
{
    /// <inheritdoc />
    public override Stream InputStream => Parent.InputStream ?? throw new InvalidOperationException();

    /// <inheritdoc />
    public override Stream OutputStream => Parent.InputStream ?? throw new InvalidOperationException();

    /// <inheritdoc />
    public override short this[Span<byte> source]
    {
        get => GetS16(source, LittleEndian);
        set => SetS16(source, value, LittleEndian);
    }

    /// <inheritdoc />
    public override short this[ReadOnlySpan<byte> source] => GetS16(source, LittleEndian);
}

/// <summary>
/// Signed 16-bit array helper.
/// </summary>
public record S16ArrayHelper(Processor Parent, bool LittleEndian) : BaseUnmanagedIntegerArrayHelper<short>
{
    /// <inheritdoc />
    public override Stream InputStream => Parent.InputStream ?? throw new InvalidOperationException();

    /// <inheritdoc />
    public override Stream OutputStream => Parent.InputStream ?? throw new InvalidOperationException();

    /// <inheritdoc />
    public override ReadOnlySpan<short> this[Span<byte> source]
    {
        get => GetS16Array(source, LittleEndian);
        set => SetS16Array(source, value, LittleEndian);
    }

    /// <inheritdoc />
    public override ReadOnlySpan<short> this[ReadOnlySpan<byte> source] => GetS16Array(source, LittleEndian);
}

/// <summary>
/// Signed 32-bit helper.
/// </summary>
public record S32Helper(Processor Parent, bool LittleEndian) : BaseUnmanagedIntegerHelper<int>
{
    /// <inheritdoc />
    public override Stream InputStream => Parent.InputStream ?? throw new InvalidOperationException();

    /// <inheritdoc />
    public override Stream OutputStream => Parent.InputStream ?? throw new InvalidOperationException();

    /// <inheritdoc />
    public override int this[Span<byte> source]
    {
        get => GetS32(source, LittleEndian);
        set => SetS32(source, value, LittleEndian);
    }

    /// <inheritdoc />
    public override int this[ReadOnlySpan<byte> source] => GetS32(source, LittleEndian);
}

/// <summary>
/// Signed 32-bit helper.
/// </summary>
public record S32ArrayHelper(Processor Parent, bool LittleEndian) : BaseUnmanagedIntegerArrayHelper<int>
{
    /// <inheritdoc />
    public override Stream InputStream => Parent.InputStream ?? throw new InvalidOperationException();

    /// <inheritdoc />
    public override Stream OutputStream => Parent.InputStream ?? throw new InvalidOperationException();

    /// <inheritdoc />
    public override ReadOnlySpan<int> this[Span<byte> source]
    {
        get => GetS32Array(source, LittleEndian);
        set => SetS32Array(source, value, LittleEndian);
    }

    /// <inheritdoc />
    public override ReadOnlySpan<int> this[ReadOnlySpan<byte> source] => GetS32Array(source, LittleEndian);
}

/// <summary>
/// Signed 64-bit helper.
/// </summary>
public record S64Helper(Processor Parent, bool LittleEndian) : BaseUnmanagedIntegerHelper<long>
{
    /// <inheritdoc />
    public override Stream InputStream => Parent.InputStream ?? throw new InvalidOperationException();

    /// <inheritdoc />
    public override Stream OutputStream => Parent.InputStream ?? throw new InvalidOperationException();

    /// <inheritdoc />
    public override long this[Span<byte> source]
    {
        get => GetS64(source, LittleEndian);
        set => SetS64(source, value, LittleEndian);
    }

    /// <inheritdoc />
    public override long this[ReadOnlySpan<byte> source] => GetS64(source, LittleEndian);
}

/// <summary>
/// Signed 64-bit helper.
/// </summary>
public record S64ArrayHelper(Processor Parent, bool LittleEndian) : BaseUnmanagedIntegerArrayHelper<long>
{
    /// <inheritdoc />
    public override Stream InputStream => Parent.InputStream ?? throw new InvalidOperationException();

    /// <inheritdoc />
    public override Stream OutputStream => Parent.InputStream ?? throw new InvalidOperationException();

    /// <inheritdoc />
    public override ReadOnlySpan<long> this[Span<byte> source]
    {
        get => GetS64Array(source, LittleEndian);
        set => SetS64Array(source, value, LittleEndian);
    }

    /// <inheritdoc />
    public override ReadOnlySpan<long> this[ReadOnlySpan<byte> source] => GetS64Array(source, LittleEndian);
}

/// <summary>
/// Unsigned 8-bit helper.
/// </summary>
public record U8Helper(Processor Parent) : BaseUnmanagedIntegerHelper<byte>
{
    /// <inheritdoc />
    public override Stream InputStream => Parent.InputStream ?? throw new InvalidOperationException();

    /// <inheritdoc />
    public override Stream OutputStream => Parent.InputStream ?? throw new InvalidOperationException();

    /// <inheritdoc />
    public override byte this[Span<byte> source]
    {
        get => source[0];
        set => source[0] = value;
    }

    /// <inheritdoc />
    public override byte this[ReadOnlySpan<byte> source] => source[0];
}

/// <summary>
/// Unsigned 8-bit helper.
/// </summary>
public record U8ArrayHelper(Processor Parent) : BaseUnmanagedIntegerArrayHelper<byte>
{
    /// <inheritdoc />
    public override Stream InputStream => Parent.InputStream ?? throw new InvalidOperationException();

    /// <inheritdoc />
    public override Stream OutputStream => Parent.InputStream ?? throw new InvalidOperationException();

    /// <inheritdoc />
    public override ReadOnlySpan<byte> this[Span<byte> source]
    {
        get => source;
        set => SetU8Array(source, value);
    }

    /// <inheritdoc />
    public override ReadOnlySpan<byte> this[ReadOnlySpan<byte> source] => source;

    /// <summary>
    /// Reads data.
    /// </summary>
    /// <param name="source">Data source.</param>
    /// <param name="range">Range.</param>
    public ReadOnlySpan<byte> this[byte[] source, Range range] =>
        this[source.AsSpan(), range];

    /// <summary>
    /// Reads data.
    /// </summary>
    /// <param name="source">Data source.</param>
    /// <param name="range">Range.</param>
    public ReadOnlySpan<byte> this[Memory<byte> source, Range range] =>
        this[source.Span, range];

    /// <summary>
    /// Reads data.
    /// </summary>
    /// <param name="source">Data source.</param>
    /// <param name="range">Range.</param>
    public virtual ReadOnlySpan<byte> this[ReadOnlyMemory<byte> source, Range range] =>
        this[source.Span, range];

    /// <summary>
    /// Reads data.
    /// </summary>
    /// <param name="source">Data source.</param>
    /// <param name="range">Range.</param>
    public ReadOnlySpan<byte> this[Span<byte> source, Range range]
    {
        get
        {
            return this[source[range]];
        }
    }

    /// <summary>
    /// Reads data.
    /// </summary>
    /// <param name="source">Data source.</param>
    /// <param name="range">Range.</param>
    public ReadOnlySpan<byte> this[ReadOnlySpan<byte> source, Range range]
    {
        get
        {
            return this[source[range]];
        }
    }

    /// <summary>
    /// Reads data.
    /// </summary>
    /// <param name="range">Range.</param>
    public virtual byte[] this[Range range] => this[range, InputStream];

    /// <summary>
    /// Reads data.
    /// </summary>
    /// <param name="range">Range.</param>
    /// <param name="stream">Data source.</param>
    public virtual byte[] this[Range range, Stream stream]
    {
        get
        {
            if (stream.Length > int.MaxValue)
            {
                throw new ArgumentException($"Stream exceeds {int.MaxValue} bytes");
            }
            (int offset, int length) = range.GetOffsetAndLength((int)stream.Length);
            byte[] arr = System.Buffers.ArrayPool<byte>.Shared.Rent(length);
            try
            {
                Span<byte> span = arr.AsSpan(0, length);
                Read(stream, offset, span, false);
                return this[arr, 0, length].ToArray();
            }
            finally
            {
                System.Buffers.ArrayPool<byte>.Shared.Return(arr);
            }
        }
    }
}

/// <summary>
/// Unsigned 16-bit helper.
/// </summary>
public record U16Helper(Processor Parent, bool LittleEndian) : BaseUnmanagedIntegerHelper<ushort>
{
    /// <inheritdoc />
    public override Stream InputStream => Parent.InputStream ?? throw new InvalidOperationException();

    /// <inheritdoc />
    public override Stream OutputStream => Parent.InputStream ?? throw new InvalidOperationException();

    /// <inheritdoc />
    public override ushort this[Span<byte> source]
    {
        get => GetU16(source, LittleEndian);
        set => SetU16(source, value, LittleEndian);
    }

    /// <inheritdoc />
    public override ushort this[ReadOnlySpan<byte> source] => GetU16(source, LittleEndian);
}

/// <summary>
/// Unsigned 16-bit helper.
/// </summary>
public record U16ArrayHelper(Processor Parent, bool LittleEndian) : BaseUnmanagedIntegerArrayHelper<ushort>
{
    /// <inheritdoc />
    public override Stream InputStream => Parent.InputStream ?? throw new InvalidOperationException();

    /// <inheritdoc />
    public override Stream OutputStream => Parent.InputStream ?? throw new InvalidOperationException();

    /// <inheritdoc />
    public override ReadOnlySpan<ushort> this[Span<byte> source]
    {
        get => GetU16Array(source, LittleEndian);
        set => SetU16Array(source, value, LittleEndian);
    }

    /// <inheritdoc />
    public override ReadOnlySpan<ushort> this[ReadOnlySpan<byte> source] => GetU16Array(source, LittleEndian);
}

/// <summary>
/// Unsigned 32-bit helper.
/// </summary>
public record U32Helper(Processor Parent, bool LittleEndian) : BaseUnmanagedIntegerHelper<uint>
{
    /// <inheritdoc />
    public override Stream InputStream => Parent.InputStream ?? throw new InvalidOperationException();

    /// <inheritdoc />
    public override Stream OutputStream => Parent.InputStream ?? throw new InvalidOperationException();

    /// <inheritdoc />
    public override uint this[Span<byte> source]
    {
        get => GetU32(source, LittleEndian);
        set => SetU32(source, value, LittleEndian);
    }

    /// <inheritdoc />
    public override uint this[ReadOnlySpan<byte> source] => GetU32(source, LittleEndian);
}

/// <summary>
/// Unsigned 32-bit helper.
/// </summary>
public record U32ArrayHelper(Processor Parent, bool LittleEndian) : BaseUnmanagedIntegerArrayHelper<uint>
{
    /// <inheritdoc />
    public override Stream InputStream => Parent.InputStream ?? throw new InvalidOperationException();

    /// <inheritdoc />
    public override Stream OutputStream => Parent.InputStream ?? throw new InvalidOperationException();

    /// <inheritdoc />
    public override ReadOnlySpan<uint> this[Span<byte> source]
    {
        get => GetU32Array(source, LittleEndian);
        set => SetU32Array(source, value, LittleEndian);
    }

    /// <inheritdoc />
    public override ReadOnlySpan<uint> this[ReadOnlySpan<byte> source] => GetU32Array(source, LittleEndian);
}

/// <summary>
/// Unsigned 64-bit helper.
/// </summary>
public record U64Helper(Processor Parent, bool LittleEndian) : BaseUnmanagedIntegerHelper<ulong>
{
    /// <inheritdoc />
    public override Stream InputStream => Parent.InputStream ?? throw new InvalidOperationException();

    /// <inheritdoc />
    public override Stream OutputStream => Parent.InputStream ?? throw new InvalidOperationException();

    /// <inheritdoc />
    public override ulong this[Span<byte> source]
    {
        get => GetU64(source, LittleEndian);
        set => SetU64(source, value, LittleEndian);
    }

    /// <inheritdoc />
    public override ulong this[ReadOnlySpan<byte> source] => GetU64(source, LittleEndian);
}

/// <summary>
/// Unsigned 64-bit helper.
/// </summary>
public record U64ArrayHelper(Processor Parent, bool LittleEndian) : BaseUnmanagedIntegerArrayHelper<ulong>
{
    /// <inheritdoc />
    public override Stream InputStream => Parent.InputStream ?? throw new InvalidOperationException();

    /// <inheritdoc />
    public override Stream OutputStream => Parent.InputStream ?? throw new InvalidOperationException();

    /// <inheritdoc />
    public override ReadOnlySpan<ulong> this[Span<byte> source]
    {
        get => GetU64Array(source, LittleEndian);
        set => SetU64Array(source, value, LittleEndian);
    }

    /// <inheritdoc />
    public override ReadOnlySpan<ulong> this[ReadOnlySpan<byte> source] => GetU64Array(source, LittleEndian);
}

/// <summary>
/// Generic helper.
/// </summary>
public record THelper<T>(Processor Parent) : BaseUnmanagedHelper<T> where T : unmanaged
{
    /// <inheritdoc />
    public override Stream InputStream => Parent.InputStream ?? throw new InvalidOperationException();

    /// <inheritdoc />
    public override Stream OutputStream => Parent.InputStream ?? throw new InvalidOperationException();

    /// <inheritdoc />
    public override T this[Span<byte> source]
    {
        get => MemoryMarshal.Cast<byte, T>(source)[0];
        set => MemoryMarshal.Cast<byte, T>(source)[0] = value;
    }

    /// <inheritdoc />
    public override T this[ReadOnlySpan<byte> source] =>
        MemoryMarshal.Cast<byte, T>(source)[0];
}

/// <summary>
/// Generic helper.
/// </summary>
public record TArrayHelper<T>(Processor Parent) : BaseUnmanagedArrayHelper<T> where T : unmanaged
{
    /// <inheritdoc />
    public override Stream InputStream => Parent.InputStream ?? throw new InvalidOperationException();

    /// <inheritdoc />
    public override Stream OutputStream => Parent.InputStream ?? throw new InvalidOperationException();

    /// <inheritdoc />
    public override ReadOnlySpan<T> this[Span<byte> source]
    {
        get => GetTArray<T>(source);
        set => SetTArray(source, value);
    }

    /// <inheritdoc />
    public override ReadOnlySpan<T> this[ReadOnlySpan<byte> source] =>
        GetTArray<T>(source);
}

/// <summary>
/// Unsigned 16-bit float helper.
/// </summary>
public record F16Helper(Processor Parent) : BaseHelper<float>
{
    /// <inheritdoc />
    public override Stream InputStream => Parent.InputStream ?? throw new InvalidOperationException();

    /// <inheritdoc />
    public override Stream OutputStream => Parent.InputStream ?? throw new InvalidOperationException();

    /// <inheritdoc />
    public override float this[Span<byte> source]
    {
        get => this[(ReadOnlySpan<byte>)source];
        set => GetBytesHalf(value, source);
    }

    /// <inheritdoc />
    public override float this[ReadOnlySpan<byte> source] => GetHalf(source);

    /// <inheritdoc />
    public override float this[long offset, Stream stream]
    {
        get => offset != -1 ? Parent.ReadHalf(offset, stream) : Parent.ReadHalf(stream);
        set => Parent.WriteHalf(value, stream, offset != -1 ? offset : null);
    }
}

/// <summary>
/// Unsigned 16-bit float array helper.
/// </summary>
public record F16ArrayHelper(Processor Parent) : BaseUnmanagedArrayHelper<float>
{
    /// <inheritdoc />
    public override int ElementSize => 2;

    /// <inheritdoc />
    public override Stream InputStream => Parent.InputStream ?? throw new InvalidOperationException();

    /// <inheritdoc />
    public override Stream OutputStream => Parent.InputStream ?? throw new InvalidOperationException();

    /// <inheritdoc />
    public override ReadOnlySpan<float> this[Span<byte> source]
    {
        get
        {
            float[] result = new float[source.Length / 2];
            // New array for aligning
            ConvertHalfArrayToFloat(source[..(source.Length / 2 * 2)], result);
            return result;
        }
        set
        {
            byte[] src = new byte[value.Length * 2];
            ConvertFloatArrayToHalf(value, src);
            src.CopyTo(source);
        }
    }

    /// <inheritdoc />
    public override ReadOnlySpan<float> this[ReadOnlySpan<byte> source]
    {
        get
        {
            float[] result = new float[source.Length / 2];
            // New array for aligning
            ConvertHalfArrayToFloat(source[..(source.Length / 2 * 2)], result);
            return result;
        }
    }
}
