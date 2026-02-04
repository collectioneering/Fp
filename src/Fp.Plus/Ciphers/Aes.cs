using System;
using System.Security.Cryptography;


// ReSharper disable once CheckNamespace
namespace Fp;

public partial class PlusUtil
{
    /// <summary>
    /// Decrypts with Aes using ECB mode and key.
    /// </summary>
    /// <param name="src">Source span.</param>
    /// <param name="key">Cipher key.</param>
    public static unsafe void DecryptAesEcb(Span<byte> src, ReadOnlySpan<byte> key)
    {
        using Aes aes = Aes.Create() ?? throw new ApplicationException();
        aes.Key = key.ToArray();
        aes.Padding = PaddingMode.None;
        aes.Mode = CipherMode.ECB;
        ICryptoTransform decryptor = aes.CreateDecryptor();
        fixed (byte* p = &src.GetPinnableReference())
        {
            using PStream ps = new(new IntPtr(p), src.Length);
            using PStream ps2 = new(new IntPtr(p), src.Length);
            using CryptoStream cs = new(ps, decryptor, CryptoStreamMode.Read);
            cs.CopyTo(ps2);
        }
    }

    /// <summary>
    /// Decrypts with Aes using CBC mode and key/IV.
    /// </summary>
    /// <param name="src">Source span.</param>
    /// <param name="key">Cipher key.</param>
    /// <param name="iv">IV (CBC/CTR).</param>
    public static unsafe void DecryptAesCbc(Span<byte> src, ReadOnlySpan<byte> key, ReadOnlySpan<byte> iv = default)
    {
        using Aes aes = Aes.Create() ?? throw new ApplicationException();
        aes.Key = key.ToArray();
        aes.Padding = PaddingMode.None;
        aes.Mode = CipherMode.CBC;
        aes.IV = iv.IsEmpty ? new byte[128 / 8] : iv.ToArray();
        ICryptoTransform decryptor = aes.CreateDecryptor();
        fixed (byte* p = &src.GetPinnableReference())
        {
            using PStream ps = new(new IntPtr(p), src.Length);
            using PStream ps2 = new(new IntPtr(p), src.Length);
            using CryptoStream cs = new(ps, decryptor, CryptoStreamMode.Read);
            cs.CopyTo(ps2);
        }
    }
}
