using System;
using System.IO;
using Fp.Tests.Utility;

namespace Fp.Tests;

public class Processor_EncodingHelpers_U8Array_Range : ProcessorTestBase
{
    private static readonly RangeInputParameters[] s_rangeInputParameters =
    [
        new(0, ..),
        new(128 * 1024, ..),
        new(128 * 1024, (64 * 1024)..(96 * 1024)),
    ];

    public static readonly TheoryData<RangeInputParameters> TdRangeInputParameters = new(s_rangeInputParameters);

    [Theory]
    [MemberData(nameof(TdRangeInputParameters))]
    public void Read_Range_ByteArray_Success(RangeInputParameters parameters)
    {
        byte[] arr = CreateByteArray(parameters.InputLength);
        Assert.True(arr.AsSpan(parameters.ParameterRange).SequenceEqual(P.buf[arr, parameters.ParameterRange]));
    }

    [Theory]
    [MemberData(nameof(TdRangeInputParameters))]
    public void Read_Range_MemoryOfByte_Success(RangeInputParameters parameters)
    {
        byte[] arr = CreateByteArray(parameters.InputLength);
        var input = arr.AsMemory();
        Assert.True(arr.AsSpan(parameters.ParameterRange).SequenceEqual(P.buf[input, parameters.ParameterRange]));
    }

    [Theory]
    [MemberData(nameof(TdRangeInputParameters))]
    public void Read_Range_ReadOnlyMemoryOfByte_Success(RangeInputParameters parameters)
    {
        byte[] arr = CreateByteArray(parameters.InputLength);
        var input = (ReadOnlyMemory<byte>)arr.AsMemory();
        Assert.True(arr.AsSpan(parameters.ParameterRange).SequenceEqual(P.buf[input, parameters.ParameterRange]));
    }

    [Theory]
    [MemberData(nameof(TdRangeInputParameters))]
    public void Read_Range_SpanOfByte_Success(RangeInputParameters parameters)
    {
        byte[] arr = CreateByteArray(parameters.InputLength);
        var input = arr.AsSpan();
        Assert.True(arr.AsSpan(parameters.ParameterRange).SequenceEqual(P.buf[input, parameters.ParameterRange]));
    }

    [Theory]
    [MemberData(nameof(TdRangeInputParameters))]
    public void Read_Range_ReadOnlySpanOfByte_Success(RangeInputParameters parameters)
    {
        byte[] arr = CreateByteArray(parameters.InputLength);
        var input = (ReadOnlySpan<byte>)arr.AsSpan();
        Assert.True(arr.AsSpan(parameters.ParameterRange).SequenceEqual(P.buf[input, parameters.ParameterRange]));
    }

    [Theory]
    [MemberData(nameof(TdRangeInputParameters))]
    public void Read_Range_InputStream_Success(RangeInputParameters parameters)
    {
        byte[] arr = CreateByteArray(parameters.InputLength);
        var ms = new MemoryStream(arr, false);
        P.InputStream = ms;
        Assert.True(arr.AsSpan(parameters.ParameterRange).SequenceEqual(P.buf[parameters.ParameterRange]));
    }

    [Theory]
    [MemberData(nameof(TdRangeInputParameters))]
    public void Read_Range_Stream_Success(RangeInputParameters parameters)
    {
        byte[] arr = CreateByteArray(parameters.InputLength);
        var ms = new MemoryStream(arr, false);
        Assert.True(arr.AsSpan(parameters.ParameterRange).SequenceEqual(P.buf[parameters.ParameterRange, ms]));
    }

    public record RangeInputParameters(int InputLength, Range ParameterRange);
}
