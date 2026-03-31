using System;
using System.IO;
using System.Text;
using static System.Buffers.ArrayPool<byte>;
using static Fp.Processor;

namespace Fp.Helpers;

/// <summary>
/// Base helper type for strings.
/// </summary>
public abstract record BaseStringHelper : Helper
{
    /// <summary>
    /// Reads data.
    /// </summary>
    /// <param name="source">Data source.</param>
    /// <param name="range">Max range to read.</param>
    public virtual StringData this[byte[] source, Range range] =>
        this[source.AsSpan(), range];

    /// <summary>
    /// Reads data.
    /// </summary>
    /// <param name="source">Data source.</param>
    /// <param name="offset">Offset.</param>
    /// <param name="maxBytes">Maximum bytes to read.</param>
    public virtual StringData this[byte[] source, int offset, int maxBytes] =>
        this[source.AsSpan(), offset, maxBytes];

    /// <summary>
    /// Writes data.
    /// </summary>
    /// <param name="source">Data source.</param>
    /// <param name="offset">Offset.</param>
    public virtual StringData this[byte[] source, int offset]
    {
        get => this[source.AsSpan(), offset];
        set => this[source.AsSpan(), offset] = value;
    }

    /// <summary>
    /// Writes data.
    /// </summary>
    /// <param name="source">Data source.</param>
    public virtual StringData this[byte[] source]
    {
        get => this[source.AsSpan()];
        set => this[source.AsSpan()] = value;
    }

    /// <summary>
    /// Reads data.
    /// </summary>
    /// <param name="source">Data source.</param>
    /// <param name="range">Max range to read.</param>
    public virtual StringData this[Memory<byte> source, Range range] =>
        this[source.Span, range];

    /// <summary>
    /// Reads data.
    /// </summary>
    /// <param name="source">Data source.</param>
    /// <param name="offset">Offset.</param>
    /// <param name="maxBytes">Maximum bytes to read.</param>
    public virtual StringData this[Memory<byte> source, int offset, int maxBytes] =>
        this[source.Span, offset, maxBytes];

    /// <summary>
    /// Reads/writes data.
    /// </summary>
    /// <param name="offset">Offset.</param>
    /// <param name="source">Data source.</param>
    public virtual StringData this[Memory<byte> source, int offset]
    {
        get => this[source.Span, offset];
        set => this[source.Span, offset] = value;
    }

    /// <summary>
    /// Reads/writes data.
    /// </summary>
    /// <param name="source">Data source.</param>
    public virtual StringData this[Memory<byte> source]
    {
        get => this[source.Span];
        set => this[source.Span] = value;
    }

    /// <summary>
    /// Reads data.
    /// </summary>
    /// <param name="source">Data source.</param>
    /// <param name="range">Max range to read.</param>
    public virtual StringData this[Span<byte> source, Range range] =>
        this[(ReadOnlySpan<byte>)source, range];

    /// <summary>
    /// Reads data.
    /// </summary>
    /// <param name="source">Data source.</param>
    /// <param name="offset">Offset.</param>
    /// <param name="maxBytes">Maximum bytes to read.</param>
    public virtual StringData this[Span<byte> source, int offset, int maxBytes] =>
        this[(ReadOnlySpan<byte>)source, offset, maxBytes];

    /// <summary>
    /// Reads/writes data.
    /// </summary>
    /// <param name="source">Data source.</param>
    /// <param name="offset">Offset.</param>
    public virtual StringData this[Span<byte> source, int offset]
    {
        get => this[source[offset..]];
        set => this[source[offset..]] = value;
    }

    /// <summary>
    /// Reads/writes data.
    /// </summary>
    /// <param name="source">Data source.</param>
    public abstract StringData this[Span<byte> source] { get; set; }

    /// <summary>
    /// Reads data.
    /// </summary>
    /// <param name="source">Data source.</param>
    /// <param name="range">Max range to read.</param>
    public virtual StringData this[ReadOnlyMemory<byte> source, Range range] =>
        this[source.Span, range];

    /// <summary>
    /// Reads data.
    /// </summary>
    /// <param name="source">Data source.</param>
    /// <param name="offset">Offset.</param>
    /// <param name="maxBytes">Maximum bytes to read.</param>
    public virtual StringData this[ReadOnlyMemory<byte> source, int offset, int maxBytes] =>
        this[source.Span, offset, maxBytes];

    /// <summary>
    /// Reads data.
    /// </summary>
    /// <param name="offset">Offset.</param>
    /// <param name="source">Data source.</param>
    public virtual StringData this[ReadOnlyMemory<byte> source, int offset] => this[source.Span, offset];

    /// <summary>
    /// Reads data.
    /// </summary>
    /// <param name="source">Data source.</param>
    /// <param name="range">Max range to read.</param>
    public virtual StringData this[ReadOnlySpan<byte> source, Range range]
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
    /// <param name="offset">Offset.</param>
    /// <param name="maxBytes">Maximum bytes to read.</param>
    public abstract StringData this[ReadOnlySpan<byte> source, int offset,
        int maxBytes] { get; }

    /// <summary>
    /// Reads data.
    /// </summary>
    /// <param name="source">Data source.</param>
    /// <param name="offset">Offset.</param>
    public virtual StringData this[ReadOnlySpan<byte> source, int offset] =>
        this[source, offset, int.MaxValue];

    /// <summary>
    /// Reads data.
    /// </summary>
    /// <param name="source">Data source.</param>
    public virtual StringData this[ReadOnlySpan<byte> source] => this[source, 0, int.MaxValue];

    /// <summary>
    /// Reads data.
    /// </summary>
    /// <param name="range">Max range to read.</param>
    /// <param name="stream">Data source.</param>
    public virtual StringData this[Range range, Stream stream]
    {
        get
        {
            if (stream.Length > int.MaxValue)
            {
                throw new ArgumentException($"Stream exceeds {int.MaxValue} bytes");
            }
            (int offset, int length) = range.GetOffsetAndLength((int)stream.Length);
            return this[offset, stream, length];
        }
    }

    /// <summary>
    /// Reads data.
    /// </summary>
    /// <param name="offset">Offset (no seeking if -1).</param>v
    /// <param name="stream">Data source.</param>
    /// <param name="maxBytes">Maximum bytes to read.</param>
    public abstract StringData this[long offset, Stream stream, int maxBytes] { get; }

    /// <summary>
    /// Reads data.
    /// </summary>
    /// <param name="range">Max range to read.</param>
    public virtual StringData this[Range range] =>
        this[range, InputStream];

    /// <summary>
    /// Reads data.
    /// </summary>
    /// <param name="offset">Offset (no seeking if -1).</param>
    /// <param name="maxBytes">Maximum bytes to read.</param>
    public virtual StringData this[long offset, int maxBytes] =>
        this[offset, InputStream, maxBytes];

    /// <summary>
    /// Writes data.
    /// </summary>
    /// <param name="offset">Offset (no seeking if -1).</param>
    /// <param name="stream">Data source.</param>
    public abstract StringData this[long offset, Stream stream] { get; set; }

    /// <summary>
    /// Writes data.
    /// </summary>
    /// <param name="offset">Offset (no seeking if -1).</param>
    public virtual StringData this[long offset]
    {
        get => this[offset, OutputStream];
        set => this[offset, OutputStream] = value;
    }
}

/// <summary>
/// ASCII string helper.
/// </summary>
public record AsciiStringHelper(Processor Parent) : BaseStringHelper
{
    /// <inheritdoc />
    public override Stream InputStream => Parent.InputStream ?? throw new InvalidOperationException();

    /// <inheritdoc />
    public override Stream OutputStream => Parent.InputStream ?? throw new InvalidOperationException();

    /// <inheritdoc />
    public override StringData this[Span<byte> source]
    {
        get => this[(ReadOnlySpan<byte>)source];
        set => Ascii(value.String).CopyTo(source);
    }

    /// <inheritdoc />
    public override StringData this[ReadOnlySpan<byte> source, int offset,
        int maxBytes] =>
        new(ReadUtf8String(source[offset..], out _, out int numBytes, maxBytes), numBytes);

    /// <inheritdoc />
    public override StringData this[long offset, Stream stream, int maxBytes] =>
        offset != -1
            ? new StringData(Parent.ReadUtf8StringFromOffset(stream, offset, out _, out int numBytes1, maxBytes),
                numBytes1)
            : new StringData(Parent.ReadUtf8String(stream, out _, out int numBytes2), numBytes2);

    /// <inheritdoc />
    public override StringData this[long offset, Stream stream]
    {
        get =>
            offset != -1
                ? new StringData(Parent.ReadUtf8StringFromOffset(stream, offset, out _, out int numBytes1),
                    numBytes1)
                : new StringData(Parent.ReadUtf8String(stream, out _, out int numBytes2), numBytes2);
        set
        {
            byte[] res = Shared.Rent(value.String.Length);
            try
            {
                Ascii(value.String, res);
                ReadOnlySpan<byte> span = res.AsSpan(0, value.String.Length);
                if (offset != -1) Write(stream, offset, span);
                else stream.Write(span);
            }
            finally
            {
                Shared.Return(res);
            }
        }
    }
}

/// <summary>
/// UTF-8 string helper.
/// </summary>
public record Utf8StringHelper(Processor Parent) : BaseStringHelper
{
    /// <inheritdoc />
    public override Stream InputStream => Parent.InputStream ?? throw new InvalidOperationException();

    /// <inheritdoc />
    public override Stream OutputStream => Parent.InputStream ?? throw new InvalidOperationException();

    /// <inheritdoc />
    public override StringData this[Span<byte> source]
    {
        get => this[(ReadOnlySpan<byte>)source];
        set => Encoding.UTF8.GetBytes(value.String).CopyTo(source);
    }

    /// <inheritdoc />
    public override StringData this[ReadOnlySpan<byte> source, int offset,
        int maxBytes] =>
        new(ReadUtf8String(source[offset..], out _, out int numBytes, maxBytes), numBytes);

    /// <inheritdoc />
    public override StringData this[long offset, Stream stream, int maxBytes] =>
        offset != -1
            ? new StringData(Parent.ReadUtf8StringFromOffset(stream, offset, out _, out int numBytes1, maxBytes),
                numBytes1)
            : new StringData(Parent.ReadUtf8String(stream, out _, out int numBytes2), numBytes2);

    /// <inheritdoc />
    public override StringData this[long offset, Stream stream]
    {
        get =>
            offset != -1
                ? new StringData(Parent.ReadUtf8StringFromOffset(stream, offset, out _, out int numBytes1),
                    numBytes1)
                : new StringData(Parent.ReadUtf8String(stream, out _, out int numBytes2), numBytes2);
        set => Parent.WriteUtf8String(value.String, false, stream, offset != -1 ? offset : null);
    }
}

/// <summary>
/// UTF-16 string helper.
/// </summary>
public record Utf16StringHelper(Processor Parent) : BaseStringHelper
{
    /// <inheritdoc />
    public override Stream InputStream => Parent.InputStream ?? throw new InvalidOperationException();

    /// <inheritdoc />
    public override Stream OutputStream => Parent.InputStream ?? throw new InvalidOperationException();

    /// <inheritdoc />
    public override StringData this[Span<byte> source]
    {
        get => this[(ReadOnlySpan<byte>)source];
        set => Encoding.Unicode.GetBytes(value.String).CopyTo(source);
    }

    /// <inheritdoc />
    public override StringData this[ReadOnlySpan<byte> source, int offset,
        int maxBytes] =>
        new(ReadUtf16String(source[offset..], out _, out int numBytes, maxBytes), numBytes);

    /// <inheritdoc />
    public override StringData this[long offset, Stream stream, int maxBytes] =>
        offset != -1
            ? new StringData(Parent.ReadUtf16StringFromOffset(stream, offset, out _, out int numBytes1, maxBytes),
                numBytes1)
            : new StringData(Parent.ReadUtf16String(stream, out _, out int numBytes2), numBytes2);

    /// <inheritdoc />
    public override StringData this[long offset, Stream stream]
    {
        get =>
            offset != -1
                ? new StringData(Parent.ReadUtf16StringFromOffset(stream, offset, out _, out int numBytes1),
                    numBytes1)
                : new StringData(Parent.ReadUtf16String(stream, out _, out int numBytes2), numBytes2);
        set => Parent.WriteUtf16String(value.String, false, false, false, stream, offset != -1 ? offset : null);
    }
}
