using System;

namespace Fp.Tests.Utility;

public class ProcessorTestBase : IDisposable
{
    protected Processor P;

    protected ProcessorTestBase()
    {
        P = new Processor();
    }

    protected static byte[] CreateByteArray(int inputLength)
    {
        byte[] result = new byte[inputLength];
        Random.Shared.NextBytes(result);
        return result;
    }

    public void Dispose()
    {
        P.Dispose();
        P = null;
    }
}
