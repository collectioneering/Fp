using System;
using System.Collections.Generic;
using System.IO;
using Fp.Helpers;

namespace Fp;

#nullable disable

public partial class Processor
{
    /// <summary>
    /// Gets an end-terminated index array from the specified buffer.
    /// </summary>
    /// <param name="buffer">Source buffer.</param>
    /// <param name="end">End position (the first position beyond the bounds of the target region the array refers to).</param>
    /// <typeparam name="T">Index type.</typeparam>
    /// <returns>Segments.</returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidDataException"></exception>
    public static OffsetSegment<T>[] GetEndTerminatedIndexArray<T>(T[] buffer, out T end) where T : System.Numerics.INumber<T>
        => GetEndTerminatedIndexArray(buffer.AsSpan(), out end);

    /// <summary>
    /// Gets an end-terminated index array from the specified buffer.
    /// </summary>
    /// <param name="buffer">Source buffer.</param>
    /// <param name="end">End position (the first position beyond the bounds of the target region the array refers to).</param>
    /// <typeparam name="T">Index type.</typeparam>
    /// <returns>Segments.</returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidDataException"></exception>
    public static OffsetSegment<T>[] GetEndTerminatedIndexArray<T>(ReadOnlyMemory<T> buffer, out T end) where T : System.Numerics.INumber<T>
        => GetEndTerminatedIndexArray(buffer.Span, out end);

    /// <summary>
    /// Gets an end-terminated index array from the specified buffer.
    /// </summary>
    /// <param name="buffer">Source buffer.</param>
    /// <param name="end">End position (the first position beyond the bounds of the target region the array refers to).</param>
    /// <typeparam name="T">Index type.</typeparam>
    /// <returns>Segments.</returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidDataException"></exception>
    public static OffsetSegment<T>[] GetEndTerminatedIndexArray<T>(ReadOnlySpan<T> buffer, out T end) where T : System.Numerics.INumber<T>
    {
        if (buffer.IsEmpty) throw new ArgumentException(); // >= 1
        if (buffer.Length == 1)
        {
            end = buffer[0];
            return Array.Empty<OffsetSegment<T>>();
        }
        OffsetSegment<T>[] result = new OffsetSegment<T>[buffer.Length - 1];
        T value = buffer[0];
        for (int i = 0; i < result.Length; i++)
        {
            T next = buffer[i + 1];
            if (value.CompareTo(next) > 0) throw new InvalidDataException($"Expected smaller end index for element {i} compared to starting index {value} (got {next})");
            result[i] = new OffsetSegment<T>(value, next);
            value = next;
        }
        end = value;
        return result;
    }

    /// <summary>
    /// Gets an end-terminated index array from the specified buffer.
    /// </summary>
    /// <param name="buffer">Source buffer.</param>
    /// <param name="transform">Transform to apply to input values to get final indices.</param>
    /// <param name="end">End position (the first position beyond the bounds of the target region the array refers to).</param>
    /// <typeparam name="T">Source index type.</typeparam>
    /// <typeparam name="TElement">Output index type.</typeparam>
    /// <returns>Segments.</returns>
    /// <exception cref="ArgumentException">Thrown for an empty buffer.</exception>
    /// <exception cref="InvalidDataException">Thrown for an invalid index value (each successive element must compare greater than or equal to the previous element).</exception>
    public static OffsetSegment<TElement>[] GetEndTerminatedIndexArray<T, TElement>(T[] buffer, Func<T, TElement> transform, out TElement end) where TElement : System.Numerics.INumber<TElement>
        => GetEndTerminatedIndexArray(buffer.AsSpan(), transform, out end);

    /// <summary>
    /// Gets an end-terminated index array from the specified buffer.
    /// </summary>
    /// <param name="buffer">Source buffer.</param>
    /// <param name="transform">Transform to apply to input values to get final indices.</param>
    /// <param name="end">End position (the first position beyond the bounds of the target region the array refers to).</param>
    /// <typeparam name="T">Source index type.</typeparam>
    /// <typeparam name="TElement">Output index type.</typeparam>
    /// <returns>Segments.</returns>
    /// <exception cref="ArgumentException">Thrown for an empty buffer.</exception>
    /// <exception cref="InvalidDataException">Thrown for an invalid index value (each successive element must compare greater than or equal to the previous element).</exception>
    public static OffsetSegment<TElement>[] GetEndTerminatedIndexArray<T, TElement>(ReadOnlyMemory<T> buffer, Func<T, TElement> transform, out TElement end) where TElement : System.Numerics.INumber<TElement>
        => GetEndTerminatedIndexArray(buffer.Span, transform, out end);

    /// <summary>
    /// Gets an end-terminated index array from the specified buffer.
    /// </summary>
    /// <param name="buffer">Source buffer.</param>
    /// <param name="transform">Transform to apply to input values to get final indices.</param>
    /// <param name="end">End position (the first position beyond the bounds of the target region the array refers to).</param>
    /// <typeparam name="T">Source index type.</typeparam>
    /// <typeparam name="TElement">Output index type.</typeparam>
    /// <returns>Segments.</returns>
    /// <exception cref="ArgumentException">Thrown for an empty buffer.</exception>
    /// <exception cref="InvalidDataException">Thrown for an invalid index value (each successive element must compare greater than or equal to the previous element).</exception>
    public static OffsetSegment<TElement>[] GetEndTerminatedIndexArray<T, TElement>(ReadOnlySpan<T> buffer, Func<T, TElement> transform, out TElement end) where TElement : System.Numerics.INumber<TElement>
    {
        if (buffer.IsEmpty) throw new ArgumentException(); // >= 1
        if (buffer.Length == 1)
        {
            end = transform(buffer[0]);
            return Array.Empty<OffsetSegment<TElement>>();
        }
        OffsetSegment<TElement>[] result = new OffsetSegment<TElement>[buffer.Length - 1];
        TElement value = transform(buffer[0]);
        for (int i = 0; i < result.Length; i++)
        {
            TElement next = transform(buffer[i + 1]);
            if (value.CompareTo(next) > 0) throw new InvalidDataException($"Expected smaller end index for element {i} compared to starting index {value} (got {next})");
            result[i] = new OffsetSegment<TElement>(value, next);
            value = next;
        }
        end = value;
        return result;
    }

    /// <summary>
    /// Gets an end-terminated index array from the specified sequence.
    /// </summary>
    /// <param name="enumerable">Source sequence.</param>
    /// <param name="end">End position (the first position beyond the bounds of the target region the array refers to).</param>
    /// <typeparam name="T">Source index type.</typeparam>
    /// <returns>Segments.</returns>
    /// <exception cref="ArgumentException">Thrown for an empty buffer.</exception>
    /// <exception cref="InvalidDataException">Thrown for an invalid index value (each successive element must compare greater than or equal to the previous element).</exception>
    public static IReadOnlyList<OffsetSegment<T>> GetEndTerminatedIndexArray<T>(IEnumerable<T> enumerable, out T end) where T : System.Numerics.INumber<T>
    {
        using var enumerator = enumerable.GetEnumerator();
        if (!enumerator.MoveNext()) throw new ArgumentException(); // >= 1
        T value = enumerator.Current;
        if (!enumerator.MoveNext())
        {
            end = value;
            return Array.Empty<OffsetSegment<T>>();
        }
        List<OffsetSegment<T>> result = new();
        int i = 0;
        do
        {
            T next = enumerator.Current;
            if (value.CompareTo(next) > 0) throw new InvalidDataException($"Expected smaller end index for element {i} compared to starting index {value} (got {next})");
            result.Add(new OffsetSegment<T>(value, next));
            value = next;
            i++;
        } while (enumerator.MoveNext());
        end = value;
        return result;
    }

    /// <summary>
    /// Gets an end-terminated index array from the specified sequence.
    /// </summary>
    /// <param name="enumerable">Source sequence.</param>
    /// <param name="transform">Transform to apply to input values to get final indices.</param>
    /// <param name="end">End position (the first position beyond the bounds of the target region the array refers to).</param>
    /// <typeparam name="T">Source index type.</typeparam>
    /// <typeparam name="TElement">Output index type.</typeparam>
    /// <returns>Segments.</returns>
    /// <exception cref="ArgumentException">Thrown for an empty buffer.</exception>
    /// <exception cref="InvalidDataException">Thrown for an invalid index value (each successive element must compare greater than or equal to the previous element).</exception>
    public static IReadOnlyList<OffsetSegment<TElement>> GetEndTerminatedIndexArray<T, TElement>(IEnumerable<T> enumerable, Func<T, TElement> transform, out TElement end) where TElement : System.Numerics.INumber<TElement>
    {
        using var enumerator = enumerable.GetEnumerator();
        if (!enumerator.MoveNext()) throw new ArgumentException(); // >= 1
        TElement value = transform(enumerator.Current);
        if (!enumerator.MoveNext())
        {
            end = value;
            return Array.Empty<OffsetSegment<TElement>>();
        }
        List<OffsetSegment<TElement>> result = new();
        int i = 0;
        do
        {
            TElement next = transform(enumerator.Current);
            if (value.CompareTo(next) > 0) throw new InvalidDataException($"Expected smaller end index for element {i} compared to starting index {value} (got {next})");
            result.Add(new OffsetSegment<TElement>(value, next));
            value = next;
            i++;
        } while (enumerator.MoveNext());
        end = value;
        return result;
    }
}
