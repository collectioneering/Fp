using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using Fp.Plus.Audio;

namespace Fp.Plus.Audio
{
    /// <summary>
    /// PCM audio data.
    /// </summary>
    public class PcmData : BufferData<byte>
    {
        /// <summary>
        /// PCM metadata.
        /// </summary>
        public readonly WaveInfo WaveInfo;

        private bool _disposed;

        /// <summary>
        /// Creates a new instance of <see cref="PcmData"/>.
        /// </summary>
        /// <param name="basePath">Base path of resource.</param>
        /// <param name="waveInfo">PCM info.</param>
        public PcmData(string basePath, WaveInfo waveInfo) : base(basePath, waveInfo.SubChunk2Size)
        {
            Dry = true;
            WaveInfo = waveInfo;
        }

        /// <summary>
        /// Creates a new instance of <see cref="PcmData"/>.
        /// </summary>
        /// <param name="basePath">Base path of resource.</param>
        /// <param name="waveInfo">PCM metadata.</param>
        /// <param name="memoryOwner">Owner of PCM data buffer.</param>
        /// <param name="count">Length of content.</param>
        public PcmData(string basePath, WaveInfo waveInfo, IMemoryOwner<byte> memoryOwner,
            int? count = default) : base(basePath, memoryOwner, count)
        {
            WaveInfo = waveInfo;
        }

        /// <summary>
        /// Creates a new instance of <see cref="PcmData"/>.
        /// </summary>
        /// <param name="basePath">Base path of resource.</param>
        /// <param name="waveInfo">PCM metadata.</param>
        /// <param name="buffer">PCM data.</param>
        public PcmData(string basePath, WaveInfo waveInfo, ReadOnlyMemory<byte> buffer) : base(basePath, buffer)
        {
            WaveInfo = waveInfo;
        }

        /// <inheritdoc />
        public override Guid DefaultFormat => Wave.WaveFormat;

        /// <inheritdoc />
        public override  IReadOnlyCollection<Guid> SupportedFormats { get; } = new[] { Wave.WaveFormat };

        /// <inheritdoc />
        public override string? GetExtension(Guid? format = null)
        {
            format ??= DefaultFormat;
            return format switch
            {
                _ when format == Wave.WaveFormat => ".wav",
                _ => base.GetExtension(format)
            };
        }

        /// <inheritdoc />
        public override bool SupportsFormat(Guid format, Dictionary<object, object>? formatOptions = null)
        {
            return format == Wave.WaveFormat;
        }

        /// <inheritdoc />
        public override bool WriteConvertedData(Stream outputStream, Guid format,
            Dictionary<object, object>? formatOptions = null)
        {
            if (Dry) throw new InvalidOperationException("Cannot convert a dry data container");
            if (_disposed)
                throw new ObjectDisposedException(nameof(PcmData));
            if (format == Wave.WaveFormat)
            {
                Wave.WriteWave(outputStream, WaveInfo, Buffer.Span);
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public override object Clone()
        {
            if (Dry)
                return new PcmData(BasePath, WaveInfo);
            if (_disposed)
                throw new ObjectDisposedException(nameof(PcmData));
            return new PcmData(BasePath, WaveInfo, Buffer.CloneBuffer());
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;
            _disposed = true;
            base.Dispose(disposing);
        }

        /// <inheritdoc />
        public override string ToString() => $"PCM {{ Path = {BasePath}, Buffer Length = {Count}, Info = {WaveInfo} }}";
    }
}

// ReSharper disable once CheckNamespace
namespace Fp.Plus {
    public partial class PlusUtil
    {
        /// <summary>
        /// Creates PCM audio data object.
        /// </summary>
        /// <param name="path">Path.</param>
        /// <param name="info">PCM information.</param>
        /// <param name="memory">Audio buffer.</param>
        /// <returns>Data object.</returns>
        public static PcmData Pcm(this FpPath path, WaveInfo info, ReadOnlyMemory<byte> memory) =>
            new(path.AsCombined(), info, memory);

        /// <summary>
        /// Creates PCM audio data object.
        /// </summary>
        /// <param name="path">Path.</param>
        /// <param name="info">PCM information.</param>
        /// <param name="memory">Audio buffer.</param>
        /// <returns>Data object.</returns>
        public static PcmData Pcm(this string path, WaveInfo info, ReadOnlyMemory<byte> memory) =>
            new(path, info, memory);
    }
}