using Fp;
using Fp.Fs.CommandLine;
using Fp.Plus;

namespace BrewBar;

public class BrewBarProcessor : FormatMultiProcessor
{
    private static readonly FileProcessorInfo s_info = new(
        "BrewBar",
        "Brew MP .bar",
        "BREW MP .bar containers with gzip-compressed files",
        ".bar");

    public BrewBarProcessor() => Info = s_info;

    private static void Main(string[] args) => CommandLineFsFormatMultiProcessor.Run<BrewBarProcessor>(args, s_info);

    public override IEnumerable<Data> Process()
    {
        int count = i4l[0x14];
        int[] offsets = i4la[0x28, count + 1];
        for (int i = 0; i < count; i++)
        {
            int offset = offsets[i];
            int hLength = i2l[offset];
            byte[] buffer = buf[(offset + hLength)..offsets[i + 1]].DeGzip();
            string ext = buffer
                ._BMP() // BMP image
                ._WAV() // WAV audio
                .__("cmid", ".cmf") // CMF seq sound (Qualcomm Compact Media Format)
                .__("«JSR184»\r\n\u001a\n", ".m3g") // M3G model (Java Mobile 3D Graphics API scene graph, JSR-184)
                .___(".bin");
            LogInfo($"{i}: {utf8[(2..hLength).WithOffset(offset)].String}");
            yield return Buffer(NamePathNoExt / $"{i}{ext}", buffer);
        }
    }
}
