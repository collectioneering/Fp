using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using static System.Buffers.ArrayPool<byte>;

namespace Fp;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public partial class Processor
{
    #region String rw utilities

    /// <summary>
    /// Get byte array from string assuming 8-bit characters.
    /// </summary>
    /// <param name="text">String to process.</param>
    /// <param name="result">Result buffer.</param>
    /// <returns>Byte array containing lower byte of each code unit in the string.</returns>
    public static byte[] Ascii(string text, byte[]? result = null) => text.Ascii(result);

    private static string DecodeSpan(ReadOnlySpan<byte> span, Encoding encoding)
    {
        if (span.Length == 0)
        {
            return string.Empty;
        }

        return encoding.GetString(span);
    }

    /// <summary>
    /// Read UTF-8 encoded string from stream.
    /// </summary>
    /// <param name="stream">Stream to read from.</param>
    /// <param name="read">Number of bytes consumed by string (including null terminator).</param>
    /// <param name="numBytes">Length of string in bytes (excluding null terminator).</param>
    /// <param name="maxLength">Maximum string length.</param>
    /// <param name="strict">If true, enforces ending stream position to original position + <paramref name="maxLength"/></param>
    /// <returns>Value.</returns>
    public string ReadUtf8String(Stream stream, out int read, out int numBytes, int maxLength = int.MaxValue,
        bool strict = false)
    {
        try
        {
            TempMs.SetLength(0);
            read = 0;
            numBytes = 0;
            do
            {
                int v = stream.ReadByte();
                read += v == -1 ? 0 : 1;
                if (v is -1 or 0) break;
                numBytes++;
                TempMs.WriteByte((byte)v);
            } while (read < maxLength);

            if (strict)
            {
                Skip(maxLength - read, stream);
            }

            return DecodeSpan(TempMs.GetBuffer().AsSpan(0, (int)TempMs.Length), Encoding.UTF8);
        }
        finally
        {
            if (TempMs.Capacity > StringExcessiveCapacity)
            {
                TempMs.Capacity = StringDefaultCapacity;
            }
        }
    }

    /// <summary>
    /// Read UTF-8 encoded string from stream at the specified offset.
    /// </summary>
    /// <param name="stream">Stream to read from.</param>
    /// <param name="offset">Offset to read from.</param>
    /// <param name="read">Number of bytes consumed by string (including null terminator).</param>
    /// <param name="numBytes">Length of string in bytes (excluding null terminator).</param>
    /// <param name="maxLength">Maximum string length.</param>
    /// <param name="strict">If true, enforces ending stream position to original position + <paramref name="maxLength"/></param>
    /// <returns>Value.</returns>
    /// <remarks>Original position of <paramref name="stream"/> is restored on completion.</remarks>
    public string ReadUtf8StringFromOffset(Stream stream, long offset, out int read, out int numBytes,
        int maxLength = int.MaxValue,
        bool strict = false)
    {
        long position = stream.Position;
        try
        {
            stream.Position = offset;
            string str = ReadUtf8String(stream, out read, out numBytes, maxLength, strict);
            return str;
        }
        finally
        {
            stream.Position = position;
        }
    }

    /// <summary>
    /// Read UTF-8 encoded string from current file's input stream.
    /// </summary>
    /// <param name="read">Number of bytes consumed by string (including null terminator).</param>
    /// <param name="numBytes">Length of string in bytes (excluding null terminator).</param>
    /// <param name="maxLength">Maximum string length.</param>
    /// <param name="strict">If true, enforces ending stream position to original position + <paramref name="maxLength"/></param>
    /// <returns>Value.</returns>
    public string ReadUtf8String(out int read, out int numBytes, int maxLength = int.MaxValue, bool strict = false)
        => ReadUtf8String(_inputStream ?? throw new InvalidOperationException(), out read, out numBytes, maxLength,
            strict);

    /// <summary>
    /// Read UTF-8 encoded string from current file's input stream at the specified offset.
    /// </summary>
    /// <param name="offset">Offset to read from.</param>
    /// <param name="read">Number of bytes consumed by string (including null terminator).</param>
    /// <param name="numBytes">Length of string in bytes (excluding null terminator).</param>
    /// <param name="maxLength">Maximum string length.</param>
    /// <param name="strict">If true, enforces ending stream position to original position + <paramref name="maxLength"/></param>
    /// <returns>Value.</returns>
    /// <remarks>Original position of <see cref="InputStream"/> is restored on completion.</remarks>
    public string ReadUtf8StringFromOffset(long offset, out int read, out int numBytes,
        int maxLength = int.MaxValue,
        bool strict = false) =>
        ReadUtf8StringFromOffset(_inputStream ?? throw new InvalidOperationException(), offset, out read,
            out numBytes, maxLength,
            strict);

    /// <summary>
    /// Read UTF-8 encoded string from span.
    /// </summary>
    /// <param name="span">Span to read from.</param>
    /// <param name="read">Number of bytes consumed by string (including null terminator).</param>
    /// <param name="numBytes">Length of string in bytes (excluding null terminator).</param>
    /// <param name="maxLength">Maximum string length.</param>
    /// <returns>Value.</returns>
    public static string ReadUtf8String(ReadOnlySpan<byte> span, out int read, out int numBytes,
        int maxLength = int.MaxValue)
    {
        int lim = Math.Min(span.Length, maxLength);
        int end = span[..lim].IndexOf((byte)0);
        if (end == -1)
        {
            read = lim;
            numBytes = lim;
        }
        else
        {
            read = Math.Min(lim, end + 1);
            numBytes = Math.Min(lim, end);
        }

        numBytes = Math.Min(lim, numBytes);
        return DecodeSpan(span[..numBytes], Encoding.UTF8);
    }

    /// <summary>
    /// Read UTF-16 encoded string from stream.
    /// </summary>
    /// <param name="stream">Stream to read from.</param>
    /// <param name="maxLength">Maximum string length (in bytes).</param>
    /// <param name="read">Number of bytes consumed by string (including null terminator).</param>
    /// <param name="numBytes">Length of string in bytes (excluding null terminator).</param>
    /// <param name="strict">If true, enforces ending stream position to original position + <paramref name="maxLength"/></param>
    /// <returns>Value.</returns>
    public string ReadUtf16String(Stream stream, out int read, out int numBytes, int maxLength = int.MaxValue,
        bool strict = false)
    {
        try
        {
            TempMs.SetLength(0);
            read = 0;
            numBytes = 0;
            Span<byte> span = stackalloc byte[2];
            do
            {
                int cc = Read(stream, span);
                read += cc;
                if (cc != 2 || span[0] == 0 && span[1] == 0) break;
                numBytes += 2;
                TempMs.Write(span);
            } while (read < maxLength);

            if (strict)
            {
                Skip(maxLength - read, stream);
            }

            return ReadUtf16String(TempMs.GetBuffer().AsSpan(0, (int)TempMs.Length), out _, out _);
        }
        finally
        {
            if (TempMs.Capacity > StringExcessiveCapacity)
            {
                TempMs.Capacity = StringDefaultCapacity;
            }
        }
    }

    /// <summary>
    /// Read UTF-16 encoded string from stream at the specified offset.
    /// </summary>
    /// <param name="stream">Stream to read from.</param>
    /// <param name="offset">Offset to read from.</param>
    /// <param name="read">Number of bytes consumed by string (including null terminator).</param>
    /// <param name="numBytes">Length of string in bytes (excluding null terminator).</param>
    /// <param name="maxLength">Maximum string length.</param>
    /// <param name="strict">If true, enforces ending stream position to original position + <paramref name="maxLength"/></param>
    /// <returns>Value.</returns>
    /// <remarks>Original position of <paramref name="stream"/> is restored on completion.</remarks>
    public string ReadUtf16StringFromOffset(Stream stream, long offset, out int read, out int numBytes,
        int maxLength = int.MaxValue,
        bool strict = false)
    {
        long position = stream.Position;
        try
        {
            stream.Position = offset;
            string str = ReadUtf16String(stream, out read, out numBytes, maxLength, strict);
            return str;
        }
        finally
        {
            stream.Position = position;
        }
    }

    /// <summary>
    /// Read UTF-16 encoded string from current file's input stream.
    /// </summary>
    /// <param name="read">Number of bytes consumed by string (including null terminator).</param>
    /// <param name="numBytes">Length of string in bytes (excluding null terminator).</param>
    /// <param name="maxLength">Maximum string length.</param>
    /// <param name="strict">If true, enforces ending stream position to original position + <paramref name="maxLength"/></param>
    /// <returns>Value.</returns>
    public string ReadUtf16String(out int read, out int numBytes, int maxLength = int.MaxValue, bool strict = false)
        => ReadUtf16String(_inputStream ?? throw new InvalidOperationException(), out read, out numBytes, maxLength,
            strict);

    /// <summary>
    /// Read UTF-16 encoded string from current file's input stream at the specified offset.
    /// </summary>
    /// <param name="offset">Offset to read from.</param>
    /// <param name="read">Number of bytes consumed by string (including null terminator).</param>
    /// <param name="numBytes">Length of string in bytes (excluding null terminator).</param>
    /// <param name="maxLength">Maximum string length.</param>
    /// <param name="strict">If true, enforces ending stream position to original position + <paramref name="maxLength"/></param>
    /// <returns>Value.</returns>
    /// <remarks>Original position of <see cref="InputStream"/> is restored on completion.</remarks>
    public string ReadUtf16StringFromOffset(long offset, out int read, out int numBytes,
        int maxLength = int.MaxValue,
        bool strict = false) =>
        ReadUtf16StringFromOffset(_inputStream ?? throw new InvalidOperationException(), offset, out read,
            out numBytes, maxLength,
            strict);


    /// <summary>
    /// Read UTF-16 encoded string from span.
    /// </summary>
    /// <param name="span">Span to read from.</param>
    /// <param name="read">Number of bytes consumed by string (including null terminator).</param>
    /// <param name="numBytes">Length of string in bytes (excluding null terminator).</param>>
    /// <param name="maxLength">Maximum string length.</param>
    /// <returns>Value.</returns>
    public static string ReadUtf16String(ReadOnlySpan<byte> span, out int read, out int numBytes,
        int maxLength = int.MaxValue)
    {
        int lim = Math.Min(span.Length, maxLength);
        lim = (lim >> 1) << 1;
        int end = -1;
        while (end + 1 + 2 <= lim && (span[end + 1] != 0 || span[end + 2] != 0))
            end += 2;
        if (end == -1)
        {
            read = lim;
            numBytes = lim;
        }
        else
        {
            read = Math.Min(lim, end + 1 + 2);
            numBytes = Math.Min(lim, end + 1);
        }

        bool big = span.Length >= 2 && span[0] == 0xFE && span[1] == 0xFF;
        bool bom = big || span.Length >= 2 && span[0] == 0xFF && span[1] == 0xFE;

        if (!bom && span.Length > 1)
        {
            const int numBytesHead = 16 * sizeof(char);
            const float threshold = 0.75f;
            int countAscii = 0, countTotal = 0, sl = span.Length;
            for (int i = 0; i < numBytesHead && i + 1 < sl; i += 2)
            {
                if (span[i] == 0 && span[i + 1] < 0x80)
                {
                    countAscii++;
                }

                countTotal++;
            }

            big = (float)countAscii / countTotal >= threshold;
        }

        return DecodeSpan(span[..numBytes], GetUtf16Encoding(big, bom));
    }

    /// <summary>
    /// Get length of UTF-8-encoded data.
    /// </summary>
    /// <param name="value">String to test.</param>
    /// <param name="nullTerminate">If true, check length with null byte.</param>
    /// <returns>Predicted length.</returns>
    public int GetUtf8Length(string value, bool nullTerminate = true)
    {
        return Utf8Encoder.GetByteCount(value, true) + (nullTerminate ? 1 : 0);
    }

    /// <summary>
    /// Write UTF-8 string to stream.
    /// </summary>
    /// <param name="value">String to write.</param>
    /// <param name="nullTerminate">If true, null-terminate string.</param>
    /// <param name="stream">Stream to write to, uses current output file if null.</param>
    /// <param name="offset">Offset to write to, current position if null.</param>
    public void WriteUtf8String(string value, bool nullTerminate = true, Stream? stream = null,
        long? offset = null)
    {
        stream ??= OutputStream ?? throw new InvalidOperationException();
        long origPos = offset.HasValue ? stream.Position : -1;
        Utf8Encoder.Reset();
        byte[] tmpBuf = Shared.Rent(4096);
        try
        {
            if (offset.HasValue)
            {
                stream.Position = offset.Value;
            }

            ReadOnlySpan<char> valueBuf = value;
            while (!valueBuf.IsEmpty)
            {
                Utf8Encoder.Convert(valueBuf, tmpBuf, false, out int numIn, out int numOut, out _);
                valueBuf = valueBuf[numIn..];
                stream.Write(tmpBuf, 0, numOut);
            }

            if (!nullTerminate)
            {
                return;
            }

            stream.WriteByte(0);
        }
        finally
        {
            Shared.Return(tmpBuf);
            if (offset.HasValue)
            {
                stream.Position = origPos;
            }
        }
    }

    /// <summary>
    /// Get length of UTF-16-encoded data.
    /// </summary>
    /// <param name="value">String to test.</param>
    /// <param name="nullTerminate">If true, check length with null byte.</param>
    /// <param name="bom">If true, check length with byte order mark.</param>
    /// <returns>Predicted length.</returns>
    public static int GetUtf16Length(string value, bool nullTerminate = true, bool bom = false)
    {
        var encoding = GetUtf16Encoding(false, bom);
        return encoding.GetPreamble().Length + encoding.GetEncoder().GetByteCount(value, true) + (nullTerminate ? 2 : 0);
    }

    /// <summary>
    /// Write UTF-16 string to stream.
    /// </summary>
    /// <param name="value">String to write.</param>
    /// <param name="nullTerminate">If true, null-terminate string.</param>
    /// <param name="bigEndian">If true, write UTF-16BE.</param>
    /// <param name="byteOrderMark">If true, write byte order mark.</param>
    /// <param name="stream">Stream to write to, uses current output file if null.</param>
    /// <param name="offset">Offset to write to, current position if null.</param>
    public void WriteUtf16String(string value, bool nullTerminate = true, bool bigEndian = false,
        bool byteOrderMark = false, Stream? stream = null, long? offset = null)
    {
        stream ??= OutputStream ?? throw new InvalidOperationException();
        long origPos = offset.HasValue ? stream.Position : -1;
        var encoding = GetUtf16Encoding(bigEndian, byteOrderMark);
        Encoder encoder = encoding.GetEncoder();
        encoder.Reset();
        byte[] tmpBuf = Shared.Rent(4096);
        try
        {
            if (offset.HasValue)
            {
                stream.Position = offset.Value;
            }

            if (encoding.GetPreamble() is { Length: > 0 } preamble)
            {
                stream.Write(preamble, 0, preamble.Length);
            }
            ReadOnlySpan<char> valueBuf = value;
            while (!valueBuf.IsEmpty)
            {
                encoder.Convert(valueBuf, tmpBuf, false, out int numIn, out int numOut, out _);
                valueBuf = valueBuf[numIn..];
                stream.Write(tmpBuf, 0, numOut);
            }

            if (!nullTerminate)
            {
                return;
            }

            Span<byte> span = stackalloc byte[2];
            span[0] = 0;
            span[1] = 0;
            stream.Write(span);
        }
        finally
        {
            Shared.Return(tmpBuf);
            if (offset.HasValue)
            {
                stream.Position = origPos;
            }
        }
    }

    #endregion
}
