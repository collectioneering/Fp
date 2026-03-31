using System;
using System.Collections;
using NUnit.Framework;

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

    [Test]
    public void SkipBits_BitArray_Functions()
    {
        BitArray ba = new BitArray(s_arr.SliceAlloc(0, 32 >> 3));
        int i = 0;
        ba.SkipBits(ref i, false);
        Assert.That(i, Is.EqualTo(4));
        ba.SkipBits(ref i, true);
        Assert.That(i, Is.EqualTo(6));
        ba.SkipBits(ref i, false);
        Assert.That(i, Is.EqualTo(16));
        ba.SkipBits(ref i, true);
        Assert.That(i, Is.EqualTo(20));
        ba.SkipBits(ref i, false);
        Assert.That(i, Is.EqualTo(22));
        ba.SkipBits(ref i, true);
        Assert.That(i, Is.EqualTo(32));
    }

    [Test]
    public void SkipBits_ByteArrayLittleEndian_Functions()
    {
        int i = 0;
        ReadOnlySpan<byte> ba = s_arr;
        ba.SkipBits(32, ref i, false, bigEndian: false);
        Assert.That(i, Is.EqualTo(4));
        ba.SkipBits(32, ref i, true, bigEndian: false);
        Assert.That(i, Is.EqualTo(6));
        ba.SkipBits(32, ref i, false, bigEndian: false);
        Assert.That(i, Is.EqualTo(16));
        ba.SkipBits(32, ref i, true, bigEndian: false);
        Assert.That(i, Is.EqualTo(20));
        ba.SkipBits(32, ref i, false, bigEndian: false);
        Assert.That(i, Is.EqualTo(22));
        ba.SkipBits(32, ref i, true, bigEndian: false);
        Assert.That(i, Is.EqualTo(32));
    }

    [Test]
    public void SkipBits_ByteArrayBigEndian_Functions()
    {
        int i = 0;
        ReadOnlySpan<byte> ba = s_arr;
        ba.SkipBits(32, ref i, false, true);
        Assert.That(i, Is.EqualTo(2));
        ba.SkipBits(32, ref i, true, true);
        Assert.That(i, Is.EqualTo(4));
        ba.SkipBits(32, ref i, false, true);
        Assert.That(i, Is.EqualTo(16));
        ba.SkipBits(32, ref i, true, true);
        Assert.That(i, Is.EqualTo(18));
        ba.SkipBits(32, ref i, false, true);
        Assert.That(i, Is.EqualTo(20));
        ba.SkipBits(32, ref i, true, true);
        Assert.That(i, Is.EqualTo(32));
    }

    [Test]
    public void ConstrainedSkipBits_BitArray_Functions()
    {
        int i = 0;
        BitArray ba = new BitArray(s_arr.SliceAlloc(0, 32 >> 3));
        Assert.That(ba.ConstrainedSkipBits(32, ref i, false), Is.True);
        Assert.That(i, Is.EqualTo(4));
        Assert.That(ba.ConstrainedSkipBits(32, ref i, true), Is.True);
        Assert.That(i, Is.EqualTo(6));
        Assert.That(ba.ConstrainedSkipBits(32, ref i, false), Is.True);
        Assert.That(i, Is.EqualTo(16));
        Assert.That(ba.ConstrainedSkipBits(32, ref i, true), Is.True);
        Assert.That(i, Is.EqualTo(20));
        Assert.That(ba.ConstrainedSkipBits(32, ref i, false), Is.True);
        Assert.That(i, Is.EqualTo(22));
        Assert.That(ba.ConstrainedSkipBits(32, ref i, true), Is.False);
        Assert.That(i, Is.EqualTo(31));
        Assert.That(ba.ConstrainedSkipBits(32, ref i, false), Is.True);
        Assert.That(i, Is.EqualTo(31));
    }

    [Test]
    public void ConstrainedSkipBits_ByteArrayLittleEndian_Functions()
    {
        int i = 0;
        ReadOnlySpan<byte> ba = s_arr;
        Assert.That(ba.ConstrainedSkipBits(32, ref i, false, bigEndian: false), Is.True);
        Assert.That(i, Is.EqualTo(4));
        Assert.That(ba.ConstrainedSkipBits(32, ref i, true, bigEndian: false), Is.True);
        Assert.That(i, Is.EqualTo(6));
        Assert.That(ba.ConstrainedSkipBits(32, ref i, false, bigEndian: false), Is.True);
        Assert.That(i, Is.EqualTo(16));
        Assert.That(ba.ConstrainedSkipBits(32, ref i, true, bigEndian: false), Is.True);
        Assert.That(i, Is.EqualTo(20));
        Assert.That(ba.ConstrainedSkipBits(32, ref i, false, bigEndian: false), Is.True);
        Assert.That(i, Is.EqualTo(22));
        Assert.That(ba.ConstrainedSkipBits(32, ref i, true, bigEndian: false), Is.False);
        Assert.That(i, Is.EqualTo(31));
        Assert.That(ba.ConstrainedSkipBits(32, ref i, false, bigEndian: false), Is.True);
        Assert.That(i, Is.EqualTo(31));
    }

    [Test]
    public void ConstrainedSkipBits_ByteArrayBigEndian_Functions()
    {
        int i = 0;
        ReadOnlySpan<byte> ba = s_arr;
        Assert.That(ba.ConstrainedSkipBits(32, ref i, false, true), Is.True);
        Assert.That(i, Is.EqualTo(2));
        Assert.That(ba.ConstrainedSkipBits(32, ref i, true, true), Is.True);
        Assert.That(i, Is.EqualTo(4));
        Assert.That(ba.ConstrainedSkipBits(32, ref i, false, true), Is.True);
        Assert.That(i, Is.EqualTo(16));
        Assert.That(ba.ConstrainedSkipBits(32, ref i, true, true), Is.True);
        Assert.That(i, Is.EqualTo(18));
        Assert.That(ba.ConstrainedSkipBits(32, ref i, false, true), Is.True);
        Assert.That(i, Is.EqualTo(20));
        Assert.That(ba.ConstrainedSkipBits(32, ref i, true, true), Is.False);
        Assert.That(i, Is.EqualTo(31));
        Assert.That(ba.ConstrainedSkipBits(32, ref i, false, true), Is.True);
        Assert.That(i, Is.EqualTo(31));
    }
}
