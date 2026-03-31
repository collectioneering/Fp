using System;

namespace Fp.Tests.Utility;

public class ProcessorTestBase : IDisposable
{
    protected Processor P;

    protected ProcessorTestBase()
    {
        P = new Processor();
    }

    public void Dispose()
    {
        P.Dispose();
        P = null;
    }
}
