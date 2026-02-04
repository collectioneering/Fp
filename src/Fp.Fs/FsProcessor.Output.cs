using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Fp.Fs;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
public partial class FsProcessor
{
    #region Output from stream utilities

    /// <summary>
    /// Outputs data from stream to file.
    /// </summary>
    /// <param name="stream">Stream to read from.</param>
    /// <param name="extension">File extension.</param>
    /// <param name="filename">File name or relative path.</param>
    /// <returns>Length of written data.</returns>
    public long OutputAll(Stream stream, string? extension = null, string? filename = null)
    {
        using Stream fileStream = OpenOutputFile(false, extension, filename);
        return OutputAll(stream, fileStream);
    }

    /// <summary>
    /// Outputs data from current file's input stream to file.
    /// </summary>
    /// <param name="extension">File extension.</param>
    /// <param name="filename">File name or relative path.</param>
    /// <returns>Length of written data.</returns>
    public long OutputAll(string? extension = null, string? filename = null)
        => OutputAll(InputStream ?? throw new InvalidOperationException(), extension, filename);

    /// <summary>
    /// Outputs data from stream to file under folder named by current file's name.
    /// </summary>
    /// <param name="stream">Stream to read from.</param>
    /// <param name="extension">File extension.</param>
    /// <param name="filename">File name or relative path.</param>
    /// <returns>Length of written data.</returns>
    public long OutputAllSub(Stream stream, string? extension = null, string? filename = null)
    {
        using Stream fileStream = OpenOutputSubFile(false, extension, filename);
        return OutputAll(stream, fileStream);
    }

    /// <summary>
    /// Outputs data from current file's input stream to file under folder named by current file's name.
    /// </summary>
    /// <param name="extension">File extension.</param>
    /// <param name="filename">File name or relative path.</param>
    /// <returns>Length of written data.</returns>
    public long OutputAllSub(string? extension = null, string? filename = null)
        => OutputAllSub(InputStream ?? throw new InvalidOperationException(), extension, filename);

    /// <summary>
    /// Outputs data from stream to file.
    /// </summary>
    /// <param name="stream">Stream to read from.</param>
    /// <param name="filename">File to write to.</param>
    /// <returns>Length of written data.</returns>
    public long Output(Stream stream, string filename)
        => OutputAll(stream, null, filename);

    /// <summary>
    /// Outputs data from stream to file.
    /// </summary>
    /// <param name="stream">Stream to read from.</param>
    /// <param name="length">Number of bytes to read.</param>
    /// <param name="extension">File extension.</param>
    /// <param name="filename">File or relative path to write to.</param>
    /// <param name="lenient">If false, throws upon failure to read required number of bytes.</param>
    /// <param name="bufferLength">Minimum buffer length.</param>
    /// <returns>Length of written data.</returns>
    /// <exception cref="IOException">Thrown if <paramref name="lenient"/> is false and not enough bytes are available from <paramref name="stream"/>.</exception>
    public long Output(Stream stream, long length, string? extension = null, string? filename = null,
        bool lenient = true, int bufferLength = 4096)
    {
        using Stream fileStream = OpenOutputFile(false, extension, filename);
        return Output(stream, length, fileStream, lenient, bufferLength);
    }

    /// <summary>
    /// Outputs data from stream to file.
    /// </summary>
    /// <param name="stream">Stream to read from.</param>
    /// <param name="offset">Offset in stream to read from.</param>
    /// <param name="length">Number of bytes to read.</param>
    /// <param name="extension">File extension.</param>
    /// <param name="filename">File or relative path to write to.</param>
    /// <param name="lenient">If false, throws upon failure to read required number of bytes.</param>
    /// <param name="bufferLength">Minimum buffer length.</param>
    /// <returns>Length of written data.</returns>
    /// <exception cref="IOException">Thrown if <paramref name="lenient"/> is false and not enough bytes are available from <paramref name="stream"/>.</exception>
    /// <remarks>Original position of <paramref name="stream"/> is restored on completion.</remarks>
    public long Output(Stream stream, long offset, long length, string? extension = null, string? filename = null,
        bool lenient = true, int bufferLength = 4096)
    {
        long origPos = stream.Position;
        try
        {
            stream.Position = offset;
            long outLen = Output(stream, length, extension, filename, lenient, bufferLength);
            return outLen;
        }
        finally
        {
            stream.Position = origPos;
        }
    }

    /// <summary>
    /// Outputs data from current file's input stream to file.
    /// </summary>
    /// <param name="length">Number of bytes to read.</param>
    /// <param name="extension">File extension.</param>
    /// <param name="filename">File or relative path to write to.</param>
    /// <param name="lenient">If false, throws upon failure to read required number of bytes.</param>
    /// <param name="bufferLength">Minimum buffer length.</param>
    /// <returns>Length of written data.</returns>
    /// <exception cref="IOException">Thrown if <paramref name="lenient"/> is false and not enough bytes are available from input stream.</exception>
    public long Output(long length, string? extension = null, string? filename = null, bool lenient = true,
        int bufferLength = 4096)
        => Output(InputStream ?? throw new InvalidOperationException(), length, extension, filename, lenient,
            bufferLength);

    /// <summary>
    /// Outputs data from current file's input stream to file.
    /// </summary>
    /// <param name="offset">Offset in stream to read from.</param>
    /// <param name="length">Number of bytes to read.</param>
    /// <param name="extension">File extension.</param>
    /// <param name="filename">File or relative path to write to.</param>
    /// <param name="lenient">If false, throws upon failure to read required number of bytes.</param>
    /// <param name="bufferLength">Minimum buffer length.</param>
    /// <returns>Length of written data.</returns>
    /// <exception cref="IOException">Thrown if <paramref name="lenient"/> is false and not enough bytes are available from input stream.</exception>
    /// <remarks>Original position of input stream is restored on completion.</remarks>
    public long Output(long offset, long length, string? extension = null, string? filename = null,
        bool lenient = true, int bufferLength = 4096)
    {
        long origPos = (InputStream ?? throw new InvalidOperationException()).Position;
        try
        {
            InputStream.Position = offset;
            long outLen = Output(InputStream, offset, length, extension, filename, lenient, bufferLength);
            return outLen;
        }
        finally
        {
            InputStream.Position = origPos;
        }
    }

    /// <summary>
    /// Outputs data from stream to file under folder named by current file's name.
    /// </summary>
    /// <param name="stream">Stream to read from.</param>
    /// <param name="length">Number of bytes to read.</param>
    /// <param name="extension">File extension.</param>
    /// <param name="filename">File or relative path to write to.</param>
    /// <param name="lenient">If false, throws upon failure to read required number of bytes.</param>
    /// <param name="bufferLength">Minimum buffer length.</param>
    /// <returns>Length of written data.</returns>
    /// <exception cref="IOException">Thrown if <paramref name="lenient"/> is false and not enough bytes are available from <paramref name="stream"/>.</exception>
    public long OutputSub(Stream stream, long length, string? extension = null, string? filename = null,
        bool lenient = true,
        int bufferLength = 4096)
    {
        using Stream fileStream = OpenOutputSubFile(false, extension, filename);
        return Output(stream, length, fileStream, lenient, bufferLength);
    }

    /// <summary>
    /// Outputs data from stream to file under folder named by current file's name.
    /// </summary>
    /// <param name="stream">Stream to read from.</param>
    /// <param name="offset">Offset in stream to read from.</param>
    /// <param name="length">Number of bytes to read.</param>
    /// <param name="extension">File extension.</param>
    /// <param name="filename">File or relative path to write to.</param>
    /// <param name="lenient">If false, throws upon failure to read required number of bytes.</param>
    /// <param name="bufferLength">Minimum buffer length.</param>
    /// <returns>Length of written data.</returns>
    /// <exception cref="IOException">Thrown if <paramref name="lenient"/> is false and not enough bytes are available from <paramref name="stream"/>.</exception>
    /// <remarks>Original position of <paramref name="stream"/> is restored on completion.</remarks>
    public long OutputSub(Stream stream, long offset, long length, string? extension = null,
        string? filename = null,
        bool lenient = true, int bufferLength = 4096)
    {
        long origPos = stream.Position;
        try
        {
            stream.Position = offset;
            long outLen = OutputSub(stream, length, extension, filename, lenient, bufferLength);
            return outLen;
        }
        finally
        {
            stream.Position = origPos;
        }
    }

    /// <summary>
    /// Outputs data from current file's input stream to file under folder named by current file's name.
    /// </summary>
    /// <param name="offset">Offset in stream to read from.</param>
    /// <param name="length">Number of bytes to read.</param>
    /// <param name="extension">File extension.</param>
    /// <param name="filename">File or relative path to write to.</param>
    /// <param name="lenient">If false, throws upon failure to read required number of bytes.</param>
    /// <param name="bufferLength">Minimum buffer length.</param>
    /// <returns>Length of written data.</returns>
    /// <exception cref="IOException">Thrown if <paramref name="lenient"/> is false and not enough bytes are available from input stream.</exception>
    /// <remarks>Original position of input stream is restored on completion.</remarks>
    public long OutputSub(long offset, long length, string? extension = null, string? filename = null,
        bool lenient = true, int bufferLength = 4096)
        => OutputSub(InputStream ?? throw new InvalidOperationException(), offset, length, extension, filename,
            lenient, bufferLength);

    #endregion

    #region Output from array utilities

    /// <summary>
    /// Outputs data from array to file.
    /// </summary>
    /// <param name="array">Array to read from.</param>
    /// <param name="extension">File extension.</param>
    /// <param name="filename">File name or relative path.</param>
    public void OutputAll(byte[] array, string? extension = null, string? filename = null)
    {
        using Stream fileStream = OpenOutputFile(false, extension, filename);
        fileStream.Write(array);
    }

    /// <summary>
    /// Outputs data from array to file under folder named by current file's name.
    /// </summary>
    /// <param name="array">Array to read from.</param>
    /// <param name="extension">File extension.</param>
    /// <param name="filename">File name or relative path.</param>
    public void OutputAllSub(byte[] array, string? extension = null, string? filename = null)
    {
        using Stream fileStream = OpenOutputSubFile(false, extension, filename);
        fileStream.Write(array);
    }

    /// <summary>
    /// Outputs data from array to file.
    /// </summary>
    /// <param name="array">Array to read from.</param>
    /// <param name="extension">File extension.</param>
    /// <param name="filename">File to write to.</param>
    public void Output(byte[] array, string? extension = null, string? filename = null)
        => Output(array, 0, array.Length, extension, filename);

    /// <summary>
    /// Outputs data from stream to file.
    /// </summary>
    /// <param name="array">Array to read from.</param>
    /// <param name="offset">Offset in stream to read from.</param>
    /// <param name="length">Number of bytes to read.</param>
    /// <param name="extension">File extension.</param>
    /// <param name="filename">File or relative path to write to.</param>
    public void Output(byte[] array, int offset, int length, string? extension = null, string? filename = null)
    {
        using Stream fileStream = OpenOutputFile(false, extension, filename);
        fileStream.Write(array, offset, length);
    }

    /// <summary>
    /// Outputs data from stream to file under folder named by current file's name.
    /// </summary>
    /// <param name="array">Array to read from.</param>
    /// <param name="offset">Offset in array to read from.</param>
    /// <param name="length">Number of bytes to read.</param>
    /// <param name="extension">File extension.</param>
    /// <param name="filename">File or relative path to write to.</param>
    public void OutputSub(byte[] array, int offset, int length, string? extension = null, string? filename = null)
    {
        using Stream fileStream = OpenOutputSubFile(false, extension, filename);
        fileStream.Write(array, offset, length);
    }

    #endregion

    #region Output from span utilities

    /// <summary>
    /// Outputs data from array to file.
    /// </summary>
    /// <param name="span">Span to read from.</param>
    /// <param name="extension">File extension.</param>
    /// <param name="filename">File name or relative path.</param>
    public void OutputAll(ReadOnlySpan<byte> span, string? extension = null, string? filename = null)
    {
        using Stream fileStream = OpenOutputFile(false, extension, filename);
        fileStream.Write(span);
    }

    /// <summary>
    /// Outputs data from array to file under folder named by current file's name.
    /// </summary>
    /// <param name="span">Span to read from.</param>
    /// <param name="extension">File extension.</param>
    /// <param name="filename">File name or relative path.</param>
    public void OutputAllSub(ReadOnlySpan<byte> span, string? extension = null, string? filename = null)
    {
        using Stream fileStream = OpenOutputSubFile(false, extension, filename);
        fileStream.Write(span);
    }

    #endregion
}
