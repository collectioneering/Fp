using Fp.Tests.Utility;

namespace Fp.Tests;

public class Processor_EncodingHelpers : ProcessorTestBase
{
    [Fact]
    public void Construct_HelpersInitialized()
    {
        Assert.NotNull(P.buf);
        Assert.NotNull(P.i1);
        Assert.NotNull(P.i1a);
        Assert.NotNull(P.u1);
        Assert.NotNull(P.u1a);
        Assert.NotNull(P.i2l);
        Assert.NotNull(P.i2la);
        Assert.NotNull(P.i4l);
        Assert.NotNull(P.i4la);
        Assert.NotNull(P.i8l);
        Assert.NotNull(P.i8la);
        Assert.NotNull(P.u2l);
        Assert.NotNull(P.u2la);
        Assert.NotNull(P.u4l);
        Assert.NotNull(P.u4la);
        Assert.NotNull(P.u8l);
        Assert.NotNull(P.u8la);
        Assert.NotNull(P.i2b);
        Assert.NotNull(P.i2ba);
        Assert.NotNull(P.i4b);
        Assert.NotNull(P.i4ba);
        Assert.NotNull(P.i8b);
        Assert.NotNull(P.i8ba);
        Assert.NotNull(P.u2b);
        Assert.NotNull(P.u2ba);
        Assert.NotNull(P.u4b);
        Assert.NotNull(P.u4ba);
        Assert.NotNull(P.u8b);
        Assert.NotNull(P.u8ba);
        Assert.NotNull(P.f2);
        Assert.NotNull(P.f2a);
        Assert.NotNull(P.f4);
        Assert.NotNull(P.f4a);
        Assert.NotNull(P.f8);
        Assert.NotNull(P.f8a);
        Assert.NotNull(P.ascii);
        Assert.NotNull(P.utf8);
        Assert.NotNull(P.utf16);
    }
}
