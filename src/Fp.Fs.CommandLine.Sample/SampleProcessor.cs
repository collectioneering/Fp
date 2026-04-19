namespace Fp.Fs.CommandLine.Sample;

internal class SampleProcessor : FormatMultiProcessor
{
    private static readonly FileProcessorInfo s_info = new(
        "Sample",
        "sample runner",
        "sample runner that just logs info",
        ".txt");

    public SampleProcessor() => Info = s_info;

    private static void Main(string[] args) => CommandLineFsFormatMultiProcessor.Run<SampleProcessor>(args, s_info);

    public override IEnumerable<Data> Process()
    {
        LogInfo($"File:{NamePath} ({InputLength} bytes)");
        return [];
    }
}
