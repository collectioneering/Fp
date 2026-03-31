using System;
using System.Linq;
using System.Text;
using Fp.Tests.Utility;

namespace Fp.Tests;

public class Processor_Debug : ProcessorTestBase
{
    [Fact]
    public void MemClear_Clears()
    {
        P.Debug = true;
        byte[] data = Encoding.UTF8.GetBytes("hello world");
        Span<byte> pattern = stackalloc byte[] { (byte)'o' };
        P.MemLabel(data, pattern);
        Assert.NotEmpty(P.MemAnnotations);
        P.MemClear();
        Assert.Empty(P.MemAnnotations);
    }

    [Fact]
    public void MemLabel_PosDebugTrue_HasAssignment()
    {
        P.Debug = true;
        byte[] data = Encoding.UTF8.GetBytes("hello world");
        P.MemLabel(data, 0, 1);
        Assert.Single(P.MemAnnotations);
        var kv = P.MemAnnotations.Single();
        Assert.Equal((ReadOnlyMemory<byte>)data.AsMemory(), kv.Key);
        Assert.Single(kv.Value);
        Assert.Equal(0, kv.Value.Values[0].Offset);
        Assert.Equal(1, kv.Value.Values[0].Length);
    }

    [Fact]
    public void MemLabel_PatternDebugTrue_HasAssignment()
    {
        P.Debug = true;
        byte[] data = Encoding.UTF8.GetBytes("hello world");
        Span<byte> pattern = stackalloc byte[] { (byte)'o' };
        var results = P.MemLabel(data, pattern);
        Assert.Equal(2, results.Count);
        Assert.Single(P.MemAnnotations);
        var kv = P.MemAnnotations.Single();
        Assert.Equal((ReadOnlyMemory<byte>)data.AsMemory(), kv.Key);
        Assert.Equal(2, kv.Value.Count);
        Assert.Equal(4, kv.Value.Values[0].Offset);
        Assert.Equal(7, kv.Value.Values[1].Offset);
    }

    [Fact]
    public void MemLabel_PosDebugFalse_HasNoAssignment()
    {
        byte[] data = Encoding.UTF8.GetBytes("hello world");
        P.MemLabel(data, 0, 1);
        Assert.Empty(P.MemAnnotations);
    }

    [Fact]
    public void MemLabel_PatternDebugFalse_HasNoAssignment()
    {
        byte[] data = Encoding.UTF8.GetBytes("hello world");
        Span<byte> pattern = stackalloc byte[] { (byte)'o' };
        var results = P.MemLabel(data, pattern);
        Assert.Equal(2, results.Count);
        Assert.Empty(P.MemAnnotations);
    }

    [Fact]
    public void MemLabel_PosDebugTrueAndSameOffset_NoRepeatAssignment()
    {
        P.Debug = true;
        byte[] data = Encoding.UTF8.GetBytes("hello world");
        P.MemLabel(data, 0, 1, "a");
        Assert.Single(P.MemAnnotations);
        var kv = P.MemAnnotations.Single();
        Assert.Equal((ReadOnlyMemory<byte>)data.AsMemory(), kv.Key);
        Assert.Single(kv.Value);
        Assert.Equal("a", kv.Value[0].Label);
        P.MemLabel(data, 0, 1, "b");
        Assert.Single(P.MemAnnotations);
        var kv2 = P.MemAnnotations.Single();
        Assert.Equal((ReadOnlyMemory<byte>)data.AsMemory(), kv2.Key);
        Assert.Single(kv2.Value);
        Assert.Equal("a", kv.Value[0].Label);
    }

    [Fact]
    public void MemLabel_PatternDebugTrueAndSameOffset_NoRepeatAssignment()
    {
        P.Debug = true;
        byte[] data = Encoding.UTF8.GetBytes("hello world");
        P.MemLabel(data, new[] { (byte)'h' }, "a");
        Assert.Single(P.MemAnnotations);
        var kv = P.MemAnnotations.Single();
        Assert.Equal((ReadOnlyMemory<byte>)data.AsMemory(), kv.Key);
        Assert.Single(kv.Value);
        Assert.Equal("a", kv.Value[0].Label);
        P.MemLabel(data, new[] { (byte)'h' }, "b");
        Assert.Single(P.MemAnnotations);
        var kv2 = P.MemAnnotations.Single();
        Assert.Equal((ReadOnlyMemory<byte>)data.AsMemory(), kv2.Key);
        Assert.Single(kv2.Value);
        Assert.Equal("a", kv.Value[0].Label);
    }

    [Fact]
    public void MemLabel_RepeatingSequence_HasConcatenated()
    {
        P.Debug = true;
        byte[] data = Encoding.UTF8.GetBytes("super_ababab_drome_abababab");
        Span<byte> pattern = stackalloc byte[] { (byte)'a', (byte)'b' };
        var results = P.MemLabel(data, pattern);
        Assert.Equal(2, results.Count);
        Assert.Single(P.MemAnnotations);
        var kv = P.MemAnnotations.Single();
        Assert.Equal((ReadOnlyMemory<byte>)data.AsMemory(), kv.Key);
        Assert.Equal(2, kv.Value.Count);
        Assert.Equal(6, kv.Value.Values[0].Offset);
        Assert.Equal(6, kv.Value.Values[0].Length);
        Assert.Equal(19, kv.Value.Values[1].Offset);
        Assert.Equal(8, kv.Value.Values[1].Length);
    }

    [Fact]
    public void MemPrint_Plain_FormatMatches()
    {
        string expected = @"
0x0000000000 00 01 02 03
0x0000000004 04 05 06 07
0x0000000008 08 09 0A 0B
0x000000000C 0C 0D 0E 0F
0x0000000010 10         ^

".TrimStart('\r', '\n').Replace("\r", "").Replace("^", "");
        P.Debug = true;
        byte[] data = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        var sbl = new StringBuilderLog { Delimiter = "\n" };
        P.LogWriter = sbl;
        P.MemPrint(data, space: true, pow2Modulus: false, displayWidth: 41);
        Assert.Equal(expected, sbl.GetContent());
    }

    [Fact]
    public void MemLabel_PatternAnnotatedWhenDebugFalse_PrintNoAnnotation()
    {
        string expected = @"
0x0000000000 00 01 02 03
0x0000000004 04 05 06 07
0x0000000008 08 09 0A 0B
0x000000000C 0C 0D 0E 0F
0x0000000010 10         ^

".TrimStart('\r', '\n').Replace("\r", "").Replace("^", "");
        byte[] data = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        var sbl = new StringBuilderLog { Delimiter = "\n" };
        P.LogWriter = sbl;
        P.MemLabel(data, new byte[] { 0 }, "label1");
        P.Debug = true;
        P.MemPrint(data, space: true, pow2Modulus: false, displayWidth: 41);
        Assert.Equal(expected, sbl.GetContent());
    }

    [Fact]
    public void MemLabel_PosAnnotatedWhenDebugFalse_PrintNoAnnotation()
    {
        string expected = @"
0x0000000000 00 01 02 03
0x0000000004 04 05 06 07
0x0000000008 08 09 0A 0B
0x000000000C 0C 0D 0E 0F
0x0000000010 10         ^

".TrimStart('\r', '\n').Replace("\r", "").Replace("^", "");
        byte[] data = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        var sbl = new StringBuilderLog { Delimiter = "\n" };
        P.LogWriter = sbl;
        P.MemLabel(data, 0, 4, "label1");
        P.Debug = true;
        P.MemPrint(data, space: true, pow2Modulus: false, displayWidth: 41);
        Assert.Equal(expected, sbl.GetContent());
    }

    [Fact]
    public void MemPrint_Annotated_FormatMatches()
    {
        string expected = @"
0x0000000000 00 01 02 03 label1
0x0000000004 04 05 06 07
0x0000000008 08 09 0A 0B
0x000000000C 0C 0D 0E 0F
0x0000000010 10         ^

".TrimStart('\r', '\n').Replace("\r", "").Replace("^", "");
        P.Debug = true;
        byte[] data = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        var sbl = new StringBuilderLog { Delimiter = "\n" };
        P.LogWriter = sbl;
        P.MemLabel(data, 0, 4, "label1");
        P.MemPrint(data, space: true, pow2Modulus: false, displayWidth: 41);
        Assert.Equal(expected, sbl.GetContent());
    }

    [Fact]
    public void MemPrint_AnnotatedLongerThanMax16_LengthTrim()
    {
        string expected = @"
0x0000000000 00 01 02 03 0123456789abcdef
0x0000000004 04 05 06 07
0x0000000008 08 09 0A 0B
0x000000000C 0C 0D 0E 0F
0x0000000010 10         ^

".TrimStart('\r', '\n').Replace("\r", "").Replace("^", "");
        P.Debug = true;
        byte[] data = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        var sbl = new StringBuilderLog { Delimiter = "\n" };
        P.LogWriter = sbl;
        P.MemLabel(data, 0, 4, "0123456789abcdefg");
        P.MemPrint(data, space: true, pow2Modulus: false, displayWidth: 41);
        Assert.Equal(expected, sbl.GetContent());
    }

    [Fact]
    public void MemPrint_OverlapDifferentStart_CarryOver()
    {
        string expected = @"
0x0000000000 00 01 02 03 label1
0x0000000004 04 05 06 07 label2
0x0000000008 08 09 0A 0B
0x000000000C 0C 0D 0E 0F
0x0000000010 10         ^

".TrimStart('\r', '\n').Replace("\r", "").Replace("^", "");
        P.Debug = true;
        byte[] data = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        var sbl = new StringBuilderLog { Delimiter = "\n" };
        P.LogWriter = sbl;
        P.MemLabel(data, 0, 2, "label1");
        P.MemLabel(data, 1, 1, "label2");
        P.MemPrint(data, space: true, pow2Modulus: false, displayWidth: 41);
        Assert.Equal(expected, sbl.GetContent());
    }

    [Fact]
    public void MemPrint_DebugFalse_Noop()
    {
        byte[] data = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        var sbl = new StringBuilderLog { Delimiter = "\n" };
        P.LogWriter = sbl;
        P.MemPrint(data, space: true, pow2Modulus: false, displayWidth: 41);
        Assert.Equal("", sbl.GetContent());
    }

    [Fact]
    public void MemPrint_AnnotatedMultipleOnSameLine_CarryOver()
    {
        string expected = @"
0x0000000000 00 01 02 03 label1
0x0000000004 04 05 06 07 label2
0x0000000008 08 09 0A 0B
0x000000000C 0C 0D 0E 0F
0x0000000010 10         ^

".TrimStart('\r', '\n').Replace("\r", "").Replace("^", "");
        P.Debug = true;
        byte[] data = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        var sbl = new StringBuilderLog { Delimiter = "\n" };
        P.LogWriter = sbl;
        P.MemLabel(data, 0, 2, "label1");
        P.MemLabel(data, 2, 2, "label2");
        P.MemPrint(data, space: true, pow2Modulus: false, displayWidth: 41);
        Assert.Equal(expected, sbl.GetContent());
    }

    [Fact]
    public void MemPrint_AnnotatedMultipleOnSameLineAfterEnd_CarryOver()
    {
        string expected = @"
0x0000000000 00 01 02 03
0x0000000004 04 05 06 07
0x0000000008 08 09 0A 0B
0x000000000C 0C 0D 0E 0F
0x0000000010 10 11       label1
                         label2^

".TrimStart('\r', '\n').Replace("\r", "").Replace("^", "");
        P.Debug = true;
        byte[] data = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17 };
        var sbl = new StringBuilderLog { Delimiter = "\n" };
        P.LogWriter = sbl;
        P.MemLabel(data, 0x10, 1, "label1");
        P.MemLabel(data, 0x11, 1, "label2");
        P.MemPrint(data, space: true, pow2Modulus: false, displayWidth: 41);
        Assert.Equal(expected, sbl.GetContent());
    }
}
