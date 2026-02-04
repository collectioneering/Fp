using System;
using System.Collections;

namespace Fp;

/// <summary>
/// Bitwise operations.
/// </summary>
public static class BitUtil
{
    /// <summary>
    /// Aligns value down.
    /// </summary>
    /// <param name="value">Input value.</param>
    /// <param name="align">Alignment.</param>
    /// <returns>Aligned value.</returns>
    public static int AlignDown(this int value, int align) => align == 0 ? value : value / align * align;

    /// <summary>
    /// Aligns value up.
    /// </summary>
    /// <param name="value">Input value.</param>
    /// <param name="align">Alignment.</param>
    /// <returns>Aligned value.</returns>
    public static int AlignUp(this int value, int align) =>
        align == 0 ? value : (value + align - 1) / align * align;

    /// <summary>
    /// Gets number of bytes required to store input bits.
    /// </summary>
    /// <param name="bits">Input bits.</param>
    /// <returns>Number of bytes required to store input bits.</returns>
    public static int GetBytesForBits(this int bits) => (bits + 7) / 8;

    /// <summary>
    /// Gets number of bits contained by input bytes.
    /// </summary>
    /// <param name="bytes">Input bytes.</param>
    /// <returns>Number of bits contained by input bytes.</returns>
    public static int GetBitsForBytes(this int bytes)
    {
        if (bytes < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(bytes), bytes, "Value must not be negative");
        }
        long value = (long)bytes * 8;
        if (value > int.MaxValue)
        {
            throw new ArgumentException("Input caused an overflow");
        }
        return (int)value;
    }

    /// <summary>
    /// Gets number of bits contained by input bytes.
    /// </summary>
    /// <param name="bytes">Input bytes.</param>
    /// <returns>Number of bits contained by input bytes.</returns>
    public static long GetBitsForBytes(this long bytes)
    {
        if (bytes < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(bytes), bytes, "Value must not be negative");
        }
        long value = bytes * 8;
        if (value < 0)
        {
            throw new ArgumentException("Input caused an overflow");
        }
        return value;
    }

    /// <summary>
    /// Skips over bits matching specified value.
    /// </summary>
    /// <param name="array">Bit array to use.</param>
    /// <param name="i">Index to modify, set to position after last occurrence of <paramref name="skipValue"/> in a row starting at <paramref name="i"/>, or <paramref name="array"/>.<see cref="BitArray.Length"/> if no such value was found.</param>
    /// <param name="skipValue">Value to skip over.</param>
    public static void SkipBits(this BitArray array, ref int i, bool skipValue)
    {
        while (i < array.Length && array[i] == skipValue) i++;
    }

    /// <summary>
    /// Skips over bits matching specified value.
    /// </summary>
    /// <param name="array">Bit array to use.</param>
    /// <param name="count">Bit count in array.</param>
    /// <param name="i">Index to modify, set to position after last occurrence of <paramref name="skipValue"/> in a row starting at <paramref name="i"/>, or <paramref name="count"/> if no such value was found.</param>
    /// <param name="skipValue">Value to skip over.</param>
    /// <param name="bigEndian">If true, treats highest order bit as first bit in each byte.</param>
    public static void SkipBits(this ReadOnlySpan<byte> array, int count, ref int i, bool skipValue, bool bigEndian = false)
    {
        if (array.Length.GetBitsForBytes() < count)
            throw new ArgumentException("Invalid number of bits for input array length", nameof(count));
        byte skipMask = skipValue ? (byte)0xff : (byte)0x00;
        if (bigEndian)
        {
            int x = i >> 3, v = array[x];
            while (i < count)
            {
                int x2 = i >> 3;
                if (x2 != x)
                {
                    x = x2;
                    v = array[x];
                }
                byte mask = (byte)(0b1000_0000 >> (i & 0b0000_0111));
                if (((skipMask & mask) ^ (v & mask)) != 0) break;
                i++;
            }
        }
        else
        {
            int x = i >> 3, v = array[x];
            while (i < count)
            {
                int x2 = i >> 3;
                if (x2 != x)
                {
                    x = x2;
                    v = array[x2];
                }
                byte mask = (byte)(0b0000_0001 << (i & 0b0000_0111));
                if (((skipMask & mask) ^ (v & mask)) != 0) break;
                i++;
            }
        }
    }

    /// <summary>
    /// Skips over bits matching specified value.
    /// </summary>
    /// <param name="array">Bit array to use.</param>
    /// <param name="maxExc">Maximum index (exclusive).</param>
    /// <param name="i">Index to modify, set to position after last occurrence of <paramref name="skipValue"/> in a row starting at <paramref name="i"/>, or <paramref name="maxExc"/>-1 if no such value was found.</param>
    /// <param name="skipValue">Value to skip over.</param>
    /// <returns>True if a value other than <paramref name="skipValue"/> was found before termination.</returns>
    public static bool ConstrainedSkipBits(this BitArray array, int maxExc, ref int i, bool skipValue)
    {
        if (array.Length < maxExc) throw new ArgumentException("Invalid exclusive end index", nameof(maxExc));
        while (i < maxExc)
        {
            if (array[i] == skipValue)
            {
                if (i + 1 < maxExc)
                {
                    i++;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Skips over bits matching specified value.
    /// </summary>
    /// <param name="array">Bit array to use.</param>
    /// <param name="maxExc">Maximum index (exclusive).</param>
    /// <param name="i">Index to modify, set to position after last occurrence of <paramref name="skipValue"/> in a row starting at <paramref name="i"/>, or <paramref name="maxExc"/>-1 if no such value was found.</param>
    /// <param name="skipValue">Value to skip over.</param>
    /// <param name="bigEndian">If true, treats highest order bit as first bit in each byte.</param>
    /// <returns>True if a value other than <paramref name="skipValue"/> was found before termination.</returns>
    public static bool ConstrainedSkipBits(this ReadOnlySpan<byte> array, int maxExc, ref int i, bool skipValue, bool bigEndian = false)
    {
        if (array.Length.GetBitsForBytes() < maxExc) throw new ArgumentException("Invalid exclusive end index", nameof(maxExc));
        byte skipMask = skipValue ? (byte)0xff : (byte)0x00;
        if (bigEndian)
        {
            int x = i >> 3, v = array[x];
            while (i < maxExc)
            {
                int x2 = i >> 3;
                if (x2 != x)
                {
                    x = x2;
                    v = array[x];
                }
                byte mask = (byte)(0b1000_0000 >> (i & 0b0000_0111));
                if (((skipMask & mask) ^ (v & mask)) == 0)
                {
                    if (i + 1 < maxExc)
                    {
                        i++;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
        }
        else
        {
            int x = i >> 3, v = array[x];
            while (i < maxExc)
            {
                int x2 = i >> 3;
                if (x2 != x)
                {
                    x = x2;
                    v = array[x];
                }
                byte mask = (byte)(0b0000_0001 << (i & 0b0000_0111));
                if (((skipMask & mask) ^ (v & mask)) == 0)
                {
                    if (i + 1 < maxExc)
                    {
                        i++;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
        }
        return false;
    }
}
