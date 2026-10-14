using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;

namespace ReraWebPortal.Helpers
{
    public class CryptoHelper
    {
        private static readonly string Key = "1234567890123456"; // 16/24/32 chars
        private static readonly string IV = "1234567890123456";  // 16 chars
        public static string Encrypt(string plainText)
        {
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(Key);
                aes.IV = Encoding.UTF8.GetBytes(IV);
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var encryptor = aes.CreateEncryptor())
                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(plainBytes, 0, plainBytes.Length);
                    cs.FlushFinalBlock();

                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }
       
        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentNullException(nameof(cipherText));

            // ✅ Correct decoding for URL-safe Base64
            byte[] cipherBytes = WebEncoders.Base64UrlDecode(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(Key);
                aes.IV = Encoding.UTF8.GetBytes(IV);
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using var decryptor = aes.CreateDecryptor();
                using var ms = new MemoryStream(cipherBytes);
                using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                using var reader = new StreamReader(cs);

                return reader.ReadToEnd();
            }
        }
    }
}
