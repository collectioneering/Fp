using System;
using System.Collections.Generic;
using Fp;
using Fp.Fs;

FptProcessor.Run<FptProcessor>(args,
    "Fpt",
    "yourDescription",
    ".yourExtension1"); // Remove if no filtering desired
public class FptProcessor : FsProcessor
{
    protected override void ProcessImpl()
    {
        // Implement your logic here

    }

    // Alternate segmented processing
    /*
    protected override IEnumerable<Data> ProcessSegmentedImpl()
    {
        // Implement your logic here
        
    }
    */
}

// One output, InputStream available
/*
public class FptProcessor : FsFormatSingleProcessor
{
    public override bool TryProcess(out Data? data)
    {
        // Implement your logic here
        
    }
}
*/

// One output of specific type, InputStream available
/*
public class FptProcessor : FsFormatSingleProcessor<BufferData<byte>>
{
    public override bool TryProcess(out BufferData<byte>? data)
    {
        // Implement your logic here
        
    }
}
*/

// Segmented, InputStream available
/*
public class FptProcessor : FsFormatMultiProcessor
{
    public override IEnumerable<Data> Process()
    {
        // Implement your logic here
        
    }
}
*/