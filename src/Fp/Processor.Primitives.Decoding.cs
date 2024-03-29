using System;
using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using static System.Buffers.ArrayPool<byte>;

namespace Fp;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public partial class Processor
{
    #region Decoding utilities

    /// <summary>
    /// Reads signed 8-bit value from stream.
    /// </summary>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    /// <returns>Value.</returns>
    public sbyte ReadS8(Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        if (stream is MemoryStream ms && ms.TryGetBuffer(out ArraySegment<byte> b))
        {
            return (sbyte)b.AsSpan((int)ms.Position)[0];
        }

        int read = stream.ReadByte();
        if (read == -1) throw new IOException($"{nameof(stream)} does not have enough data to fill the specified buffer; 0 bytes available, 1 bytes requested");
        return (sbyte)read;
    }

    /// <summary>
    /// Reads signed 8-bit value from stream at the specified offset.
    /// </summary>
    /// <param name="offset">Offset to read from.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    /// <returns>Value.</returns>
    public sbyte ReadS8(long offset, Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        if (stream is MemoryStream ms && ms.TryGetBuffer(out ArraySegment<byte> b))
        {
            return (sbyte)b.AsSpan((int)offset)[0];
        }

        int read = stream.ReadByte();
        if (read == -1) throw new IOException($"{nameof(stream)} does not have enough data to fill the specified buffer; 0 bytes available, 1 bytes requested");
        return (sbyte)read;
    }

    /// <summary>
    /// Reads signed 8-bit value from span at the specified offset.
    /// </summary>
    /// <param name="span">Span to read from.</param>
    /// <param name="offset">Offset to read from.</param>
    /// <returns>Value.</returns>
    public sbyte GetS8(ReadOnlySpan<byte> span, int offset = 0) => (sbyte)span[offset];

    /// <summary>
    /// Writes signed 8-bit value to span at the specified offset.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="value">Value.</param>
    /// <param name="offset">Offset to write to.</param>
    /// <returns>Value.</returns>
    public sbyte SetS8(Span<byte> span, sbyte value, int offset = 0) => (sbyte)(span[offset] = (byte)value);

    /// <summary>
    /// Reads unsigned 8-bit value from stream.
    /// </summary>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    /// <returns>Value.</returns>
    public byte ReadU8(Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        if (stream is MemoryStream ms && ms.TryGetBuffer(out ArraySegment<byte> b))
        {
            return b.AsSpan((int)ms.Position)[0];
        }

        int read = stream.ReadByte();
        if (read == -1) throw new IOException($"{nameof(stream)} does not have enough data to fill the specified buffer; 0 bytes available, 1 bytes requested");
        return (byte)read;
    }

    /// <summary>
    /// Reads unsigned 8-bit value from stream at the specified offset.
    /// </summary>
    /// <param name="offset">Offset to read from.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    /// <returns>Value.</returns>
    public byte ReadU8(long offset, Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        if (stream is MemoryStream ms && ms.TryGetBuffer(out ArraySegment<byte> b))
        {
            return b.AsSpan((int)offset)[0];
        }

        int read = stream.ReadByte();
        if (read == -1) throw new IOException($"{nameof(stream)} does not have enough data to fill the specified buffer; 0 bytes available, 1 bytes requested");
        return (byte)read;
    }

    /// <summary>
    /// Reads unsigned 8-bit value from span at the specified offset.
    /// </summary>
    /// <param name="span">Span to read from.</param>
    /// <param name="offset">Offset to read from.</param>
    /// <returns>Value.</returns>
    public byte GetU8(ReadOnlySpan<byte> span, int offset = 0) => span[offset];

    /// <summary>
    /// Writes unsigned 8-bit value to span at the specified offset.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="value">Value.</param>
    /// <param name="offset">Offset to write to.</param>
    /// <returns>Value.</returns>
    public byte SetU8(Span<byte> span, byte value, int offset = 0) => span[offset] = value;

    #region Static endian

    /// <summary>
    /// Reads signed 16-bit value from span at the specified offset.
    /// </summary>
    /// <param name="span">Span to read from.</param>
    /// <param name="littleEndian">If true, use little-endian encoding.</param>
    /// <param name="offset">Offset to read from.</param>
    /// <returns>Value.</returns>
    public static unsafe short GetS16(ReadOnlySpan<byte> span, bool littleEndian, int offset = 0)
    {
        short value;
        span.Slice(offset, 2).CopyTo(new Span<byte>(&value, 2));
        return littleEndian ^ BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value;
    }

    /// <summary>
    /// Writes signed 16-bit value to span at the specified offset.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="value">Value.</param>
    /// <param name="littleEndian">If true, use little-endian encoding.</param>
    /// <param name="offset">Offset to write to.</param>
    /// <returns>Value.</returns>
    public static unsafe short SetS16(Span<byte> span, short value, bool littleEndian, int offset = 0)
    {
        if (littleEndian ^ BitConverter.IsLittleEndian) value = BinaryPrimitives.ReverseEndianness(value);
        new Span<byte>(&value, 2).CopyTo(span.Slice(offset, 2));
        return value;
    }

    /// <summary>
    /// Reads unsigned 16-bit value from span at the specified offset.
    /// </summary>
    /// <param name="span">Span to read from.</param>
    /// <param name="littleEndian">If true, use little-endian encoding.</param>
    /// <param name="offset">Offset to read from.</param>
    /// <returns>Value.</returns>
    public static unsafe ushort GetU16(ReadOnlySpan<byte> span, bool littleEndian, int offset = 0)
    {
        ushort value;
        span.Slice(offset, 2).CopyTo(new Span<byte>(&value, 2));
        return littleEndian ^ BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value;
    }

    /// <summary>
    /// Writes unsigned 16-bit value to span at the specified offset.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="value">Value.</param>
    /// <param name="littleEndian">If true, use little-endian encoding.</param>
    /// <param name="offset">Offset to write to.</param>
    /// <returns>Value.</returns>
    public static unsafe ushort SetU16(Span<byte> span, ushort value, bool littleEndian, int offset = 0)
    {
        if (littleEndian ^ BitConverter.IsLittleEndian) value = BinaryPrimitives.ReverseEndianness(value);
        new Span<byte>(&value, 2).CopyTo(span.Slice(offset, 2));
        return value;
    }

    /// <summary>
    /// Reads signed 32-bit value from span at the specified offset.
    /// </summary>
    /// <param name="span">Span to read from.</param>
    /// <param name="littleEndian">If true, use little-endian encoding.</param>
    /// <param name="offset">Offset to read from.</param>
    /// <returns>Value.</returns>
    public static unsafe int GetS32(ReadOnlySpan<byte> span, bool littleEndian, int offset = 0)
    {
        int value;
        span.Slice(offset, 4).CopyTo(new Span<byte>(&value, 4));
        return littleEndian ^ BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value;
    }

    /// <summary>
    /// Writes signed 32-bit value to span at the specified offset.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="value">Value.</param>
    /// <param name="littleEndian">If true, use little-endian encoding.</param>
    /// <param name="offset">Offset to write to.</param>
    /// <returns>Value.</returns>
    public static unsafe int SetS32(Span<byte> span, int value, bool littleEndian, int offset = 0)
    {
        if (littleEndian ^ BitConverter.IsLittleEndian) value = BinaryPrimitives.ReverseEndianness(value);
        new Span<byte>(&value, 4).CopyTo(span.Slice(offset, 4));
        return value;
    }

    /// <summary>
    /// Reads unsigned 32-bit value from span at the specified offset.
    /// </summary>
    /// <param name="span">Span to read from.</param>
    /// <param name="littleEndian">If true, use little-endian encoding.</param>
    /// <param name="offset">Offset to read from.</param>
    /// <returns>Value.</returns>
    public static unsafe uint GetU32(ReadOnlySpan<byte> span, bool littleEndian, int offset = 0)
    {
        uint value;
        span.Slice(offset, 4).CopyTo(new Span<byte>(&value, 4));
        return littleEndian ^ BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value;
    }

    /// <summary>
    /// Writes unsigned 32-bit value to span at the specified offset.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="value">Value.</param>
    /// <param name="littleEndian">If true, use little-endian encoding.</param>
    /// <param name="offset">Offset to write to.</param>
    /// <returns>Value.</returns>
    public static unsafe uint SetU32(Span<byte> span, uint value, bool littleEndian, int offset = 0)
    {
        if (littleEndian ^ BitConverter.IsLittleEndian) value = BinaryPrimitives.ReverseEndianness(value);
        new Span<byte>(&value, 4).CopyTo(span.Slice(offset, 4));
        return value;
    }

    /// <summary>
    /// Reads signed 64-bit value from span at the specified offset.
    /// </summary>
    /// <param name="span">Span to read from.</param>
    /// <param name="littleEndian">If true, use little-endian encoding.</param>
    /// <param name="offset">Offset to read from.</param>
    /// <returns>Value.</returns>
    public static unsafe long GetS64(ReadOnlySpan<byte> span, bool littleEndian, int offset = 0)
    {
        long value;
        span.Slice(offset, 8).CopyTo(new Span<byte>(&value, 8));
        return littleEndian ^ BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value;
    }

    /// <summary>
    /// Writes signed 64-bit value to span at the specified offset.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="value">Value.</param>
    /// <param name="littleEndian">If true, use little-endian encoding.</param>
    /// <param name="offset">Offset to write to.</param>
    public static unsafe long SetS64(Span<byte> span, long value, bool littleEndian, int offset = 0)
    {
        if (littleEndian ^ BitConverter.IsLittleEndian) value = BinaryPrimitives.ReverseEndianness(value);
        new Span<byte>(&value, 8).CopyTo(span.Slice(offset, 8));
        return value;
    }

    /// <summary>
    /// Reads unsigned 64-bit value from span at the specified offset.
    /// </summary>
    /// <param name="span">Span to read from.</param>
    /// <param name="littleEndian">If true, use little-endian encoding.</param>
    /// <param name="offset">Offset to read from.</param>
    /// <returns>Value.</returns>
    public static unsafe ulong GetU64(ReadOnlySpan<byte> span, bool littleEndian, int offset = 0)
    {
        ulong value;
        span.Slice(offset, 8).CopyTo(new Span<byte>(&value, 8));
        return littleEndian ^ BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value;
    }

    /// <summary>
    /// Writes unsigned 64-bit value to span at the specified offset.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="value">Value.</param>
    /// <param name="littleEndian">If true, use little-endian encoding.</param>
    /// <param name="offset">Offset to write to.</param>
    /// <returns>Value.</returns>
    public static unsafe ulong SetU64(Span<byte> span, ulong value, bool littleEndian, int offset = 0)
    {
        if (littleEndian ^ BitConverter.IsLittleEndian) value = BinaryPrimitives.ReverseEndianness(value);
        new Span<byte>(&value, 8).CopyTo(span.Slice(offset, 8));
        return value;
    }

    /// <summary>
    /// Converts endianness of signed 16-bit array between source and platform's endianness.
    /// </summary>
    /// <param name="span">Span to convert.</param>
    /// <param name="littleEndian">If true, use little-endian encoding.</param>
    public static void ConvertS16Array(Span<short> span, bool littleEndian)
    {
        if (littleEndian == BitConverter.IsLittleEndian) return;
        for (int i = 0; i < span.Length; i++) span[i] = BinaryPrimitives.ReverseEndianness(span[i]);
    }

    /// <summary>
    /// Converts endianness of signed 16-bit array between source and platform's endianness.
    /// </summary>
    /// <param name="span">Span to convert.</param>
    /// <param name="littleEndian">If true, use little-endian encoding.</param>
    public static void ConvertS16Array(Span<byte> span, bool littleEndian) =>
        ConvertS16Array(MemoryMarshal.Cast<byte, short>(span), littleEndian);

    /// <summary>
    /// Converts endianness of unsigned 16-bit array between source and platform's endianness.
    /// </summary>
    /// <param name="span">Span to convert.</param>
    /// <param name="littleEndian">If true, use little-endian encoding.</param>
    public static void ConvertU16Array(Span<ushort> span, bool littleEndian)
    {
        if (littleEndian == BitConverter.IsLittleEndian) return;
        for (int i = 0; i < span.Length; i++) span[i] = BinaryPrimitives.ReverseEndianness(span[i]);
    }

    /// <summary>
    /// Converts endianness of unsigned 16-bit array between source and platform's endianness.
    /// </summary>
    /// <param name="span">Span to convert.</param>
    /// <param name="littleEndian">If true, use little-endian encoding.</param>
    public static void ConvertU16Array(Span<byte> span, bool littleEndian) =>
        ConvertU16Array(MemoryMarshal.Cast<byte, ushort>(span), littleEndian);

    /// <summary>
    /// Converts endianness of signed 32-bit array between source and platform's endianness.
    /// </summary>
    /// <param name="span">Span to convert.</param>
    /// <param name="littleEndian">If true, use little-endian encoding.</param>
    public static void ConvertS32Array(Span<int> span, bool littleEndian)
    {
        if (littleEndian == BitConverter.IsLittleEndian) return;
        for (int i = 0; i < span.Length; i++) span[i] = BinaryPrimitives.ReverseEndianness(span[i]);
    }

    /// <summary>
    /// Converts endianness of signed 32-bit array between source and platform's endianness.
    /// </summary>
    /// <param name="span">Span to convert.</param>
    /// <param name="littleEndian">If true, use little-endian encoding.</param>
    public static void ConvertS32Array(Span<byte> span, bool littleEndian) =>
        ConvertS32Array(MemoryMarshal.Cast<byte, int>(span), littleEndian);

    /// <summary>
    /// Converts endianness of unsigned 32-bit array between source and platform's endianness.
    /// </summary>
    /// <param name="span">Span to convert.</param>
    /// <param name="littleEndian">If true, use little-endian encoding.</param>
    public static void ConvertU32Array(Span<uint> span, bool littleEndian)
    {
        if (littleEndian == BitConverter.IsLittleEndian) return;
        for (int i = 0; i < span.Length; i++) span[i] = BinaryPrimitives.ReverseEndianness(span[i]);
    }

    /// <summary>
    /// Converts endianness of unsigned 32-bit array between source and platform's endianness.
    /// </summary>
    /// <param name="span">Span to convert.</param>
    /// <param name="littleEndian">If true, use little-endian encoding.</param>
    public static void ConvertU32Array(Span<byte> span, bool littleEndian) =>
        ConvertU32Array(MemoryMarshal.Cast<byte, uint>(span), littleEndian);

    /// <summary>
    /// Converts endianness of signed 64-bit array between source and platform's endianness.
    /// </summary>
    /// <param name="span">Span to convert.</param>
    /// <param name="littleEndian">If true, use little-endian encoding.</param>
    public static void ConvertS64Array(Span<long> span, bool littleEndian)
    {
        if (littleEndian == BitConverter.IsLittleEndian) return;
        for (int i = 0; i < span.Length; i++) span[i] = BinaryPrimitives.ReverseEndianness(span[i]);
    }

    /// <summary>
    /// Converts endianness of signed 64-bit array between source and platform's endianness.
    /// </summary>
    /// <param name="span">Span to convert.</param>
    /// <param name="littleEndian">If true, use little-endian encoding.</param>
    public static void ConvertS64Array(Span<byte> span, bool littleEndian) =>
        ConvertS64Array(MemoryMarshal.Cast<byte, long>(span), littleEndian);

    /// <summary>
    /// Converts endianness of unsigned 64-bit array between source and platform's endianness.
    /// </summary>
    /// <param name="span">Span to convert.</param>
    /// <param name="littleEndian">If true, use little-endian encoding.</param>
    public static void ConvertU64Array(Span<ulong> span, bool littleEndian)
    {
        if (littleEndian == BitConverter.IsLittleEndian) return;
        for (int i = 0; i < span.Length; i++) span[i] = BinaryPrimitives.ReverseEndianness(span[i]);
    }

    /// <summary>
    /// Converts endianness of unsigned 64-bit array between source and platform's endianness.
    /// </summary>
    /// <param name="span">Span to convert.</param>
    /// <param name="littleEndian">If true, use little-endian encoding.</param>
    public static void ConvertU64Array(Span<byte> span, bool littleEndian) =>
        ConvertU64Array(MemoryMarshal.Cast<byte, ulong>(span), littleEndian);

    #endregion

    #region Instance endian

    /// <summary>
    /// Reads signed 16-bit value from stream.
    /// </summary>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    /// <returns>Value.</returns>
    public short ReadS16(Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        if (stream is MemoryStream)
        {
            Read(stream, 2, out Span<byte> span2, false);
            return GetS16(span2);
        }

        Span<byte> span = stackalloc byte[2];
        Read(stream, span, false);
        return GetS16(span);
    }

    /// <summary>
    /// Reads signed 16-bit value from stream at the specified offset.
    /// </summary>
    /// <param name="offset">Offset to read from.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    /// <returns>Value.</returns>
    public short ReadS16(long offset, Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        if (stream is MemoryStream)
        {
            Read(stream, offset, 2, out Span<byte> span2, false);
            return GetS16(span2);
        }

        Span<byte> span = stackalloc byte[2];
        Read(stream, offset, span, false);
        return GetS16(span);
    }

    /// <summary>
    /// Reads signed 16-bit value from span at the specified offset.
    /// </summary>
    /// <param name="offset">Offset to read from.</param>
    /// <param name="span">Span to read from.</param>
    /// <returns>Value.</returns>
    public short GetS16(ReadOnlySpan<byte> span, int offset = 0) => GetS16(span, LittleEndian, offset);

    /// <summary>
    /// Writes signed 16-bit value to span at the specified offset.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="value">Value.</param>
    /// <param name="offset">Offset to write to.</param>
    /// <returns>Value.</returns>
    public short SetS16(Span<byte> span, short value, int offset = 0) => SetS16(span, value, LittleEndian, offset);

    /// <summary>
    /// Reads unsigned 16-bit value from stream.
    /// </summary>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    /// <returns>Value.</returns>
    public ushort ReadU16(Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        if (stream is MemoryStream)
        {
            Read(stream, 2, out Span<byte> span2, false);
            return GetU16(span2);
        }

        Span<byte> span = stackalloc byte[2];
        Read(stream, span, false);
        return GetU16(span);
    }

    /// <summary>
    /// Reads unsigned 16-bit value from stream at the specified offset.
    /// </summary>
    /// <param name="offset">Offset to read from.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    /// <returns>Value.</returns>
    public ushort ReadU16(long offset, Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        if (stream is MemoryStream)
        {
            Read(stream, offset, 2, out Span<byte> span2, false);
            return GetU16(span2);
        }

        Span<byte> span = stackalloc byte[2];
        Read(stream, offset, span, false);
        return GetU16(span);
    }

    /// <summary>
    /// Reads unsigned 16-bit value from span at the specified offset.
    /// </summary>
    /// <param name="offset">Offset to read from.</param>
    /// <param name="span">Span to read from.</param>
    /// <returns>Value.</returns>
    public ushort GetU16(ReadOnlySpan<byte> span, int offset = 0) => GetU16(span, LittleEndian, offset);

    /// <summary>
    /// Writes unsigned 16-bit value to span at the specified offset.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="value">Value.</param>
    /// <param name="offset">Offset to write to.</param>
    /// <returns>Value.</returns>
    public ushort SetU16(Span<byte> span, ushort value, int offset = 0) =>
        SetU16(span, value, LittleEndian, offset);

    /// <summary>
    /// Reads signed 32-bit value from stream.
    /// </summary>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    /// <returns>Value.</returns>
    public int ReadS32(Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        if (stream is MemoryStream)
        {
            Read(stream, 4, out Span<byte> span2, false);
            return GetS32(span2);
        }

        Span<byte> span = stackalloc byte[4];
        Read(stream, span, false);
        return GetS32(span);
    }

    /// <summary>
    /// Reads signed 32-bit value from stream at the specified offset.
    /// </summary>
    /// <param name="offset">Offset to read from.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    /// <returns>Value.</returns>
    public int ReadS32(long offset, Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        if (stream is MemoryStream)
        {
            Read(stream, offset, 4, out Span<byte> span2, false);
            return GetS32(span2);
        }

        Span<byte> span = stackalloc byte[4];
        Read(stream, offset, span, false);
        return GetS32(span);
    }

    /// <summary>
    /// Reads signed 32-bit value from span at the specified offset.
    /// </summary>
    /// <param name="offset">Offset to read from.</param>
    /// <param name="span">Span to read from.</param>
    /// <returns>Value.</returns>
    public int GetS32(ReadOnlySpan<byte> span, int offset = 0) => GetS32(span, LittleEndian, offset);

    /// <summary>
    /// Writes signed 32-bit value to span at the specified offset.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="value">Value.</param>
    /// <param name="offset">Offset to write to.</param>
    /// <returns>Value.</returns>
    public int SetS32(Span<byte> span, int value, int offset = 0) => SetS32(span, value, LittleEndian, offset);

    /// <summary>
    /// Reads unsigned 32-bit value from stream.
    /// </summary>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    /// <returns>Value.</returns>
    public uint ReadU32(Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        if (stream is MemoryStream)
        {
            Read(stream, 4, out Span<byte> span2, false);
            return GetU32(span2);
        }

        Span<byte> span = stackalloc byte[4];
        Read(stream, span, false);
        return GetU32(span);
    }

    /// <summary>
    /// Reads unsigned 32-bit value from stream at the specified offset.
    /// </summary>
    /// <param name="offset">Offset to read from.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    /// <returns>Value.</returns>
    public uint ReadU32(long offset, Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        if (stream is MemoryStream)
        {
            Read(stream, offset, 4, out Span<byte> span2, false);
            return GetU32(span2);
        }

        Span<byte> span = stackalloc byte[4];
        Read(stream, offset, span, false);
        return GetU32(span);
    }

    /// <summary>
    /// Reads unsigned 32-bit value from span at the specified offset.
    /// </summary>
    /// <param name="offset">Offset to read from.</param>
    /// <param name="span">Span to read from.</param>
    /// <returns>Value.</returns>
    public uint GetU32(ReadOnlySpan<byte> span, int offset = 0) => GetU32(span, LittleEndian, offset);

    /// <summary>
    /// Writes unsigned 32-bit value to span at the specified offset.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="value">Value.</param>
    /// <param name="offset">Offset to write to.</param>
    /// <returns>Value.</returns>
    public uint SetU32(Span<byte> span, uint value, int offset = 0) => SetU32(span, value, LittleEndian, offset);

    /// <summary>
    /// Reads signed 64-bit value from stream.
    /// </summary>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    /// <returns>Value.</returns>
    public long ReadS64(Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        if (stream is MemoryStream)
        {
            Read(stream, 8, out Span<byte> span2, false);
            return GetS64(span2);
        }

        Span<byte> span = stackalloc byte[8];
        Read(stream, span, false);
        return GetS64(span);
    }

    /// <summary>
    /// Reads signed 64-bit value from stream at the specified offset.
    /// </summary>
    /// <param name="offset">Offset to read from.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    /// <returns>Value.</returns>
    public long ReadS64(long offset, Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        if (stream is MemoryStream)
        {
            Read(stream, offset, 8, out Span<byte> span2, false);
            return GetS64(span2);
        }

        Span<byte> span = stackalloc byte[8];
        Read(stream, offset, span, false);
        return GetS64(span);
    }

    /// <summary>
    /// Reads signed 64-bit value from span at the specified offset.
    /// </summary>
    /// <param name="offset">Offset to read from.</param>
    /// <param name="span">Span to read from.</param>
    /// <returns>Value.</returns>
    public long GetS64(ReadOnlySpan<byte> span, int offset = 0) => GetS64(span, LittleEndian, offset);

    /// <summary>
    /// Writes signed 64-bit value to span at the specified offset.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="value">Value.</param>
    /// <param name="offset">Offset to write to.</param>
    public long SetS64(Span<byte> span, long value, int offset = 0) => SetS64(span, value, LittleEndian, offset);

    /// <summary>
    /// Reads unsigned 64-bit value from stream.
    /// </summary>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    /// <returns>Value.</returns>
    /// <returns>Value.</returns>
    public ulong ReadU64(Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        if (stream is MemoryStream)
        {
            Read(stream, 8, out Span<byte> span2, false);
            return GetU64(span2);
        }

        Span<byte> span = stackalloc byte[8];
        Read(stream, span, false);
        return GetU64(span);
    }

    /// <summary>
    /// Reads unsigned 64-bit value from stream at the specified offset.
    /// </summary>
    /// <param name="offset">Offset to read from.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    /// <returns>Value.</returns>
    public ulong ReadU64(long offset, Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        if (stream is MemoryStream)
        {
            Read(stream, offset, 8, out Span<byte> span2, false);
            return GetU64(span2);
        }

        Span<byte> span = stackalloc byte[8];
        Read(stream, offset, span, false);
        return GetU64(span);
    }

    /// <summary>
    /// Reads unsigned 64-bit value from span at the specified offset.
    /// </summary>
    /// <param name="offset">Offset to read from.</param>
    /// <param name="span">Span to read from.</param>
    /// <returns>Value.</returns>
    public ulong GetU64(ReadOnlySpan<byte> span, int offset = 0) => GetU64(span, LittleEndian, offset);

    /// <summary>
    /// Writes unsigned 64-bit value to span at the specified offset.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="value">Value.</param>
    /// <param name="offset">Offset to write to.</param>
    /// <returns>Value.</returns>
    public ulong SetU64(Span<byte> span, ulong value, int offset = 0) => SetU64(span, value, LittleEndian, offset);

    /// <summary>
    /// Converts endianness of signed 16-bit array between source and platform's endianness.
    /// </summary>
    /// <param name="span">Span to convert.</param>
    public void ConvertS16Array(Span<short> span) => ConvertS16Array(span, LittleEndian);

    /// <summary>
    /// Converts endianness of signed 16-bit array between source and platform's endianness.
    /// </summary>
    /// <param name="span">Span to convert.</param>
    public void ConvertS16Array(Span<byte> span) => ConvertS16Array(MemoryMarshal.Cast<byte, short>(span), LittleEndian);

    /// <summary>
    /// Converts endianness of unsigned 16-bit array between source and platform's endianness.
    /// </summary>
    /// <param name="span">Span to convert.</param>
    public void ConvertU16Array(Span<ushort> span) => ConvertU16Array(span, LittleEndian);

    /// <summary>
    /// Converts endianness of unsigned 16-bit array between source and platform's endianness.
    /// </summary>
    /// <param name="span">Span to convert.</param>
    public void ConvertU16Array(Span<byte> span) => ConvertU16Array(MemoryMarshal.Cast<byte, ushort>(span), LittleEndian);

    /// <summary>
    /// Converts endianness of signed 32-bit array between source and platform's endianness.
    /// </summary>
    /// <param name="span">Span to convert.</param>
    public void ConvertS32Array(Span<int> span) => ConvertS32Array(span, LittleEndian);

    /// <summary>
    /// Converts endianness of signed 32-bit array between source and platform's endianness.
    /// </summary>
    /// <param name="span">Span to convert.</param>
    public void ConvertS32Array(Span<byte> span) => ConvertS32Array(MemoryMarshal.Cast<byte, int>(span), LittleEndian);

    /// <summary>
    /// Converts endianness of unsigned 32-bit array between source and platform's endianness.
    /// </summary>
    /// <param name="span">Span to convert.</param>
    public void ConvertU32Array(Span<uint> span) => ConvertU32Array(span, LittleEndian);

    /// <summary>
    /// Converts endianness of unsigned 32-bit array between source and platform's endianness.
    /// </summary>
    /// <param name="span">Span to convert.</param>
    public void ConvertU32Array(Span<byte> span) => ConvertU32Array(MemoryMarshal.Cast<byte, uint>(span), LittleEndian);

    /// <summary>
    /// Converts endianness of signed 64-bit array between source and platform's endianness.
    /// </summary>
    /// <param name="span">Span to convert.</param>
    public void ConvertS64Array(Span<long> span) => ConvertS64Array(span, LittleEndian);

    /// <summary>
    /// Converts endianness of signed 64-bit array between source and platform's endianness.
    /// </summary>
    /// <param name="span">Span to convert.</param>
    public void ConvertS64Array(Span<byte> span) => ConvertS64Array(MemoryMarshal.Cast<byte, long>(span), LittleEndian);

    /// <summary>
    /// Converts endianness of unsigned 64-bit array between source and platform's endianness.
    /// </summary>
    /// <param name="span">Span to convert.</param>
    public void ConvertU64Array(Span<ulong> span) => ConvertU64Array(span, LittleEndian);

    /// <summary>
    /// Converts endianness of unsigned 64-bit array between source and platform's endianness.
    /// </summary>
    /// <param name="span">Span to convert.</param>
    public void ConvertU64Array(Span<byte> span) => ConvertU64Array(MemoryMarshal.Cast<byte, ulong>(span), LittleEndian);

    #endregion

    /// <summary>
    /// Reads 16-bit float value from stream.
    /// </summary>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    /// <returns>Value.</returns>
    public float ReadHalf(Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        if (stream is MemoryStream)
        {
            Read(stream, 2, out Span<byte> span2, false);
            return GetSingle(span2);
        }

        Span<byte> span = stackalloc byte[2];
        Read(stream, span, false);
        return GetSingle(span);
    }

    /// <summary>
    /// Reads 16-bit float value from stream at the specified offset.
    /// </summary>
    /// <param name="offset">Offset to read from.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    /// <returns>Value.</returns>
    public float ReadHalf(long offset, Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        if (stream is MemoryStream)
        {
            Read(stream, offset, 2, out Span<byte> span2, false);
            return GetSingle(span2);
        }

        Span<byte> span = stackalloc byte[2];
        Read(stream, offset, span, false);
        return GetSingle(span);
    }

    /// <summary>
    /// Reads 16-bit float value from span at the specified offset.
    /// </summary>
    /// <param name="offset">Offset to read from.</param>
    /// <param name="span">Span to read from.</param>
    /// <returns>Value.</returns>
    public static float GetHalf(ReadOnlySpan<byte> span, int offset = 0)
    {
        Span<byte> span2 = stackalloc byte[2];
        span.Slice(offset, 2).CopyTo(span2);
        return HalfHelper.HalfToSingle(MemoryMarshal.Read<ushort>(span2));
    }

    /// <summary>
    /// Writes 16-bit float value to span at the specified offset.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="value">Value.</param>
    /// <param name="offset">Offset to write to.</param>
    /// <returns>Value.</returns>
    public static float SetHalf(Span<byte> span, float value, int offset = 0)
    {
        Span<byte> span2 = stackalloc byte[2];
        MemoryMarshal.Cast<byte, ushort>(span2)[0] = HalfHelper.SingleToHalf(value);
        span2.CopyTo(span.Slice(offset, 2));
        return value;
    }

    /// <summary>
    /// Reads 32-bit float value from stream.
    /// </summary>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    /// <returns>Value.</returns>
    public float ReadSingle(Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        if (stream is MemoryStream)
        {
            Read(stream, 4, out Span<byte> span2, false);
            return GetSingle(span2);
        }

        Span<byte> span = stackalloc byte[4];
        Read(stream, span, false);
        return GetSingle(span);
    }

    /// <summary>
    /// Reads 32-bit float value from stream at the specified offset.
    /// </summary>
    /// <param name="offset">Offset to read from.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    /// <returns>Value.</returns>
    public float ReadSingle(long offset, Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        if (stream is MemoryStream)
        {
            Read(stream, offset, 4, out Span<byte> span2, false);
            return GetSingle(span2);
        }

        Span<byte> span = stackalloc byte[4];
        Read(stream, offset, span, false);
        return GetSingle(span);
    }

    /// <summary>
    /// Reads 32-bit float value from span at the specified offset.
    /// </summary>
    /// <param name="offset">Offset to read from.</param>
    /// <param name="span">Span to read from.</param>
    /// <returns>Value.</returns>
    public static float GetSingle(ReadOnlySpan<byte> span, int offset = 0)
    {
        Span<byte> span2 = stackalloc byte[4];
        span.Slice(offset, 4).CopyTo(span2);
        return MemoryMarshal.Read<float>(span2);
    }

    /// <summary>
    /// Writes 32-bit float value to span at the specified offset.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="value">Value.</param>
    /// <param name="offset">Offset to write to.</param>
    /// <returns>Value.</returns>
    public static float SetSingle(Span<byte> span, float value, int offset = 0)
    {
        Span<byte> span2 = stackalloc byte[4];
        MemoryMarshal.Cast<byte, float>(span2)[0] = value;
        span2.CopyTo(span.Slice(offset, 4));
        return value;
    }

    /// <summary>
    /// Reads 64-bit float value from stream.
    /// </summary>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    /// <returns>Value.</returns>
    public double ReadDouble(Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        Span<byte> span = stackalloc byte[8];
        Read(stream, span, false);
        return GetDouble(span);
    }

    /// <summary>
    /// Reads 64-bit float value from stream at the specified offset.
    /// </summary>
    /// <param name="offset">Offset to read from.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    /// <returns>Value.</returns>
    public double ReadDouble(long offset, Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        if (stream is MemoryStream)
        {
            Read(stream, offset, 8, out Span<byte> span2, false);
            return GetDouble(span2);
        }

        Span<byte> span = stackalloc byte[8];
        Read(stream, offset, span, false);
        return GetDouble(span);
    }

    /// <summary>
    /// Reads 64-bit float value from span at the specified offset.
    /// </summary>
    /// <param name="offset">Offset to read from.</param>
    /// <param name="span">Span to read from.</param>
    /// <returns>Value.</returns>
    public static double GetDouble(ReadOnlySpan<byte> span, int offset = 0)
    {
        Span<byte> span2 = stackalloc byte[8];
        span.Slice(offset, 8).CopyTo(span2);
        return MemoryMarshal.Read<double>(span2);
    }

    /// <summary>
    /// Writes 32-bit float value to span at the specified offset.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="value">Value.</param>
    /// <param name="offset">Offset to write to.</param>
    /// <returns>Value.</returns>
    public static double SetDouble(Span<byte> span, double value, int offset = 0)
    {
        Span<byte> span2 = stackalloc byte[8];
        MemoryMarshal.Cast<byte, double>(span2)[0] = value;
        span2.CopyTo(span.Slice(offset, 8));
        return value;
    }

    /// <summary>
    /// Reads array of signed 8-bit values from stream.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    public void ReadS8Array(Span<sbyte> span, Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        Read(stream, MemoryMarshal.Cast<sbyte, byte>(span), false);
    }

    /// <summary>
    /// Reads array of signed 8-bit values from stream.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="offset">Offset in stream to read from.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    public void ReadS8Array(Span<sbyte> span, long offset, Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        Read(stream, offset, MemoryMarshal.Cast<sbyte, byte>(span), false);
    }

    /// <summary>
    /// Reads array of signed 8-bit values from stream.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    public void ReadS8Array(Span<byte> span, Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        Read(stream, span, false);
    }

    /// <summary>
    /// Reads array of signed 8-bit values from stream.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="offset">Offset in stream to read from.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    public void ReadS8Array(Span<byte> span, long offset, Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        Read(stream, offset, span, false);
    }

    /// <summary>
    /// Reads array of signed 8-bit values from stream.
    /// </summary>
    /// <param name="count">Number of elements to read.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    /// <returns>Newly allocated array.</returns>
    public sbyte[] ReadS8Array(int count, Stream? stream = null)
    {
        sbyte[] arr = new sbyte[count];
        Span<byte> span = MemoryMarshal.Cast<sbyte, byte>(arr);
        ReadS8Array(span, stream);
        return arr;
    }

    /// <summary>
    /// Reads array of signed 8-bit values from stream.
    /// </summary>
    /// <param name="offset">Offset in stream to read from.</param>
    /// <param name="count">Number of elements to read.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    /// <returns>Newly allocated array.</returns>
    public sbyte[] ReadS8Array(long offset, int count, Stream? stream = null)
    {
        sbyte[] arr = new sbyte[count];
        Span<byte> span = MemoryMarshal.Cast<sbyte, byte>(arr);
        ReadS8Array(span, offset, stream);
        return arr;
    }

    //

    /// <summary>
    /// Reads array of signed 8-bit values from stream.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    public void ReadU8Array(Span<byte> span, Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        Read(stream, span, false);
    }

    /// <summary>
    /// Reads array of signed 8-bit values from stream.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="offset">Offset in stream to read from.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    public void ReadU8Array(Span<byte> span, long offset, Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        Read(stream, offset, span, false);
    }

    /// <summary>
    /// Reads array of signed 8-bit values from stream.
    /// </summary>
    /// <param name="count">Number of elements to read.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    /// <returns>Newly allocated array.</returns>
    public byte[] ReadU8Array(int count, Stream? stream = null)
    {
        byte[] arr = new byte[count];
        ReadU8Array(arr, stream);
        return arr;
    }

    /// <summary>
    /// Reads array of signed 8-bit values from stream.
    /// </summary>
    /// <param name="offset">Offset in stream to read from.</param>
    /// <param name="count">Number of elements to read.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    /// <returns>Newly allocated array.</returns>
    public byte[] ReadU8Array(long offset, int count, Stream? stream = null)
    {
        byte[] arr = new byte[count];
        ReadU8Array(arr, offset, stream);
        return arr;
    }

    /// <summary>
    /// Reads array of signed 16-bit values from stream.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    public void ReadS16Array(Span<short> span, Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        Read(stream, MemoryMarshal.Cast<short, byte>(span), false);
        ConvertS16Array(span);
    }

    /// <summary>
    /// Reads array of signed 16-bit values from stream.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="offset">Offset in stream to read from.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    public void ReadS16Array(Span<short> span, long offset, Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        Read(stream, offset, MemoryMarshal.Cast<short, byte>(span), false);
        ConvertS16Array(span);
    }

    /// <summary>
    /// Reads array of signed 16-bit values from stream.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    public void ReadS16Array(Span<byte> span, Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        Read(stream, span, false);
        ConvertS16Array(span);
    }

    /// <summary>
    /// Reads array of signed 16-bit values from stream.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="offset">Offset in stream to read from.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    public void ReadS16Array(Span<byte> span, long offset, Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        Read(stream, offset, span, false);
        ConvertS16Array(span);
    }

    /// <summary>
    /// Reads array of signed 16-bit values from stream.
    /// </summary>
    /// <param name="count">Number of elements to read.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    /// <returns>Newly allocated array.</returns>
    public short[] ReadS16Array(int count, Stream? stream = null)
    {
        short[] arr = new short[count];
        Span<byte> span = MemoryMarshal.Cast<short, byte>(arr);
        ReadS16Array(span, stream);
        return arr;
    }

    /// <summary>
    /// Reads array of signed 16-bit values from stream.
    /// </summary>
    /// <param name="offset">Offset in stream to read from.</param>
    /// <param name="count">Number of elements to read.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    /// <returns>Newly allocated array.</returns>
    public short[] ReadS16Array(long offset, int count, Stream? stream = null)
    {
        short[] arr = new short[count];
        Span<byte> span = MemoryMarshal.Cast<short, byte>(arr);
        ReadS16Array(span, offset, stream);
        return arr;
    }

    /// <summary>
    /// Reads array of unsigned 16-bit values from stream.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    public void ReadU16Array(Span<ushort> span, Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        Read(stream, MemoryMarshal.Cast<ushort, byte>(span), false);
        ConvertU16Array(span);
    }

    /// <summary>
    /// Reads array of unsigned 16-bit values from stream.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="offset">Offset in stream to read from.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    public void ReadU16Array(Span<ushort> span, long offset, Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        Read(stream, offset, MemoryMarshal.Cast<ushort, byte>(span), false);
        ConvertU16Array(span);
    }

    /// <summary>
    /// Reads array of unsigned 16-bit values from stream.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    public void ReadU16Array(Span<byte> span, Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        Read(stream, span, false);
        ConvertU16Array(span);
    }

    /// <summary>
    /// Reads array of unsigned 16-bit values from stream.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="offset">Offset in stream to read from.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    public void ReadU16Array(Span<byte> span, long offset, Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        Read(stream, offset, span, false);
        ConvertU16Array(span);
    }

    /// <summary>
    /// Reads array of unsigned 16-bit values from stream.
    /// </summary>
    /// <param name="count">Number of elements to read.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    /// <returns>Newly allocated array.</returns>
    public ushort[] ReadU16Array(int count, Stream? stream = null)
    {
        ushort[] arr = new ushort[count];
        Span<byte> span = MemoryMarshal.Cast<ushort, byte>(arr);
        ReadU16Array(span, stream);
        return arr;
    }

    /// <summary>
    /// Reads array of unsigned 16-bit values from stream.
    /// </summary>
    /// <param name="offset">Offset in stream to read from.</param>
    /// <param name="count">Number of elements to read.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    /// <returns>Newly allocated array.</returns>
    public ushort[] ReadU16Array(long offset, int count, Stream? stream = null)
    {
        ushort[] arr = new ushort[count];
        Span<byte> span = MemoryMarshal.Cast<ushort, byte>(arr);
        ReadU16Array(span, offset, stream);
        return arr;
    }

    /// <summary>
    /// Reads array of signed 32-bit values from stream.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    public void ReadS32Array(Span<int> span, Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        Read(stream, MemoryMarshal.Cast<int, byte>(span), false);
        ConvertS32Array(span);
    }

    /// <summary>
    /// Reads array of signed 32-bit values from stream.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="offset">Offset in stream to read from.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    public void ReadS32Array(Span<int> span, long offset, Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        Read(stream, offset, MemoryMarshal.Cast<int, byte>(span), false);
        ConvertS32Array(span);
    }

    /// <summary>
    /// Reads array of signed 32-bit values from stream.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    public void ReadS32Array(Span<byte> span, Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        Read(stream, span, false);
        ConvertS32Array(span);
    }

    /// <summary>
    /// Reads array of signed 32-bit values from stream.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="offset">Offset in stream to read from.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    public void ReadS32Array(Span<byte> span, long offset, Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        Read(stream, offset, span, false);
        ConvertS32Array(span);
    }

    /// <summary>
    /// Reads array of signed 32-bit values from stream.
    /// </summary>
    /// <param name="count">Number of elements to read.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    /// <returns>Newly allocated array.</returns>
    public int[] ReadS32Array(int count, Stream? stream = null)
    {
        int[] arr = new int[count];
        Span<byte> span = MemoryMarshal.Cast<int, byte>(arr);
        ReadS32Array(span, stream);
        return arr;
    }

    /// <summary>
    /// Reads array of signed 32-bit values from stream.
    /// </summary>
    /// <param name="offset">Offset in stream to read from.</param>
    /// <param name="count">Number of elements to read.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    /// <returns>Newly allocated array.</returns>
    public int[] ReadS32Array(long offset, int count, Stream? stream = null)
    {
        int[] arr = new int[count];
        Span<byte> span = MemoryMarshal.Cast<int, byte>(arr);
        ReadS32Array(span, offset, stream);
        return arr;
    }

    /// <summary>
    /// Reads array of unsigned 32-bit values from stream.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    public void ReadU32Array(Span<uint> span, Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        Read(stream, MemoryMarshal.Cast<uint, byte>(span), false);
        ConvertU32Array(span);
    }

    /// <summary>
    /// Reads array of unsigned 32-bit values from stream.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="offset">Offset in stream to read from.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    public void ReadU32Array(Span<uint> span, long offset, Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        Read(stream, offset, MemoryMarshal.Cast<uint, byte>(span), false);
        ConvertU32Array(span);
    }

    /// <summary>
    /// Reads array of unsigned 32-bit values from stream.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    public void ReadU32Array(Span<byte> span, Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        Read(stream, span, false);
        ConvertU32Array(span);
    }

    /// <summary>
    /// Reads array of unsigned 32-bit values from stream.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="offset">Offset in stream to read from.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    public void ReadU32Array(Span<byte> span, long offset, Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        Read(stream, offset, span, false);
        ConvertU32Array(span);
    }

    /// <summary>
    /// Reads array of unsigned 32-bit values from stream.
    /// </summary>
    /// <param name="count">Number of elements to read.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    /// <returns>Newly allocated array.</returns>
    public uint[] ReadU32Array(int count, Stream? stream = null)
    {
        uint[] arr = new uint[count];
        Span<byte> span = MemoryMarshal.Cast<uint, byte>(arr);
        ReadU32Array(span, stream);
        return arr;
    }

    /// <summary>
    /// Reads array of unsigned 32-bit values from stream.
    /// </summary>
    /// <param name="offset">Offset in stream to read from.</param>
    /// <param name="count">Number of elements to read.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    /// <returns>Newly allocated array.</returns>
    public uint[] ReadU32Array(long offset, int count, Stream? stream = null)
    {
        uint[] arr = new uint[count];
        Span<byte> span = MemoryMarshal.Cast<uint, byte>(arr);
        ReadU32Array(span, offset, stream);
        return arr;
    }

    /// <summary>
    /// Reads array of signed 64-bit values from stream.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    public void ReadS64Array(Span<long> span, Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        Read(stream, MemoryMarshal.Cast<long, byte>(span), false);
        ConvertS64Array(span);
    }

    /// <summary>
    /// Reads array of signed 64-bit values from stream.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="offset">Offset in stream to read from.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    public void ReadS64Array(Span<long> span, long offset, Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        Read(stream, offset, MemoryMarshal.Cast<long, byte>(span), false);
        ConvertS64Array(span);
    }

    /// <summary>
    /// Reads array of signed 64-bit values from stream.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    public void ReadS64Array(Span<byte> span, Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        Read(stream, span, false);
        ConvertS64Array(span);
    }

    /// <summary>
    /// Reads array of signed 64-bit values from stream.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="offset">Offset in stream to read from.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    public void ReadS64Array(Span<byte> span, long offset, Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        Read(stream, offset, span, false);
        ConvertS64Array(span);
    }

    /// <summary>
    /// Reads array of signed 64-bit values from stream.
    /// </summary>
    /// <param name="count">Number of elements to read.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    /// <returns>Newly allocated array.</returns>
    public long[] ReadS64Array(int count, Stream? stream = null)
    {
        long[] arr = new long[count];
        Span<byte> span = MemoryMarshal.Cast<long, byte>(arr);
        ReadS64Array(span, stream);
        return arr;
    }

    /// <summary>
    /// Reads array of signed 64-bit values from stream.
    /// </summary>
    /// <param name="offset">Offset in stream to read from.</param>
    /// <param name="count">Number of elements to read.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    /// <returns>Newly allocated array.</returns>
    public long[] ReadS64Array(long offset, int count, Stream? stream = null)
    {
        long[] arr = new long[count];
        Span<byte> span = MemoryMarshal.Cast<long, byte>(arr);
        ReadS64Array(span, offset, stream);
        return arr;
    }

    /// <summary>
    /// Reads array of unsigned 64-bit values from stream.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    public void ReadU64Array(Span<ulong> span, Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        Read(stream, MemoryMarshal.Cast<ulong, byte>(span), false);
        ConvertU64Array(span);
    }

    /// <summary>
    /// Reads array of unsigned 64-bit values from stream.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="offset">Offset in stream to read from.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    public void ReadU64Array(Span<ulong> span, long offset, Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        Read(stream, offset, MemoryMarshal.Cast<ulong, byte>(span), false);
        ConvertU64Array(span);
    }

    /// <summary>
    /// Reads array of unsigned 64-bit values from stream.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    public void ReadU64Array(Span<byte> span, Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        Read(stream, span, false);
        ConvertU64Array(span);
    }

    /// <summary>
    /// Reads array of unsigned 64-bit values from stream.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="offset">Offset in stream to read from.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    public void ReadU64Array(Span<byte> span, long offset, Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        Read(stream, offset, span, false);
        ConvertU64Array(span);
    }

    /// <summary>
    /// Reads array of unsigned 64-bit values from stream.
    /// </summary>
    /// <param name="count">Number of elements to read.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    /// <returns>Newly allocated array.</returns>
    public ulong[] ReadU64Array(int count, Stream? stream = null)
    {
        ulong[] arr = new ulong[count];
        Span<byte> span = MemoryMarshal.Cast<ulong, byte>(arr);
        ReadS16Array(span, stream);
        return arr;
    }

    /// <summary>
    /// Reads array of unsigned 64-bit values from stream.
    /// </summary>
    /// <param name="offset">Offset in stream to read from.</param>
    /// <param name="count">Number of elements to read.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    /// <returns>Newly allocated array.</returns>
    public ulong[] ReadU64Array(long offset, int count, Stream? stream = null)
    {
        ulong[] arr = new ulong[count];
        Span<byte> span = MemoryMarshal.Cast<ulong, byte>(arr);
        ReadS16Array(span, offset, stream);
        return arr;
    }

    /// <summary>
    /// Converts array of half-precision floating-point values to single-precision.
    /// </summary>
    /// <param name="source">Source span.</param>
    /// <param name="target">Target span.</param>
    public static void ConvertHalfArrayToFloat(ReadOnlySpan<byte> source, Span<float> target)
    {
        ReadOnlySpan<ushort> span = MemoryMarshal.Cast<byte, ushort>(source);
        for (int i = 0; i < span.Length; i++) target[i] = HalfHelper.HalfToSingle(span[i]);
    }

    /// <summary>
    /// Converts array of single-precision floating-point values to half-precision.
    /// </summary>
    /// <param name="source">Source span.</param>
    /// <param name="target">Target span.</param>
    public static void ConvertFloatArrayToHalf(ReadOnlySpan<float> source, Span<byte> target)
    {
        Span<ushort> span = MemoryMarshal.Cast<byte, ushort>(target);
        for (int i = 0; i < source.Length; i++) span[i] = HalfHelper.SingleToHalf(source[i]);
    }

    /// <summary>
    /// Reads array of half-precision floating-point values as single-precision from stream.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    public void ReadHalfArray(Span<float> span, Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        byte[] arr = Shared.Rent(span.Length * 2);
        try
        {
            Span<byte> span2 = arr.AsSpan(0, span.Length * 2);
            Read(stream, span2, false);
            ConvertHalfArrayToFloat(span2, span);
        }
        finally
        {
            Shared.Return(arr);
        }
    }

    /// <summary>
    /// Reads array of single-precision floating-point values as single-precision from stream.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="offset">Offset in stream to read from.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    public void ReadHalfArray(Span<float> span, long offset, Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        byte[] arr = Shared.Rent(span.Length * 2);
        try
        {
            Span<byte> span2 = arr.AsSpan(0, span.Length * 2);
            Read(stream, offset, span2, false);
            ConvertHalfArrayToFloat(span2, span);
        }
        finally
        {
            Shared.Return(arr);
        }
    }

    /// <summary>
    /// Reads array of single-precision floating-point values as single-precision from stream.
    /// </summary>
    /// <param name="count">Number of elements to read.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    /// <returns>Newly allocated array.</returns>
    public float[] ReadHalfArray(int count, Stream? stream = null)
    {
        float[] arr = new float[count];
        ReadHalfArray(arr, stream);
        return arr;
    }

    /// <summary>
    /// Reads array of single-precision floating-point values as single-precision from stream.
    /// </summary>
    /// <param name="offset">Offset in stream to read from.</param>
    /// <param name="count">Number of elements to read.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    /// <returns>Newly allocated array.</returns>
    public float[] ReadHalfArray(long offset, int count, Stream? stream = null)
    {
        float[] arr = new float[count];
        ReadHalfArray(arr, offset, stream);
        return arr;
    }

    /// <summary>
    /// Reads array of single-precision floating-point values from stream.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    public void ReadSingleArray(Span<float> span, Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        Read(stream, MemoryMarshal.Cast<float, byte>(span), false);
    }

    /// <summary>
    /// Reads array of single-precision floating-point values from stream.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="offset">Offset in stream to read from.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    public void ReadSingleArray(Span<float> span, long offset, Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        Read(stream, offset, MemoryMarshal.Cast<float, byte>(span), false);
    }

    /// <summary>
    /// Reads array of single-precision floating-point values from stream.
    /// </summary>
    /// <param name="count">Number of elements to read.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    /// <returns>Newly allocated array.</returns>
    public float[] ReadSingleArray(int count, Stream? stream = null)
    {
        float[] arr = new float[count];
        ReadSingleArray(arr, stream);
        return arr;
    }

    /// <summary>
    /// Reads array of single-precision floating-point values from stream.
    /// </summary>
    /// <param name="offset">Offset in stream to read from.</param>
    /// <param name="count">Number of elements to read.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    /// <returns>Newly allocated array.</returns>
    public float[] ReadSingleArray(long offset, int count, Stream? stream = null)
    {
        float[] arr = new float[count];
        ReadSingleArray(arr, offset, stream);
        return arr;
    }

    /// <summary>
    /// Reads array of double-precision floating-point values from stream.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    public void ReadDoubleArray(Span<double> span, Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        Read(stream, MemoryMarshal.Cast<double, byte>(span), false);
    }

    /// <summary>
    /// Reads array of double-precision floating-point values from stream.
    /// </summary>
    /// <param name="span">Span to write to.</param>
    /// <param name="offset">Offset in stream to read from.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    public void ReadDoubleArray(Span<double> span, long offset, Stream? stream = null)
    {
        stream ??= _inputStream ?? throw new InvalidOperationException();
        Read(stream, offset, MemoryMarshal.Cast<double, byte>(span), false);
    }

    /// <summary>
    /// Reads array of double-precision floating-point values from stream.
    /// </summary>
    /// <param name="count">Number of elements to read.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    /// <returns>Newly allocated array.</returns>
    public double[] ReadDoubleArray(int count, Stream? stream = null)
    {
        double[] arr = new double[count];
        ReadDoubleArray(arr, stream);
        return arr;
    }

    /// <summary>
    /// Reads array of double-precision floating-point values from stream.
    /// </summary>
    /// <param name="offset">Offset in stream to read from.</param>
    /// <param name="count">Number of elements to read.</param>
    /// <param name="stream">Stream to read from, uses current file if null.</param>
    /// <returns>Newly allocated array.</returns>
    public double[] ReadDoubleArray(long offset, int count, Stream? stream = null)
    {
        double[] arr = new double[count];
        ReadDoubleArray(arr, offset, stream);
        return arr;
    }

    #endregion
}
