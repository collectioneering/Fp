using System;
using System.Runtime.CompilerServices;
using Fp.Plus.Images;
using static Fp.Detector;
using static Fp.Processor;

// ReSharper disable once CheckNamespace
namespace Fp.Plus;

public partial class PlusUtil
{
    #region Detectors

    #region BMP

    /// <summary>
    /// Detects BMP bitmap files.
    /// </summary>
    /// <returns>Detector.</returns>
    public static Detector _BMP() => _BMP(null);

    /// <summary>
    /// Detects BMP bitmap files.
    /// </summary>
    /// <param name="source">Data source.</param>
    /// <returns>Detector.</returns>
    public static Detector _BMP(this object source) => _BMP(null, source);

    /// <summary>
    /// Detects BMP bitmap files.
    /// </summary>
    /// <param name="detector">Existing detector.</param>
    /// <param name="source">Data source.</param>
    /// <returns>Detector.</returns>
    public static Detector _BMP(this Detector? detector, object? source = null) =>
        new(detector, ResolveSource(detector, source), o => o switch
        {
            Processor p when
                p.HasMagic((ReadOnlySpan<byte>)new[] { (byte)'B', (byte)'M' }) &&
                p.i4l[2] == p.InputLength => ".bmp",
            ReadOnlyMemory<byte> m when
                HasMagic(m.Span, (ReadOnlySpan<byte>)new[] { (byte)'B', (byte)'M' }) &&
                Instance.i4l[m, 2] == m.Length => ".bmp",
            _ => null
        });

    #endregion

    #region PNG

    /// <summary>
    /// Detects PNG image files.
    /// </summary>
    /// <returns>Detector.</returns>
    public static Detector _PNG() => _PNG(null);

    /// <summary>
    /// Detects PNG image files.
    /// </summary>
    /// <param name="source">Data source.</param>
    /// <returns>Detector.</returns>
    public static Detector _PNG(this object source) => _PNG(null, source);

    /// <summary>
    /// Detects PNG image files.
    /// </summary>
    /// <param name="detector">Existing detector.</param>
    /// <param name="source">Data source.</param>
    /// <returns>Detector.</returns>
    public static Detector _PNG(this Detector? detector, object? source = null) =>
        new(detector, ResolveSource(detector, source), o => o switch
        {
            Processor p when
                p.HasMagic((ReadOnlySpan<byte>)new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }) => ".png",
            ReadOnlyMemory<byte> m when
                HasMagic(m.Span, (ReadOnlySpan<byte>)new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }) => ".png",
            _ => null
        });

    #endregion


    #region JPEG

    /// <summary>
    /// Detects JPEG image files.
    /// </summary>
    /// <returns>Detector.</returns>
    public static Detector _JPEG() => _JPEG(null);

    /// <summary>
    /// Detects JPEG image files.
    /// </summary>
    /// <param name="source">Data source.</param>
    /// <returns>Detector.</returns>
    public static Detector _JPEG(this object source) => _JPEG(null, source);

    /// <summary>
    /// Detects JPEG image files.
    /// </summary>
    /// <param name="detector">Existing detector.</param>
    /// <param name="source">Data source.</param>
    /// <returns>Detector.</returns>
    public static Detector _JPEG(this Detector? detector, object? source = null) =>
        new(detector, ResolveSource(detector, source), o => o switch
        {
            Processor p when
                p.HasMagic((ReadOnlySpan<byte>)new byte[] { 0xff, 0xd8, 0xff }) => ".jpg",
            ReadOnlyMemory<byte> m when
                HasMagic(m.Span, (ReadOnlySpan<byte>)new byte[] { 0xff, 0xd8, 0xff }) => ".jpg",
            _ => null
        });

    #endregion

    #endregion

    #region Creation

    /// <summary>
    /// Creates 32bpp RGBA image data object.
    /// </summary>
    /// <param name="path">Base path (without extension).</param>
    /// <param name="width">Image width.</param>
    /// <param name="height">Image height.</param>
    /// <param name="buffer">32bpp RGBA buffer.</param>
    /// <returns>Data object.</returns>
    public static Rgba32Data Image(this FpPath path, int width, int height, ReadOnlyMemory<uint> buffer) =>
        new(path.AsCombined(), width, height, buffer);

    /// <summary>
    /// Creates 32bpp RGBA image data object.
    /// </summary>
    /// <param name="name">Base path (without extension).</param>
    /// <param name="width">Image width.</param>
    /// <param name="height">Image height.</param>
    /// <param name="buffer">32bpp RGBA buffer.</param>
    /// <returns>Data object.</returns>
    public static Rgba32Data Image(this string name, int width, int height, ReadOnlyMemory<uint> buffer) =>
        new(name, width, height, buffer);

    /// <summary>
    /// Creates image write context.
    /// </summary>
    /// <param name="path">Base path (without extension).</param>
    /// <param name="width">Image width.</param>
    /// <param name="height">Image height.</param>
    /// <param name="image">Data object.</param>
    /// <returns>32bpp RGBA buffer.</returns>
    public static WriteContext<uint> CreateImage(this FpPath path, int width, int height, out Data image)
    {
        uint[] a = new uint[width * height];
        image = path.Image(width, height, a);
        return new WriteContext<uint>(a);
    }

    #endregion

    #region Validation

    /// <summary>
    /// Ensures value is within pixel range between 0 and max.
    /// </summary>
    /// <param name="x">Input.</param>
    /// <param name="max">Maximum size.</param>
    /// <returns>Input.</returns>
    /// <exception cref="ArgumentException">Thrown when size is negative or above max.</exception>
    public static sbyte ImageRes(this sbyte x, sbyte max)
    {
        if ((byte)(x - 1) >= max) throw new ArgumentException();
        return x;
    }

    /// <summary>
    /// Ensures value is within pixel range between 0 and max.
    /// </summary>
    /// <param name="x">Input.</param>
    /// <param name="max">Maximum size.</param>
    /// <returns>Input.</returns>
    /// <exception cref="ArgumentException">Thrown when size is negative or above max.</exception>
    public static byte ImageRes(this byte x, byte max)
    {
        if ((byte)(x - 1) >= max) throw new ArgumentException();
        return x;
    }

    /// <summary>
    /// Ensures value is within pixel range between 0 and max.
    /// </summary>
    /// <param name="x">Input.</param>
    /// <param name="max">Maximum size.</param>
    /// <returns>Input.</returns>
    /// <exception cref="ArgumentException">Thrown when size is negative or above max.</exception>
    public static short ImageRes(this short x, short max)
    {
        if ((ushort)(x - 1) >= max) throw new ArgumentException();
        return x;
    }

    /// <summary>
    /// Ensures value is within pixel range between 0 and max.
    /// </summary>
    /// <param name="x">Input.</param>
    /// <param name="max">Maximum size.</param>
    /// <returns>Input.</returns>
    /// <exception cref="ArgumentException">Thrown when size is negative or above max.</exception>
    public static ushort ImageRes(this ushort x, ushort max)
    {
        if ((ushort)(x - 1) >= max) throw new ArgumentException();
        return x;
    }

    /// <summary>
    /// Ensures value is within pixel range between 0 and max.
    /// </summary>
    /// <param name="x">Input.</param>
    /// <param name="max">Maximum size.</param>
    /// <returns>Input.</returns>
    /// <exception cref="ArgumentException">Thrown when size is negative or above max.</exception>
    public static int ImageRes(this int x, int max)
    {
        if ((uint)(x - 1) >= max) throw new ArgumentException();
        return x;
    }

    /// <summary>
    /// Ensures value is within pixel range between 0 and max.
    /// </summary>
    /// <param name="x">Input.</param>
    /// <param name="max">Maximum size.</param>
    /// <returns>Input.</returns>
    /// <exception cref="ArgumentException">Thrown when size is negative or above max.</exception>
    public static uint ImageRes(this uint x, uint max)
    {
        if (x - 1 >= max) throw new ArgumentException();
        return x;
    }

    #endregion

    #region Pixel conversion

    /// <summary>
    /// Converts 24bpp RGB data to 32bpp RGBA.
    /// </summary>
    /// <param name="data">Source.</param>
    /// <param name="target">Target.</param>
    /// <param name="a">Alpha value.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void FromRgb(this ReadOnlySpan<byte> data, Span<byte> target, byte a = 255)
    {
        data.Slice(0, 3).CopyTo(target);
        target[3] = a;
    }

    /// <summary>
    /// Converts 24bpp RGB data to 32bpp RGBA.
    /// </summary>
    /// <param name="data">Source.</param>
    /// <param name="a">Alpha value.</param>
    /// <returns>32-bit value with color.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe uint FromRgb(this ReadOnlySpan<byte> data, byte a = 255)
    {
        uint result;
        byte* target = (byte*)&result;
        target[0] = data[0];
        target[1] = data[1];
        target[2] = data[2];
        target[3] = a;
        return result;
    }

    /// <summary>
    /// Converts 32bpp BGRA data to 32bpp RGBA.
    /// </summary>
    /// <param name="data">Source.</param>
    /// <param name="target">Target.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void FromBgra(this ReadOnlySpan<byte> data, Span<byte> target)
    {
        target[2] = data[0];
        target[1] = data[1];
        target[0] = data[2];
        target[3] = data[3];
    }

    /// <summary>
    /// Converts 32bpp BGRA data to 32bpp RGBA.
    /// </summary>
    /// <param name="data">Source.</param>
    /// <returns>32-bit value with color.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe uint FromBgra(this ReadOnlySpan<byte> data)
    {
        uint result;
        byte* target = (byte*)&result;
        target[2] = data[0];
        target[1] = data[1];
        target[0] = data[2];
        target[3] = data[3];
        return result;
    }

    #endregion
}