using System;
using static Fp.Detector;
using static Fp.Processor;

// ReSharper disable once CheckNamespace
namespace Fp.Plus;

public partial class PlusUtil
{
    #region Detectors

    #region AVI

    /// <summary>
    /// Detects WAV audio files.
    /// </summary>
    /// <returns>Detector.</returns>
    public static Detector _AVI() => _AVI(null);

    /// <summary>
    /// Detects WAV audio files.
    /// </summary>
    /// <param name="source">Data source.</param>
    /// <returns>Detector.</returns>
    public static Detector _AVI(this object source) => _AVI(null, source);

    /// <summary>
    /// Detects WAV audio files.
    /// </summary>
    /// <param name="detector">Existing detector.</param>
    /// <param name="source">Data source.</param>
    /// <returns>Detector.</returns>
    public static Detector _AVI(this Detector? detector, object? source = null) =>
        new(detector, ResolveSource(detector, source), o => o switch
        {
            Processor p when
                p.HasMagic((ReadOnlySpan<byte>)new[] { (byte)'R', (byte)'I', (byte)'F', (byte)'F' }) &&
                p.HasMagic((ReadOnlySpan<byte>)new[] { (byte)'A', (byte)'V', (byte)'I', (byte)' ' }, 8) => ".avi",
            ReadOnlyMemory<byte> m when
                HasMagic(m.Span, (ReadOnlySpan<byte>)new[] { (byte)'R', (byte)'I', (byte)'F', (byte)'F' }) &&
                HasMagic(m.Span, (ReadOnlySpan<byte>)new[] { (byte)'A', (byte)'V', (byte)'I', (byte)' ' },
                    8) => ".avi",
            _ => null
        });

    #endregion

    #region Ogv

    /// <summary>
    /// Detects Ogg video containers.
    /// </summary>
    /// <returns>Detector.</returns>
    public static Detector _OGV() => _OGV(null);

    /// <summary>
    /// Detects Ogg video containers.
    /// </summary>
    /// <param name="source">Data source.</param>
    /// <returns>Detector.</returns>
    public static Detector _OGV(this object source) => _OGV(null, source);

    /// <summary>
    /// Detects Ogg video containers.
    /// </summary>
    /// <param name="detector">Existing detector.</param>
    /// <param name="source">Data source.</param>
    /// <returns>Detector.</returns>
    public static Detector _OGV(this Detector? detector, object? source = null) =>
        new(detector, ResolveSource(detector, source), o => o switch
        {
            Processor p when
                p.HasMagic((ReadOnlySpan<byte>)new byte[] { 0x4f, 0x67, 0x67, 0x53 }) => ".ogv",
            ReadOnlyMemory<byte> m when
                HasMagic(m.Span, (ReadOnlySpan<byte>)new byte[] { 0x4f, 0x67, 0x67, 0x53 }) => ".ogv",
            _ => null
        });

    #endregion

    #endregion
}
