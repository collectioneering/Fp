using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Fp;
using Fp.Fs;
using Fp.Fs.CommandLine;

public partial class FptProcessor
{
    private static readonly FileProcessorInfo s_info = new(
        "Fpt",
        "yourDescription",
        ".yourExtension1");

    public FptProcessor() => Info = s_info;
}

public partial class FptProcessor : FsProcessor
{
    private static void Main(string[] args) => CommandLineFsProcessor.Run<FptProcessor>(args, s_info);

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
public partial class FptProcessor : FsFormatSingleProcessor
{
    private static void Main(string[] args) => CommandLineFsFormatSingleProcessor.Run<FptProcessor>(args, s_info);

    public override bool TryProcess([NotNullWhen(true)] out Data? data)
    {
        // Implement your logic here
        
    }
}
*/

// One output of specific type, InputStream available
/*
public partial class FptProcessor : FsFormatSingleProcessor<BufferData<byte>>
{
    private static void Main(string[] args) => CommandLineFsFormatSingleProcessor.Run<FptProcessor>(args, s_info);

    public override bool TryProcess([NotNullWhen(true)] out BufferData<byte>? data)
    {
        // Implement your logic here
        
    }
}
*/

// Segmented, InputStream available
/*
public partial class FptProcessor : FsFormatMultiProcessor
{
    private static void Main(string[] args) => CommandLineFsFormatMultiProcessor.Run<FptProcessor>(args, s_info);

    public override IEnumerable<Data> Process()
    {
        // Implement your logic here
        
    }
}
*/