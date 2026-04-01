using System;
using System.Numerics;

namespace Fp;

public partial class Processor
{
    /// <summary>
    /// Reads converted array (with endianness switch).
    /// </summary>
    /// <param name="span">Source span.</param>
    /// <returns>Converted array.</returns>
    public short[] GetS16Array(ReadOnlySpan<byte> span) => GetS16Array(span, LittleEndian);

    /// <summary>
    /// Writes array (with endianness switch).
    /// </summary>
    /// <param name="span">Target span.</param>
    /// <param name="array">Source array.</param>
    public void SetS16Array(Span<byte> span, short[] array) => SetS16Array(span, array, LittleEndian);

    /// <summary>
    /// Reads converted array (with endianness switch).
    /// </summary>
    /// <param name="span">Source span.</param>
    /// <returns>Converted array.</returns>
    public int[] GetS32Array(ReadOnlySpan<byte> span) => GetS32Array(span, LittleEndian);

    /// <summary>
    /// Writes array (with endianness switch).
    /// </summary>
    /// <param name="span">Target span.</param>
    /// <param name="array">Source array.</param>
    public void SetS32Array(Span<byte> span, int[] array) => SetS32Array(span, array, LittleEndian);

    /// <summary>
    /// Reads converted array (with endianness switch).
    /// </summary>
    /// <param name="span">Source span.</param>
    /// <returns>Converted array.</returns>
    public long[] GetS64Array(ReadOnlySpan<byte> span) => GetS64Array(span, LittleEndian);

    /// <summary>
    /// Writes array (with endianness switch).
    /// </summary>
    /// <param name="span">Target span.</param>
    /// <param name="array">Source array.</param>
    public void SetS64Array(Span<byte> span, long[] array) => SetS64Array(span, array, LittleEndian);

    /// <summary>
    /// Reads converted array (with endianness switch).
    /// </summary>
    /// <param name="span">Source span.</param>
    /// <returns>Converted array.</returns>
    public ushort[] GetU16Array(ReadOnlySpan<byte> span) => GetU16Array(span, LittleEndian);

    /// <summary>
    /// Writes array (with endianness switch).
    /// </summary>
    /// <param name="span">Target span.</param>
    /// <param name="array">Source array.</param>
    public void SetU16Array(Span<byte> span, ushort[] array) => SetU16Array(span, array, LittleEndian);

    /// <summary>
    /// Reads converted array (with endianness switch).
    /// </summary>
    /// <param name="span">Source span.</param>
    /// <returns>Converted array.</returns>
    public uint[] GetU32Array(ReadOnlySpan<byte> span) => GetU32Array(span, LittleEndian);

    /// <summary>
    /// Writes array (with endianness switch).
    /// </summary>
    /// <param name="span">Target span.</param>
    /// <param name="array">Source array.</param>
    public void SetU32Array(Span<byte> span, uint[] array) => SetU32Array(span, array, LittleEndian);

    /// <summary>
    /// Reads converted array (with endianness switch).
    /// </summary>
    /// <param name="span">Source span.</param>
    /// <returns>Converted array.</returns>
    public ulong[] GetU64Array(ReadOnlySpan<byte> span) => GetU64Array(span, LittleEndian);

    /// <summary>
    /// Writes array (with endianness switch).
    /// </summary>
    /// <param name="span">Target span.</param>
    /// <param name="array">Source array.</param>
    public void SetU64Array(Span<byte> span, ulong[] array) => SetU64Array(span, array, LittleEndian);

    /// <summary>
    /// Reads converted array (with endianness switch).
    /// </summary>
    /// <param name="span">Source span.</param>
    /// <returns>Converted array.</returns>
    public T[] GetNumberArray<T>(ReadOnlySpan<byte> span) where T : unmanaged, INumber<T>
        => GetNumberArray<T>(span, LittleEndian);

    /// <summary>
    /// Writes array (with endianness switch).
    /// </summary>
    /// <param name="span">Target span.</param>
    /// <param name="array">Source array.</param>
    public void SetNumberArray<T>(Span<byte> span, T[] array) where T : unmanaged, INumber<T>
        => SetNumberArray(span, array, LittleEndian);
}
