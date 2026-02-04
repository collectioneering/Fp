using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using Fp.Tests.Utility;
using NUnit.Framework;

namespace Fp.Tests;

public class Processor_Bitwise_Or : ProcessorTestBase
{
    private const byte OrByte = 0xd5;

    [Test]
    public void SingleByteApplyOrArm_LargeBuffer_MatchesExpected()
    {
        if (!AdvSimd.IsSupported) Assert.Ignore("AdvSimd intrinsics not supported");

        Span<byte> arr = new byte[1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyOrAdvSimd(arr, OrByte);
        Processor.ApplyOrFallback(arr2, OrByte);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

    [Test]
    public void SingleByteApplyOrArm_Misaligned_MatchesExpected()
    {
        if (!AdvSimd.IsSupported) Assert.Ignore("AdvSimd intrinsics not supported");

        // Cut somewhere in 0..31 for misalignment
        Span<byte> arr = MemoryMarshal.Cast<int, byte>((Span<int>)new int[(1097 + sizeof(int) - 1) / sizeof(int)])[14..1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyOrAdvSimd(arr, OrByte);
        Processor.ApplyOrFallback(arr2, OrByte);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

    [Test]
    public void SingleByteApplyOrSse2_LargeBuffer_MatchesExpected()
    {
        if (!Sse2.IsSupported) Assert.Ignore("Sse2 intrinsics not supported");

        Span<byte> arr = new byte[1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);


        Processor.ApplyOrSse2(arr, OrByte);
        Processor.ApplyOrFallback(arr2, OrByte);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

    [Test]
    public void SingleByteApplyOrSse2_Misaligned_MatchesExpected()
    {
        if (!Sse2.IsSupported) Assert.Ignore("Sse2 intrinsics not supported");

        // Cut somewhere in 0..31 for misalignment
        Span<byte> arr = MemoryMarshal.Cast<int, byte>((Span<int>)new int[(1097 + sizeof(int) - 1) / sizeof(int)])[14..1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyOrSse2(arr, OrByte);
        Processor.ApplyOrFallback(arr2, OrByte);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

    [Test]
    public void SingleByteApplyOrAvx2_LargeBuffer_MatchesExpected()
    {
        if (!Avx2.IsSupported) Assert.Ignore("Avx2 intrinsics not supported");

        Span<byte> arr = new byte[1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);


        Processor.ApplyOrAvx2(arr, OrByte);
        Processor.ApplyOrFallback(arr2, OrByte);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

    [Test]
    public void SingleByteApplyOrAvx2_Misaligned_MatchesExpected()
    {
        if (!Avx2.IsSupported) Assert.Ignore("Avx2 intrinsics not supported");

        // Cut somewhere in 0..31 for misalignment
        Span<byte> arr = MemoryMarshal.Cast<int, byte>((Span<int>)new int[(1097 + sizeof(int) - 1) / sizeof(int)])[14..1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyOrAvx2(arr, OrByte);
        Processor.ApplyOrFallback(arr2, OrByte);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

    [Test]
    public void SingleByteApplyOr_LargeBuffer_MatchesExpected()
    {
        if (!Vector.IsHardwareAccelerated) Assert.Ignore("Hardware vector acceleration not supported");
        Span<byte> arr = new byte[1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);


        Processor.ApplyOrVectorized(arr, OrByte);
        Processor.ApplyOrFallback(arr2, OrByte);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

    [Test]
    public void SingleByteApplyOr_Misaligned_MatchesExpected()
    {
        if (!Vector.IsHardwareAccelerated) Assert.Ignore("Hardware vector acceleration not supported");
        // Cut somewhere in 0..31 for misalignment
        Span<byte> arr = MemoryMarshal.Cast<int, byte>((Span<int>)new int[(1097 + sizeof(int) - 1) / sizeof(int)])[14..1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyOrVectorized(arr, OrByte);
        Processor.ApplyOrFallback(arr2, OrByte);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

    [Test]
    public void BufferApplyOr_SmallBufferTruncate_MatchesExpected()
    {
        if (!Vector.IsHardwareAccelerated) Assert.Ignore("Hardware vector acceleration not supported");
        Span<byte> arr = new byte[91];
        Random.Shared.NextBytes(arr);
        Span<byte> orArr = new byte[1843];
        Random.Shared.NextBytes(orArr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyOrVectorized(arr, orArr, SequenceBehaviour.Truncate);
        Processor.ApplyOrFallback(arr2, orArr, SequenceBehaviour.Truncate);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

    [Test]
    public void BufferApplyOr_LargeBufferTruncate_MatchesExpected()
    {
        if (!Vector.IsHardwareAccelerated) Assert.Ignore("Hardware vector acceleration not supported");
        Span<byte> arr = new byte[1097];
        Random.Shared.NextBytes(arr);
        Span<byte> orArr = new byte[53];
        Random.Shared.NextBytes(orArr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyOrVectorized(arr, orArr, SequenceBehaviour.Truncate);
        Processor.ApplyOrFallback(arr2, orArr, SequenceBehaviour.Truncate);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

    [Test]
    public void BufferApplyOr_SmallBufferRepeat_MatchesExpected()
    {
        if (!Vector.IsHardwareAccelerated) Assert.Ignore("Hardware vector acceleration not supported");
        Span<byte> arr = new byte[91];
        Random.Shared.NextBytes(arr);
        Span<byte> orArr = new byte[1843];
        Random.Shared.NextBytes(orArr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyOrVectorized(arr, orArr, SequenceBehaviour.Repeat);
        Processor.ApplyOrFallback(arr2, orArr, SequenceBehaviour.Repeat);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

    [Test]
    public void BufferApplyOr_LargeBufferRepeat_MatchesExpected()
    {
        if (!Vector.IsHardwareAccelerated) Assert.Ignore("Hardware vector acceleration not supported");
        Span<byte> arr = new byte[1097];
        Random.Shared.NextBytes(arr);
        Span<byte> orArr = new byte[53];
        Random.Shared.NextBytes(orArr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyOrVectorized(arr, orArr, SequenceBehaviour.Repeat);
        Processor.ApplyOrFallback(arr2, orArr, SequenceBehaviour.Repeat);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

    [Test]
    public void BufferApplyOr_EmptyBuffer_Noop()
    {
        if (!Vector.IsHardwareAccelerated) Assert.Ignore("Hardware vector acceleration not supported");
        Span<byte> orArr = new byte[53];
        Random.Shared.NextBytes(orArr);

        Processor.ApplyOrVectorized(Span<byte>.Empty, orArr, SequenceBehaviour.Repeat);
        Processor.ApplyOrVectorized(Span<byte>.Empty, orArr, SequenceBehaviour.Truncate);
        Processor.ApplyOrFallback(Span<byte>.Empty, orArr, SequenceBehaviour.Repeat);
        Processor.ApplyOrFallback(Span<byte>.Empty, orArr, SequenceBehaviour.Truncate);
    }

    [Test]
    public void BufferApplyOr_EmptyPattern_Noop()
    {
        if (!Vector.IsHardwareAccelerated) Assert.Ignore("Hardware vector acceleration not supported");
        Span<byte> arr = new byte[1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyOrVectorized(arr, ReadOnlySpan<byte>.Empty, SequenceBehaviour.Repeat);
        Assert.That(arr.SequenceEqual(arr2), Is.True);
        Processor.ApplyOrFallback(arr, ReadOnlySpan<byte>.Empty, SequenceBehaviour.Repeat);
        Assert.That(arr.SequenceEqual(arr2), Is.True);
        Processor.ApplyOrVectorized(arr, ReadOnlySpan<byte>.Empty, SequenceBehaviour.Truncate);
        Assert.That(arr.SequenceEqual(arr2), Is.True);
        Processor.ApplyOrFallback(arr, ReadOnlySpan<byte>.Empty, SequenceBehaviour.Truncate);
        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }
}
