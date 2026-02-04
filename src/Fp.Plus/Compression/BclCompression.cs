using System;
using System.IO;

// ReSharper disable once CheckNamespace
namespace Fp.Plus;

public partial class PlusUtil
{
    /// <summary>
    /// Decompresses deflate data.
    /// </summary>
    /// <param name="buffer">Buffer to read.</param>
    /// <returns>Decompressed data.</returns>
    public static byte[] DeDeflate(this ReadOnlyMemory<byte> buffer)
        => Processor.Dump(new System.IO.Compression.DeflateStream(new MStream(buffer), System.IO.Compression.CompressionMode.Decompress));

    /// <summary>
    /// Decompresses deflate data.
    /// </summary>
    /// <param name="buffer">Buffer to read.</param>
    /// <returns>Decompressed data.</returns>
    public static byte[] DeDeflate(this Memory<byte> buffer)
        => Processor.Dump(new System.IO.Compression.DeflateStream(new MStream(buffer), System.IO.Compression.CompressionMode.Decompress));

    /// <summary>
    /// Decompresses deflate data.
    /// </summary>
    /// <param name="buffer">Buffer to read.</param>
    /// <returns>Decompressed data.</returns>
    public static byte[] DeDeflate(this byte[] buffer)
        => Processor.Dump(new System.IO.Compression.DeflateStream(new MStream(buffer), System.IO.Compression.CompressionMode.Decompress));

    /// <summary>
    /// Decompresses deflate data.
    /// </summary>
    /// <param name="stream">Stream to read.</param>
    /// <returns>Decompressed data.</returns>
    public static byte[] DeDeflate(this Stream stream)
        => Processor.Dump(new System.IO.Compression.DeflateStream(stream, System.IO.Compression.CompressionMode.Decompress, true));

    /// <summary>
    /// Decompresses gzipped data.
    /// </summary>
    /// <param name="buffer">Buffer to read.</param>
    /// <returns>Decompressed data.</returns>
    public static byte[] DeGzip(this ReadOnlyMemory<byte> buffer)
        => Processor.Dump(new System.IO.Compression.GZipStream(new MStream(buffer), System.IO.Compression.CompressionMode.Decompress));

    /// <summary>
    /// Decompresses gzipped data.
    /// </summary>
    /// <param name="buffer">Buffer to read.</param>
    /// <returns>Decompressed data.</returns>
    public static byte[] DeGzip(this Memory<byte> buffer)
        => Processor.Dump(new System.IO.Compression.GZipStream(new MStream(buffer), System.IO.Compression.CompressionMode.Decompress));

    /// <summary>
    /// Decompresses gzipped data.
    /// </summary>
    /// <param name="buffer">Buffer to read.</param>
    /// <returns>Decompressed data.</returns>
    public static byte[] DeGzip(this byte[] buffer)
        => Processor.Dump(new System.IO.Compression.GZipStream(new MStream(buffer), System.IO.Compression.CompressionMode.Decompress));

    /// <summary>
    /// Decompresses gzipped data.
    /// </summary>
    /// <param name="stream">Stream to read.</param>
    /// <returns>Decompressed data.</returns>
    public static byte[] DeGzip(this Stream stream)
        => Processor.Dump(new System.IO.Compression.GZipStream(stream, System.IO.Compression.CompressionMode.Decompress, true));

#if NET6_0_OR_GREATER

    /// <summary>
    /// Decompresses zlib data.
    /// </summary>
    /// <param name="buffer">Buffer to read.</param>
    /// <returns>Decompressed data.</returns>
    public static byte[] DeZlib(this ReadOnlyMemory<byte> buffer)
        => Processor.Dump(new System.IO.Compression.ZLibStream(new MStream(buffer), System.IO.Compression.CompressionMode.Decompress));

    /// <summary>
    /// Decompresses zlib data.
    /// </summary>
    /// <param name="buffer">Buffer to read.</param>
    /// <returns>Decompressed data.</returns>
    public static byte[] DeZlib(this Memory<byte> buffer)
        => Processor.Dump(new System.IO.Compression.ZLibStream(new MStream(buffer), System.IO.Compression.CompressionMode.Decompress));

    /// <summary>
    /// Decompresses zlib data.
    /// </summary>
    /// <param name="buffer">Buffer to read.</param>
    /// <returns>Decompressed data.</returns>
    public static byte[] DeZlib(this byte[] buffer)
        => Processor.Dump(new System.IO.Compression.ZLibStream(new MStream(buffer), System.IO.Compression.CompressionMode.Decompress));

    /// <summary>
    /// Decompresses zlib data.
    /// </summary>
    /// <param name="stream">Stream to read.</param>
    /// <returns>Decompressed data.</returns>
    public static byte[] DeZlib(this Stream stream)
        => Processor.Dump(new System.IO.Compression.ZLibStream(stream, System.IO.Compression.CompressionMode.Decompress, true));
#endif
}