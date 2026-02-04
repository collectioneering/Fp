using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Fp
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
    public partial class Processor
    {
        #region Stream read utilities

        /// <summary>
        /// Gets a read-optimized stream from an existing stream, disposing the passed stream if it is replaced.
        /// </summary>
        /// <param name="stream">Stream to read.</param>
        /// <returns>Equivalent stream optimized for reading.</returns>
        public Stream GetReadingStream(Stream stream)
        {
            if (Preload && (stream is not MemoryStream alreadyMs || !alreadyMs.TryGetBuffer(out _)))
            {
                MemoryStream ms = new(new byte[stream.Length]);
                stream.CopyTo(ms);
                stream.Dispose();
                stream = ms;
            }

            if (stream is FileStream)
                stream = new MultiBufferStream(stream, true);
            return stream;
        }

        /// <summary>
        /// Gets a seekable stream (closes base stream if replaced).
        /// </summary>
        /// <param name="stream">Base stream.</param>
        /// <returns>Seekable stream.</returns>
        /// <remarks>
        /// This method conditionally creates a seekable stream from a non-seekable stream by copying the
        /// stream's contents to a new <see cref="MemoryStream"/> instance. The returned object is either
        /// this newly created stream or the passed argument <paramref name="stream"/> if it was already seekable.
        /// </remarks>
        public static Stream GetSeekableStream(Stream stream)
        {
            if (stream.CanSeek)
            {
                return stream;
            }

            MemoryStream ms;
            try
            {
                long length = stream.Length;
                ms = length > int.MaxValue ? new MemoryStream() : new MemoryStream(new byte[length]);
            }
            catch
            {
                ms = new MemoryStream();
            }

            stream.CopyTo(ms);
            stream.Close();
            ms.Position = 0;
            stream = ms;
            return stream;
        }

        /// <summary>
        /// Skips data in stream.
        /// </summary>
        /// <param name="bytes">Number of bytes to skip.</param>
        /// <param name="stream">Stream to operate on.</param>
        /// <returns>New position in stream.</returns>
        public static long Skip(long bytes, Stream stream)
            => stream.Seek(bytes, SeekOrigin.Current);

        /// <summary>
        /// Skips data in current file's input stream.
        /// </summary>
        /// <param name="bytes">Number of bytes to skip.</param>
        /// <returns>New position in stream.</returns>
        public long Skip(long bytes)
            => (_inputStream ?? throw new InvalidOperationException()).Seek(bytes, SeekOrigin.Current);

        internal static int ReadBaseArray(Stream stream, byte[] array, int offset, int length, bool lenient)
        {
            int left = length, read, tot = 0;
            do
            {
                read = stream.Read(array, offset + tot, left);
                left -= read;
                tot += read;
            } while (left > 0 && read != 0);

            if (left > 0 && read == 0 && !lenient)
            {
                throw new IOException(
                    $"Failed to read required number of bytes! 0x{read:X} read, 0x{left:X} left, 0x{stream.Position:X} end position");
            }

            return tot;
        }

        internal static int ReadBaseSpan(Stream stream, Span<byte> span, bool lenient)
        {
            int left = span.Length, read, tot = 0;
            do
            {
                read = stream.Read(span[tot..]);
                read = Math.Clamp(read, 0, left); // sus
                left -= read;
                tot += read;
            } while (left > 0 && read != 0);

            if (left > 0 && !lenient)
            {
                throw new IOException(
                    $"Failed to read required number of bytes! 0x{read:X} read, 0x{left:X} left, 0x{stream.Position:X} end position");
            }

            return tot;
        }

        internal static bool TryReadBaseSpan(Stream stream, Span<byte> span, out int count)
        {
            int left = span.Length, read;
            count = 0;
            do
            {
                read = stream.Read(span[count..]);
                read = Math.Clamp(read, 0, left); // sus
                left -= read;
                count += read;
            } while (left > 0 && read != 0);
            return left <= 0;
        }

        private static bool TryGetRawMemoryFromStream(Stream stream, out Memory<byte> buffer, out int position)
        {
            if (!stream.CanWrite)
            {
                buffer = Memory<byte>.Empty;
                position = -1;
                return false;
            }
            switch (stream)
            {
                case MStream mStream:
                    {
                        buffer = mStream.GetWriteableMemory();
                        position = Math.Clamp((int)mStream.Position, 0, buffer.Length);
                        break;
                    }
                case MemoryStream ms:
                    {
                        if (ms.TryGetBuffer(out ArraySegment<byte> buf))
                        {
                            buffer = buf;
                            position = Math.Clamp((int)ms.Position, 0, buffer.Length);
                            return true;
                        }
                        break;
                    }
            }
            buffer = Memory<byte>.Empty;
            position = -1;
            return false;
        }

        private static bool TryGetRawReadOnlyMemoryFromStream(Stream stream, out ReadOnlyMemory<byte> buffer, out int position)
        {
            if (!stream.CanRead)
            {
                buffer = ReadOnlyMemory<byte>.Empty;
                position = -1;
                return false;
            }
            switch (stream)
            {
                case MStream mStream:
                    {
                        buffer = mStream.GetMemory();
                        position = Math.Clamp((int)mStream.Position, 0, buffer.Length);
                        break;
                    }
                case MemoryStream memoryStream:
                    {
                        if (memoryStream.TryGetBuffer(out ArraySegment<byte> buf))
                        {
                            buffer = buf;
                            position = Math.Clamp((int)memoryStream.Position, 0, buffer.Length);
                            return true;
                        }
                        break;
                    }
            }
            buffer = ReadOnlyMemory<byte>.Empty;
            position = -1;
            return false;
        }

        #region Stream to span

        /// <summary>
        /// Reads data from stream, optionally replacing reference to provided span to prevent copy when reading from <see cref="MemoryStream"/>.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <param name="span">Target to copy to, may be replaced by an internal memory buffer.</param>
        /// <param name="lenient">If false, throws when failed to fill target.</param>
        /// <param name="forceUseCurrentSpan">Force use provided span.</param>
        /// <returns>Number of bytes read.</returns>
        /// <exception cref="IOException">Thrown when <paramref name="lenient"/> is false
        /// and stream cannot provide enough data to fill target.</exception>
        /// <remarks>
        /// This method advances <paramref name="stream"/> to the end of the read data.
        /// </remarks>
        public static int Read(Stream stream, ref Span<byte> span, bool lenient = true, bool forceUseCurrentSpan = false)
        {
            if (forceUseCurrentSpan)
            {
                if (!TryGetRawReadOnlyMemoryFromStream(stream, out var buffer, out int position))
                    return ReadBaseSpan(stream, span, lenient);
                int availableBytes = buffer.Span.Length - position;
                int readLength = span.Length;
                if (availableBytes < readLength)
                {
                    if (!lenient) throw new IOException($"{nameof(stream)} does not have enough data to fill the specified buffer; {availableBytes} bytes available, {readLength} bytes requested");
                    readLength = availableBytes;
                }
                stream.Seek(readLength, SeekOrigin.Current);
                buffer.Span.Slice(position, readLength).CopyTo(span);
                return readLength;
            }
            else
            {
                if (!TryGetRawMemoryFromStream(stream, out var buffer, out int position))
                    return ReadBaseSpan(stream, span, lenient);
                int availableBytes = buffer.Span.Length - position;
                int readLength = span.Length;
                if (availableBytes < readLength)
                {
                    if (!lenient) throw new IOException($"{nameof(stream)} does not have enough data to fill the specified buffer; {availableBytes} bytes available, {readLength} bytes requested");
                    readLength = availableBytes;
                }
                stream.Seek(readLength, SeekOrigin.Current);
                span = buffer.Span.Slice(position, readLength);
                return readLength;
            }
        }

        /// <summary>
        /// Reads data from stream.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <param name="length">Number of bytes to try to read.</param>
        /// <param name="span">Result buffer, valid if completed read or <paramref name="lenient"/> is true.</param>
        /// <param name="lenient">If false, throws when failed to fill target.</param>
        /// <param name="forceNew">Force use newly allocated buffer.</param>
        /// <returns>Number of bytes read.</returns>
        /// <exception cref="IOException">Thrown when <paramref name="lenient"/> is false
        /// and stream cannot provide enough data to fill target.</exception>
        /// <remarks>
        /// This method advances <paramref name="stream"/> to the end of the read data.
        /// </remarks>
        public static int Read(Stream stream, int length, out Span<byte> span, bool lenient = true, bool forceNew = false)
        {
            if (forceNew)
            {
                if (!TryGetRawReadOnlyMemoryFromStream(stream, out var buffer, out int position))
                    return ReadBaseSpan(stream, span = new Span<byte>(new byte[length]), lenient);
                int availableBytes = buffer.Span.Length - position;
                int readLength = length;
                if (availableBytes < readLength)
                {
                    if (!lenient) throw new IOException($"{nameof(stream)} does not have enough data to fill the requested number of bytes; {availableBytes} bytes available, {readLength} bytes requested");
                    readLength = availableBytes;
                }
                stream.Seek(readLength, SeekOrigin.Current);
                span = new Span<byte>(new byte[readLength]);
                buffer.Span.Slice(position, readLength).CopyTo(span);
                return readLength;
            }
            else
            {
                if (!TryGetRawMemoryFromStream(stream, out var buffer, out int position))
                    return ReadBaseSpan(stream, span = new Span<byte>(new byte[length]), lenient);
                int availableBytes = buffer.Span.Length - position;
                int readLength = length;
                if (availableBytes < readLength)
                {
                    if (!lenient) throw new IOException($"{nameof(stream)} does not have enough data to fill the requested number of bytes; {availableBytes} bytes available, {readLength} bytes requested");
                    readLength = availableBytes;
                }
                stream.Seek(readLength, SeekOrigin.Current);
                span = buffer.Span.Slice(position, readLength);
                return readLength;
            }
        }

        /// <summary>
        /// Attempts to read data from stream.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <param name="length">Number of bytes to try to read.</param>
        /// <param name="span">Result buffer, valid if completed read (buffer filled and returned true).</param>
        /// <param name="forceNew">Force use newly allocated buffer.</param>
        /// <returns>True if the buffer could be filled.</returns>
        /// <remarks>
        /// This method advances <paramref name="stream"/> to the end of the read data.
        /// </remarks>
        public static bool TryRead(Stream stream, int length, out Span<byte> span, bool forceNew = false)
        {
            if (forceNew)
            {
                if (!TryGetRawReadOnlyMemoryFromStream(stream, out var buffer, out int position))
                    return TryReadBaseSpan(stream, span = new Span<byte>(new byte[length]), out _);
                int availableBytes = buffer.Span.Length - position;
                if (availableBytes < length)
                {
                    stream.Seek(availableBytes, SeekOrigin.Current);
                    span = Span<byte>.Empty;
                    return false;
                }
                stream.Seek(length, SeekOrigin.Current);
                span = new Span<byte>(new byte[length]);
                buffer.Span.Slice(position, length).CopyTo(span);
                return true;
            }
            else
            {
                if (!TryGetRawMemoryFromStream(stream, out var buffer, out int position))
                    return TryReadBaseSpan(stream, span = new Span<byte>(new byte[length]), out _);
                int availableBytes = buffer.Span.Length - position;
                if (availableBytes < length)
                {
                    stream.Seek(length, SeekOrigin.Current);
                    span = Span<byte>.Empty;
                    return false;
                }
                stream.Seek(length, SeekOrigin.Current);
                span = buffer.Span.Slice(position, length);
                return true;
            }
        }

        /// <summary>
        /// Reads data from stream.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <param name="span">Target to copy to.</param>
        /// <param name="lenient">If false, throws when failed to fill target.</param>
        /// <returns>Number of bytes read.</returns>
        /// <exception cref="IOException">Thrown when <paramref name="lenient"/> is false
        /// and stream cannot provide enough data to fill target.</exception>
        /// <remarks>
        /// This method advances <paramref name="stream"/> to the end of the read data.
        /// </remarks>
        public static int Read(Stream stream, Span<byte> span, bool lenient = true)
        {
            if (!TryGetRawReadOnlyMemoryFromStream(stream, out var buffer, out int position))
                return ReadBaseSpan(stream, span, lenient);
            int availableBytes = buffer.Span.Length - position;
            int readLength = span.Length;
            if (availableBytes < readLength)
            {
                if (!lenient) throw new IOException($"{nameof(stream)} does not have enough data to fill the specified buffer; {availableBytes} bytes available, {readLength} bytes requested");
                readLength = availableBytes;
            }
            stream.Seek(readLength, SeekOrigin.Current);
            buffer.Span.Slice(position, readLength).CopyTo(span);
            return readLength;
        }

        /// <summary>
        /// Attempts to read data from stream.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <param name="span">Target to copy to.</param>
        /// <param name="readBytes">Number of successfully read bytes.</param>
        /// <returns>True if the buffer could be filled.</returns>
        /// <remarks>
        /// This method advances <paramref name="stream"/> to the end of the read data.
        /// </remarks>
        public static bool TryRead(Stream stream, Span<byte> span, out int readBytes)
        {
            if (!TryGetRawReadOnlyMemoryFromStream(stream, out var buffer, out int position))
                return TryReadBaseSpan(stream, span, out readBytes);
            int availableBytes = buffer.Span.Length - position;
            int requestedReadBytes = span.Length;
            readBytes = Math.Min(availableBytes, requestedReadBytes);
            stream.Seek(readBytes, SeekOrigin.Current);
            buffer.Span.Slice(position, readBytes).CopyTo(span);
            return requestedReadBytes == readBytes;
        }

        #endregion

        #region Implicit input stream to span

        /// <summary>
        /// Reads data from current file's input stream, optionally replacing reference to provided span to prevent copy when reading from <see cref="MemoryStream"/>.
        /// </summary>
        /// <param name="span">Target to copy to, may be replaced by an internal memory buffer.</param>
        /// <param name="lenient">If false, throws when failed to fill target.</param>
        /// <param name="forceUseCurrentSpan">Force use provided span.</param>
        /// <returns>Number of bytes read.</returns>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="InputStream"/> is not set.</exception>
        /// <exception cref="IOException">Thrown when <paramref name="lenient"/> is false
        /// and stream cannot provide enough data to fill target.</exception>
        /// <remarks>
        /// This method advances <see cref="InputStream"/> to the end of the read data.
        /// </remarks>
        public int Read(ref Span<byte> span, bool lenient = true, bool forceUseCurrentSpan = false)
            => Read(_inputStream ?? throw new InvalidOperationException(), ref span, lenient, forceUseCurrentSpan);

        /// <summary>
        /// Reads data from current file's input stream.
        /// </summary>
        /// <param name="length">Number of bytes to try to read.</param>
        /// <param name="span">Result buffer, valid if completed read or <paramref name="lenient"/> is true.</param>
        /// <param name="lenient">If false, throws when failed to fill target.</param>
        /// <param name="forceNew">Force use newly allocated buffer.</param>
        /// <returns>Number of bytes read.</returns>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="InputStream"/> is not set.</exception>
        /// <exception cref="IOException">Thrown when <paramref name="lenient"/> is false
        /// and stream cannot provide enough data to fill target.</exception>
        /// <remarks>
        /// This method advances <see cref="InputStream"/> to the end of the read data.
        /// </remarks>
        public int Read(int length, out Span<byte> span, bool lenient = true, bool forceNew = false)
            => Read(_inputStream ?? throw new InvalidOperationException(), length, out span, lenient, forceNew);

        /// <summary>
        /// Attempts to read data from current file's input stream.
        /// </summary>
        /// <param name="length">Number of bytes to try to read.</param>
        /// <param name="span">Result buffer, valid if completed read (buffer filled and returned true).</param>
        /// <param name="forceNew">Force use newly allocated buffer.</param>
        /// <returns>True if the buffer could be filled.</returns>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="InputStream"/> is not set.</exception>
        /// <remarks>
        /// This method advances <see cref="InputStream"/> to the end of the read data.
        /// </remarks>
        public bool TryRead(int length, out Span<byte> span, bool forceNew = false)
            => TryRead(_inputStream ?? throw new InvalidOperationException(), length, out span, forceNew);

        /// <summary>
        /// Reads data from current file's input stream.
        /// </summary>
        /// <param name="span">Target to copy to.</param>
        /// <param name="lenient">If false, throws when failed to fill target.</param>
        /// <returns>Number of bytes read.</returns>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="InputStream"/> is not set.</exception>
        /// <exception cref="IOException">Thrown when <paramref name="lenient"/> is false
        /// and stream cannot provide enough data to fill target.</exception>
        /// <remarks>
        /// This method advances <see cref="InputStream"/> to the end of the read data.
        /// </remarks>
        public int Read(Span<byte> span, bool lenient = true)
            => Read(_inputStream ?? throw new InvalidOperationException(), span, lenient);

        /// <summary>
        /// Attempts to read data from current file's input stream.
        /// </summary>
        /// <param name="span">Target to copy to.</param>
        /// <param name="readBytes">Number of successfully read bytes.</param>
        /// <returns>True if the buffer could be filled.</returns>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="InputStream"/> is not set.</exception>
        /// <remarks>
        /// This method advances <see cref="InputStream"/> to the end of the read data.
        /// </remarks>
        public bool TryRead(Span<byte> span, out int readBytes)
            => TryRead(_inputStream ?? throw new InvalidOperationException(), span, out readBytes);

        #endregion

        #region Offset stream to span

        /// <summary>
        /// Reads data from stream at the specified offset.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <param name="offset">Offset to read from.</param>
        /// <param name="span">Target to copy to.</param>
        /// <param name="lenient">If false, throws when failed to fill target.</param>
        /// <param name="forceUseCurrentSpan">Force use provided span.</param>
        /// <returns>Number of bytes read.</returns>
        /// <exception cref="IOException">Thrown when <paramref name="lenient"/> is false
        /// and stream cannot provide enough data to fill target.</exception>
        /// <remarks>Original position of <paramref name="stream"/> is restored on completion.</remarks>
        public static int Read(Stream stream, long offset, ref Span<byte> span, bool lenient = true,
            bool forceUseCurrentSpan = false)
        {
            long position = stream.Position;
            try
            {
                stream.Position = offset;
                int count = Read(stream, ref span, lenient, forceUseCurrentSpan);
                return count;
            }
            finally
            {
                stream.Position = position;
            }
        }

        /// <summary>
        /// Reads data from stream at the specified offset.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <param name="offset">Offset to read from.</param>
        /// <param name="length">Number of bytes to try to read.</param>
        /// <param name="span">Target to copy to.</param>
        /// <param name="lenient">If false, throws when failed to fill target.</param>
        /// <param name="forceNew">Force use newly allocated buffer.</param>
        /// <returns>Number of bytes read.</returns>
        /// <exception cref="IOException">Thrown when <paramref name="lenient"/> is false
        /// and stream cannot provide enough data to fill target.</exception>
        /// <remarks>Original position of <paramref name="stream"/> is restored on completion.</remarks>
        public static int Read(Stream stream, long offset, int length, out Span<byte> span, bool lenient = true,
            bool forceNew = false)
        {
            long position = stream.Position;
            try
            {
                stream.Position = offset;
                int count = Read(stream, length, out span, lenient, forceNew);
                return count;
            }
            finally
            {
                stream.Position = position;
            }
        }

        /// <summary>
        /// Attempts to read data from stream at the specified offset.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <param name="offset">Offset to read from.</param>
        /// <param name="length">Number of bytes to try to read.</param>
        /// <param name="span">Result buffer, valid if completed read (buffer filled and returned true).</param>
        /// <param name="forceNew">Force use newly allocated buffer.</param>
        /// <returns>True if the buffer could be filled.</returns>
        /// <remarks>Original position of <paramref name="stream"/> is restored on completion.</remarks>
        public static bool TryRead(Stream stream, long offset, int length, out Span<byte> span, bool forceNew = false)
        {
            long position = stream.Position;
            try
            {
                stream.Position = offset;
                return TryRead(stream, length, out span, forceNew);
            }
            finally
            {
                stream.Position = position;
            }
        }

        /// <summary>
        /// Reads data from stream at the specified offset.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <param name="offset">Offset to read from.</param>
        /// <param name="span">Target to copy to.</param>
        /// <param name="lenient">If false, throws when failed to fill target.</param>
        /// <returns>Number of bytes read.</returns>
        /// <exception cref="IOException">Thrown when <paramref name="lenient"/> is false
        /// and stream cannot provide enough data to fill target.</exception>
        /// <remarks>Original position of <paramref name="stream"/> is restored on completion.</remarks>
        public static int Read(Stream stream, long offset, Span<byte> span, bool lenient = true)
        {
            long position = stream.Position;
            try
            {
                stream.Position = offset;
                int count = Read(stream, span, lenient);
                return count;
            }
            finally
            {
                stream.Position = position;
            }
        }

        /// <summary>
        /// Attempts to read data from stream at the specified offset.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <param name="offset">Offset to read from.</param>
        /// <param name="span">Target to copy to.</param>
        /// <param name="readCount">Number of successfully read bytes.</param>
        /// <returns>True if the buffer could be filled.</returns>
        /// <remarks>Original position of <paramref name="stream"/> is restored on completion.</remarks>
        public static bool TryRead(Stream stream, long offset, Span<byte> span, out int readCount)
        {
            long position = stream.Position;
            try
            {
                stream.Position = offset;
                return TryRead(stream, span, out readCount);
            }
            finally
            {
                stream.Position = position;
            }
        }

        #endregion

        #region Offset implicit input stream to span

        /// <summary>
        /// Reads data from current file's input stream at the specified offset.
        /// </summary>
        /// <param name="offset">Offset to read from.</param>
        /// <param name="span">Target to copy to.</param>
        /// <param name="lenient">If false, throws when failed to fill target.</param>
        /// <param name="forceUseCurrentSpan">Force use provided span.</param>
        /// <returns>Number of bytes read.</returns>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="InputStream"/> is not set.</exception>
        /// <exception cref="IOException">Thrown when <paramref name="lenient"/> is false
        /// and stream cannot provide enough data to fill target.</exception>
        /// <remarks>Original position of <see cref="InputStream"/> is restored on completion.</remarks>
        public int Read(long offset, ref Span<byte> span, bool lenient = true, bool forceUseCurrentSpan = false)
            => Read(_inputStream ?? throw new InvalidOperationException(), offset, ref span, lenient, forceUseCurrentSpan);

        /// <summary>
        /// Reads data from current file's input stream at the specified offset.
        /// </summary>
        /// <param name="offset">Offset to read from.</param>
        /// <param name="length">Number of bytes to try to read.</param>
        /// <param name="span">Target to copy to.</param>
        /// <param name="lenient">If false, throws when failed to fill target.</param>
        /// <param name="forceNew">Force use newly allocated buffer.</param>
        /// <returns>Number of bytes read.</returns>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="InputStream"/> is not set.</exception>
        /// <exception cref="IOException">Thrown when <paramref name="lenient"/> is false
        /// and stream cannot provide enough data to fill target.</exception>
        /// <remarks>Original position of <see cref="InputStream"/> is restored on completion.</remarks>
        public int Read(long offset, int length, out Span<byte> span, bool lenient = true, bool forceNew = false)
            => Read(_inputStream ?? throw new InvalidOperationException(), offset, length, out span, lenient, forceNew);

        /// <summary>
        /// Attempts to read data from current file's input stream at the specified offset.
        /// </summary>
        /// <param name="offset">Offset to read from.</param>
        /// <param name="length">Number of bytes to try to read.</param>
        /// <param name="span">Result buffer, valid if completed read (buffer filled and returned true).</param>
        /// <param name="forceNew">Force use newly allocated buffer.</param>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="InputStream"/> is not set.</exception>
        /// <returns>True if the buffer could be filled.</returns>
        /// <remarks>Original position of <see cref="InputStream"/> is restored on completion.</remarks>
        public bool TryRead(long offset, int length, out Span<byte> span, bool forceNew = false)
            => TryRead(_inputStream ?? throw new InvalidOperationException(), offset, length, out span, forceNew);

        /// <summary>
        /// Reads data from current file's input stream at the specified offset.
        /// </summary>
        /// <param name="offset">Offset to read from.</param>
        /// <param name="span">Target to copy to.</param>
        /// <param name="lenient">If false, throws when failed to fill target.</param>
        /// <returns>Number of bytes read.</returns>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="InputStream"/> is not set.</exception>
        /// <exception cref="IOException">Thrown when <paramref name="lenient"/> is false
        /// and stream cannot provide enough data to fill target.</exception>
        /// <remarks>Original position of <see cref="InputStream"/> is restored on completion.</remarks>
        public int Read(long offset, Span<byte> span, bool lenient = true)
            => Read(_inputStream ?? throw new InvalidOperationException(), offset, span, lenient);

        /// <summary>
        /// Attempts to read data from current file's input stream at the specified offset.
        /// </summary>
        /// <param name="offset">Offset to read from.</param>
        /// <param name="span">Target to copy to.</param>
        /// <param name="readCount">Number of successfully read bytes.</param>
        /// <returns>True if the buffer could be filled.</returns>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="InputStream"/> is not set.</exception>
        /// <remarks>Original position of <see cref="InputStream"/> is restored on completion.</remarks>
        public bool TryRead(long offset, Span<byte> span, out int readCount)
            => TryRead(_inputStream ?? throw new InvalidOperationException(), offset, span, out readCount);

        #endregion

        #region Stream to byte array

        /// <summary>
        /// Reads data from stream.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <param name="array">Target to copy to.</param>
        /// <param name="arrayOffset">Offset in array to write to.</param>
        /// <param name="arrayLength">Length to write.</param>
        /// <param name="lenient">If false, throws when failed to fill target.</param>
        /// <returns>Number of bytes read.</returns>
        /// <exception cref="IOException">Thrown when <paramref name="lenient"/> is false
        /// and stream cannot provide enough data to fill target.</exception>
        [Obsolete("Succeeded by: public static int Read(Stream stream, Span<byte> span, bool lenient)")]
        public static int Read(Stream stream, byte[] array, int arrayOffset, int arrayLength, bool lenient = true) =>
            ReadBaseArray(stream, array, arrayOffset, arrayLength, lenient);

        /// <summary>
        /// Reads data from stream.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <param name="array">Target to copy to.</param>
        /// <param name="lenient">If false, throws when failed to fill target.</param>
        /// <returns>Number of bytes read.</returns>
        /// <exception cref="IOException">Thrown when <paramref name="lenient"/> is false
        /// and stream cannot provide enough data to fill target.</exception>
        [Obsolete("Succeeded by: public static int Read(Stream stream, Span<byte> span, bool lenient)")]
        public static int Read(Stream stream, byte[] array, bool lenient = true)
            => Read(stream, array, 0, array.Length, lenient);

        #endregion

        #region Implicit input stream to byte array

        /// <summary>
        /// Reads data from current file's input stream.
        /// </summary>
        /// <param name="array">Target to copy to.</param>
        /// <param name="arrayOffset">Offset in array to write to.</param>
        /// <param name="arrayLength">Length to write.</param>
        /// <param name="lenient">If false, throws when failed to fill target.</param>
        /// <returns>Number of bytes read.</returns>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="InputStream"/> is not set.</exception>
        /// <exception cref="IOException">Thrown when <paramref name="lenient"/> is false
        /// and stream cannot provide enough data to fill target.</exception>
        [Obsolete("Succeeded by: public static int Read(Span<byte> span, bool lenient)")]
        public int Read(byte[] array, int arrayOffset, int arrayLength, bool lenient = true)
            => Read(_inputStream ?? throw new InvalidOperationException(), array, arrayOffset, arrayLength, lenient);

        /// <summary>
        /// Reads data from current file's input stream.
        /// </summary>
        /// <param name="array">Target to copy to.</param>
        /// <param name="lenient">If false, throws when failed to fill target.</param>
        /// <returns>Number of bytes read.</returns>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="InputStream"/> is not set.</exception>
        /// <exception cref="IOException">Thrown when <paramref name="lenient"/> is false
        /// and stream cannot provide enough data to fill target.</exception>
        [Obsolete("Succeeded by: public static int Read(Span<byte> span, bool lenient)")]
        public int Read(byte[] array, bool lenient = true)
            => Read(_inputStream ?? throw new InvalidOperationException(), array, 0, array.Length, lenient);

        #endregion

        #region Offset stream to byte array

        /// <summary>
        /// Reads data from stream at the specified offset.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <param name="offset">Offset to read from.</param>
        /// <param name="array">Target to copy to.</param>
        /// <param name="arrayOffset">Offset in array to write to.</param>
        /// <param name="arrayLength">Length to write.</param>
        /// <param name="lenient">If false, throws when failed to fill target.</param>
        /// <returns>Number of bytes read.</returns>
        /// <exception cref="IOException">Thrown when <paramref name="lenient"/> is false
        /// and stream cannot provide enough data to fill target.</exception>
        /// <remarks>Original position of <paramref name="stream"/> is restored on completion.</remarks>
        [Obsolete("Succeeded by: public static int Read(Stream stream, long offset, Span<byte> span, bool lenient)")]
        public static int Read(Stream stream, long offset, byte[] array, int arrayOffset, int arrayLength, bool lenient = true)
        {
            long position = stream.Position;
            try
            {
                stream.Position = offset;
                int count = Read(stream, array, arrayOffset, arrayLength, lenient);
                return count;
            }
            finally
            {
                stream.Position = position;
            }
        }

        /// <summary>
        /// Reads data from stream at the specified offset.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <param name="offset">Offset to read from.</param>
        /// <param name="array">Target to copy to.</param>
        /// <param name="lenient">If false, throws when failed to fill target.</param>
        /// <returns>Number of bytes read.</returns>
        /// <exception cref="IOException">Thrown when <paramref name="lenient"/> is false
        /// and stream cannot provide enough data to fill target.</exception>
        /// <remarks>Original position of <paramref name="stream"/> is restored on completion.</remarks>
        [Obsolete("Succeeded by: public static int Read(Stream stream, long offset, Span<byte> span, bool lenient)")]
        public static int Read(Stream stream, long offset, byte[] array, bool lenient = true)
            => Read(stream, offset, array, 0, array.Length, lenient);

        #endregion

        #region Offset implicit input stream to byte array

        /// <summary>
        /// Reads data from current file's input stream at the specified offset.
        /// </summary>
        /// <param name="offset">Offset to read from.</param>
        /// <param name="array">Target to copy to.</param>
        /// <param name="arrayOffset">Offset in array to write to.</param>
        /// <param name="arrayLength">Length to write.</param>
        /// <param name="lenient">If false, throws when failed to fill target.</param>
        /// <returns>Number of bytes read.</returns>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="InputStream"/> is not set.</exception>
        /// <exception cref="IOException">Thrown when <paramref name="lenient"/> is false
        /// and stream cannot provide enough data to fill target.</exception>
        /// <remarks>Original position of <see cref="InputStream"/> is restored on completion.</remarks>
        [Obsolete("Succeeded by: public static int Read(long offset, Span<byte> span, bool lenient)")]
        public int Read(long offset, byte[] array, int arrayOffset, int arrayLength, bool lenient = true)
            => Read(_inputStream ?? throw new InvalidOperationException(), offset, array, arrayOffset, arrayLength, lenient);

        /// <summary>
        /// Reads data from current file's input stream at the specified offset.
        /// </summary>
        /// <param name="offset">Offset to read from.</param>
        /// <param name="array">Target to copy to.</param>
        /// <param name="lenient">If false, throws when failed to fill target.</param>
        /// <returns>Number of bytes read.</returns>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="InputStream"/> is not set.</exception>
        /// <exception cref="IOException">Thrown when <paramref name="lenient"/> is false
        /// and stream cannot provide enough data to fill target.</exception>
        /// <remarks>Original position of <see cref="InputStream"/> is restored on completion.</remarks>
        [Obsolete("Succeeded by: public static int Read(long offset, Span<byte> span, bool lenient)")]
        public int Read(long offset, byte[] array, bool lenient = true)
            => Read(offset, array, 0, array.Length, lenient);

        #endregion

        /// <summary>
        /// Gets byte array from stream.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <param name="forceNew">Force use newly allocated buffer.</param>
        /// <returns>Array with file contents.</returns>
        public static byte[] GetArray(Stream stream, bool forceNew = false)
        {
            if (!stream.CanSeek)
                throw new NotSupportedException("Getting memory from non-seekable stream is unsupported");
            switch (stream)
            {
                case MStream mes:
                    return mes.GetMemory().ToArray();
                case MemoryStream ms when !forceNew:
                    return ms.Capacity == ms.Length && ms.TryGetBuffer(out _) ? ms.GetBuffer() : ms.ToArray();
                default:
                    stream.Position = 0;
                    try
                    {
                        byte[] arr = new byte[stream.Length];
                        Read(stream, (Span<byte>)arr, false);
                        return arr;
                    }
                    catch (Exception)
                    {
                        // Fallback to MemoryStream copy
                        stream.Position = 0;
                        MemoryStream ms2 = new();
                        stream.CopyTo(ms2);
                        return ms2.ToArray();
                    }
            }
        }

        /// <summary>
        /// Loads newly allocated byte array from current file's input stream.
        /// </summary>
        /// <returns>Array with file contents.</returns>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="InputStream"/> is not set.</exception>
        public byte[] Load()
            => GetArray(_inputStream ?? throw new InvalidOperationException(), true);

        /// <summary>
        /// Gets byte array from current file's input stream.
        /// </summary>
        /// <param name="forceNew">Force use newly allocated buffer.</param>
        /// <returns>Array with file contents.</returns>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="InputStream"/> is not set.</exception>
        public byte[] GetArray(bool forceNew = false)
            => GetArray(_inputStream ?? throw new InvalidOperationException(), forceNew);

        /// <summary>
        /// Gets byte array from stream.
        /// </summary>
        /// <param name="offset">Offset in stream.</param>
        /// <param name="length">Length of segment.</param>
        /// <param name="stream">Stream to read from.</param>
        /// <param name="forceNew">Force use newly allocated buffer.</param>
        /// <returns>Array with file contents.</returns>
        /// <exception cref="NotSupportedException">Thrown if the stream is not seekable.</exception>
        public static byte[] GetArray(int offset, int length, Stream stream, bool forceNew = false)
        {
            if (!stream.CanSeek)
                throw new NotSupportedException("Getting memory from non-seekable stream is unsupported");
            switch (stream)
            {
                case MStream mes:
                    return mes.GetMemory().Slice(offset, length).ToArray();
                case MemoryStream ms when !forceNew:
                    return offset == 0 && ms.Length == length && ms.Capacity == ms.Length && ms.TryGetBuffer(out _)
                        ? ms.GetBuffer()
                        : ms.ToArray();
                default:
                    if (offset + length > stream.Length)
                        throw new IOException("Target range exceeds stream bounds");
                    stream.Position = offset;
                    try
                    {
                        byte[] arr = new byte[length];
                        Read(stream, (Span<byte>)arr, false);
                        return arr;
                    }
                    catch (Exception)
                    {
                        // Fallback to MemoryStream copy
                        stream.Position = offset;
                        MemoryStream ms2 = new();
                        new SStream(stream, length).CopyTo(ms2);
                        return ms2.ToArray();
                    }
            }
        }

        /// <summary>
        /// Gets byte array from curernt file's input stream.
        /// </summary>
        /// <param name="offset">Offset in stream.</param>
        /// <param name="length">Length of segment.</param>
        /// <param name="forceNew">Force use newly allocated buffer.</param>
        /// <returns>Array with file contents.</returns>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="InputStream"/> is not set.</exception>
        public byte[] GetArray(int offset, int length, bool forceNew = false)
            => GetArray(offset, length, _inputStream ?? throw new InvalidOperationException(), forceNew);

        /// <summary>
        /// Dumps remaining content from stream to byte array.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <param name="maxLength">Maximum length.</param>
        /// <returns>Array with file contents.</returns>
        public static byte[] Dump(Stream stream, int maxLength = int.MaxValue)
        {
            MemoryStream ms2 = new();
            new SStream(stream, maxLength, false).CopyTo(ms2);
            return ms2.ToArray();
        }

        /// <summary>
        /// Dumps remaining content from current file's input stream to byte array.
        /// </summary>
        /// <param name="maxLength">Maximum length.</param>
        /// <returns>Array with file contents.</returns>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="InputStream"/> is not set.</exception>
        public byte[] Dump(int maxLength = int.MaxValue)
            => Dump(_inputStream ?? throw new InvalidOperationException(), maxLength);

        /// <summary>
        /// Gets read-only memory from stream from the beginning of the stream.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <returns>Array with file contents.</returns>
        /// <exception cref="NotSupportedException">Thrown for non-seekable streams (<see cref="Stream.CanSeek"/> is false).</exception>
        /// <remarks>Non-allocating requisition of memory from <see cref="MemoryStream"/> and <see cref="MStream"/> is supported.</remarks>
        public static ReadOnlyMemory<byte> GetMemory(Stream stream)
        {
            if (!stream.CanSeek)
                throw new NotSupportedException("Getting memory from non-seekable stream is unsupported");
            if (TryGetRawReadOnlyMemoryFromStream(stream, out var rawBuffer, out _)) return rawBuffer;
            stream.Position = 0;
            try
            {
                byte[] arr = new byte[stream.Length];
                Read(stream, (Span<byte>)arr, false);
                return arr;
            }
            catch (Exception)
            {
                // Fallback to MemoryStream copy
                stream.Position = 0;
                MemoryStream ms2 = new();
                stream.CopyTo(ms2);
                return ms2.GetBuffer().AsMemory(0, (int)ms2.Length);
            }
        }

        /// <summary>
        /// Gets read-only memory from current file's input stream from the beginning of the stream.
        /// </summary>
        /// <returns>Array with file contents.</returns>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="InputStream"/> is not set.</exception>
        /// <exception cref="NotSupportedException">Thrown for non-seekable streams (<see cref="Stream.CanSeek"/> is false).</exception>
        /// <remarks>Non-allocating requisition of memory from <see cref="MemoryStream"/> and <see cref="MStream"/> is supported.</remarks>
        public ReadOnlyMemory<byte> GetMemory()
            => GetMemory(_inputStream ?? throw new InvalidOperationException());

        /// <summary>
        /// Gets read-only memory from stream.
        /// </summary>
        /// <param name="offset">Offset in stream.</param>
        /// <param name="length">Length of segment.</param>
        /// <param name="stream">Stream to read from.</param>
        /// <returns>Array with file contents.</returns>
        /// <exception cref="NotSupportedException">Thrown for non-seekable streams (<see cref="Stream.CanSeek"/> is false).</exception>
        /// <exception cref="IOException">Thrown for I/O errors, including a stream with insufficient data to fulfill the request.</exception>
        /// <remarks>Non-allocating requisition of memory from <see cref="MemoryStream"/> and <see cref="MStream"/> is supported.</remarks>
        public static ReadOnlyMemory<byte> GetMemory(long offset, int length, Stream stream)
        {
            if (!stream.CanSeek)
                throw new NotSupportedException("Getting memory from non-seekable stream is unsupported");
            if (offset + length > stream.Length)
                throw new IOException("Target range exceeds stream bounds");
            stream.Position = offset;
            if (TryGetRawReadOnlyMemoryFromStream(stream, out var rawBuffer, out int rawBufferPosition))
                return rawBuffer.Slice(rawBufferPosition, length);
            try
            {
                byte[] arr = new byte[length];
                Read(stream, (Span<byte>)arr, false);
                return arr;
            }
            catch (Exception)
            {
                // Fallback to MemoryStream copy
                stream.Position = offset;
                MemoryStream ms2 = new();
                new SStream(stream, length).CopyTo(ms2);
                return ms2.GetBuffer().AsMemory(0, length);
            }
        }

        /// <summary>
        /// Gets read-only memory from current file's input stream.
        /// </summary>
        /// <param name="offset">Offset in stream.</param>
        /// <param name="length">Length of segment.</param>
        /// <returns>Array with file contents.</returns>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="InputStream"/> is not set.</exception>
        /// <exception cref="NotSupportedException">Thrown for non-seekable streams (<see cref="Stream.CanSeek"/> is false).</exception>
        /// <exception cref="IOException">Thrown for I/O errors, including a stream with insufficient data to fulfill the request.</exception>
        /// <remarks>Non-allocating requisition of memory from <see cref="MemoryStream"/> and <see cref="MStream"/> is supported.</remarks>
        public ReadOnlyMemory<byte> GetMemory(long offset, int length)
            => GetMemory(offset, length, _inputStream ?? throw new InvalidOperationException());

        /// <summary>
        /// Ensures a file is loaded / initialized with nonzero length.
        /// </summary>
        /// <param name="target">Target memory variable.</param>
        /// <param name="init">Initialization flag.</param>
        /// <param name="openDelegate">Delegate for opening file (stream will be disposed).</param>
        /// <param name="storeDelegate">Delegate for getting data.</param>
        /// <returns>True if file is loaded.</returns>
        public bool EnsureFile(ref Memory<byte> target, ref bool init, Func<Stream> openDelegate, Func<Stream, Memory<byte>> storeDelegate)
        {
            if (init) return target.Length != 0;
            init = true;
            try
            {
                using var fs = openDelegate();
                target = storeDelegate(fs);
                return true;
            }
            catch (Exception e)
            {
                LogFail($"Failed to load file: {e}");
                target = Memory<byte>.Empty;
                return false;
            }
        }

        #endregion
    }

    public partial class FpUtil
    {
        /// <summary>
        /// Dumps byte array from a stream.
        /// </summary>
        /// <param name="stream">Stream to dump.</param>
        /// <param name="maxLength">Maximum input length.</param>
        /// <returns>Byte array.</returns>
        public static byte[] Dump(this Stream stream, int maxLength = int.MaxValue) =>
            Processor.Dump(stream, maxLength);

        /// <summary>
        /// Creates stream from array.
        /// </summary>
        /// <param name="source">Source array.</param>
        /// <returns>Stream.</returns>
        public static Stream Stream(this byte[] source) => new MStream(source);

        /// <summary>
        /// Creates stream from memory.
        /// </summary>
        /// <param name="source">Source memory.</param>
        /// <returns>Stream.</returns>
        public static Stream Stream(this Memory<byte> source) => new MStream(source);

        /// <summary>
        /// Creates stream from memory.
        /// </summary>
        /// <param name="source">Source memory.</param>
        /// <returns>Stream.</returns>
        public static Stream Stream(this ReadOnlyMemory<byte> source) => new MStream(source);

        /// <summary>
        /// Gets seekable stream (closes base stream if replaced).
        /// </summary>
        /// <param name="stream">Base stream.</param>
        /// <returns>Seekable stream.</returns>
        /// <remarks>
        /// This method conditionally creates a seekable stream from a non-seekable stream by copying the
        /// stream's contents to a new <see cref="MemoryStream"/> instance. The returned object is either
        /// this newly created stream or the passed argument <paramref name="stream"/> if it was already seekable.
        /// </remarks>
        public static Stream Seekable(this Stream stream) => Processor.GetSeekableStream(stream);
    }
}
