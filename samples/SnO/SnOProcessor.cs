using System.Runtime.InteropServices;
using System.Text;
using Fp;
using Fp.Fs;
using Fp.Plus;
using Fp.Plus.Images;


public class SnOProcessor : FormatMultiProcessor
{
    private static readonly FileProcessorInfo s_info = new(
        "SnO",
        "Sousei no Onmyouji containers",
        "Sousei no Onmyouji containers");

    public SnOProcessor() => Info = s_info;

    private static void Main(string[] args) => FsFormatMultiProcessor.Run<SnOProcessor>(args, s_info);

    public override IEnumerable<Data> Process()
    {
        switch (ReadUtf8String(out _, out _, 4))
        {
            case "bg  ":
                {
                    int w = i4l[4], h = i4l[8];
                    WriteContext<uint> wc = NamePathNoExt.CreateImage(w, h, out Data image);
                    Read(28, MemoryMarshal.Cast<uint, byte>(wc.Source), false);
                    yield return image;
                    break;
                }
            case "fig ":
                {
                    int px = i4l[20];
                    Memory<uint> pxBuf = new uint[px];
                    Read(24, MemoryMarshal.Cast<uint, byte>(pxBuf.Span), false);
                    int bPos = 24 + px * 4;
                    int numImages = i4l[bPos];
                    for (int i = 0; i < numImages; i++)
                    {
                        int cPos = bPos + 4 + i * 20;
                        int bx = i4l[cPos], by = i4l[cPos + 4], w = i4l[cPos + 8], h = i4l[cPos + 12], o = i4l[cPos + 16];
                        string name = $"{NameNoExt}_I{i:D4}";
                        yield return new Rgba32Data((NamePathNoExt / name).ToString(), w, h, pxBuf.Slice(o, w * h));
                        StringBuilder ib = new();
                        ib.Append("base_x:").Append(bx).AppendLine();
                        ib.Append("base_y:").Append(by).AppendLine();
                        ib.Append("width:").Append(w).AppendLine();
                        ib.Append("height:").Append(h).AppendLine();
                        yield return new BufferData<byte>((NamePathNoExt / $"{name}.txt").ToString(), Encoding.UTF8.GetBytes(ib.ToString()));
                    }
                    int dPos = bPos + 4 + numImages * 20;
                    int numSequences = i4l[dPos + 4];
                    int ePos = dPos + 8;
                    for (int i = 0; i < numSequences; i++)
                    {
                        string id = utf8[ePos, 32].String, type = utf8[ePos, 32 * 2].String, num = utf8[ePos, 32 * 3].String;
                        ePos += 32 * 3;
                        int numEntries = i4l[ePos];
                        ePos += 4;
                        StringBuilder ib = new();
                        ib.Append("id:").Append(id).AppendLine();
                        ib.Append("type:").Append(type).AppendLine();
                        ib.Append("num:").Append(num).AppendLine();
                        ib.Append("entries:").Append(numEntries).AppendLine();
                        for (int j = 0; j < numEntries; j++)
                        {
                            ib.Append('(').Append(i4l[ePos]).Append(',').Append(i4l[ePos + 4]).Append(')').AppendLine();
                            ePos += 8;
                        }
                        string name = $"{NameNoExt}_A{i:D4}";
                        yield return new BufferData<byte>((NamePathNoExt / $"{name}.txt").ToString(), Encoding.UTF8.GetBytes(ib.ToString()));
                    }
                    break;
                }
            case "RYHP":
                {
                    // ?????????????????

                    // BEN_BT_C1_s.png.phyre
                    // offset 494015
                    // 113992 bytes
                    // assume start at 3183
                    // 604824 data bytes?
                    // 151206 (0x0x24EA6) pixels?
                    // looks like width 1440 bytes, 360 pixels
                    // looks like 360*420 starting at 3207
                    /*for (int i = 3143; i < 3183; i += 4)
                    {
                        LogInfo(i4l[i].ToString());
                    }*/
                    // width @ 3068,
                    //int post3 = 4319; // btlch.pyre
                    //int dpos = 4560;
                    int post3 = 3056; // BEN_BT_C1_s.png.phyre
                    int dpos = 3207;
                    //int post3 = 3048; // BEN_BT_C1.png.phyre
                    //int dpos = 3199;

                    // pregroups 358
                    int w = i4l[post3 + 12], h = i4l[post3 + 16];
                    WriteContext<uint> wc = NamePathNoExt.CreateImage(w, h, out Data image);
                    Read(dpos, MemoryMarshal.Cast<uint, byte>(wc.Source), false);
                    // TODO more "research"
                    //yield return image;
                    break;
                }
            default:
                yield break;
        }
    }
}
