using System;
using System.Collections;

namespace Fp.Tests;

// BitArray is little endian bit order
public class BitUtil_SkipBits
{
    private static readonly byte[] s_arr =
    {
        0b0011_0000, //
        0b0000_0000, //
        0b1100_1111, //
        0b1111_1111, //
        0b1111_1111, //
        0b0000_0000, //
        0b1111_1111, //
        0b0000_0000 //
    };

    [Fact]
    public void SkipBits_BitArray_Functions()
    {
        BitArray ba = new BitArray(s_arr.SliceAlloc(0, 32 >> 3));
        int i = 0;
        ba.SkipBits(ref i, false);
        Assert.Equal(4, i);
        ba.SkipBits(ref i, true);
        Assert.Equal(6, i);
        ba.SkipBits(ref i, false);
        Assert.Equal(16, i);
        ba.SkipBits(ref i, true);
        Assert.Equal(20, i);
        ba.SkipBits(ref i, false);
        Assert.Equal(22, i);
        ba.SkipBits(ref i, true);
        Assert.Equal(32, i);
    }

    [Fact]
    public void SkipBits_ByteArrayLittleEndian_Functions()
    {
        int i = 0;
        ReadOnlySpan<byte> ba = s_arr;
        ba.SkipBits(32, ref i, false, bigEndian: false);
        Assert.Equal(4, i);
        ba.SkipBits(32, ref i, true, bigEndian: false);
        Assert.Equal(6, i);
        ba.SkipBits(32, ref i, false, bigEndian: false);
        Assert.Equal(16, i);
        ba.SkipBits(32, ref i, true, bigEndian: false);
        Assert.Equal(20, i);
        ba.SkipBits(32, ref i, false, bigEndian: false);
        Assert.Equal(22, i);
        ba.SkipBits(32, ref i, true, bigEndian: false);
        Assert.Equal(32, i);
    }

    [Fact]
    public void SkipBits_ByteArrayBigEndian_Functions()
    {
        int i = 0;
        ReadOnlySpan<byte> ba = s_arr;
        ba.SkipBits(32, ref i, false, true);
        Assert.Equal(2, i);
        ba.SkipBits(32, ref i, true, true);
        Assert.Equal(4, i);
        ba.SkipBits(32, ref i, false, true);
        Assert.Equal(16, i);
        ba.SkipBits(32, ref i, true, true);
        Assert.Equal(18, i);
        ba.SkipBits(32, ref i, false, true);
        Assert.Equal(20, i);
        ba.SkipBits(32, ref i, true, true);
        Assert.Equal(32, i);
    }

    [Fact]
    public void ConstrainedSkipBits_BitArray_Functions()
    {
        int i = 0;
        BitArray ba = new BitArray(s_arr.SliceAlloc(0, 32 >> 3));
        Assert.True(ba.ConstrainedSkipBits(32, ref i, false));
        Assert.Equal(4, i);
        Assert.True(ba.ConstrainedSkipBits(32, ref i, true));
        Assert.Equal(6, i);
        Assert.True(ba.ConstrainedSkipBits(32, ref i, false));
        Assert.Equal(16, i);
        Assert.True(ba.ConstrainedSkipBits(32, ref i, true));
        Assert.Equal(20, i);
        Assert.True(ba.ConstrainedSkipBits(32, ref i, false));
        Assert.Equal(22, i);
        Assert.False(ba.ConstrainedSkipBits(32, ref i, true));
        Assert.Equal(31, i);
        Assert.True(ba.ConstrainedSkipBits(32, ref i, false));
        Assert.Equal(31, i);
    }

    [Fact]
    public void ConstrainedSkipBits_ByteArrayLittleEndian_Functions()
    {
        int i = 0;
        ReadOnlySpan<byte> ba = s_arr;
        Assert.True(ba.ConstrainedSkipBits(32, ref i, false, bigEndian: false));
        Assert.Equal(4, i);
        Assert.True(ba.ConstrainedSkipBits(32, ref i, true, bigEndian: false));
        Assert.Equal(6, i);
        Assert.True(ba.ConstrainedSkipBits(32, ref i, false, bigEndian: false));
        Assert.Equal(16, i);
        Assert.True(ba.ConstrainedSkipBits(32, ref i, true, bigEndian: false));
        Assert.Equal(20, i);
        Assert.True(ba.ConstrainedSkipBits(32, ref i, false, bigEndian: false));
        Assert.Equal(22, i);
        Assert.False(ba.ConstrainedSkipBits(32, ref i, true, bigEndian: false));
        Assert.Equal(31, i);
        Assert.True(ba.ConstrainedSkipBits(32, ref i, false, bigEndian: false));
        Assert.Equal(31, i);
    }

    [Fact]
    public void ConstrainedSkipBits_ByteArrayBigEndian_Functions()
    {
        int i = 0;
        ReadOnlySpan<byte> ba = s_arr;
        Assert.True(ba.ConstrainedSkipBits(32, ref i, false, true));
        Assert.Equal(2, i);
        Assert.True(ba.ConstrainedSkipBits(32, ref i, true, true));
        Assert.Equal(4, i);
        Assert.True(ba.ConstrainedSkipBits(32, ref i, false, true));
        Assert.Equal(16, i);
        Assert.True(ba.ConstrainedSkipBits(32, ref i, true, true));
        Assert.Equal(18, i);
        Assert.True(ba.ConstrainedSkipBits(32, ref i, false, true));
        Assert.Equal(20, i);
        Assert.False(ba.ConstrainedSkipBits(32, ref i, true, true));
        Assert.Equal(31, i);
        Assert.True(ba.ConstrainedSkipBits(32, ref i, false, true));
        Assert.Equal(31, i);
    }
}
