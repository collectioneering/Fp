using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using Fp.Tests.Utility;

namespace Fp.Tests;

public class Processor_Bitwise_Xor : ProcessorTestBase
{
    private const byte XorByte = 0xd5;

    [SkippableFact]
    public void SingleByteApplyXorArm_LargeBuffer_MatchesExpected()
    {
        Skip.IfNot(AdvSimd.IsSupported, "AdvSimd intrinsics not supported");

        Span<byte> arr = new byte[1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyXorAdvSimd(arr, XorByte);
        Processor.ApplyXorFallback(arr2, XorByte);

        Assert.True(arr.SequenceEqual(arr2));
    }

    [SkippableFact]
    public void SingleByteApplyXorArm_Misaligned_MatchesExpected()
    {
        Skip.IfNot(AdvSimd.IsSupported, "AdvSimd intrinsics not supported");

        // Cut somewhere in 0..31 for misalignment
        Span<byte> arr = MemoryMarshal.Cast<int, byte>((Span<int>)new int[(1097 + sizeof(int) - 1) / sizeof(int)])[14..1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyXorAdvSimd(arr, XorByte);
        Processor.ApplyXorFallback(arr2, XorByte);

        Assert.True(arr.SequenceEqual(arr2));
    }

    [SkippableFact]
    public void SingleByteApplyXorSse2_LargeBuffer_MatchesExpected()
    {
        Skip.IfNot(Sse2.IsSupported, "Sse2 intrinsics not supported");

        Span<byte> arr = new byte[1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);


        Processor.ApplyXorSse2(arr, XorByte);
        Processor.ApplyXorFallback(arr2, XorByte);

        Assert.True(arr.SequenceEqual(arr2));
    }

    [SkippableFact]
    public void SingleByteApplyXorSse2_Misaligned_MatchesExpected()
    {
        Skip.IfNot(Sse2.IsSupported, "Sse2 intrinsics not supported");

        // Cut somewhere in 0..31 for misalignment
        Span<byte> arr = MemoryMarshal.Cast<int, byte>((Span<int>)new int[(1097 + sizeof(int) - 1) / sizeof(int)])[14..1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyXorSse2(arr, XorByte);
        Processor.ApplyXorFallback(arr2, XorByte);

        Assert.True(arr.SequenceEqual(arr2));
    }

    [SkippableFact]
    public void SingleByteApplyXorAvx2_LargeBuffer_MatchesExpected()
    {
        Skip.IfNot(Avx2.IsSupported, "Avx2 intrinsics not supported");

        Span<byte> arr = new byte[1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);


        Processor.ApplyXorAvx2(arr, XorByte);
        Processor.ApplyXorFallback(arr2, XorByte);

        Assert.True(arr.SequenceEqual(arr2));
    }

    [SkippableFact]
    public void SingleByteApplyXorAvx2_Misaligned_MatchesExpected()
    {
        Skip.IfNot(Avx2.IsSupported, "Avx2 intrinsics not supported");

        // Cut somewhere in 0..31 for misalignment
        Span<byte> arr = MemoryMarshal.Cast<int, byte>((Span<int>)new int[(1097 + sizeof(int) - 1) / sizeof(int)])[14..1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyXorAvx2(arr, XorByte);
        Processor.ApplyXorFallback(arr2, XorByte);

        Assert.True(arr.SequenceEqual(arr2));
    }

    [SkippableFact]
    public void SingleByteApplyXor_LargeBuffer_MatchesExpected()
    {
        Skip.IfNot(Vector.IsHardwareAccelerated, "Hardware vector acceleration not supported");
        Span<byte> arr = new byte[1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);


        Processor.ApplyXorVectorized(arr, XorByte);
        Processor.ApplyXorFallback(arr2, XorByte);

        Assert.True(arr.SequenceEqual(arr2));
    }

    [SkippableFact]
    public void SingleByteApplyXor_Misaligned_MatchesExpected()
    {
        Skip.IfNot(Vector.IsHardwareAccelerated, "Hardware vector acceleration not supported");
        // Cut somewhere in 0..31 for misalignment
        Span<byte> arr = MemoryMarshal.Cast<int, byte>((Span<int>)new int[(1097 + sizeof(int) - 1) / sizeof(int)])[14..1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyXorVectorized(arr, XorByte);
        Processor.ApplyXorFallback(arr2, XorByte);

        Assert.True(arr.SequenceEqual(arr2));
    }

    [SkippableFact]
    public void BufferApplyXor_SmallBufferTruncate_MatchesExpected()
    {
        Skip.IfNot(Vector.IsHardwareAccelerated, "Hardware vector acceleration not supported");
        Span<byte> arr = new byte[91];
        Random.Shared.NextBytes(arr);
        Span<byte> xorArr = new byte[1843];
        Random.Shared.NextBytes(xorArr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyXorVectorized(arr, xorArr, SequenceBehaviour.Truncate);
        Processor.ApplyXorFallback(arr2, xorArr, SequenceBehaviour.Truncate);

        Assert.True(arr.SequenceEqual(arr2));
    }

    [SkippableFact]
    public void BufferApplyXor_LargeBufferTruncate_MatchesExpected()
    {
        Skip.IfNot(Vector.IsHardwareAccelerated, "Hardware vector acceleration not supported");
        Span<byte> arr = new byte[1097];
        Random.Shared.NextBytes(arr);
        Span<byte> xorArr = new byte[53];
        Random.Shared.NextBytes(xorArr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyXorVectorized(arr, xorArr, SequenceBehaviour.Truncate);
        Processor.ApplyXorFallback(arr2, xorArr, SequenceBehaviour.Truncate);

        Assert.True(arr.SequenceEqual(arr2));
    }

    [SkippableFact]
    public void BufferApplyXor_SmallBufferRepeat_MatchesExpected()
    {
        Skip.IfNot(Vector.IsHardwareAccelerated, "Hardware vector acceleration not supported");
        Span<byte> arr = new byte[91];
        Random.Shared.NextBytes(arr);
        Span<byte> xorArr = new byte[1843];
        Random.Shared.NextBytes(xorArr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyXorVectorized(arr, xorArr, SequenceBehaviour.Repeat);
        Processor.ApplyXorFallback(arr2, xorArr, SequenceBehaviour.Repeat);

        Assert.True(arr.SequenceEqual(arr2));
    }

    [SkippableFact]
    public void BufferApplyXor_LargeBufferRepeat_MatchesExpected()
    {
        Skip.IfNot(Vector.IsHardwareAccelerated, "Hardware vector acceleration not supported");
        Span<byte> arr = new byte[1097];
        Random.Shared.NextBytes(arr);
        Span<byte> xorArr = new byte[53];
        Random.Shared.NextBytes(xorArr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyXorVectorized(arr, xorArr, SequenceBehaviour.Repeat);
        Processor.ApplyXorFallback(arr2, xorArr, SequenceBehaviour.Repeat);

        Assert.True(arr.SequenceEqual(arr2));
    }

    [SkippableFact]
    public void BufferApplyAnd_EmptyBuffer_Noop()
    {
        Skip.IfNot(Vector.IsHardwareAccelerated, "Hardware vector acceleration not supported");
        Span<byte> xorArr = new byte[53];
        Random.Shared.NextBytes(xorArr);

        Processor.ApplyXorVectorized(Span<byte>.Empty, xorArr, SequenceBehaviour.Repeat);
        Processor.ApplyXorVectorized(Span<byte>.Empty, xorArr, SequenceBehaviour.Truncate);
        Processor.ApplyXorFallback(Span<byte>.Empty, xorArr, SequenceBehaviour.Repeat);
        Processor.ApplyXorFallback(Span<byte>.Empty, xorArr, SequenceBehaviour.Truncate);
    }

    [SkippableFact]
    public void BufferApplyXor_EmptyPattern_Noop()
    {
        Skip.IfNot(Vector.IsHardwareAccelerated, "Hardware vector acceleration not supported");
        Span<byte> arr = new byte[1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyXorVectorized(arr, ReadOnlySpan<byte>.Empty, SequenceBehaviour.Repeat);
        Assert.True(arr.SequenceEqual(arr2));
        Processor.ApplyXorFallback(arr, ReadOnlySpan<byte>.Empty, SequenceBehaviour.Repeat);
        Assert.True(arr.SequenceEqual(arr2));
        Processor.ApplyXorVectorized(arr, ReadOnlySpan<byte>.Empty, SequenceBehaviour.Truncate);
        Assert.True(arr.SequenceEqual(arr2));
        Processor.ApplyXorFallback(arr, ReadOnlySpan<byte>.Empty, SequenceBehaviour.Truncate);
        Assert.True(arr.SequenceEqual(arr2));
    }
}
