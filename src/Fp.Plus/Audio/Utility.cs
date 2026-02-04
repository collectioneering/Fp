using System;
using static Fp.Detector;
using static Fp.Processor;

// ReSharper disable once CheckNamespace
namespace Fp.Plus;

public partial class PlusUtil
{
    #region Detectors

    #region WAV

    /// <summary>
    /// Detects WAV audio files.
    /// </summary>
    /// <returns>Detector.</returns>
    public static Detector _WAV() => _WAV(null);

    /// <summary>
    /// Detects WAV audio files.
    /// </summary>
    /// <param name="source">Data source.</param>
    /// <returns>Detector.</returns>
    public static Detector _WAV(this object source) => _WAV(null, source);

    /// <summary>
    /// Detects WAV audio files.
    /// </summary>
    /// <param name="detector">Existing detector.</param>
    /// <param name="source">Data source.</param>
    /// <returns>Detector.</returns>
    public static Detector _WAV(this Detector? detector, object? source = null) =>
        new(detector, ResolveSource(detector, source), o => o switch
        {
            Processor p when
                p.HasMagic((ReadOnlySpan<byte>)new[] { (byte)'R', (byte)'I', (byte)'F', (byte)'F' }) &&
                p.HasMagic((ReadOnlySpan<byte>)new[] { (byte)'W', (byte)'A', (byte)'V', (byte)'E' }, 8) => ".wav",
            ReadOnlyMemory<byte> m when
                HasMagic(m.Span, (ReadOnlySpan<byte>)new[] { (byte)'R', (byte)'I', (byte)'F', (byte)'F' }) &&
                HasMagic(m.Span, (ReadOnlySpan<byte>)new[] { (byte)'W', (byte)'A', (byte)'V', (byte)'E' },
                    8) => ".wav",
            _ => null
        });

    #endregion

    #region MP3

    /// <summary>
    /// Detects MP3 audio files.
    /// </summary>
    /// <returns>Detector.</returns>
    public static Detector _MP3() => _MP3(null);

    /// <summary>
    /// Detects MP3 audio files.
    /// </summary>
    /// <param name="source">Data source.</param>
    /// <returns>Detector.</returns>
    public static Detector _MP3(this object source) => _MP3(null, source);

    /// <summary>
    /// Detects MP3 audio files.
    /// </summary>
    /// <param name="detector">Existing detector.</param>
    /// <param name="source">Data source.</param>
    /// <returns>Detector.</returns>
    public static Detector _MP3(this Detector? detector, object? source = null) =>
        new(detector, ResolveSource(detector, source), o => o switch
        {
            Processor p when
                p.HasMagic((ReadOnlySpan<byte>)new byte[] { 0xff, 0xf2 }) ||
                p.HasMagic((ReadOnlySpan<byte>)new byte[] { 0xff, 0xf3 }) ||
                p.HasMagic((ReadOnlySpan<byte>)new byte[] { 0xff, 0xfb }) ||
                p.HasMagic((ReadOnlySpan<byte>)new byte[] { 0x49, 0x44, 0x33 }) => ".mp3",
            ReadOnlyMemory<byte> m when
                HasMagic(m.Span, (ReadOnlySpan<byte>)new byte[] { 0xff, 0xf2 }) ||
                HasMagic(m.Span, (ReadOnlySpan<byte>)new byte[] { 0xff, 0xf3 }) ||
                HasMagic(m.Span, (ReadOnlySpan<byte>)new byte[] { 0xff, 0xfb }) ||
                HasMagic(m.Span, (ReadOnlySpan<byte>)new byte[] { 0x49, 0x44, 0x33 }) => ".mp3",
            _ => null
        });

    #endregion

    #region Ogg/Oga

    /// <summary>
    /// Detects Ogg containers.
    /// </summary>
    /// <returns>Detector.</returns>
    public static Detector _OGG() => _OGG(null);

    /// <summary>
    /// Detects Ogg containers.
    /// </summary>
    /// <param name="source">Data source.</param>
    /// <returns>Detector.</returns>
    public static Detector _OGG(this object source) => _OGG(null, source);

    /// <summary>
    /// Detects Ogg containers.
    /// </summary>
    /// <param name="detector">Existing detector.</param>
    /// <param name="source">Data source.</param>
    /// <returns>Detector.</returns>
    public static Detector _OGG(this Detector? detector, object? source = null) =>
        new(detector, ResolveSource(detector, source), o => o switch
        {
            Processor p when
                p.HasMagic((ReadOnlySpan<byte>)new byte[] { 0x4f, 0x67, 0x67, 0x53 }) => ".ogg",
            ReadOnlyMemory<byte> m when
                HasMagic(m.Span, (ReadOnlySpan<byte>)new byte[] { 0x4f, 0x67, 0x67, 0x53 }) => ".ogg",
            _ => null
        });

    /// <summary>
    /// Detects Ogg audio containers.
    /// </summary>
    /// <returns>Detector.</returns>
    public static Detector _OGA() => _OGA(null);

    /// <summary>
    /// Detects Ogg audio containers.
    /// </summary>
    /// <param name="source">Data source.</param>
    /// <returns>Detector.</returns>
    public static Detector _OGA(this object source) => _OGA(null, source);

    /// <summary>
    /// Detects Ogg audio containers.
    /// </summary>
    /// <param name="detector">Existing detector.</param>
    /// <param name="source">Data source.</param>
    /// <returns>Detector.</returns>
    public static Detector _OGA(this Detector? detector, object? source = null) =>
        new(detector, ResolveSource(detector, source), o => o switch
        {
            Processor p when
                p.HasMagic((ReadOnlySpan<byte>)new byte[] { 0x4f, 0x67, 0x67, 0x53 }) => ".oga",
            ReadOnlyMemory<byte> m when
                HasMagic(m.Span, (ReadOnlySpan<byte>)new byte[] { 0x4f, 0x67, 0x67, 0x53 }) => ".oga",
            _ => null
        });

    #endregion

    #endregion
}