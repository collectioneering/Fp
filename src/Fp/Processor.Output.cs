using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using static System.Buffers.ArrayPool<byte>;

namespace Fp;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
public partial class Processor
{
    #region Output from stream utilities

    private static long WriteBaseStream(Stream stream, long length, Stream outputStream, bool lenient, int bufferLength)
    {
        long outLen = 0;
        byte[] buffer = Shared.Rent(bufferLength);
        try
        {
            long left = length;
            int read;
            do
            {
                read = stream.Read(buffer, 0, (int)Math.Min(left, buffer.Length));
                outputStream.Write(buffer, 0, read);
                left -= read;
                outLen += read;
            } while (left > 0 && read != 0);

            if (left > 0 && read != 0 && !lenient)
                throw new IOException($"Failed to read required number of bytes! 0x{read:X} read, 0x{left:X} left, 0x{stream.Position:X} end position");
        }
        finally
        {
            Shared.Return(buffer);
        }

        return outLen;
    }

    /// <summary>
    /// Outputs data from stream to stream.
    /// </summary>
    /// <param name="stream">Stream to read from.</param>
    /// <param name="outputStream">Stream to write to.</param>
    /// <returns>Length of written data.</returns>
    public static long OutputAll(Stream stream, Stream outputStream)
    {
        long pos = outputStream.Position;
        stream.CopyTo(outputStream);
        return outputStream.Position - pos;
    }

    /// <summary>
    /// Outputs data from stream to stream.
    /// </summary>
    /// <param name="stream">Stream to read from.</param>
    /// <param name="length">Number of bytes to read.</param>
    /// <param name="outputStream">Stream to write to.</param>
    /// <param name="lenient">If false, throws upon failure to read required number of bytes.</param>
    /// <param name="bufferLength">Minimum buffer length.</param>
    /// <returns>Length of written data.</returns>
    /// <exception cref="IOException">Thrown if <paramref name="lenient"/> is false and not enough bytes are available from <paramref name="stream"/></exception>
    public long Output(Stream stream, long length, Stream? outputStream = null, bool lenient = true, int bufferLength = 4096)
        => WriteBaseStream(stream, length, outputStream ?? OutputStream ?? throw new InvalidOperationException(), lenient, bufferLength);

    /// <summary>
    /// Outputs data from stream to stream.
    /// </summary>
    /// <param name="stream">Stream to read from.</param>
    /// <param name="offset">Offset in stream to read from.</param>
    /// <param name="length">Number of bytes to read.</param>
    /// <param name="outputStream">Stream to write to.</param>
    /// <param name="lenient">If false, throws upon failure to read required number of bytes.</param>
    /// <param name="bufferLength">Minimum buffer length.</param>
    /// <returns>Length of written data.</returns>
    /// <exception cref="IOException">Thrown if <paramref name="lenient"/> is false and not enough bytes are available from <paramref name="stream"/></exception>
    /// <remarks>Original position of <paramref name="stream"/> is restored on completion.</remarks>
    public long Output(Stream stream, long offset, long length, Stream? outputStream = null, bool lenient = true,
        int bufferLength = 4096)
    {
        outputStream ??= OutputStream ?? throw new InvalidOperationException();
        long origPos = stream.Position;
        try
        {
            stream.Position = offset;
            long outLen = Output(stream, length, outputStream, lenient, bufferLength);
            return outLen;
        }
        finally
        {
            stream.Position = origPos;
        }
    }

    /// <summary>
    /// Outputs data from current file's input stream to output stream.
    /// </summary>
    /// <param name="lenient">If false, throws upon failure to read required number of bytes.</param>
    /// <param name="bufferLength">Minimum buffer length.</param>
    /// <returns>Length of written data.</returns>
    /// <exception cref="IOException">Thrown if <paramref name="lenient"/> is false and not enough bytes are available from input stream.</exception>
    public long Output(bool lenient = true, int bufferLength = 4096)
        => Output(_inputStream ?? throw new InvalidOperationException(), long.MaxValue, OutputStream, lenient, bufferLength);

    /// <summary>
    /// Outputs data from current file's input stream to output stream.
    /// </summary>
    /// <param name="length">Number of bytes to read.</param>
    /// <param name="lenient">If false, throws upon failure to read required number of bytes.</param>
    /// <param name="bufferLength">Minimum buffer length.</param>
    /// <returns>Length of written data.</returns>
    /// <exception cref="IOException">Thrown if <paramref name="lenient"/> is false and not enough bytes are available from input stream.</exception>
    public long Output(long length, bool lenient = true, int bufferLength = 4096)
        => Output(_inputStream ?? throw new InvalidOperationException(), length, OutputStream, lenient, bufferLength);

    /// <summary>
    /// Outputs data from current file's input stream to output stream.
    /// </summary>
    /// <param name="offset">Offset in stream to read from.</param>
    /// <param name="length">Number of bytes to read.</param>
    /// <param name="lenient">If false, throws upon failure to read required number of bytes.</param>
    /// <param name="bufferLength">Minimum buffer length.</param>
    /// <returns>Length of written data.</returns>
    /// <exception cref="IOException">Thrown if <paramref name="lenient"/> is false and not enough bytes are available from input stream.</exception>
    /// <remarks>Original position of input stream is restored on completion.</remarks>
    public long Output(long offset, long length, bool lenient = true, int bufferLength = 4096)
    {
        long origPos = (_inputStream ?? throw new InvalidOperationException()).Position;
        try
        {
            _inputStream.Position = offset;
            long outLen = Output(_inputStream, length, OutputStream, lenient, bufferLength);
            return outLen;
        }
        finally
        {
            _inputStream.Position = origPos;
        }
    }

    #endregion

    #region Output from array utilities

    private static void WriteBaseArray(Stream stream, byte[] array, int offset, int length) =>
        stream.Write(array, offset, length);

    /// <summary>
    /// Outputs data from array to stream from specified offset.
    /// </summary>
    /// <param name="stream">Stream to write to.</param>
    /// <param name="offset">Offset to write to.</param>
    /// <param name="array">Span to read from.</param>
    [Obsolete("Succeeded by: public static void Write(Stream stream, long offset, ReadOnlySpan<byte> span)")]
    public static void Write(Stream stream, long offset, byte[] array)
    {
        stream.Position = offset;
        stream.Write(array, 0, array.Length);
    }

    /// <summary>
    /// Outputs data from array to stream from specified offset.
    /// </summary>
    /// <param name="stream">Stream to write to.</param>
    /// <param name="offset">Offset to write to.</param>
    /// <param name="array">Span to read from.</param>
    /// <param name="arrayOffset">Array offset.</param>
    /// <param name="arrayLength">Array length.</param>
    [Obsolete("Succeeded by: public static void Write(Stream stream, long offset, ReadOnlySpan<byte> span)")]
    public static void Write(Stream stream, long offset, byte[] array, int arrayOffset, int arrayLength)
    {
        stream.Position = offset;
        stream.Write(array, arrayOffset, arrayLength);
    }

    /// <summary>
    /// Outputs data from array to stream.
    /// </summary>
    /// <param name="stream">Stream to write to.</param>
    /// <param name="array">Span to read from.</param>
    [Obsolete("Succeeded by: public void Stream.Write(ReadOnlySpan<byte> buffer)")]
    public static void Write(Stream stream, byte[] array) =>
        WriteBaseArray(stream, array, 0, array.Length);

    /// <summary>
    /// Outputs data from array to stream.
    /// </summary>
    /// <param name="stream">Stream to write to.</param>
    /// <param name="array">Span to read from.</param>
    /// <param name="arrayOffset">Array offset.</param>
    /// <param name="arrayLength">Array length.</param>
    [Obsolete("Succeeded by: public void Stream.Write(ReadOnlySpan<byte> buffer)")]
    public static void Write(Stream stream, byte[] array, int arrayOffset, int arrayLength) =>
        WriteBaseArray(stream, array, arrayOffset, arrayLength);

    /// <summary>
    /// Outputs data from array to stream.
    /// </summary>
    /// <param name="array">Array to read from.</param>
    /// <param name="outputStream">Stream to write to.</param>
    [Obsolete("Succeeded by: public void Stream.Write(ReadOnlySpan<byte> buffer)")]
    public void OutputAll(byte[] array, Stream outputStream)
        => WriteBaseArray(outputStream, array, 0, array.Length);

    /// <summary>
    /// Outputs data from array to stream.
    /// </summary>
    /// <param name="array">Array to read from.</param>
    /// <param name="offset">Offset in stream to read from.</param>
    /// <param name="length">Number of bytes to read.</param>
    /// <param name="outputStream">Stream to write to, or <see cref="OutputStream"/> if null.</param>
    /// <exception cref="InvalidOperationException">Thrown when no stream is provided and <see cref="OutputStream"/> is not set.</exception>
    [Obsolete("Succeeded by: public void Stream.Write(ReadOnlySpan<byte> buffer)")]
    public void Output(byte[] array, int offset, int length, Stream? outputStream = null) =>
        WriteBaseArray(outputStream ?? OutputStream ?? throw new InvalidOperationException(), array, offset, length);

    #endregion

    #region Output from span utilities

    /// <summary>
    /// Outputs data from span to stream from specified offset.
    /// </summary>
    /// <param name="stream">Stream to write to.</param>
    /// <param name="offset">Offset to write to.</param>
    /// <param name="span">Span to read from.</param>
    public static void Write(Stream stream, long offset, ReadOnlySpan<byte> span)
    {
        stream.Position = offset;
        stream.Write(span);
    }

    /// <summary>
    /// Outputs data from span to stream.
    /// </summary>
    /// <param name="stream">Stream to write to.</param>
    /// <param name="span">Span to read from.</param>
    public static void Write(Stream stream, ReadOnlySpan<byte> span) => stream.Write(span);

    /// <summary>
    /// Outputs data from span to current file's output stream.
    /// </summary>
    /// <param name="span">Span to read from.</param>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="OutputStream"/> is not set.</exception>
    public void OutputAll(ReadOnlySpan<byte> span) => (OutputStream ?? throw new InvalidOperationException()).Write(span);

    #endregion
}
