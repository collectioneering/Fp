using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using Fp.Tests.Utility;
using NUnit.Framework;

namespace Fp.Tests;

public class Processor_Bitwise_And : ProcessorTestBase
{
    private const byte AndByte = 0xd5;

    [Test]
    public void SingleByteApplyAndArm_LargeBuffer_MatchesExpected()
    {
        if (!AdvSimd.IsSupported) Assert.Ignore("AdvSimd intrinsics not supported");

        Span<byte> arr = new byte[1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyAndAdvSimd(arr, AndByte);
        Processor.ApplyAndFallback(arr2, AndByte);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

    [Test]
    public void SingleByteApplyAndArm_Misaligned_MatchesExpected()
    {
        if (!AdvSimd.IsSupported) Assert.Ignore("AdvSimd intrinsics not supported");

        // Cut somewhere in 0..31 for misalignment
        Span<byte> arr = MemoryMarshal.Cast<int, byte>((Span<int>)new int[(1097 + sizeof(int) - 1) / sizeof(int)])[14..1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyAndAdvSimd(arr, AndByte);
        Processor.ApplyAndFallback(arr2, AndByte);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

    [Test]
    public void SingleByteApplyAndSse2_LargeBuffer_MatchesExpected()
    {
        if (!Sse2.IsSupported) Assert.Ignore("Sse2 intrinsics not supported");

        Span<byte> arr = new byte[1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);


        Processor.ApplyAndSse2(arr, AndByte);
        Processor.ApplyAndFallback(arr2, AndByte);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

    [Test]
    public void SingleByteApplyAndSse2_Misaligned_MatchesExpected()
    {
        if (!Sse2.IsSupported) Assert.Ignore("Sse2 intrinsics not supported");

        // Cut somewhere in 0..31 for misalignment
        Span<byte> arr = MemoryMarshal.Cast<int, byte>((Span<int>)new int[(1097 + sizeof(int) - 1) / sizeof(int)])[14..1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyAndSse2(arr, AndByte);
        Processor.ApplyAndFallback(arr2, AndByte);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

    [Test]
    public void SingleByteApplyAndAvx2_LargeBuffer_MatchesExpected()
    {
        if (!Avx2.IsSupported) Assert.Ignore("Avx2 intrinsics not supported");

        Span<byte> arr = new byte[1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);


        Processor.ApplyAndAvx2(arr, AndByte);
        Processor.ApplyAndFallback(arr2, AndByte);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

    [Test]
    public void SingleByteApplyAndAvx2_Misaligned_MatchesExpected()
    {
        if (!Avx2.IsSupported) Assert.Ignore("Avx2 intrinsics not supported");

        // Cut somewhere in 0..31 for misalignment
        Span<byte> arr = MemoryMarshal.Cast<int, byte>((Span<int>)new int[(1097 + sizeof(int) - 1) / sizeof(int)])[14..1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyAndAvx2(arr, AndByte);
        Processor.ApplyAndFallback(arr2, AndByte);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

    [Test]
    public void SingleByteApplyAnd_LargeBuffer_MatchesExpected()
    {
        if (!Vector.IsHardwareAccelerated) Assert.Ignore("Hardware vector acceleration not supported");
        Span<byte> arr = new byte[1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);


        Processor.ApplyAndVectorized(arr, AndByte);
        Processor.ApplyAndFallback(arr2, AndByte);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

    [Test]
    public void SingleByteApplyAnd_Misaligned_MatchesExpected()
    {
        if (!Vector.IsHardwareAccelerated) Assert.Ignore("Hardware vector acceleration not supported");
        // Cut somewhere in 0..31 for misalignment
        Span<byte> arr = MemoryMarshal.Cast<int, byte>((Span<int>)new int[(1097 + sizeof(int) - 1) / sizeof(int)])[14..1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyAndVectorized(arr, AndByte);
        Processor.ApplyAndFallback(arr2, AndByte);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

    [Test]
    public void BufferApplyAnd_SmallBufferTruncate_MatchesExpected()
    {
        if (!Vector.IsHardwareAccelerated) Assert.Ignore("Hardware vector acceleration not supported");
        Span<byte> arr = new byte[91];
        Random.Shared.NextBytes(arr);
        Span<byte> andArr = new byte[1843];
        Random.Shared.NextBytes(andArr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyAndVectorized(arr, andArr, SequenceBehaviour.Truncate);
        Processor.ApplyAndFallback(arr2, andArr, SequenceBehaviour.Truncate);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

    [Test]
    public void BufferApplyAnd_LargeBufferTruncate_MatchesExpected()
    {
        if (!Vector.IsHardwareAccelerated) Assert.Ignore("Hardware vector acceleration not supported");
        Span<byte> arr = new byte[1097];
        Random.Shared.NextBytes(arr);
        Span<byte> andArr = new byte[53];
        Random.Shared.NextBytes(andArr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyAndVectorized(arr, andArr, SequenceBehaviour.Truncate);
        Processor.ApplyAndFallback(arr2, andArr, SequenceBehaviour.Truncate);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

    [Test]
    public void BufferApplyAnd_SmallBufferRepeat_MatchesExpected()
    {
        if (!Vector.IsHardwareAccelerated) Assert.Ignore("Hardware vector acceleration not supported");
        Span<byte> arr = new byte[91];
        Random.Shared.NextBytes(arr);
        Span<byte> andArr = new byte[1843];
        Random.Shared.NextBytes(andArr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyAndVectorized(arr, andArr, SequenceBehaviour.Repeat);
        Processor.ApplyAndFallback(arr2, andArr, SequenceBehaviour.Repeat);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

    [Test]
    public void BufferApplyAnd_LargeBufferRepeat_MatchesExpected()
    {
        if (!Vector.IsHardwareAccelerated) Assert.Ignore("Hardware vector acceleration not supported");
        Span<byte> arr = new byte[1097];
        Random.Shared.NextBytes(arr);
        Span<byte> andArr = new byte[53];
        Random.Shared.NextBytes(andArr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyAndVectorized(arr, andArr, SequenceBehaviour.Repeat);
        Processor.ApplyAndFallback(arr2, andArr, SequenceBehaviour.Repeat);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

    [Test]
    public void BufferApplyAnd_EmptyBuffer_Noop()
    {
        if (!Vector.IsHardwareAccelerated) Assert.Ignore("Hardware vector acceleration not supported");
        Span<byte> andArr = new byte[53];
        Random.Shared.NextBytes(andArr);

        Processor.ApplyAndVectorized(Span<byte>.Empty, andArr, SequenceBehaviour.Repeat);
        Processor.ApplyAndVectorized(Span<byte>.Empty, andArr, SequenceBehaviour.Truncate);
        Processor.ApplyAndFallback(Span<byte>.Empty, andArr, SequenceBehaviour.Repeat);
        Processor.ApplyAndFallback(Span<byte>.Empty, andArr, SequenceBehaviour.Truncate);
    }

    [Test]
    public void BufferApplyAnd_EmptyPattern_Noop()
    {
        if (!Vector.IsHardwareAccelerated) Assert.Ignore("Hardware vector acceleration not supported");
        Span<byte> arr = new byte[1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyAndVectorized(arr, ReadOnlySpan<byte>.Empty, SequenceBehaviour.Repeat);
        Assert.That(arr.SequenceEqual(arr2), Is.True);
        Processor.ApplyAndFallback(arr, ReadOnlySpan<byte>.Empty, SequenceBehaviour.Repeat);
        Assert.That(arr.SequenceEqual(arr2), Is.True);
        Processor.ApplyAndVectorized(arr, ReadOnlySpan<byte>.Empty, SequenceBehaviour.Truncate);
        Assert.That(arr.SequenceEqual(arr2), Is.True);
        Processor.ApplyAndFallback(arr, ReadOnlySpan<byte>.Empty, SequenceBehaviour.Truncate);
        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }
}
