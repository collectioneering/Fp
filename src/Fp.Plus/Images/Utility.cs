﻿using System;
using System.Runtime.CompilerServices;
using Fp.Plus.Images;
using static Fp.Detector;
using static Fp.Processor;

// ReSharper disable once CheckNamespace
namespace Fp
{
    public partial class PlusUtil
    {
        #region Detectors

        #region BMP

        /// <summary>
        /// Detect BMP bitmap files.
        /// </summary>
        /// <returns>Detector.</returns>
        public static Detector _BMP() => _BMP(null);

        /// <summary>
        /// Detect BMP bitmap files.
        /// </summary>
        /// <param name="source">Data source.</param>
        /// <returns>Detector.</returns>
        public static Detector _BMP(this object source) => _BMP(null, source);

        /// <summary>
        /// Detect BMP bitmap files.
        /// </summary>
        /// <param name="detector">Existing detector.</param>
        /// <param name="source">Data source.</param>
        /// <returns>Detector.</returns>
        public static Detector _BMP(this Detector? detector, object? source = null) =>
            new(detector, ResolveSource(detector, source), o => o switch
            {
                Processor p when
                    p.HasMagic((ReadOnlySpan<byte>)new[] {(byte)'B', (byte)'M'}) &&
                    p.i4l[2] == p.InputLength => ".bmp",
                ReadOnlyMemory<byte> m when
                    HasMagic(m.Span, (ReadOnlySpan<byte>)new[] {(byte)'B', (byte)'M'}) &&
                    Instance.i4l[m, 2] == m.Length => ".bmp",
                _ => null
            });

        #endregion

        #region PNG

        /// <summary>
        /// Detect PNG image files.
        /// </summary>
        /// <returns>Detector.</returns>
        public static Detector _PNG() => _PNG(null);

        /// <summary>
        /// Detect PNG image files.
        /// </summary>
        /// <param name="source">Data source.</param>
        /// <returns>Detector.</returns>
        public static Detector _PNG(this object source) => _PNG(null, source);

        /// <summary>
        /// Detect PNG image files.
        /// </summary>
        /// <param name="detector">Existing detector.</param>
        /// <param name="source">Data source.</param>
        /// <returns>Detector.</returns>
        public static Detector _PNG(this Detector? detector, object? source = null) =>
            new(detector, ResolveSource(detector, source), o => o switch
            {
                Processor p when
                    p.HasMagic((ReadOnlySpan<byte>)new byte[] {137, 80, 78, 71, 13, 10, 26, 10}) => ".png",
                ReadOnlyMemory<byte> m when
                    HasMagic(m.Span, (ReadOnlySpan<byte>)new byte[] {137, 80, 78, 71, 13, 10, 26, 10}) => ".png",
                _ => null
            });

        #endregion


        #region JPEG

        /// <summary>
        /// Detect JPEG image files.
        /// </summary>
        /// <returns>Detector.</returns>
        public static Detector _JPEG() => _JPEG(null);

        /// <summary>
        /// Detect JPEG image files.
        /// </summary>
        /// <param name="source">Data source.</param>
        /// <returns>Detector.</returns>
        public static Detector _JPEG(this object source) => _JPEG(null, source);

        /// <summary>
        /// Detect JPEG image files.
        /// </summary>
        /// <param name="detector">Existing detector.</param>
        /// <param name="source">Data source.</param>
        /// <returns>Detector.</returns>
        public static Detector _JPEG(this Detector? detector, object? source = null) =>
            new(detector, ResolveSource(detector, source), o => o switch
            {
                Processor p when
                    p.HasMagic((ReadOnlySpan<byte>)new byte[] {0xff, 0xd8, 0xff}) => ".jpg",
                ReadOnlyMemory<byte> m when
                    HasMagic(m.Span, (ReadOnlySpan<byte>)new byte[] {0xff, 0xd8, 0xff}) => ".jpg",
                _ => null
            });

        #endregion

        #endregion

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
        /// Convert 24bpp RGB data to 32bpp RGBA.
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
        /// Convert 24bpp RGB data to 32bpp RGBA.
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
        /// Convert 32bpp BGRA data to 32bpp RGBA.
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
        /// Convert 32bpp BGRA data to 32bpp RGBA.
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
    }
}
