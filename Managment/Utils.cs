using System.Security.Cryptography;
using System.Text;

namespace ProjectManagementSystem
{
    public static class Utils
    {
        private static readonly string EncryptionKey = "your-encryption-key"; // Should be more secure

        public static string Encrypt(string text)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(EncryptionKey);
                aes.IV = new byte[16]; // Initialization vector with zeros

                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                var bytes = Encoding.UTF8.GetBytes(text);

                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(bytes, 0, bytes.Length);
                        cs.FlushFinalBlock();
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
        }

        public static string Decrypt(string encryptedText)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(EncryptionKey);
                aes.IV = new byte[16]; // Initialization vector with zeros

                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                var bytes = Convert.FromBase64String(encryptedText);

                using (var ms = new MemoryStream(bytes))
                {
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (var sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
