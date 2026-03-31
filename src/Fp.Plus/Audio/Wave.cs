using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO;
using System.Runtime.InteropServices;

namespace Fp.Plus.Audio;

/// <summary>
/// Provides WAVE output support.
/// </summary>
public class Wave
{
    /// <summary>
    /// WAVE format.
    /// </summary>
    public static readonly Guid WaveFormat = Guid.Parse("3487955E-29BC-49FD-B374-C5AD6D2B145C");

    /// <summary>
    /// Index of "RIFF" chunk in <see cref="ChunkNames"/>.
    /// </summary>
    public const int IndexChunkRiff = 0;
    /// <summary>
    /// Index of "WAVE" chunk in <see cref="ChunkNames"/>.
    /// </summary>
    public const int IndexChunkWave = 4;
    /// <summary>
    /// Index of "fmt " chunk in <see cref="ChunkNames"/>.
    /// </summary>
    public const int IndexChunkFmt = 8;
    /// <summary>
    /// Index of "data" chunk in <see cref="ChunkNames"/>.
    /// </summary>
    public const int IndexChunkData = 12;
    /// <summary>
    /// Index of "fact" chunk in <see cref="ChunkNames"/>.
    /// </summary>
    public const int IndexChunkFact = 16;

    /// <summary>
    /// Standard chunk names.
    /// </summary>
    public static ReadOnlySpan<byte> ChunkNames => new byte[]
    {
        // "RIFF"
        0x52, 0x49, 0x46, 0x46,
        // "WAVE"
        0x57, 0x41, 0x56, 0x45,
        // "fmt "
        0x66, 0x6d, 0x74, 0x20,
        // "data"
        0x64, 0x61, 0x74, 0x61,
        // "fact"
        0x66, 0x61, 0x63, 0x74
    };

    /// <summary>
    /// Writes a WAVE format file with float data.
    /// </summary>
    /// <param name="outputStream">Output stream.</param>
    /// <param name="numChannels">Mono = 1, Stereo = 2, etc.</param>
    /// <param name="sampleRate">8000, 44100, etc.</param>
    /// <param name="numSamples">Number of samples (shared count between channels).</param>
    /// <param name="data">Raw content buffer.</param>
    /// <exception cref="ArgumentException">Thrown if invalid data parameters were detected.</exception>
    /// <remarks>
    /// Implementation derived from format details at http://soundfile.sapp.org/doc/WaveFormat/
    /// </remarks>
    public static void WriteFloatWave<T>(Stream outputStream, short numChannels, int sampleRate, int numSamples, ReadOnlySpan<T> data) where T : unmanaged
    {
        if (typeof(T) != typeof(float) && typeof(T) != typeof(double)) throw new ArgumentException("Invalid data type for float data");
        WriteWave(outputStream, WaveInfo.CreateFloat<T>(numChannels, sampleRate, numSamples), MemoryMarshal.Cast<T, byte>(data));
    }

    /// <summary>
    /// Writes a WAVE format file with float data.
    /// </summary>
    /// <param name="outputStream">Output stream.</param>
    /// <param name="waveInfo">Float info.</param>
    /// <param name="data">Raw content buffer.</param>
    /// <exception cref="ArgumentException">Thrown if invalid data parameters were detected.</exception>
    /// <remarks>
    /// Implementation derived from format details at http://soundfile.sapp.org/doc/WaveFormat/
    /// </remarks>
    public static void WriteFloatWave<T>(Stream outputStream, WaveInfo waveInfo, ReadOnlySpan<T> data) where T : unmanaged
    {
        if (typeof(T) != typeof(float) && typeof(T) != typeof(double)) throw new ArgumentException("Invalid data type for float data");
        if (waveInfo.AudioFormat != 3) throw new ArgumentException($"Audio format {waveInfo.AudioFormat} was unexpected for float which should have format 3");
        WriteWave(outputStream, waveInfo, MemoryMarshal.Cast<T, byte>(data));
    }

    /// <summary>
    /// Writes a WAVE format file with stereo float data.
    /// </summary>
    /// <param name="outputStream">Output stream.</param>
    /// <param name="sampleRate">8000, 44100, etc.</param>
    /// <param name="numSamples">Number of samples (shared count between channels).</param>
    /// <param name="left">Raw content buffer for left channel.</param>
    /// <param name="right">Raw content buffer for right channel.</param>
    /// <exception cref="ArgumentException">Thrown if invalid data parameters were detected.</exception>
    /// <remarks>
    /// Implementation derived from format details at http://soundfile.sapp.org/doc/WaveFormat/
    /// </remarks>
    public static void WriteStereoFloatWave<T>(Stream outputStream, int sampleRate, int numSamples, ReadOnlySpan<T> left, ReadOnlySpan<T> right) where T : unmanaged
    {
        if (typeof(T) != typeof(float) && typeof(T) != typeof(double)) throw new ArgumentException("Invalid data type for float data");
        WriteStereoFloatWave(outputStream, WaveInfo.CreateFloat<T>(2, sampleRate, numSamples), left, right);
    }

    /// <summary>
    /// Writes a WAVE format file with stereo float data.
    /// </summary>
    /// <param name="outputStream">Output stream.</param>
    /// <param name="waveInfo">Float info.</param>
    /// <param name="left">Raw content buffer for left channel.</param>
    /// <param name="right">Raw content buffer for right channel.</param>
    /// <exception cref="ArgumentException">Thrown if invalid data parameters were detected.</exception>
    /// <remarks>
    /// Implementation derived from format details at http://soundfile.sapp.org/doc/WaveFormat/
    /// </remarks>
    public static void WriteStereoFloatWave<T>(Stream outputStream, WaveInfo waveInfo, ReadOnlySpan<T> left, ReadOnlySpan<T> right) where T : unmanaged
    {
        if (typeof(T) != typeof(float) && typeof(T) != typeof(double)) throw new ArgumentException("Invalid data type for float data");
        if (waveInfo.AudioFormat != 3) throw new ArgumentException($"Audio format {waveInfo.AudioFormat} was unexpected for float which should have format 3");
        if (left.Length != right.Length) throw new ArgumentException("Left and right buffers should have same length");
        T[] iBuf = ArrayPool<T>.Shared.Rent(left.Length * 2);
        try
        {
            for (int i = 0; i < left.Length; i++)
            {
                iBuf[i * 2] = left[i];
                iBuf[i * 2 + 1] = right[i];
            }
            WriteWave(outputStream, waveInfo, iBuf.AsSpan(0, left.Length * 2));
        }
        finally
        {
            ArrayPool<T>.Shared.Return(iBuf);
        }
    }

    /// <summary>
    /// Writes a WAVE format file with PCM.
    /// </summary>
    /// <param name="outputStream">Output stream.</param>
    /// <param name="numChannels">Mono = 1, Stereo = 2, etc.</param>
    /// <param name="sampleRate">8000, 44100, etc.</param>
    /// <param name="numSamples">Number of samples (shared count between channels).</param>
    /// <param name="data">Raw content buffer.</param>
    /// <exception cref="ArgumentException">Thrown if invalid data parameters were detected.</exception>
    /// <remarks>
    /// Implementation derived from format details at http://soundfile.sapp.org/doc/WaveFormat/
    /// </remarks>
    public static void WritePcmWave<T>(Stream outputStream, short numChannels, int sampleRate, int numSamples, ReadOnlySpan<T> data) where T : unmanaged
    {
        if (typeof(T) != typeof(byte) && typeof(T) != typeof(sbyte) &&
            typeof(T) != typeof(ushort) && typeof(T) != typeof(short) &&
            typeof(T) != typeof(uint) && typeof(T) != typeof(int) &&
            typeof(T) != typeof(ulong) && typeof(T) != typeof(long))
            throw new ArgumentException("Invalid data type for PCM data");
        WriteWave(outputStream, WaveInfo.CreateInteger<T>(numChannels, sampleRate, numSamples), MemoryMarshal.Cast<T, byte>(data));
    }

    /// <summary>
    /// Writes a WAVE format file with PCM.
    /// </summary>
    /// <param name="outputStream">Output stream.</param>
    /// <param name="waveInfo">PCM or float info.</param>
    /// <param name="data">Raw content buffer.</param>
    /// <exception cref="ArgumentException">Thrown if invalid data parameters were detected.</exception>
    /// <remarks>
    /// Implementation derived from format details at http://soundfile.sapp.org/doc/WaveFormat/
    /// </remarks>
    public static unsafe void WritePcmWave<T>(Stream outputStream, WaveInfo waveInfo, ReadOnlySpan<T> data) where T : unmanaged
    {
        if (typeof(T) != typeof(byte) && typeof(T) != typeof(sbyte) &&
            typeof(T) != typeof(ushort) && typeof(T) != typeof(short) &&
            typeof(T) != typeof(uint) && typeof(T) != typeof(int) &&
            typeof(T) != typeof(ulong) && typeof(T) != typeof(long))
            throw new ArgumentException("Invalid data type for PCM data");
        if (waveInfo.AudioFormat != 1) throw new ArgumentException($"Audio format {waveInfo.AudioFormat} was unexpected for PCM which should have format 1");
        if (waveInfo.BitsPerSample != sizeof(T) * 8) throw new ArgumentException($"Mismatch between {nameof(WaveInfo.BitsPerSample)} and data type size");
        WriteWave(outputStream, waveInfo, data);
    }

    /// <summary>
    /// Writes a WAVE format file.
    /// </summary>
    /// <param name="outputStream">Output stream.</param>
    /// <param name="waveInfo">PCM or float info.</param>
    /// <param name="data">Raw content buffer.</param>
    /// <exception cref="ArgumentException">Thrown if invalid data parameters were detected.</exception>
    /// <remarks>
    /// Implementation derived from format details at http://soundfile.sapp.org/doc/WaveFormat/
    /// </remarks>
    public static void WriteWave<T>(Stream outputStream, WaveInfo waveInfo, ReadOnlySpan<T> data) where T : unmanaged
    {
        ReadOnlySpan<byte> bData = MemoryMarshal.Cast<T, byte>(data);
        if (waveInfo.SubChunk1Size is > 0x10 and < 0x12) throw new ArgumentException($"Unexpected {nameof(WaveInfo.SubChunk1Size)}");
        if (waveInfo.SubChunk2Size > bData.Length) throw new ArgumentException($"{nameof(data)} not long enough to populate {nameof(WaveInfo.SubChunk2Size)} bytes");
        int factSize = 8 + (waveInfo.Fact?.Length ?? -8);
        int hLen = 12 + 8 + waveInfo.SubChunk1Size + factSize + 8;
        byte[] buffer = ArrayPool<byte>.Shared.Rent(hLen);
        Span<byte> bufferSpan = buffer.AsSpan(0, hLen);
        bufferSpan.Clear();
        try
        {
            // RIFF (main chunk)
            ChunkNames.Slice(IndexChunkRiff, 4).CopyTo(bufferSpan.Slice(0));
            BinaryPrimitives.WriteInt32LittleEndian(bufferSpan.Slice(4), 4 + 8 + waveInfo.SubChunk1Size + factSize + 8 + waveInfo.SubChunk2Size);
            // WAVE chunks
            ChunkNames.Slice(IndexChunkWave, 4).CopyTo(bufferSpan.Slice(8));
            // fmt (subchunk1)
            ChunkNames.Slice(IndexChunkFmt, 4).CopyTo(bufferSpan.Slice(0xC));
            BinaryPrimitives.WriteInt32LittleEndian(bufferSpan.Slice(0x10), waveInfo.SubChunk1Size);
            BinaryPrimitives.WriteInt16LittleEndian(bufferSpan.Slice(0x14), waveInfo.AudioFormat);
            BinaryPrimitives.WriteInt16LittleEndian(bufferSpan.Slice(0x16), waveInfo.NumChannels);
            BinaryPrimitives.WriteInt32LittleEndian(bufferSpan.Slice(0x18), waveInfo.SampleRate);
            BinaryPrimitives.WriteInt32LittleEndian(bufferSpan.Slice(0x1C), waveInfo.ByteRate);
            BinaryPrimitives.WriteInt16LittleEndian(bufferSpan.Slice(0x20), waveInfo.BlockAlign);
            BinaryPrimitives.WriteInt16LittleEndian(bufferSpan.Slice(0x22), waveInfo.BitsPerSample);
            if (waveInfo.SubChunk1Size >= 0x12)
            {
                BinaryPrimitives.WriteInt16LittleEndian(bufferSpan.Slice(0x24), waveInfo.ExtraParamSize);
                waveInfo.ExtraParams?.Span.Slice(0, waveInfo.ExtraParamSize).CopyTo(bufferSpan.Slice(0x26));
            }
            // fact (if applicable)
            if (waveInfo.Fact is { } fact)
            {
                ChunkNames.Slice(IndexChunkFact, 4).CopyTo(bufferSpan.Slice(12 + 8 + waveInfo.SubChunk1Size));
                BinaryPrimitives.WriteInt32LittleEndian(bufferSpan.Slice(12 + 8 + waveInfo.SubChunk1Size + 4), fact.Length);
                fact.Span.CopyTo(bufferSpan.Slice(12 + 8 + waveInfo.SubChunk1Size + 8));
            }
            // data (subchunk2)
            int dataPos = 12 + 8 + waveInfo.SubChunk1Size + factSize;
            ChunkNames.Slice(IndexChunkData, 4).CopyTo(bufferSpan.Slice(dataPos));
            BinaryPrimitives.WriteInt32LittleEndian(bufferSpan.Slice(dataPos + 4), waveInfo.SubChunk2Size);

            outputStream.Write(buffer, 0, hLen);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }

        Processor.Write(outputStream, bData);
    }
}
