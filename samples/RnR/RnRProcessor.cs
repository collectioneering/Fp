using Fp;
using Fp.Fs.CommandLine;
using Fp.Platforms.Nitro;

namespace RnR;

public class RnRProcessor : FormatMultiProcessor
{
    private static readonly FileProcessorInfo s_info = new(
        "RnR",
        "Ryuusei no Rockman 1/2/3 .bin containers",
        "Ryuusei no Rockman 1/2/3 .bin containers",
        ".bin");

    public RnRProcessor() => Info = s_info;

    private static void Main(string[] args) => CommandLineFsFormatMultiProcessor.Run<RnRProcessor>(args, s_info);

    public override IEnumerable<Data> Process()
    {
        /*
         * Format
         * (little)
         * {
         *   u32 offset
         *   u32 fake (top bit: compressed, rest: decomp len)
         * } * files
         * u32 filesize
         * u32 0xffff (RnR1/2) or 0 (RnR3)
         */
        for (int i = 0, h = 0, offset, next = i4l[0]; (offset = next) < InputLength; h += 8)
        {
            next = i4l[h + 8];
            byte[] file = buf[offset..next];
            int fake = i4l[h + 4];
            if (fake < 0 && next != offset) file = file.DeNitro(fake & int.MaxValue);
            yield return Buffer(NamePathNoExt / ($"{i++:D4}" + file.GetNitroExtension()), file);
        }
    }
}
