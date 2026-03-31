using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;

namespace Fp;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public partial class Processor
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe Vector128<byte> FillVector128AdvSimd(byte value)
    {
        int* srcPtr = stackalloc int[128 / 8 / 4];
        int iValue = (value << 8) | value;
        iValue |= iValue << 16;
        srcPtr[0] = iValue;
        srcPtr[1] = iValue;
        srcPtr[2] = iValue;
        srcPtr[3] = iValue;
        return AdvSimd.LoadVector128((byte*)srcPtr);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe Vector128<byte> FillVector128Sse2(byte value)
    {
        int* srcPtr = stackalloc int[128 / 8 / 4];
        int iValue = (value << 8) | value;
        iValue |= iValue << 16;
        srcPtr[0] = iValue;
        srcPtr[1] = iValue;
        srcPtr[2] = iValue;
        srcPtr[3] = iValue;
        return Sse2.LoadVector128((byte*)srcPtr);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe Vector256<byte> FillVector256Avx(byte value)
    {
        int* srcPtr = stackalloc int[256 / 8 / 4];
        int iValue = (value << 8) | value;
        iValue |= iValue << 16;
        srcPtr[0] = iValue;
        srcPtr[1] = iValue;
        srcPtr[2] = iValue;
        srcPtr[3] = iValue;
        srcPtr[4] = iValue;
        srcPtr[5] = iValue;
        srcPtr[6] = iValue;
        srcPtr[7] = iValue;
        return Avx.LoadVector256((byte*)srcPtr);
    }

    /// <summary>
    /// Gets the first aligned index or <paramref name="buffer"/>.<see cref="ReadOnlySpan{T}.Length"/> if no aligned values are contained.
    /// </summary>
    /// <param name="buffer">Buffer to check.</param>
    /// <param name="alignment">Alignment value in bytes.</param>
    /// <returns>First aligned index or <paramref name="buffer"/>.<see cref="ReadOnlySpan{T}.Length"/> if no aligned values are contained.</returns>
    public static unsafe int GetAlignmentStart(ReadOnlySpan<byte> buffer, int alignment)
    {
        fixed (byte* p = buffer)
        {
            return GetAlignmentStart((ulong)p, alignment, buffer.Length);
        }
    }

    private static int GetAlignmentStart(ulong ptr, int alignment, int length)
    {
        return Math.Min((int)unchecked(((uint)alignment - ptr) % (uint)alignment), length);
    }

    /// <summary>
    /// Checks if a buffer contains at least one element aligned to the specified number of bytes.
    /// </summary>
    /// <param name="buffer">Buffer to check.</param>
    /// <param name="alignment">Alignment value in bytes.</param>
    /// <returns>True if buffer has at least one aligned value.</returns>
    /// <remarks>
    /// This is intended for use as a precondition for using intrinsics.
    /// </remarks>
    public static bool ContainsAtLeastOneAligned(ReadOnlySpan<byte> buffer, int alignment)
    {
        return buffer.Length - GetAlignmentStart(buffer, alignment) >= alignment;
    }

    /// <summary>
    /// Transform delegate.
    /// </summary>
    /// <param name="input">Input value.</param>
    /// <param name="index">Index.</param>
    /// <typeparam name="T">Element type.</typeparam>
    public delegate T TransformDelegate<T>(T input, int index);

    /// <summary>
    /// Transforms memory region.
    /// </summary>
    /// <param name="span">Memory to modify.</param>
    /// <param name="func">Transformation delegate.</param>
    /// <typeparam name="T">Element type.</typeparam>
    public static void ApplyTransform<T>(Span<T> span, TransformDelegate<T> func)
    {
        for (int i = 0; i < span.Length; i++)
            span[i] = func(span[i], i);
    }
}
