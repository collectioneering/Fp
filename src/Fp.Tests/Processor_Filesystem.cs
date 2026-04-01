using System;
using System.IO;
using Fp.Tests.Utility;

namespace Fp.Tests;

public class Processor_Filesystem : ProcessorTestBase
{
    [Fact]
    public void UseStream_AssignsInputStream()
    {
        var ms1 = new MemoryStream();
        P.UseStream(ms1);
        Assert.Same(ms1, P.InputStream);
    }

    [Fact]
    public void UseOutputStream_AssignsOutputStream()
    {
        var ms1 = new MemoryStream();
        P.UseOutputStream(ms1);
        Assert.Same(ms1, P.OutputStream);
    }

    [Fact]
    public void CloseFile_AsMainFalse_Noop()
    {
        var ms1 = new MemoryStream();
        P.UseStream(ms1);
        Assert.Same(ms1, P.InputStream);
        Assert.Equal(-1, ms1.ReadByte());
        P.CloseFile(false);
        Assert.Same(ms1, P.InputStream);
        Assert.Equal(-1, ms1.ReadByte());
    }

    [Fact]
    public void CloseFile_AsMainFalseCustomStream_ClosesAndDisposesInputStreamButDoesNotUnset()
    {
        var ms1 = new MemoryStream();
        P.UseStream(ms1);
        Assert.Same(ms1, P.InputStream);
        Assert.Equal(-1, ms1.ReadByte());
        var ms2 = new MemoryStream();
        Assert.Equal(-1, ms2.ReadByte());
        P.CloseFile(false, ms2);
        Assert.Equal(-1, ms1.ReadByte());
        Assert.Throws<ObjectDisposedException>(() => ms2.ReadByte());
        Assert.Same(ms1, P.InputStream);
    }

    [Fact]
    public void CloseFile_AsMainTrue_ClosesAndDisposesInputStreamAndUnsets()
    {
        var ms1 = new MemoryStream();
        P.UseStream(ms1);
        Assert.Same(ms1, P.InputStream);
        Assert.Equal(-1, ms1.ReadByte());
        P.CloseFile(true);
        Assert.Throws<ObjectDisposedException>(() => ms1.ReadByte());
        Assert.Null(P.InputStream);
    }

    [Fact]
    public void CloseFile_AsMainTrueCustomStream_ClosesAndDisposesStreamAndUnsets()
    {
        var ms1 = new MemoryStream();
        P.UseStream(ms1);
        Assert.Same(ms1, P.InputStream);
        Assert.Equal(-1, ms1.ReadByte());
        var ms2 = new MemoryStream();
        Assert.Equal(-1, ms2.ReadByte());
        P.CloseFile(true, ms2);
        Assert.Equal(-1, ms1.ReadByte());
        Assert.Throws<ObjectDisposedException>(() => ms2.ReadByte());
        Assert.Null(P.InputStream);
    }

    [Fact]
    public void CloseFile_ClosesAndDisposesInputStream()
    {
        var ms1 = new MemoryStream();
        P.UseStream(ms1);
        Assert.Same(ms1, P.InputStream);
        Assert.Equal(-1, ms1.ReadByte());
        P.CloseFile();
        Assert.Throws<ObjectDisposedException>(() => ms1.ReadByte());
    }

    [Fact]
    public void CloseOutputFile_ClosesAndDisposesInputStream()
    {
        var ms1 = new MemoryStream();
        P.UseOutputStream(ms1);
        Assert.Same(ms1, P.OutputStream);
        Assert.Equal(-1, ms1.ReadByte());
        P.CloseOutputFile();
        Assert.Throws<ObjectDisposedException>(() => ms1.ReadByte());
    }

    [Fact]
    public void CloseOutputFile_AsMainFalse_Noop()
    {
        var ms1 = new MemoryStream();
        P.UseOutputStream(ms1);
        Assert.Same(ms1, P.OutputStream);
        Assert.Equal(-1, ms1.ReadByte());
        P.CloseOutputFile(false);
        Assert.Same(ms1, P.OutputStream);
        Assert.Equal(-1, ms1.ReadByte());
    }

    [Fact]
    public void CloseOutputFile_AsMainFalseCustomStream_ClosesAndDisposesInputStreamButDoesNotUnset()
    {
        var ms1 = new MemoryStream();
        P.UseOutputStream(ms1);
        Assert.Same(ms1, P.OutputStream);
        Assert.Equal(-1, ms1.ReadByte());
        var ms2 = new MemoryStream();
        Assert.Equal(-1, ms2.ReadByte());
        P.CloseOutputFile(false, ms2);
        Assert.Equal(-1, ms1.ReadByte());
        Assert.Throws<ObjectDisposedException>(() => ms2.ReadByte());
        Assert.Same(ms1, P.OutputStream);
    }

    [Fact]
    public void CloseOutputFile_AsMainTrue_ClosesAndDisposesInputStreamAndUnsets()
    {
        var ms1 = new MemoryStream();
        P.UseOutputStream(ms1);
        Assert.Same(ms1, P.OutputStream);
        Assert.Equal(-1, ms1.ReadByte());
        P.CloseOutputFile(true);
        Assert.Throws<ObjectDisposedException>(() => ms1.ReadByte());
        Assert.Null(P.OutputStream);
    }

    [Fact]
    public void CloseOutputFile_AsMainTrueCustomStream_ClosesAndDisposesStreamAndUnsets()
    {
        var ms1 = new MemoryStream();
        P.UseOutputStream(ms1);
        Assert.Same(ms1, P.OutputStream);
        Assert.Equal(-1, ms1.ReadByte());
        var ms2 = new MemoryStream();
        Assert.Equal(-1, ms2.ReadByte());
        P.CloseOutputFile(true, ms2);
        Assert.Equal(-1, ms1.ReadByte());
        Assert.Throws<ObjectDisposedException>(() => ms2.ReadByte());
        Assert.Null(P.OutputStream);
    }

    // TODO
}
