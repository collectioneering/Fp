using Fp;
using Fp.Fs.CommandLine;

namespace Fewss;

public class FewssProcessor : FormatMultiProcessor
{
    private static readonly FileProcessorInfo s_info = new(
        "FireEmblemWarriorsSwitchSound",
        "Fire Emblem Warriors Switch Sound Archives",
        "Fire Emblem Warriors Switch Sound Archives",
        ".bin.gz", ".bin", ".g1l");

    public FewssProcessor() => Info = s_info;

    private static void Main(string[] args) => CommandLineFsFormatMultiProcessor.Run<FewssProcessor>(args, s_info);

    public override IEnumerable<Data> Process() => SelectedExtension switch
    {
        ".bin.gz" => ProcessBinGz(),
        ".bin" => ProcessBin(),
        ".g1l" => ProcessG1L(),
        _ => Nothing
    };

    private IEnumerable<Data> ProcessBinGz()
    {
        int wbhOff = i4l[0];
        int wbhLen = i4l[4];
        int wbdOff = i4l[8];
        byte[] h = buf[wbhOff, wbhLen];
        int pos = -1, entryOffset;
        // skip to first nonzero header entry
        do entryOffset = i4l[h, 0x20 + ++pos * 4];
        while (entryOffset == 0);
        // set target offset (end of entry table)
        int initOff = entryOffset;
        // iterate through entries, prepare next offset
        while (0x20 + pos * 4 != initOff)
        {
            // find next nonzero entry to get entry length (or use header end if hit first offset)
            int c = 0;
            int nextOffset;
            do
            {
                nextOffset = 0x20 + (pos + ++c) * 4 == initOff
                    // Target offset reached entry starting point
                    ? wbhLen
                    // Load next possible offset
                    : i4l[h, 0x20 + (pos + c) * 4];
            } while (nextOffset == 0); // Continue iteration while keep hitting 0's for target end

            // work on current entry
            int subEntryCount = h[entryOffset + 0xC + 0x03];
            for (int i = 0; i < subEntryCount; i++)
            {
                //work on subentry
                int hOfs = entryOffset + 0xC + 0x34 + i * 0xAC;
                string name = $"{pos:D8}_{i:D8}.ktss";
                byte[] head = buf[wbhOff + hOfs, 0xAC];
                byte[] body = buf[wbdOff + i4l[h, hOfs + 0x10] + 0xC, i4l[h, hOfs + 0x14]];
                yield return Buffer(name, RestoreBinGz(head, body));
            }

            entryOffset = nextOffset; // Use next valid entry offset as entry offset
            pos += c; // Use next valid header index as header index
        }
    }

    private Memory<byte> RestoreBinGz(ReadOnlySpan<byte> head, ReadOnlySpan<byte> body)
    {
        byte[] b = new byte[0xA0 + body.Length];
        utf8[b] = "KTSS";
        i4l[b, 4] = 0xA0 + body.Length;
        b[0x20] = 0x02; // unknown, assumed
        b[0x22] = 0x03; // version ID, assumed (3 = Warriors)
        b[0x23] = 0x03; // unknown, assumed
        b[0x24] = 0x80; // start offset, assumed
        b[0x28] = 0x01; // channel multiplier, assumed
        b[0x29] = 0x01; // channel count, assumed
        i4l[b, 0x30] = i4l[head, 0x4C]; // num samples
        i4l[b, 0x2C] = i4l[head, 0x54]; // sample rate, usually 0x80BB0000
        buf[b, 0x40] = buf[head, 0x4C, 0x60]; // raw header data (+DSP coefficients)
        buf[b, 0xA0] = body; // body
        return b;
    }

    private IEnumerable<Data> ProcessBin()
    {
        int count = i4l[0];
        for (int i = 0; i < count; i++)
        {
            int ofs = i4l[4 + i * 4];
            int len = i4l[4 + i * 4 + 4];
            yield return Buffer($"{i:D8}.ktss", buf[ofs, len]);
        }
    }

    private IEnumerable<Data> ProcessG1L()
    {
        if (!HasMagic("_L1G0000")) yield break;
        int count = i4l[0x14];
        for (int i = 0; i < count; i++)
        {
            int ofs = i4l[0x18 + i * 4];
            int len = i4l[ofs + 4];
            yield return Buffer($"{i:D8}.ktss", buf[ofs, len]);
        }
    }
}
