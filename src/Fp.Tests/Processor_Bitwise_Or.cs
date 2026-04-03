using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using Fp.Tests.Utility;

namespace Fp.Tests;

public class Processor_Bitwise_Or : ProcessorTestBase
{
    private const byte OrByte = 0xd5;

    [Fact]
    public void SingleByteApplyOrArm_LargeBuffer_MatchesExpected()
    {
        Assert.SkipUnless(AdvSimd.IsSupported, "AdvSimd intrinsics not supported");

        Span<byte> arr = new byte[1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyOrAdvSimd(arr, OrByte);
        Processor.ApplyOrFallback(arr2, OrByte);

        Assert.True(arr.SequenceEqual(arr2));
    }

    [Fact]
    public void SingleByteApplyOrArm_Misaligned_MatchesExpected()
    {
        Assert.SkipUnless(AdvSimd.IsSupported, "AdvSimd intrinsics not supported");

        // Cut somewhere in 0..31 for misalignment
        Span<byte> arr = MemoryMarshal.Cast<int, byte>((Span<int>)new int[(1097 + sizeof(int) - 1) / sizeof(int)])[14..1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyOrAdvSimd(arr, OrByte);
        Processor.ApplyOrFallback(arr2, OrByte);

        Assert.True(arr.SequenceEqual(arr2));
    }

    [Fact]
    public void SingleByteApplyOrSse2_LargeBuffer_MatchesExpected()
    {
        Assert.SkipUnless(Sse2.IsSupported, "Sse2 intrinsics not supported");

        Span<byte> arr = new byte[1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);


        Processor.ApplyOrSse2(arr, OrByte);
        Processor.ApplyOrFallback(arr2, OrByte);

        Assert.True(arr.SequenceEqual(arr2));
    }

    [Fact]
    public void SingleByteApplyOrSse2_Misaligned_MatchesExpected()
    {
        Assert.SkipUnless(Sse2.IsSupported, "Sse2 intrinsics not supported");

        // Cut somewhere in 0..31 for misalignment
        Span<byte> arr = MemoryMarshal.Cast<int, byte>((Span<int>)new int[(1097 + sizeof(int) - 1) / sizeof(int)])[14..1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyOrSse2(arr, OrByte);
        Processor.ApplyOrFallback(arr2, OrByte);

        Assert.True(arr.SequenceEqual(arr2));
    }

    [Fact]
    public void SingleByteApplyOrAvx2_LargeBuffer_MatchesExpected()
    {
        Assert.SkipUnless(Avx2.IsSupported, "Avx2 intrinsics not supported");

        Span<byte> arr = new byte[1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);


        Processor.ApplyOrAvx2(arr, OrByte);
        Processor.ApplyOrFallback(arr2, OrByte);

        Assert.True(arr.SequenceEqual(arr2));
    }

    [Fact]
    public void SingleByteApplyOrAvx2_Misaligned_MatchesExpected()
    {
        Assert.SkipUnless(Avx2.IsSupported, "Avx2 intrinsics not supported");

        // Cut somewhere in 0..31 for misalignment
        Span<byte> arr = MemoryMarshal.Cast<int, byte>((Span<int>)new int[(1097 + sizeof(int) - 1) / sizeof(int)])[14..1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyOrAvx2(arr, OrByte);
        Processor.ApplyOrFallback(arr2, OrByte);

        Assert.True(arr.SequenceEqual(arr2));
    }

    [Fact]
    public void SingleByteApplyOr_LargeBuffer_MatchesExpected()
    {
        Assert.SkipUnless(Vector.IsHardwareAccelerated, "Hardware vector acceleration not supported");
        Span<byte> arr = new byte[1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);


        Processor.ApplyOrVectorized(arr, OrByte);
        Processor.ApplyOrFallback(arr2, OrByte);

        Assert.True(arr.SequenceEqual(arr2));
    }

    [Fact]
    public void SingleByteApplyOr_Misaligned_MatchesExpected()
    {
        Assert.SkipUnless(Vector.IsHardwareAccelerated, "Hardware vector acceleration not supported");
        // Cut somewhere in 0..31 for misalignment
        Span<byte> arr = MemoryMarshal.Cast<int, byte>((Span<int>)new int[(1097 + sizeof(int) - 1) / sizeof(int)])[14..1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyOrVectorized(arr, OrByte);
        Processor.ApplyOrFallback(arr2, OrByte);

        Assert.True(arr.SequenceEqual(arr2));
    }

    [Fact]
    public void BufferApplyOr_SmallBufferTruncate_MatchesExpected()
    {
        Assert.SkipUnless(Vector.IsHardwareAccelerated, "Hardware vector acceleration not supported");
        Span<byte> arr = new byte[91];
        Random.Shared.NextBytes(arr);
        Span<byte> orArr = new byte[1843];
        Random.Shared.NextBytes(orArr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyOrVectorized(arr, orArr, SequenceBehaviour.Truncate);
        Processor.ApplyOrFallback(arr2, orArr, SequenceBehaviour.Truncate);

        Assert.True(arr.SequenceEqual(arr2));
    }

    [Fact]
    public void BufferApplyOr_LargeBufferTruncate_MatchesExpected()
    {
        Assert.SkipUnless(Vector.IsHardwareAccelerated, "Hardware vector acceleration not supported");
        Span<byte> arr = new byte[1097];
        Random.Shared.NextBytes(arr);
        Span<byte> orArr = new byte[53];
        Random.Shared.NextBytes(orArr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyOrVectorized(arr, orArr, SequenceBehaviour.Truncate);
        Processor.ApplyOrFallback(arr2, orArr, SequenceBehaviour.Truncate);

        Assert.True(arr.SequenceEqual(arr2));
    }

    [Fact]
    public void BufferApplyOr_SmallBufferRepeat_MatchesExpected()
    {
        Assert.SkipUnless(Vector.IsHardwareAccelerated, "Hardware vector acceleration not supported");
        Span<byte> arr = new byte[91];
        Random.Shared.NextBytes(arr);
        Span<byte> orArr = new byte[1843];
        Random.Shared.NextBytes(orArr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyOrVectorized(arr, orArr, SequenceBehaviour.Repeat);
        Processor.ApplyOrFallback(arr2, orArr, SequenceBehaviour.Repeat);

        Assert.True(arr.SequenceEqual(arr2));
    }

    [Fact]
    public void BufferApplyOr_LargeBufferRepeat_MatchesExpected()
    {
        Assert.SkipUnless(Vector.IsHardwareAccelerated, "Hardware vector acceleration not supported");
        Span<byte> arr = new byte[1097];
        Random.Shared.NextBytes(arr);
        Span<byte> orArr = new byte[53];
        Random.Shared.NextBytes(orArr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyOrVectorized(arr, orArr, SequenceBehaviour.Repeat);
        Processor.ApplyOrFallback(arr2, orArr, SequenceBehaviour.Repeat);

        Assert.True(arr.SequenceEqual(arr2));
    }

    [Fact]
    public void BufferApplyOr_EmptyBuffer_Noop()
    {
        Assert.SkipUnless(Vector.IsHardwareAccelerated, "Hardware vector acceleration not supported");
        Span<byte> orArr = new byte[53];
        Random.Shared.NextBytes(orArr);

        Processor.ApplyOrVectorized(Span<byte>.Empty, orArr, SequenceBehaviour.Repeat);
        Processor.ApplyOrVectorized(Span<byte>.Empty, orArr, SequenceBehaviour.Truncate);
        Processor.ApplyOrFallback(Span<byte>.Empty, orArr, SequenceBehaviour.Repeat);
        Processor.ApplyOrFallback(Span<byte>.Empty, orArr, SequenceBehaviour.Truncate);
    }

    [Fact]
    public void BufferApplyOr_EmptyPattern_Noop()
    {
        Assert.SkipUnless(Vector.IsHardwareAccelerated, "Hardware vector acceleration not supported");
        Span<byte> arr = new byte[1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyOrVectorized(arr, ReadOnlySpan<byte>.Empty, SequenceBehaviour.Repeat);
        Assert.True(arr.SequenceEqual(arr2));
        Processor.ApplyOrFallback(arr, ReadOnlySpan<byte>.Empty, SequenceBehaviour.Repeat);
        Assert.True(arr.SequenceEqual(arr2));
        Processor.ApplyOrVectorized(arr, ReadOnlySpan<byte>.Empty, SequenceBehaviour.Truncate);
        Assert.True(arr.SequenceEqual(arr2));
        Processor.ApplyOrFallback(arr, ReadOnlySpan<byte>.Empty, SequenceBehaviour.Truncate);
        Assert.True(arr.SequenceEqual(arr2));
    }
}
