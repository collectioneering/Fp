using System;
using System.IO;

namespace Fp.Helpers;

public abstract partial record BaseUnmanagedIntegerArrayHelper<T>
{
    /// <summary>
    /// Gets an end-terminated index array from the specified buffer.
    /// </summary>
    /// <param name="source">Source buffer.</param>
    /// <param name="offset">Byte offset in buffer.</param>
    /// <param name="count">Number of entries in buffer.</param>
    /// <param name="end">End position (the first position beyond the bounds of the target region the array refers to).</param>
    /// <returns>Segments.</returns>
    /// <exception cref="InvalidDataException">Thrown for an invalid index value (each successive element must compare greater than or equal to the previous element).</exception>
    public OffsetSegment<T>[] GetEndTerminatedIndexArray(byte[] source, int offset, int count, out T end)
    {
        return Processor.GetEndTerminatedIndexArray(this[source, offset, count + 1], out end);
    }

    /// <summary>
    /// Gets an end-terminated index array from the specified buffer.
    /// </summary>
    /// <param name="source">Source buffer.</param>
    /// <param name="offset">Byte offset in buffer.</param>
    /// <param name="count">Number of entries in buffer.</param>
    /// <param name="transform">Transform to apply to input values to get final indices.</param>
    /// <param name="end">End position (the first position beyond the bounds of the target region the array refers to).</param>
    /// <returns>Segments.</returns>
    /// <typeparam name="TElement">Output index type.</typeparam>
    /// <exception cref="InvalidDataException">Thrown for an invalid index value (each successive element must compare greater than or equal to the previous element).</exception>
    public OffsetSegment<TElement>[] GetEndTerminatedIndexArray<TElement>(byte[] source, int offset, int count, Func<T, TElement> transform, out TElement end) where TElement : System.Numerics.INumber<TElement>
    {
        return Processor.GetEndTerminatedIndexArray(this[source, offset, count + 1], transform, out end);
    }

    /// <summary>
    /// Gets an end-terminated index array from the specified buffer.
    /// </summary>
    /// <param name="source">Source buffer.</param>
    /// <param name="offset">Byte offset in buffer.</param>
    /// <param name="count">Number of entries in buffer.</param>
    /// <param name="end">End position (the first position beyond the bounds of the target region the array refers to).</param>
    /// <returns>Segments.</returns>
    /// <exception cref="InvalidDataException">Thrown for an invalid index value (each successive element must compare greater than or equal to the previous element).</exception>
    public OffsetSegment<T>[] GetEndTerminatedIndexArray(Memory<byte> source, int offset, int count, out T end)
    {
        return Processor.GetEndTerminatedIndexArray(this[source, offset, count + 1], out end);
    }

    /// <summary>
    /// Gets an end-terminated index array from the specified buffer.
    /// </summary>
    /// <param name="source">Source buffer.</param>
    /// <param name="offset">Byte offset in buffer.</param>
    /// <param name="count">Number of entries in buffer.</param>
    /// <param name="transform">Transform to apply to input values to get final indices.</param>
    /// <param name="end">End position (the first position beyond the bounds of the target region the array refers to).</param>
    /// <returns>Segments.</returns>
    /// <typeparam name="TElement">Output index type.</typeparam>
    /// <exception cref="InvalidDataException">Thrown for an invalid index value (each successive element must compare greater than or equal to the previous element).</exception>
    public OffsetSegment<TElement>[] GetEndTerminatedIndexArray<TElement>(Memory<byte> source, int offset, int count, Func<T, TElement> transform, out TElement end) where TElement : System.Numerics.INumber<TElement>
    {
        return Processor.GetEndTerminatedIndexArray(this[source, offset, count + 1], transform, out end);
    }

    /// <summary>
    /// Gets an end-terminated index array from the specified buffer.
    /// </summary>
    /// <param name="source">Source buffer.</param>
    /// <param name="offset">Byte offset in buffer.</param>
    /// <param name="count">Number of entries in buffer.</param>
    /// <param name="end">End position (the first position beyond the bounds of the target region the array refers to).</param>
    /// <returns>Segments.</returns>
    /// <exception cref="InvalidDataException">Thrown for an invalid index value (each successive element must compare greater than or equal to the previous element).</exception>
    public OffsetSegment<T>[] GetEndTerminatedIndexArray(Span<byte> source, int offset, int count, out T end)
    {
        return Processor.GetEndTerminatedIndexArray(this[source, offset, count + 1], out end);
    }

    /// <summary>
    /// Gets an end-terminated index array from the specified buffer.
    /// </summary>
    /// <param name="source">Source buffer.</param>
    /// <param name="offset">Byte offset in buffer.</param>
    /// <param name="count">Number of entries in buffer.</param>
    /// <param name="transform">Transform to apply to input values to get final indices.</param>
    /// <param name="end">End position (the first position beyond the bounds of the target region the array refers to).</param>
    /// <returns>Segments.</returns>
    /// <typeparam name="TElement">Output index type.</typeparam>
    /// <exception cref="InvalidDataException">Thrown for an invalid index value (each successive element must compare greater than or equal to the previous element).</exception>
    public OffsetSegment<TElement>[] GetEndTerminatedIndexArray<TElement>(Span<byte> source, int offset, int count, Func<T, TElement> transform, out TElement end) where TElement : System.Numerics.INumber<TElement>
    {
        return Processor.GetEndTerminatedIndexArray(this[source, offset, count + 1], transform, out end);
    }

    /// <summary>
    /// Gets an end-terminated index array from the specified buffer.
    /// </summary>
    /// <param name="source">Source buffer.</param>
    /// <param name="offset">Byte offset in buffer.</param>
    /// <param name="count">Number of entries in buffer.</param>
    /// <param name="end">End position (the first position beyond the bounds of the target region the array refers to).</param>
    /// <returns>Segments.</returns>
    /// <exception cref="InvalidDataException">Thrown for an invalid index value (each successive element must compare greater than or equal to the previous element).</exception>
    public OffsetSegment<T>[] GetEndTerminatedIndexArray(ReadOnlyMemory<byte> source, int offset, int count, out T end)
    {
        return Processor.GetEndTerminatedIndexArray(this[source, offset, count + 1], out end);
    }

    /// <summary>
    /// Gets an end-terminated index array from the specified buffer.
    /// </summary>
    /// <param name="source">Source buffer.</param>
    /// <param name="offset">Byte offset in buffer.</param>
    /// <param name="count">Number of entries in buffer.</param>
    /// <param name="transform">Transform to apply to input values to get final indices.</param>
    /// <param name="end">End position (the first position beyond the bounds of the target region the array refers to).</param>
    /// <returns>Segments.</returns>
    /// <typeparam name="TElement">Output index type.</typeparam>
    /// <exception cref="InvalidDataException">Thrown for an invalid index value (each successive element must compare greater than or equal to the previous element).</exception>
    public OffsetSegment<TElement>[] GetEndTerminatedIndexArray<TElement>(ReadOnlyMemory<byte> source, int offset, int count, Func<T, TElement> transform, out TElement end) where TElement : System.Numerics.INumber<TElement>
    {
        return Processor.GetEndTerminatedIndexArray(this[source, offset, count + 1], transform, out end);
    }

    /// <summary>
    /// Gets an end-terminated index array from the specified buffer.
    /// </summary>
    /// <param name="source">Source buffer.</param>
    /// <param name="offset">Byte offset in buffer.</param>
    /// <param name="count">Number of entries in buffer.</param>
    /// <param name="end">End position (the first position beyond the bounds of the target region the array refers to).</param>
    /// <returns>Segments.</returns>
    /// <exception cref="InvalidDataException">Thrown for an invalid index value (each successive element must compare greater than or equal to the previous element).</exception>
    public OffsetSegment<T>[] GetEndTerminatedIndexArray(ReadOnlySpan<byte> source, int offset, int count, out T end)
    {
        return Processor.GetEndTerminatedIndexArray(this[source, offset, count + 1], out end);
    }

    /// <summary>
    /// Gets an end-terminated index array from the specified buffer.
    /// </summary>
    /// <param name="source">Source buffer.</param>
    /// <param name="offset">Byte offset in buffer.</param>
    /// <param name="count">Number of entries in buffer.</param>
    /// <param name="transform">Transform to apply to input values to get final indices.</param>
    /// <param name="end">End position (the first position beyond the bounds of the target region the array refers to).</param>
    /// <returns>Segments.</returns>
    /// <typeparam name="TElement">Output index type.</typeparam>
    /// <exception cref="InvalidDataException">Thrown for an invalid index value (each successive element must compare greater than or equal to the previous element).</exception>
    public OffsetSegment<TElement>[] GetEndTerminatedIndexArray<TElement>(ReadOnlySpan<byte> source, int offset, int count, Func<T, TElement> transform, out TElement end) where TElement : System.Numerics.INumber<TElement>
    {
        return Processor.GetEndTerminatedIndexArray(this[source, offset, count + 1], transform, out end);
    }

    /// <summary>
    /// Gets an end-terminated index array from <see cref="Helper.InputStream"/>.
    /// </summary>
    /// <param name="offset">Byte offset in stream.</param>
    /// <param name="count">Number of entries in stream.</param>
    /// <param name="end">End position (the first position beyond the bounds of the target region the array refers to).</param>
    /// <returns>Segments.</returns>
    /// <exception cref="InvalidDataException">Thrown for an invalid index value (each successive element must compare greater than or equal to the previous element).</exception>
    public OffsetSegment<T>[] GetEndTerminatedIndexArray(long offset, int count, out T end)
    {
        return Processor.GetEndTerminatedIndexArray(this[offset, count + 1], out end);
    }

    /// <summary>
    /// Gets an end-terminated index array from <see cref="Helper.InputStream"/>.
    /// </summary>
    /// <param name="offset">Byte offset in stream.</param>
    /// <param name="count">Number of entries in stream.</param>
    /// <param name="transform">Transform to apply to input values to get final indices.</param>
    /// <param name="end">End position (the first position beyond the bounds of the target region the array refers to).</param>
    /// <returns>Segments.</returns>
    /// <typeparam name="TElement">Output index type.</typeparam>
    /// <exception cref="InvalidDataException">Thrown for an invalid index value (each successive element must compare greater than or equal to the previous element).</exception>
    public OffsetSegment<TElement>[] GetEndTerminatedIndexArray<TElement>(long offset, int count, Func<T, TElement> transform, out TElement end) where TElement : System.Numerics.INumber<TElement>
    {
        return Processor.GetEndTerminatedIndexArray(this[offset, count + 1], transform, out end);
    }

    /// <summary>
    /// Gets an end-terminated index array from a stream.
    /// </summary>
    /// <param name="offset">Byte offset in stream.</param>
    /// <param name="count">Number of entries in stream.</param>
    /// <param name="stream">Source stream.</param>
    /// <param name="end">End position (the first position beyond the bounds of the target region the array refers to).</param>
    /// <returns>Segments.</returns>
    /// <exception cref="InvalidDataException">Thrown for an invalid index value (each successive element must compare greater than or equal to the previous element).</exception>
    public OffsetSegment<T>[] GetEndTerminatedIndexArray(long offset, int count, Stream stream, out T end)
    {
        return Processor.GetEndTerminatedIndexArray(this[offset, count + 1, stream], out end);
    }

    /// <summary>
    /// Gets an end-terminated index array from a stream.
    /// </summary>
    /// <param name="offset">Byte offset in stream.</param>
    /// <param name="count">Number of entries in stream.</param>
    /// <param name="stream">Source stream.</param>
    /// <param name="transform">Transform to apply to input values to get final indices.</param>
    /// <param name="end">End position (the first position beyond the bounds of the target region the array refers to).</param>
    /// <returns>Segments.</returns>
    /// <typeparam name="TElement">Output index type.</typeparam>
    /// <exception cref="InvalidDataException">Thrown for an invalid index value (each successive element must compare greater than or equal to the previous element).</exception>
    public OffsetSegment<TElement>[] GetEndTerminatedIndexArray<TElement>(long offset, int count, Stream stream, Func<T, TElement> transform, out TElement end) where TElement : System.Numerics.INumber<TElement>
    {
        return Processor.GetEndTerminatedIndexArray(this[offset, count + 1, stream], transform, out end);
    }

}
