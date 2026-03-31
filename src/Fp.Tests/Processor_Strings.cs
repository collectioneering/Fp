using System.IO;
using Fp.Tests.Utility;

namespace Fp.Tests;

public class Processor_Strings : ProcessorTestBase
{
    private const string Message = "pontoon";
    private const string MessageBOM = "\ufeffpontoon";
    private static readonly byte[] Message_UTF8 = { 0x70, 0x6F, 0x6E, 0x74, 0x6F, 0x6F, 0x6E };
    private static readonly byte[] Message_UTF8Null = { 0x70, 0x6F, 0x6E, 0x74, 0x6F, 0x6F, 0x6E, 0x00 };
    private static readonly byte[] Message_UTF16LE = { 0x70, 0x00, 0x6F, 0x00, 0x6E, 0x00, 0x74, 0x00, 0x6F, 0x00, 0x6F, 0x00, 0x6E, 0x00 };
    private static readonly byte[] Message_UTF16LENull = { 0x70, 0x00, 0x6F, 0x00, 0x6E, 0x00, 0x74, 0x00, 0x6F, 0x00, 0x6F, 0x00, 0x6E, 0x00, 0x00, 0x00 };
    private static readonly byte[] Message_UTF16BE = { 0x00, 0x70, 0x00, 0x6F, 0x00, 0x6E, 0x00, 0x74, 0x00, 0x6F, 0x00, 0x6F, 0x00, 0x6E };
    private static readonly byte[] Message_UTF16BENull = { 0x00, 0x70, 0x00, 0x6F, 0x00, 0x6E, 0x00, 0x74, 0x00, 0x6F, 0x00, 0x6F, 0x00, 0x6E, 0x00, 0x00 };
    private static readonly byte[] Message_UTF16LEBOM = { 0xFF, 0xFE, 0x70, 0x00, 0x6F, 0x00, 0x6E, 0x00, 0x74, 0x00, 0x6F, 0x00, 0x6F, 0x00, 0x6E, 0x00 };
    private static readonly byte[] Message_UTF16LEBOMNull = { 0xFF, 0xFE, 0x70, 0x00, 0x6F, 0x00, 0x6E, 0x00, 0x74, 0x00, 0x6F, 0x00, 0x6F, 0x00, 0x6E, 0x00, 0x00, 0x00 };
    private static readonly byte[] Message_UTF16BEBOM = { 0xFE, 0xFF, 0x00, 0x70, 0x00, 0x6F, 0x00, 0x6E, 0x00, 0x74, 0x00, 0x6F, 0x00, 0x6F, 0x00, 0x6E };
    private static readonly byte[] Message_UTF16BEBOMNull = { 0xFE, 0xFF, 0x00, 0x70, 0x00, 0x6F, 0x00, 0x6E, 0x00, 0x74, 0x00, 0x6F, 0x00, 0x6F, 0x00, 0x6E, 0x00, 0x00 };

    [Fact]
    public void WriteUtf8String_Basic_Correct()
    {
        MemoryStream ms = new();
        P.WriteUtf8String(Message, false, ms);
        Assert.Equal(Message_UTF8, ms.ToArray());
        ms.SetLength(0);
        P.WriteUtf8String(Message, true, ms);
        Assert.Equal(Message_UTF8Null, ms.ToArray());
    }

    [Fact]
    public void ReadUtf8String_Basic_Correct()
    {
        MemoryStream ms = new();
        ms.Write(Message_UTF8);
        ms.Position = 0;
        Assert.Equal(Message, P.ReadUtf8String(ms, out int read, out int numBytes));
        Assert.Equal(Message_UTF8.Length, read);
        Assert.Equal(Message_UTF8.Length, numBytes);
        ms.SetLength(0);
        ms.Write(Message_UTF8Null);
        ms.Position = 0;
        Assert.Equal(Message, P.ReadUtf8String(ms, out read, out numBytes));
        Assert.Equal(Message_UTF8Null.Length, read);
        Assert.Equal(Message_UTF8.Length, numBytes);
    }

    [Fact]
    public void WriteUtf16String_BasicLE_Correct()
    {
        MemoryStream ms = new();
        P.WriteUtf16String(Message, false, false, false, ms);
        Assert.Equal(Message_UTF16LE, ms.ToArray());
        ms.SetLength(0);
        P.WriteUtf16String(Message, true, false, false, ms);
        Assert.Equal(Message_UTF16LENull, ms.ToArray());
    }

    [Fact]
    public void ReadUtf16String_BasicLE_Correct()
    {
        MemoryStream ms = new();
        ms.Write(Message_UTF16LE);
        ms.Position = 0;
        Assert.Equal(Message, P.ReadUtf16String(ms, out int read, out int numBytes));
        Assert.Equal(Message_UTF16LE.Length, read);
        Assert.Equal(Message_UTF16LE.Length, numBytes);
        ms.SetLength(0);
        ms.Write(Message_UTF16LENull);
        ms.Position = 0;
        Assert.Equal(Message, P.ReadUtf16String(ms, out read, out numBytes));
        Assert.Equal(Message_UTF16LENull.Length, read);
        Assert.Equal(Message_UTF16LE.Length, numBytes);
    }

    [Fact]
    public void WriteUtf16String_BasicBE_Correct()
    {
        MemoryStream ms = new();
        P.WriteUtf16String(Message, false, true, false, ms);
        Assert.Equal(Message_UTF16BE, ms.ToArray());
        ms.SetLength(0);
        P.WriteUtf16String(Message, true, true, false, ms);
        Assert.Equal(Message_UTF16BENull, ms.ToArray());
    }

    [Fact]
    public void ReadUtf16String_BasicBE_Correct()
    {
        MemoryStream ms = new();
        ms.Write(Message_UTF16BE);
        ms.Position = 0;
        Assert.Equal(Message, P.ReadUtf16String(ms, out int read, out int numBytes));
        Assert.Equal(Message_UTF16BE.Length, read);
        Assert.Equal(Message_UTF16BE.Length, numBytes);
        ms.SetLength(0);
        ms.Write(Message_UTF16BENull);
        ms.Position = 0;
        Assert.Equal(Message, P.ReadUtf16String(ms, out read, out numBytes));
        Assert.Equal(Message_UTF16BENull.Length, read);
        Assert.Equal(Message_UTF16BE.Length, numBytes);
    }

    [Fact]
    public void WriteUtf16String_LEBOM_Correct()
    {
        MemoryStream ms = new();
        P.WriteUtf16String(Message, false, false, true, ms);
        Assert.Equal(Message_UTF16LEBOM, ms.ToArray());
        ms.SetLength(0);
        P.WriteUtf16String(Message, true, false, true, ms);
        Assert.Equal(Message_UTF16LEBOMNull, ms.ToArray());
    }

    [Fact]
    public void ReadUtf16String_LEBOM_Correct()
    {
        MemoryStream ms = new();
        ms.Write(Message_UTF16LEBOM);
        ms.Position = 0;
        Assert.Equal(MessageBOM, P.ReadUtf16String(ms, out int read, out int numBytes));
        Assert.Equal(Message_UTF16LEBOM.Length, read);
        Assert.Equal(Message_UTF16LEBOM.Length, numBytes);
        ms.SetLength(0);
        ms.Write(Message_UTF16LEBOMNull);
        ms.Position = 0;
        Assert.Equal(MessageBOM, P.ReadUtf16String(ms, out read, out numBytes));
        Assert.Equal(Message_UTF16LEBOMNull.Length, read);
        Assert.Equal(Message_UTF16LEBOM.Length, numBytes);
    }

    [Fact]
    public void WriteUtf16String_BEBOM_Correct()
    {
        MemoryStream ms = new();
        P.WriteUtf16String(Message, false, true, true, ms);
        Assert.Equal(Message_UTF16BEBOM, ms.ToArray());
        ms.SetLength(0);
        P.WriteUtf16String(Message, true, true, true, ms);
        Assert.Equal(Message_UTF16BEBOMNull, ms.ToArray());
    }

    [Fact]
    public void ReadUtf16String_BEBOM_Correct()
    {
        MemoryStream ms = new();
        ms.Write(Message_UTF16BEBOM);
        ms.Position = 0;
        Assert.Equal(MessageBOM, P.ReadUtf16String(ms, out int read, out int numBytes));
        Assert.Equal(Message_UTF16BEBOM.Length, read);
        Assert.Equal(Message_UTF16BEBOM.Length, numBytes);
        ms.SetLength(0);
        ms.Write(Message_UTF16BEBOMNull);
        ms.Position = 0;
        Assert.Equal(MessageBOM, P.ReadUtf16String(ms, out read, out numBytes));
        Assert.Equal(Message_UTF16BEBOMNull.Length, read);
        Assert.Equal(Message_UTF16BEBOM.Length, numBytes);
    }

    // TODO
}
