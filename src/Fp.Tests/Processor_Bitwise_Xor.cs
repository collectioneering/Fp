using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using Fp.Tests.Utility;
using NUnit.Framework;

namespace Fp.Tests;

public class Processor_Bitwise_Xor : ProcessorTestBase
{
    private const byte XorByte = 0xd5;

    [Test]
    public void SingleByteApplyXorArm_LargeBuffer_MatchesExpected()
    {
        if (!AdvSimd.IsSupported) Assert.Ignore("AdvSimd intrinsics not supported");

        Span<byte> arr = new byte[1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyXorAdvSimd(arr, XorByte);
        Processor.ApplyXorFallback(arr2, XorByte);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

    [Test]
    public void SingleByteApplyXorArm_Misaligned_MatchesExpected()
    {
        if (!AdvSimd.IsSupported) Assert.Ignore("AdvSimd intrinsics not supported");

        // Cut somewhere in 0..31 for misalignment
        Span<byte> arr = MemoryMarshal.Cast<int, byte>((Span<int>)new int[(1097 + sizeof(int) - 1) / sizeof(int)])[14..1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyXorAdvSimd(arr, XorByte);
        Processor.ApplyXorFallback(arr2, XorByte);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

    [Test]
    public void SingleByteApplyXorSse2_LargeBuffer_MatchesExpected()
    {
        if (!Sse2.IsSupported) Assert.Ignore("Sse2 intrinsics not supported");

        Span<byte> arr = new byte[1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);


        Processor.ApplyXorSse2(arr, XorByte);
        Processor.ApplyXorFallback(arr2, XorByte);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

    [Test]
    public void SingleByteApplyXorSse2_Misaligned_MatchesExpected()
    {
        if (!Sse2.IsSupported) Assert.Ignore("Sse2 intrinsics not supported");

        // Cut somewhere in 0..31 for misalignment
        Span<byte> arr = MemoryMarshal.Cast<int, byte>((Span<int>)new int[(1097 + sizeof(int) - 1) / sizeof(int)])[14..1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyXorSse2(arr, XorByte);
        Processor.ApplyXorFallback(arr2, XorByte);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

    [Test]
    public void SingleByteApplyXorAvx2_LargeBuffer_MatchesExpected()
    {
        if (!Avx2.IsSupported) Assert.Ignore("Avx2 intrinsics not supported");

        Span<byte> arr = new byte[1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);


        Processor.ApplyXorAvx2(arr, XorByte);
        Processor.ApplyXorFallback(arr2, XorByte);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

    [Test]
    public void SingleByteApplyXorAvx2_Misaligned_MatchesExpected()
    {
        if (!Avx2.IsSupported) Assert.Ignore("Avx2 intrinsics not supported");

        // Cut somewhere in 0..31 for misalignment
        Span<byte> arr = MemoryMarshal.Cast<int, byte>((Span<int>)new int[(1097 + sizeof(int) - 1) / sizeof(int)])[14..1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyXorAvx2(arr, XorByte);
        Processor.ApplyXorFallback(arr2, XorByte);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

    [Test]
    public void SingleByteApplyXor_LargeBuffer_MatchesExpected()
    {
        if (!Vector.IsHardwareAccelerated) Assert.Ignore("Hardware vector acceleration not supported");
        Span<byte> arr = new byte[1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);


        Processor.ApplyXorVectorized(arr, XorByte);
        Processor.ApplyXorFallback(arr2, XorByte);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

    [Test]
    public void SingleByteApplyXor_Misaligned_MatchesExpected()
    {
        if (!Vector.IsHardwareAccelerated) Assert.Ignore("Hardware vector acceleration not supported");
        // Cut somewhere in 0..31 for misalignment
        Span<byte> arr = MemoryMarshal.Cast<int, byte>((Span<int>)new int[(1097 + sizeof(int) - 1) / sizeof(int)])[14..1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyXorVectorized(arr, XorByte);
        Processor.ApplyXorFallback(arr2, XorByte);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

    [Test]
    public void BufferApplyXor_SmallBufferTruncate_MatchesExpected()
    {
        if (!Vector.IsHardwareAccelerated) Assert.Ignore("Hardware vector acceleration not supported");
        Span<byte> arr = new byte[91];
        Random.Shared.NextBytes(arr);
        Span<byte> xorArr = new byte[1843];
        Random.Shared.NextBytes(xorArr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyXorVectorized(arr, xorArr, SequenceBehaviour.Truncate);
        Processor.ApplyXorFallback(arr2, xorArr, SequenceBehaviour.Truncate);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

    [Test]
    public void BufferApplyXor_LargeBufferTruncate_MatchesExpected()
    {
        if (!Vector.IsHardwareAccelerated) Assert.Ignore("Hardware vector acceleration not supported");
        Span<byte> arr = new byte[1097];
        Random.Shared.NextBytes(arr);
        Span<byte> xorArr = new byte[53];
        Random.Shared.NextBytes(xorArr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyXorVectorized(arr, xorArr, SequenceBehaviour.Truncate);
        Processor.ApplyXorFallback(arr2, xorArr, SequenceBehaviour.Truncate);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

    [Test]
    public void BufferApplyXor_SmallBufferRepeat_MatchesExpected()
    {
        if (!Vector.IsHardwareAccelerated) Assert.Ignore("Hardware vector acceleration not supported");
        Span<byte> arr = new byte[91];
        Random.Shared.NextBytes(arr);
        Span<byte> xorArr = new byte[1843];
        Random.Shared.NextBytes(xorArr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyXorVectorized(arr, xorArr, SequenceBehaviour.Repeat);
        Processor.ApplyXorFallback(arr2, xorArr, SequenceBehaviour.Repeat);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

    [Test]
    public void BufferApplyXor_LargeBufferRepeat_MatchesExpected()
    {
        if (!Vector.IsHardwareAccelerated) Assert.Ignore("Hardware vector acceleration not supported");
        Span<byte> arr = new byte[1097];
        Random.Shared.NextBytes(arr);
        Span<byte> xorArr = new byte[53];
        Random.Shared.NextBytes(xorArr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyXorVectorized(arr, xorArr, SequenceBehaviour.Repeat);
        Processor.ApplyXorFallback(arr2, xorArr, SequenceBehaviour.Repeat);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

    [Test]
    public void BufferApplyAnd_EmptyBuffer_Noop()
    {
        if (!Vector.IsHardwareAccelerated) Assert.Ignore("Hardware vector acceleration not supported");
        Span<byte> xorArr = new byte[53];
        Random.Shared.NextBytes(xorArr);

        Processor.ApplyXorVectorized(Span<byte>.Empty, xorArr, SequenceBehaviour.Repeat);
        Processor.ApplyXorVectorized(Span<byte>.Empty, xorArr, SequenceBehaviour.Truncate);
        Processor.ApplyXorFallback(Span<byte>.Empty, xorArr, SequenceBehaviour.Repeat);
        Processor.ApplyXorFallback(Span<byte>.Empty, xorArr, SequenceBehaviour.Truncate);
    }

    [Test]
    public void BufferApplyXor_EmptyPattern_Noop()
    {
        if (!Vector.IsHardwareAccelerated) Assert.Ignore("Hardware vector acceleration not supported");
        Span<byte> arr = new byte[1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyXorVectorized(arr, ReadOnlySpan<byte>.Empty, SequenceBehaviour.Repeat);
        Assert.That(arr.SequenceEqual(arr2), Is.True);
        Processor.ApplyXorFallback(arr, ReadOnlySpan<byte>.Empty, SequenceBehaviour.Repeat);
        Assert.That(arr.SequenceEqual(arr2), Is.True);
        Processor.ApplyXorVectorized(arr, ReadOnlySpan<byte>.Empty, SequenceBehaviour.Truncate);
        Assert.That(arr.SequenceEqual(arr2), Is.True);
        Processor.ApplyXorFallback(arr, ReadOnlySpan<byte>.Empty, SequenceBehaviour.Truncate);
        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }
}
