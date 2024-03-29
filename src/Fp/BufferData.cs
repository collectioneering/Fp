using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Fp;

/// <summary>
/// Buffer containing unstructured data.
/// </summary>
public abstract class BufferData : Data
{
    /// <summary>
    /// Creates a new instance of <see cref="BufferData{T}"/>.
    /// </summary>
    /// <param name="basePath">Base path of resource.</param>
    protected BufferData(string basePath) : base(basePath)
    {
    }

    /// <summary>
    /// Gets span of specified type from buffer.
    /// </summary>
    /// <typeparam name="TWant">Target type.</typeparam>
    /// <returns>Span.</returns>
    /// <exception cref="ObjectDisposedException">Thrown if object was disposed.</exception>
    public abstract ReadOnlySpan<TWant> AsSpan<TWant>() where TWant : unmanaged;
}

/// <inheritdoc />
public class BufferData<T> : BufferData where T : unmanaged
{
    private bool _disposed;

    /// <inheritdoc />
    public override Guid DefaultFormat => Generic;

    /// <inheritdoc />
    public override IReadOnlyCollection<Guid> SupportedFormats { get; } = new[] { Generic };

    /// <summary>
    /// Memory owner.
    /// </summary>
    public IMemoryOwner<T>? MemoryOwner { get; }

    /// <summary>
    /// Buffer.
    /// </summary>
    public ReadOnlyMemory<T> Buffer { get; private set; }

    /// <summary>
    /// Buffer content length.
    /// </summary>
    public readonly int Count;

    /// <summary>
    /// Creates a new instance of <see cref="BufferData{T}"/>.
    /// </summary>
    /// <param name="basePath">Base path of resource.</param>
    /// <param name="count">Length of content.</param>
    public BufferData(string basePath, int count) : base(basePath)
    {
        Dry = true;
        MemoryOwner = null;
        Buffer = Memory<T>.Empty;
        Count = count;
    }

    /// <summary>
    /// Creates a new instance of <see cref="BufferData{T}"/>.
    /// </summary>
    /// <param name="basePath">Base path of resource.</param>
    /// <param name="memoryOwner">Owner of data buffer.</param>
    /// <param name="count">Length of content.</param>
    public BufferData(string basePath, IMemoryOwner<T> memoryOwner, int? count = default) : base(basePath)
    {
        MemoryOwner = memoryOwner;
        Dry = false;
        Count = count ?? MemoryOwner.Memory.Length;
        Buffer = MemoryOwner.Memory[..Count];
    }

    /// <summary>
    /// Creates a new instance of <see cref="BufferData{T}"/>.
    /// </summary>
    /// <param name="basePath">Base path of resource.</param>
    /// <param name="buffer">Data in container.</param>
    public BufferData(string basePath, ReadOnlyMemory<T> buffer) : base(basePath)
    {
        Buffer = buffer;
        Dry = false;
        MemoryOwner = null;
        Count = buffer.Length;
    }

    /// <inheritdoc />
    public override ReadOnlySpan<TWant> AsSpan<TWant>()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(BufferData<T>));
        return MemoryMarshal.Cast<T, TWant>(Buffer.Span);
    }

    /// <inheritdoc />
    public override bool SupportsFormat(Guid format, Dictionary<object, object>? formatOptions = null)
    {
        return format == Generic;
    }

    /// <inheritdoc />
    public override bool WriteConvertedData(Stream outputStream, Guid format,
        Dictionary<object, object>? formatOptions = null)
    {
        if (Dry) throw new InvalidOperationException("Cannot convert a dry data container");
        if (_disposed) throw new ObjectDisposedException(nameof(BufferData<T>));
        if (format == Generic)
        {
            outputStream.Write(MemoryMarshal.Cast<T, byte>(Buffer.Span));
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public override object Clone()
    {
        if (Dry)
            return new BufferData<T>(BasePath, Count);
        if (_disposed)
            throw new ObjectDisposedException(nameof(BufferData<T>));
        return new BufferData<T>(BasePath, Buffer.CloneBuffer());
    }

    /// <summary>
    /// Disposes object.
    /// </summary>
    /// <param name="disposing">False if called from finalizer.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        _disposed = true;

        if (disposing)
        {
            Buffer = Memory<T>.Empty;
            MemoryOwner?.Dispose();
        }
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public override unsafe string ToString() =>
        $"Buffer {{ Path = {BasePath}, Element Size = {sizeof(T)}, Element Count = {Count}, Buffer Length = {sizeof(T) * Count} }}";
}

public partial class Processor
{
    /// <summary>
    /// Creates byte data object.
    /// </summary>
    /// <param name="path">Path.</param>
    /// <param name="memory">Data.</param>
    /// <returns>Data object.</returns>
    public static BufferData<byte> Buffer(FpPath path, ReadOnlyMemory<byte> memory) =>
        new(path.AsCombined(), memory);

    /// <summary>
    /// Creates byte data object.
    /// </summary>
    /// <param name="name">Path.</param>
    /// <param name="memory">Data.</param>
    /// <returns>Data object.</returns>
    public static BufferData<byte> Buffer(string name, ReadOnlyMemory<byte> memory) =>
        new(name, memory);
}
