using System;
using System.IO;

namespace Fp.Helpers;

/// <summary>
/// Base single-unit unmanaged data helper.
/// </summary>
/// <typeparam name="T">Element type.</typeparam>
public abstract record BaseHelper<T> : Helper
{
    /// <summary>
    /// Reads/writes value.
    /// </summary>
    /// <param name="source">Data source.</param>
    /// <param name="offset">Offset.</param>
    public virtual T this[byte[] source, int offset]
    {
        get => this[source.AsSpan(), offset];
        set => this[source.AsSpan(), offset] = value;
    }

    /// <summary>
    /// Reads/writes value.
    /// </summary>
    /// <param name="source">Data source.</param>
    public virtual T this[byte[] source]
    {
        get => this[source.AsSpan(), 0];
        set => this[source.AsSpan(), 0] = value;
    }

    /// <summary>
    /// Reads/writes value.
    /// </summary>
    /// <param name="source">Data source.</param>
    /// <param name="offset">Offset.</param>
    public virtual T this[Memory<byte> source, int offset]
    {
        get => this[source.Span, offset];
        set => this[source.Span, offset] = value;
    }

    /// <summary>
    /// Reads/writes value.
    /// </summary>
    /// <param name="source">Data source.</param>
    public virtual T this[Memory<byte> source]
    {
        get => this[source.Span, 0];
        set => this[source.Span, 0] = value;
    }

    /// <summary>
    /// Reads/writes value.
    /// </summary>
    /// <param name="source">Data source.</param>
    /// <param name="offset">Offset.</param>
    public virtual T this[Span<byte> source, int offset]
    {
        get => this[source[offset..]];
        set => this[source[offset..]] = value;
    }

    /// <summary>
    /// Reads/writes value.
    /// </summary>
    /// <param name="source">Data source.</param>
    public abstract T this[Span<byte> source] { get; set; }

    /// <summary>
    /// Reads value.
    /// </summary>
    /// <param name="source">Data source.</param>
    /// <param name="offset">Offset.</param>
    public virtual T this[ReadOnlyMemory<byte> source, int offset] => this[source.Span, offset];

    /// <summary>
    /// Reads value.
    /// </summary>
    /// <param name="source">Data source.</param>
    public virtual T this[ReadOnlyMemory<byte> source] => this[source.Span, 0];

    /// <summary>
    /// Reads value.
    /// </summary>
    /// <param name="source">Data source.</param>
    /// <param name="offset">Offset.</param>
    public virtual T this[ReadOnlySpan<byte> source, int offset] => this[source[offset..]];

    /// <summary>
    /// Reads value.
    /// </summary>
    /// <param name="source">Data source.</param>
    public abstract T this[ReadOnlySpan<byte> source] { get; }

    /// <summary>
    /// Reads/writes value.
    /// </summary>
    /// <param name="stream">Data source.</param>
    /// <param name="offset">Offset (no seeking if -1).</param>
    public abstract T this[long offset, Stream stream] { get; set; }

    /// <summary>
    /// Reads/writes value.
    /// </summary>
    /// <param name="stream">Data source.</param>
    /// <param name="offset">Offset (no seeking if -1).</param>
    public virtual T this[int offset, Stream stream]
    {
        get => this[(long)offset, stream];
        set => this[(long)offset, stream] = value;
    }

    /// <summary>
    /// Reads/writes value.
    /// </summary>
    /// <param name="offset">Offset (no seeking if -1).</param>
    public virtual T this[long offset]
    {
        get => this[offset, InputStream];
        set => this[offset, OutputStream] = value;
    }

    /// <summary>
    /// Reads/writes value.
    /// </summary>
    /// <param name="offset">Offset (no seeking if -1).</param>
    public virtual T this[int offset]
    {
        get => this[offset, InputStream];
        set => this[offset, OutputStream] = value;
    }
}