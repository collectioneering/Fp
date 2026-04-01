using System;
using System.Buffers.Binary;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using Fp.Helpers;
using Fp.Tests.Utility;

namespace Fp.Tests;

public abstract unsafe class Processor_EncodingHelpers_NumberArray<T> : ProcessorTestBase where T : unmanaged, INumber<T>
{
    private static readonly SectionInputParameters[] s_sectionInputParameters =
    [
        new(0, 0, 0),
        new(128, 7, 4), // test misaligned
        new(128 * 1024, 0, 128 * 1024),
        new(128 * 1024, 64 * 1024, 32 * 1024),
    ];

    public static readonly TheoryData<SectionInputParameters> TdSectionInputParameters = new(s_sectionInputParameters);

    protected abstract BaseUnmanagedIntegerArrayHelper<T> Helper { get; }

    protected virtual bool IsLittleEndian => true;

    protected bool FlipEndian => IsLittleEndian ^ BitConverter.IsLittleEndian;

    [Theory]
    [MemberData(nameof(TdSectionInputParameters))]
    public void Read_ByteArray_Success(SectionInputParameters parameters)
    {
        byte[] arr = CreateByteArray(parameters.InputLength * sizeof(T));
        var result = FlipEndiannessIfNecessary(Helper[arr, parameters.StartIndex, parameters.Length].ToArray());
        Assert.True(
            arr.AsSpan(parameters.StartIndex, parameters.Length * sizeof(T))
                .SequenceEqual(
                    MemoryMarshal.Cast<T, byte>(result)
                )
        );
    }

    [Theory]
    [MemberData(nameof(TdSectionInputParameters))]
    public void Read_MemoryOfByte_Success(SectionInputParameters parameters)
    {
        byte[] arr = CreateByteArray(parameters.InputLength * sizeof(T));
        var input = arr.AsMemory();
        var result = FlipEndiannessIfNecessary(Helper[input, parameters.StartIndex, parameters.Length].ToArray());
        Assert.True(
            arr.AsSpan(parameters.StartIndex, parameters.Length * sizeof(T))
                .SequenceEqual(
                    MemoryMarshal.Cast<T, byte>(result)
                )
        );
    }

    [Theory]
    [MemberData(nameof(TdSectionInputParameters))]
    public void Read_ReadOnlyMemoryOfByte_Success(SectionInputParameters parameters)
    {
        byte[] arr = CreateByteArray(parameters.InputLength * sizeof(T));
        var input = (ReadOnlyMemory<byte>)arr.AsMemory();
        var result = FlipEndiannessIfNecessary(Helper[input, parameters.StartIndex, parameters.Length].ToArray());
        Assert.True(
            arr.AsSpan(parameters.StartIndex, parameters.Length * sizeof(T))
                .SequenceEqual(
                    MemoryMarshal.Cast<T, byte>(result)
                )
        );
    }

    [Theory]
    [MemberData(nameof(TdSectionInputParameters))]
    public void Read_SpanOfByte_Success(SectionInputParameters parameters)
    {
        byte[] arr = CreateByteArray(parameters.InputLength * sizeof(T));
        var input = arr.AsSpan();
        var result = FlipEndiannessIfNecessary(Helper[input, parameters.StartIndex, parameters.Length].ToArray());
        Assert.True(
            arr.AsSpan(parameters.StartIndex, parameters.Length * sizeof(T))
                .SequenceEqual(
                    MemoryMarshal.Cast<T, byte>(result)
                )
        );
    }

    [Theory]
    [MemberData(nameof(TdSectionInputParameters))]
    public void Read_ReadOnlySpanOfByte_Success(SectionInputParameters parameters)
    {
        byte[] arr = CreateByteArray(parameters.InputLength * sizeof(T));
        var input = (ReadOnlySpan<byte>)arr.AsSpan();
        var result = FlipEndiannessIfNecessary(Helper[input, parameters.StartIndex, parameters.Length].ToArray());
        Assert.True(
            arr.AsSpan(parameters.StartIndex, parameters.Length * sizeof(T))
                .SequenceEqual(
                    MemoryMarshal.Cast<T, byte>(result)
                )
        );
    }

    [Theory]
    [MemberData(nameof(TdSectionInputParameters))]
    public void Read_InputStream_Success(SectionInputParameters parameters)
    {
        byte[] arr = CreateByteArray(parameters.InputLength * sizeof(T));
        var ms = new MemoryStream(arr, false);
        P.InputStream = ms;
        var result = FlipEndiannessIfNecessary(Helper[parameters.StartIndex, parameters.Length]);
        Assert.True(
            arr.AsSpan(parameters.StartIndex, parameters.Length * sizeof(T))
                .SequenceEqual(
                    MemoryMarshal.Cast<T, byte>(result)
                )
        );
    }

    [Theory]
    [MemberData(nameof(TdSectionInputParameters))]
    public void Read_Stream_Success(SectionInputParameters parameters)
    {
        byte[] arr = CreateByteArray(parameters.InputLength * sizeof(T));
        var ms = new MemoryStream(arr, false);
        var result = FlipEndiannessIfNecessary(Helper[parameters.StartIndex, parameters.Length, ms]);
        Assert.True(
            arr.AsSpan(parameters.StartIndex, parameters.Length * sizeof(T))
                .SequenceEqual(
                    MemoryMarshal.Cast<T, byte>(result)
                )
        );
    }

    private Span<T> FlipEndiannessIfNecessary(Span<T> value)
    {
        if (!FlipEndian)
        {
            return value;
        }
        switch (sizeof(T))
        {
            case 1:
                return value;
            case 2:
                {
                    var result = new T[value.Length];
                    BinaryPrimitives.ReverseEndianness(MemoryMarshal.Cast<T, ushort>(value), MemoryMarshal.Cast<T, ushort>((Span<T>)result));
                    return result;
                }
            case 4:
                {
                    var result = new T[value.Length];
                    BinaryPrimitives.ReverseEndianness(MemoryMarshal.Cast<T, uint>(value), MemoryMarshal.Cast<T, uint>((Span<T>)result));
                    return result;
                }
            case 8:
                {
                    var result = new T[value.Length];
                    BinaryPrimitives.ReverseEndianness(MemoryMarshal.Cast<T, ulong>(value), MemoryMarshal.Cast<T, ulong>((Span<T>)result));
                    return result;
                }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public record SectionInputParameters(int InputLength, int StartIndex, int Length);
}

public class Processor_EncodingHelpers_U8Array_Buf
    : Processor_EncodingHelpers_NumberArray<byte>
{
    protected override BaseUnmanagedIntegerArrayHelper<byte> Helper => P.buf;
}

public class Processor_EncodingHelpers_U8Array
    : Processor_EncodingHelpers_NumberArray<byte>
{
    protected override BaseUnmanagedIntegerArrayHelper<byte> Helper => P.u1a;
}

public class Processor_EncodingHelpers_S8Array
    : Processor_EncodingHelpers_NumberArray<sbyte>
{
    protected override BaseUnmanagedIntegerArrayHelper<sbyte> Helper => P.i1a;
}

public class Processor_EncodingHelpers_U16BArray
    : Processor_EncodingHelpers_NumberArray<ushort>
{
    protected override bool IsLittleEndian => false;
    protected override BaseUnmanagedIntegerArrayHelper<ushort> Helper => P.u2ba;
}

public class Processor_EncodingHelpers_U16LArray
    : Processor_EncodingHelpers_NumberArray<ushort>
{
    protected override BaseUnmanagedIntegerArrayHelper<ushort> Helper => P.u2la;
}

public class Processor_EncodingHelpers_S16BArray
    : Processor_EncodingHelpers_NumberArray<short>
{
    protected override bool IsLittleEndian => false;
    protected override BaseUnmanagedIntegerArrayHelper<short> Helper => P.i2ba;
}

public class Processor_EncodingHelpers_S16LArray
    : Processor_EncodingHelpers_NumberArray<short>
{
    protected override BaseUnmanagedIntegerArrayHelper<short> Helper => P.i2la;
}

public class Processor_EncodingHelpers_U32BArray
    : Processor_EncodingHelpers_NumberArray<uint>
{
    protected override bool IsLittleEndian => false;
    protected override BaseUnmanagedIntegerArrayHelper<uint> Helper => P.u4ba;
}

public class Processor_EncodingHelpers_U32LArray
    : Processor_EncodingHelpers_NumberArray<uint>
{
    protected override BaseUnmanagedIntegerArrayHelper<uint> Helper => P.u4la;
}

public class Processor_EncodingHelpers_S32BArray
    : Processor_EncodingHelpers_NumberArray<int>
{
    protected override bool IsLittleEndian => false;
    protected override BaseUnmanagedIntegerArrayHelper<int> Helper => P.i4ba;
}

public class Processor_EncodingHelpers_S32LArray
    : Processor_EncodingHelpers_NumberArray<int>
{
    protected override BaseUnmanagedIntegerArrayHelper<int> Helper => P.i4la;
}

public class Processor_EncodingHelpers_U64BArray
    : Processor_EncodingHelpers_NumberArray<ulong>
{
    protected override bool IsLittleEndian => false;
    protected override BaseUnmanagedIntegerArrayHelper<ulong> Helper => P.u8ba;
}

public class Processor_EncodingHelpers_U64LArray
    : Processor_EncodingHelpers_NumberArray<ulong>
{
    protected override BaseUnmanagedIntegerArrayHelper<ulong> Helper => P.u8la;
}

public class Processor_EncodingHelpers_S64BArray
    : Processor_EncodingHelpers_NumberArray<long>
{
    protected override bool IsLittleEndian => false;
    protected override BaseUnmanagedIntegerArrayHelper<long> Helper => P.i8ba;
}

public class Processor_EncodingHelpers_S64LArray
    : Processor_EncodingHelpers_NumberArray<long>
{
    protected override BaseUnmanagedIntegerArrayHelper<long> Helper => P.i8la;
}
