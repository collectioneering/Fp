using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Fp;

/// <summary>
/// Utility methods.
/// </summary>
public static partial class FpUtil
{
    #region Argument parsing

    private static readonly HashSet<string> s_emptyStringHashSet = new();

    /// <summary>
    /// Isolates flags from argument list.
    /// </summary>
    /// <param name="arguments">Argument lines to sort.</param>
    /// <param name="optKeys">Option keys.</param>
    /// <returns>Flags, options, and arguments.</returns>
    /// <remarks>https://github.com/The-Council-of-Wills/HacknetSharp/blob/58d5ccdf09203a38092b01aba3fea5df3e06d483/src/HacknetSharp.Server/ServerUtil.cs#L436</remarks>
    public static (HashSet<string> flags, Dictionary<string, string> opts, List<string> args) IsolateFlags(
        this IReadOnlyList<string> arguments, IReadOnlyCollection<string>? optKeys = null)
    {
        optKeys ??= s_emptyStringHashSet;
        HashSet<string> flags = new();
        Dictionary<string, string> opts = new();
        List<string> args = new();
        bool argTime = false;
        for (int i = 0; i < arguments.Count; i++)
        {
            string str = arguments[i];
            if (argTime)
            {
                args.Add(str);
                continue;
            }

            if (str.Length < 2 || str[0] != '-')
            {
                args.Add(str);
                continue;
            }

            if (str[1] == '-')
            {
                if (str.Length == 2)
                {
                    argTime = true;
                    continue;
                }

                string id = str[2..];
                if (optKeys.Contains(id))
                {
                    string? res = GetArg(arguments, i);
                    if (res != null) opts[id] = res;
                    i++;
                }
                else
                    flags.Add(id);
            }
            else
            {
                string firstId = str[1].ToString();
                if (str.Length == 2 && optKeys.Contains(firstId))
                {
                    string? res = GetArg(arguments, i);
                    if (res != null) opts[firstId] = res;
                    i++;
                }
                else
                    flags.UnionWith(str.Skip(1).Select(c => c.ToString()));
            }
        }

        return (flags, opts, args);
    }

    private static string? GetArg(IReadOnlyList<string> list, int i)
    {
        if (i + 1 < list.Count)
        {
            return list[i + 1];
        }

        return null;
    }

    #endregion

    #region Array slicing

    /// <summary>
    /// Slices an array and allocates a new array.
    /// </summary>
    /// <param name="array">Source.</param>
    /// <param name="start">Start index.</param>
    /// <param name="length">Length.</param>
    /// <typeparam name="T">Element type.</typeparam>
    /// <returns>Allocated array.</returns>
    public static T[] SliceAlloc<T>(this T[] array, int start, int length)
    {
        return array.AsSpan().SliceAlloc(start, length);
    }

    /// <summary>
    /// Slices a span and allocates a new array.
    /// </summary>
    /// <param name="span">Source.</param>
    /// <param name="start">Start index.</param>
    /// <param name="length">Length.</param>
    /// <typeparam name="T">Element type.</typeparam>
    /// <returns>Allocated array.</returns>
    public static T[] SliceAlloc<T>(this Span<T> span, int start, int length)
    {
        return span.Slice(start, length).ToArray();
    }

    /// <summary>
    /// Slices a span and allocates a new array.
    /// </summary>
    /// <param name="span">Source.</param>
    /// <param name="start">Start index.</param>
    /// <param name="length">Length.</param>
    /// <typeparam name="T">Element type.</typeparam>
    /// <returns>Allocated array.</returns>
    public static T[] SliceAlloc<T>(this ReadOnlySpan<T> span, int start, int length)
    {
        return span.Slice(start, length).ToArray();
    }

    #endregion

    #region Enumerable

    /// <summary>
    /// Disposes of an enumerable's elements after they have been yielded.
    /// </summary>
    /// <param name="enumerable">Enumerable.</param>
    /// <typeparam name="T">Element type.</typeparam>
    /// <returns>Input elements.</returns>
    public static IEnumerable<T?> PostDispose<T>(this IEnumerable<T?> enumerable) where T : IDisposable
    {
        foreach (var x in enumerable)
        {
            yield return x;
            x?.Dispose();
        }
    }

    /// <summary>
    /// Creates tuple sequence from an enumerable.
    /// </summary>
    /// <param name="enumerable">Enumerable.</param>
    /// <typeparam name="T">Element type.</typeparam>
    /// <returns>Tuples joining contiguous elements (packed, mutually exclusive).</returns>
    public static IEnumerable<ValueTuple<T, T>> Tuplify2<T>(this IEnumerable<T> enumerable)
    {
        using var enumerator = enumerable.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var v0 = enumerator.Current;
            if (!enumerator.MoveNext()) yield break;
            var v1 = enumerator.Current;
            yield return (v0, v1);
        }
    }

    /// <summary>
    /// Creates tuple sequence from an enumerable.
    /// </summary>
    /// <param name="enumerable">Enumerable.</param>
    /// <typeparam name="T">Element type.</typeparam>
    /// <returns>Tuples joining contiguous elements (packed, mutually exclusive).</returns>
    public static IEnumerable<ValueTuple<T, T, T>> Tuplify3<T>(this IEnumerable<T> enumerable)
    {
        using var enumerator = enumerable.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var v0 = enumerator.Current;
            if (!enumerator.MoveNext()) yield break;
            var v1 = enumerator.Current;
            if (!enumerator.MoveNext()) yield break;
            var v2 = enumerator.Current;
            yield return (v0, v1, v2);
        }
    }

    /// <summary>
    /// Creates tuple sequence from an enumerable.
    /// </summary>
    /// <param name="enumerable">Enumerable.</param>
    /// <typeparam name="T">Element type.</typeparam>
    /// <returns>Tuples joining contiguous elements (packed, mutually exclusive).</returns>
    public static IEnumerable<ValueTuple<T, T, T, T>> Tuplify4<T>(this IEnumerable<T> enumerable)
    {
        using var enumerator = enumerable.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var v0 = enumerator.Current;
            if (!enumerator.MoveNext()) yield break;
            var v1 = enumerator.Current;
            if (!enumerator.MoveNext()) yield break;
            var v2 = enumerator.Current;
            if (!enumerator.MoveNext()) yield break;
            var v3 = enumerator.Current;
            yield return (v0, v1, v2, v3);
        }
    }

    #endregion

    #region Strings

    /// <summary>
    /// Gets a byte array from a string assuming 8-bit characters.
    /// </summary>
    /// <param name="text">String to process.</param>
    /// <param name="result">Result buffer.</param>
    /// <returns>Byte array containing lower byte of each code unit in the string.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="result"/> is too small.</exception>
    public static byte[] Ascii(this string text, byte[]? result = null)
    {
        var sp = text.AsSpan();
        int l = sp.Length;
        if ((result?.Length ?? int.MaxValue) < l)
            throw new ArgumentException("Buffer not long enough to contain string", nameof(result));
        result ??= new byte[l];
        for (int i = 0; i < l; i++) result[i] = (byte)sp[i];
        return result;
    }

    /// <summary>
    /// Populates a span with a string's content assuming 8-bit characters.
    /// </summary>
    /// <param name="text">String to process.</param>
    /// <param name="result">Result buffer.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="result"/> is too small.</exception>
    public static void Ascii(this string text, Span<byte> result)
    {
        var sp = text.AsSpan();
        int l = sp.Length;
        if (result.Length < l)
            throw new ArgumentException("Buffer not long enough to contain string", nameof(result));
        for (int i = 0; i < l; i++) result[i] = (byte)sp[i];
    }

    #endregion

    private static readonly bool s_subNormalize = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

    #region Paths

    /// <summary>
    /// Normalizes the specified path.
    /// </summary>
    /// <param name="path">Path to normalize.</param>
    /// <returns>Normalized path.</returns>
    public static string NormalizeAndStripWindowsDrive(this string path)
    {
        string sub = Path.GetFullPath(path);
        if (s_subNormalize && sub.Length >= 2 && char.IsLetter(sub[0]) && sub[1] == Path.VolumeSeparatorChar)
            sub = sub[2..];
        return sub;
    }

    #endregion

    #region Numbers

    /// <summary>
    /// Casts number.
    /// </summary>
    /// <param name="value">Input value.</param>
    /// <typeparam name="TIn">Input type.</typeparam>
    /// <typeparam name="TOut">Output type.</typeparam>
    /// <returns>Converted value.</returns>
    /// <exception cref="ArgumentException">Thrown for invalid output type.</exception>
    /// <exception cref="InvalidCastException">Thrown for invalid cast.</exception>
    public static unsafe TOut CastNumber<TIn, TOut>(TIn value) where TOut : unmanaged
    {
        if (value?.GetType() == typeof(string))
        {
            if (typeof(TOut) == typeof(float) || typeof(TOut) == typeof(double))
                return CastNumber<double, TOut>(double.Parse(value.ToString()!, CultureInfo.InvariantCulture));
            if (typeof(TOut) == typeof(ulong))
                return CastNumber<ulong, TOut>(ulong.Parse(value.ToString()!, CultureInfo.InvariantCulture));
            return CastNumber<long, TOut>(long.Parse(value.ToString()!, CultureInfo.InvariantCulture));
        }

        TOut target;
        if (typeof(TOut) == typeof(byte))
        {
            *(byte*)&target = CastByte(value);
            return target;
        }

        if (typeof(TOut) == typeof(sbyte))
        {
            *(sbyte*)&target = CastSByte(value);
            return target;
        }

        if (typeof(TOut) == typeof(ushort))
        {
            *(ushort*)&target = CastUShort(value);
            return target;
        }

        if (typeof(TOut) == typeof(short))
        {
            *(short*)&target = CastShort(value);
            return target;
        }

        if (typeof(TOut) == typeof(uint))
        {
            *(uint*)&target = CastUInt(value);
            return target;
        }

        if (typeof(TOut) == typeof(int))
        {
            *(int*)&target = CastInt(value);
            return target;
        }

        if (typeof(TOut) == typeof(ulong))
        {
            *(ulong*)&target = CastULong(value);
            return target;
        }

        if (typeof(TOut) == typeof(long))
        {
            *(long*)&target = CastLong(value);
            return target;
        }

        if (typeof(TOut) == typeof(float))
        {
            *(float*)&target = CastFloat(value);
            return target;
        }

        if (typeof(TOut) == typeof(double))
        {
            *(double*)&target = CastDouble(value);
            return target;
        }

        throw new ArgumentException($"Unsupported output type {typeof(TOut)}");
    }

    private static byte CastByte<TValue>(TValue number)
    {
        return number switch
        {
            byte b => b,
            sbyte b => (byte)b,
            ushort b => (byte)b,
            short b => (byte)b,
            uint b => (byte)b,
            int b => (byte)b,
            ulong b => (byte)b,
            long b => (byte)b,
            float b => (byte)b,
            double b => (byte)b,
            _ => throw new InvalidCastException(
                $"Could not cast from type {number?.GetType().FullName ?? "null"} to {typeof(byte)}")
        };
    }

    private static sbyte CastSByte<TValue>(TValue number)
    {
        return number switch
        {
            byte b => (sbyte)b,
            sbyte b => b,
            ushort b => (sbyte)b,
            short b => (sbyte)b,
            uint b => (sbyte)b,
            int b => (sbyte)b,
            ulong b => (sbyte)b,
            long b => (sbyte)b,
            float b => (sbyte)b,
            double b => (sbyte)b,
            _ => throw new InvalidCastException(
                $"Could not cast from type {number?.GetType().FullName ?? "null"} to {typeof(sbyte)}")
        };
    }

    private static ushort CastUShort<TValue>(TValue number)
    {
        return number switch
        {
            byte b => b,
            sbyte b => (ushort)b,
            ushort b => b,
            short b => (ushort)b,
            uint b => (ushort)b,
            int b => (ushort)b,
            ulong b => (ushort)b,
            long b => (ushort)b,
            float b => (ushort)b,
            double b => (ushort)b,
            _ => throw new InvalidCastException(
                $"Could not cast from type {number?.GetType().FullName ?? "null"} to {typeof(ushort)}")
        };
    }

    private static short CastShort<TValue>(TValue number)
    {
        return number switch
        {
            byte b => b,
            sbyte b => b,
            ushort b => (short)b,
            short b => b,
            uint b => (short)b,
            int b => (short)b,
            ulong b => (short)b,
            long b => (short)b,
            float b => (short)b,
            double b => (short)b,
            _ => throw new InvalidCastException(
                $"Could not cast from type {number?.GetType().FullName ?? "null"} to {typeof(short)}")
        };
    }

    private static uint CastUInt<TValue>(TValue number)
    {
        return number switch
        {
            byte b => b,
            sbyte b => (uint)b,
            ushort b => b,
            short b => (uint)b,
            uint b => b,
            int b => (uint)b,
            ulong b => (uint)b,
            long b => (uint)b,
            float b => (uint)b,
            double b => (uint)b,
            _ => throw new InvalidCastException(
                $"Could not cast from type {number?.GetType().FullName ?? "null"} to {typeof(uint)}")
        };
    }

    private static int CastInt<TValue>(TValue number)
    {
        return number switch
        {
            byte b => b,
            sbyte b => b,
            ushort b => b,
            short b => b,
            uint b => (int)b,
            int b => b,
            ulong b => (int)b,
            long b => (int)b,
            float b => (int)b,
            double b => (int)b,
            _ => throw new InvalidCastException(
                $"Could not cast from type {number?.GetType().FullName ?? "null"} to {typeof(int)}")
        };
    }


    private static ulong CastULong<TValue>(TValue number)
    {
        return number switch
        {
            byte b => b,
            sbyte b => (ulong)b,
            ushort b => b,
            short b => (ulong)b,
            uint b => b,
            int b => (ulong)b,
            ulong b => b,
            long b => (ulong)b,
            float b => (ulong)b,
            double b => (ulong)b,
            _ => throw new InvalidCastException(
                $"Could not cast from type {number?.GetType().FullName ?? "null"} to {typeof(ulong)}")
        };
    }

    private static long CastLong<TValue>(TValue number)
    {
        return number switch
        {
            byte b => b,
            sbyte b => b,
            ushort b => b,
            short b => b,
            uint b => b,
            int b => b,
            ulong b => (long)b,
            long b => b,
            float b => (long)b,
            double b => (long)b,
            _ => throw new InvalidCastException(
                $"Could not cast from type {number?.GetType().FullName ?? "null"} to {typeof(long)}")
        };
    }

    private static float CastFloat<TValue>(TValue number)
    {
        return number switch
        {
            byte b => b,
            sbyte b => b,
            ushort b => b,
            short b => b,
            uint b => b,
            int b => b,
            ulong b => b,
            long b => b,
            float b => b,
            double b => (float)b,
            _ => throw new InvalidCastException(
                $"Could not cast from type {number?.GetType().FullName ?? "null"} to {typeof(float)}")
        };
    }

    private static double CastDouble<TValue>(TValue number)
    {
        return number switch
        {
            byte b => b,
            sbyte b => b,
            ushort b => b,
            short b => b,
            uint b => b,
            int b => b,
            ulong b => b,
            long b => b,
            float b => b,
            double b => b,
            _ => throw new InvalidCastException(
                $"Could not cast from type {number?.GetType().FullName ?? "null"} to {typeof(double)}")
        };
    }

    #endregion
}
