using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MeteringStation.Mobile.Extensions
{
    public static class StringExtensions
    {
        private const string MessageEncryptionKey = "287de7751edd4b3d9897d2f2ebc7e869";
        private const string MessageIV = "a3b334557cfe4613";

        public static string Decrypt(this string data)
        {
            byte[] key = Encoding.ASCII.GetBytes(MessageEncryptionKey);
            byte[] iv = Encoding.ASCII.GetBytes(MessageIV);

            using (var rijndaelManaged =
                    new RijndaelManaged { Key = key, IV = iv, Mode = CipherMode.CBC })
            {
                rijndaelManaged.BlockSize = 128;
                rijndaelManaged.KeySize = 256;
                using (var memoryStream =
                       new MemoryStream(Convert.FromBase64String(data)))
                using (var cryptoStream =
                       new CryptoStream(memoryStream,
                           rijndaelManaged.CreateDecryptor(key, iv),
                           CryptoStreamMode.Read))
                {
                    return new StreamReader(cryptoStream).ReadToEnd();
                }
            }
        }

        public static string Encrypt(this string data)
        {
            byte[] key = Encoding.ASCII.GetBytes(MessageEncryptionKey);
            byte[] iv = Encoding.ASCII.GetBytes(MessageIV);

            using (var rijndaelManaged =
                    new RijndaelManaged { Key = key, IV = iv, Mode = CipherMode.CBC })
            {
                rijndaelManaged.BlockSize = 128;
                rijndaelManaged.KeySize = 256;
                using (var memoryStream =
                       new MemoryStream())
                using (var cryptoStream =
                       new CryptoStream(memoryStream,
                           rijndaelManaged.CreateEncryptor(key, iv),
                           CryptoStreamMode.Write))
                {
                    using (var source = new StreamWriter(cryptoStream))
                        source.Write(data);
                    return Convert.ToBase64String(memoryStream.ToArray());
                }
            }
        }
    }
}
