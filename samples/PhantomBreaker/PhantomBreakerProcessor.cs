using Fp;
using System.Collections;
using Fp.Fs;


public partial class PhantomBreakerProcessor : FormatMultiProcessor
{
    private static readonly FileProcessorInfo s_info = new(
        "PhantomBreaker",
        "Decode / extract assets from Phantom Breaker series",
        "Decode / extract assets from Phantom Breaker series"
    );

    public PhantomBreakerProcessor() => Info = s_info;

    private static void Main(string[] args) => FsFormatMultiProcessor.Run<PhantomBreakerProcessor>(args, s_info);

    public override IEnumerable<Data> Process()
    {
        Dictionary<(int i, int j), BufferData<byte>> dict = new();
        byte[] a = Load();
        int pos = i4l[a] ^= -1, numT1 = i4l[a, 4] ^= 0x12876623, posT2 = 8 + numT1 * 4;
        for (int i = 8, hSrc = 0xa9; i < pos; i++, hSrc += 6) a[i] ^= (byte)hSrc;
        for (int i = 0; i < numT1; i++)
        {
            int ofsT1 = i4l[a, 8 + i * 4];
            if (ofsT1 == -1) continue;
            int numT2 = i2l[a, posT2 + ofsT1], t2Bytes = numT2.GetBytesForBits();
            var bits = new BitArray(a.SliceAlloc(posT2 + ofsT1 + 2, t2Bytes));
            for (int j = 0, p = posT2 + ofsT1 + 2 + t2Bytes; j < numT2; j++, p += 8)
            {
                bits.ConstrainedSkipBits(ref j, false, numT2);
                var memory = a.AsMemory(pos + i4l[a, p], i4l[a, p + 4]);
                var sp = memory.Span;
                byte s = (byte)(((sp.Length >> 3) & 0x5e) + (i & 0x7a) * 3 + (j & 0x5f) * 7);
                for (int x = 0; x < sp.Length; x++) sp[x] ^= (byte)((0xe6 & x) ^ s);
                dict[(i, j)] = Buffer(NamePathNoExt / $"{i:D4}_{j:D4}{(string)memory._WAV().___("")}", memory);
            }
        }
        // Side note: assuming data objects get disposed after use, raw items should be exported last (since buffer gets reset to empty buffer)
        return RunMariko(dict).Concat(RunYunomi(dict)).Concat(dict.Values);
    }
}
