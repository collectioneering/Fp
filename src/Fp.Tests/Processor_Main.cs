using System;
using System.IO;
using System.Linq;
using System.Text;
using Fp.Tests.Utility;

namespace Fp.Tests;

public class Processor_Main : ProcessorTestBase
{
    [Fact]
    public void DefaultProperties_Correct()
    {
        Assert.Equal(new string[] { }, P.Args.ToArray());
        Assert.Equal(NullLog.Instance, P.LogWriter);
        Assert.False(P.Preload);
        Assert.False(P.Debug);
        Assert.False(P.Nop);
    }

    [Fact]
    public void Prepare_PropertiesInitialized()
    {
        string[] args = { "a", "b" };
        var sbl = new StringBuilderLog();
        var conf = new ProcessorConfiguration(args, preload: true, debug: true, nop: true, logWriter: sbl);
        P.Prepare(conf);
        Assert.Equal(args, P.Args.ToArray());
        Assert.Equal(sbl, P.LogWriter);
        Assert.True(P.Preload);
        Assert.True(P.Debug);
        Assert.True(P.Nop);
    }

    [Fact]
    public void Initialize_InstanceWithArgs_PropertiesInitialized()
    {
        string[] args = { "a", "b" };
        var sbl = new StringBuilderLog();
        var conf = new ProcessorConfiguration(args, preload: true, debug: true, nop: true, logWriter: sbl);
        P.Prepare(conf);
        var child = P.Initialize<Processor>(args);
        Assert.Equal(args, child.Args.ToArray());
        Assert.Equal(sbl, child.LogWriter);
        Assert.True(child.Preload);
        Assert.True(child.Debug);
        Assert.True(child.Nop);
    }

    [Fact]
    public void Initialize_StaticWithConfiguration_PropertiesInitialized()
    {
        string[] args = { "a", "b" };
        var sbl = new StringBuilderLog();
        var conf = new ProcessorConfiguration(args, preload: true, debug: true, nop: true, logWriter: sbl);
        var child = Processor.Initialize<Processor>(conf);
        Assert.Equal(args, child.Args.ToArray());
        Assert.Equal(sbl, child.LogWriter);
        Assert.True(child.Preload);
        Assert.True(child.Debug);
        Assert.True(child.Nop);
    }

    [Fact]
    public void LogInfo_Works()
    {
        var sbl = new StringBuilderLog("\n");
        P.Prepare(ProcessorConfiguration.Default with { LogWriter = sbl });
        P.LogInfo("text2impeach");
        Assert.Equal("text2impeach\n", sbl.GetContent());
    }

    [Fact]
    public void LogWarn_Works()
    {
        var sbl = new StringBuilderLog("\n");
        P.Prepare(ProcessorConfiguration.Default with { LogWriter = sbl });
        P.LogWarn("text2impeach");
        Assert.Equal("text2impeach\n", sbl.GetContent());
    }

    [Fact]
    public void LogFail_Works()
    {
        var sbl = new StringBuilderLog("\n");
        P.Prepare(ProcessorConfiguration.Default with { LogWriter = sbl });
        P.LogFail("text2impeach");
        Assert.Equal("text2impeach\n", sbl.GetContent());
    }

    [Fact]
    public void LogChunk_Works()
    {
        var sbl = new StringBuilderLog("\n");
        P.Prepare(ProcessorConfiguration.Default with { LogWriter = sbl });
        P.LogChunk("text", false);
        Assert.Equal("text", sbl.GetContent());
        P.LogChunk("2", true);
        Assert.Equal("text2\n", sbl.GetContent());
    }

    [Fact]
    public void Region_Exit_RestoresStream()
    {
        var ms1 = new MemoryStream();
        P.UseStream(ms1);
        using (_ = P.Region(0, 0))
        {
            Assert.False(ReferenceEquals(ms1, P.InputStream));
            Assert.NotNull(P.InputStream!);
        }
        Assert.True(ReferenceEquals(ms1, P.InputStream));
    }

    [Fact]
    public void Region_ExitAfterChangingStreamInside_RestoresStreamOutside()
    {
        var ms1 = new MemoryStream();
        P.UseStream(ms1);
        using (_ = P.Region(0, 0))
        {
            Assert.False(ReferenceEquals(ms1, P.InputStream));
            Assert.NotNull(P.InputStream!);
            var sub = P.InputStream;
            var ms2 = new MemoryStream();
            P.UseStream(ms2);
            Assert.False(ReferenceEquals(ms1, P.InputStream));
            Assert.False(ReferenceEquals(sub, P.InputStream));
            Assert.True(ReferenceEquals(ms2, P.InputStream));
        }
        Assert.True(ReferenceEquals(ms1, P.InputStream));
    }

    [Fact]
    public void Region_NestedEnterExitAfterChangingStream_RestoresChangedStream()
    {
        var ms1 = new MemoryStream(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 });
        P.UseStream(ms1);
        using (_ = P.Region(1, 4))
        {
            Assert.False(ReferenceEquals(ms1, P.InputStream));
            Assert.NotNull(P.InputStream!);
            var sub = P.InputStream;
            var ms2 = new MemoryStream();
            P.UseStream(ms2);
            Assert.False(ReferenceEquals(ms1, P.InputStream));
            Assert.False(ReferenceEquals(sub, P.InputStream));
            Assert.True(ReferenceEquals(ms2, P.InputStream));
            using (_ = P.Region(1, 2))
            {
                Assert.False(ReferenceEquals(ms1, P.InputStream));
                Assert.False(ReferenceEquals(sub, P.InputStream));
                Assert.False(ReferenceEquals(ms2, P.InputStream));
                Assert.NotNull(P.InputStream!);
                var ms3 = new MemoryStream();
                P.UseStream(ms3);
                Assert.False(ReferenceEquals(ms1, P.InputStream));
                Assert.False(ReferenceEquals(sub, P.InputStream));
                Assert.False(ReferenceEquals(ms2, P.InputStream));
                Assert.True(ReferenceEquals(ms3, P.InputStream));
            }
            Assert.True(ReferenceEquals(ms2, P.InputStream));
        }
        Assert.True(ReferenceEquals(ms1, P.InputStream));
    }

    [Fact]
    public void Region_CreatesCorrectSubStream()
    {
        var ms1 = new MemoryStream(new byte[] { 0, 1, 2, 3 });
        P.UseStream(ms1);
        using (_ = P.Region(1, 2))
        {
            var stream = P.InputStream;
            Assert.NotNull(stream!);
            Assert.Equal(2, stream.Length);
            Assert.Equal(1, stream.ReadByte());
            Assert.Equal(2, stream.ReadByte());
            Assert.Equal(-1, stream.ReadByte());
        }
    }

    [Fact]
    public void Region_CreatesCorrectAutomaticLength()
    {
        var ms1 = new MemoryStream(new byte[] { 0, 1, 2, 3 });
        P.UseStream(ms1);
        using (_ = P.Region(1))
        {
            var stream = P.InputStream;
            Assert.NotNull(stream!);
            Assert.Equal(3, stream.Length);
            Assert.Equal(1, stream.ReadByte());
            Assert.Equal(2, stream.ReadByte());
            Assert.Equal(3, stream.ReadByte());
            Assert.Equal(-1, stream.ReadByte());
        }
    }

    [Fact]
    public void Region_UnwrapsSStreamCorrectly()
    {
        var ms1 = new MemoryStream(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 });
        ms1.Position = 1;
        var ss1 = new SStream(ms1, 6);
        ss1.Position = 1;
        var ss2 = new SStream(ss1, 4);
        ss2.Position = 1;
        var ss3 = new SStream(ss2, 2);
        P.UseStream(ss3);
        using (_ = P.Region(0, 2))
        {
            Assert.False(ReferenceEquals(ms1, P.InputStream));
            var stream = P.InputStream as SStream;
            Assert.NotNull(stream!);
            Assert.True(ReferenceEquals(ms1, stream.BaseStream));
            Assert.Equal(2, stream.Length);
            Assert.Equal(3, stream.ReadByte());
            Assert.Equal(4, stream.ReadByte());
            Assert.Equal(-1, stream.ReadByte());
            stream.Position = 0;
            Assert.Equal(3, ms1.Position);
        }
    }

    [Fact]
    public void Cleanup_ClearsInputStream()
    {
        var input = new MemoryStream();
        P.UseStream(input);
        Assert.NotNull(P.InputStream);
        P.Cleanup();
        Assert.Null(P.InputStream);
        Assert.Throws<ObjectDisposedException>(() => input.ReadByte());
    }

    [Fact]
    public void Dispose_ClearsInputStream()
    {
        var input = new MemoryStream();
        P.UseStream(input);
        Assert.NotNull(P.InputStream);
        P.Dispose();
        Assert.Null(P.InputStream);
        Assert.Throws<ObjectDisposedException>(() => input.ReadByte());
    }

    [Fact]
    public void Cleanup_ClearsOutputStream()
    {
        var output = new MemoryStream();
        P.UseOutputStream(output);
        Assert.NotNull(P.OutputStream);
        P.Cleanup();
        Assert.Null(P.OutputStream);
        Assert.Throws<ObjectDisposedException>(() => output.WriteByte(0));
    }

    [Fact]
    public void Dispose_ClearsOutputStream()
    {
        var output = new MemoryStream();
        P.UseOutputStream(output);
        Assert.NotNull(P.OutputStream);
        P.Dispose();
        Assert.Null(P.OutputStream);
        Assert.Throws<ObjectDisposedException>(() => output.WriteByte(0));
    }

    [Fact]
    public void Cleanup_ClearsMemoryAnnotations()
    {
        P.Debug = true;
        byte[] data = Encoding.UTF8.GetBytes("hello world");
        Span<byte> pattern = stackalloc byte[] { (byte)'o' };
        P.MemLabel(data, pattern);
        Assert.Single(P.MemAnnotations);
        P.Cleanup();
        Assert.Empty(P.MemAnnotations);
    }

    [Fact]
    public void Dispose_ClearsMemoryAnnotations()
    {
        P.Debug = true;
        byte[] data = Encoding.UTF8.GetBytes("hello world");
        Span<byte> pattern = stackalloc byte[] { (byte)'o' };
        P.MemLabel(data, pattern);
        Assert.Single(P.MemAnnotations);
        P.Dispose();
        Assert.Empty(P.MemAnnotations);
    }
}
