using System.IO.Compression;
using Esper.Zstandard;
using Fp;
using Fp.Fs;


public class EdelweissTgpProcessor : FormatMultiProcessor
{
    private static readonly FileProcessorInfo s_info = new(
        "EdelweissTgp",
        "Edelweiss TGP container",
        "Edelweiss TGP container",
        ".tgp");

    public EdelweissTgpProcessor() => Info = s_info;

    private static void Main(string[] args) => FsFormatMultiProcessor.Run<EdelweissTgpProcessor>(args, s_info);

    public override IEnumerable<Data> Process()
    {
        if (!HasMagic("TGP0")) return Nothing;
        ushort ver = u2l[0x4];
        return ver switch
        {
            0x0003 => Process_0x0003(),
            _ => UnsupportedVersion(ver)
        };
    }

    private enum Mode
    {
        Uncompressed,
        SingleSolidCompress,
        PerFileCompress
    }

    private IEnumerable<Data> Process_0x0003()
    {
        ushort flags = u2l[0x6];
        int count = i4l[0x8];
        int mainOff = 0x10 + count * 0x70;
        bool compress = (flags & 0x2) != 0;
        List<int>? positions = null;
        if (Debug)
        {
            LogInfo(Name);
        }
        int endAll = (int)InputLength;
        byte[]? decompressedSource = null;
        Mode mode;
        if (compress)
        {
            positions = new List<int>(Match(mainOff, "\x28\xb5\x2f\xfd").Select(v => (int)v));
            if (positions.Count != count)
            {
                if (positions.Count == 1 && count >= 1)
                {
                    int offset = positions[0];
                    decompressedSource = new ZstandardStream(buf[offset, (int)InputLength - offset].Stream(), CompressionMode.Decompress).Dump();
                    endAll = decompressedSource.Length;
                    mainOff = 0;
                    positions = null;
                    compress = false;
                    mode = Mode.SingleSolidCompress;
                }
                else
                {
                    LogWarn($"Incorrect match count for zstd magic {positions.Count}, expected {count}");
                    yield break;
                }
            }
            else
            {
                mode = Mode.PerFileCompress;
            }
        }
        else
        {
            mode = Mode.Uncompressed;
        }
        if (Debug)
        {
            LogInfo($"{Name}: {mode}");
        }
        for (int i = 0; i < count; i++)
        {
            byte[] entry = buf[0x10 + i * 0x70, 0x70];
            string name = utf8[entry, 0, 0x60].String;
            int offset = positions?[i] ?? mainOff + i4l[entry, 0x60];
            int uncompressedLength = i4l[entry, 0x64];
            int end = i + 1 == count ? endAll : positions?[i + 1] ?? offset + uncompressedLength;
            if (Debug) LogInfo($"#{i} {offset:X8}..{offset + uncompressedLength:X8}({uncompressedLength:X8})/{endAll:X8} {name}");
            var stream = compress
                ? new ZstandardStream(buf[offset, end - offset].Stream(), CompressionMode.Decompress)
                : decompressedSource != null
                    ? buf[decompressedSource, offset, end - offset].ToArray().Stream()
                    : buf[offset, end - offset].Stream();
            yield return Buffer(name, stream.Dump());
        }
    }
}
