using PDCore.Services.IServ;
using System;
using System.Security.Cryptography;
using System.Text;

namespace PDCore.Services.Serv
{
    public class SymmetricEncryptionService : ISymmetricEncryptionService
    {
        private readonly string _encryptionKey;
        private readonly string _encryptionIV;

        public SymmetricEncryptionService(string encryptionKey, string encryptionIV)
        {
            _encryptionKey = encryptionKey;

            if (string.IsNullOrWhiteSpace(_encryptionKey))
                throw new ArgumentException($"{nameof(encryptionKey)} is not set!");

            _encryptionIV = encryptionIV;

            if (string.IsNullOrWhiteSpace(_encryptionIV))
                throw new ArgumentException($"{nameof(encryptionIV)} is not set!");
        }

        public string Encrypt(string text)
        {
            return Encrypt(text, _encryptionIV, _encryptionKey);
        }

        public string Decrypt(string text)
        {
            return Decrypt(text, _encryptionIV, _encryptionKey);
        }

        public Tuple<string, string> GetKeys()
        {
            var (Key, IVBase64) = CreateKeys();

            return new Tuple<string, string>(Key, IVBase64);
        }

        private (string Key, string IVBase64) CreateKeys()
        {
            using (Aes cipher = CreateCipher())
            {
                string key = Convert.ToBase64String(cipher.Key);

                string IVBase64 = Convert.ToBase64String(cipher.IV);

                return (key, IVBase64);
            }
        }

        private Aes CreateCipher()
        {
            // Default values: Keysize 256, Padding PKC27
            Aes cipher = Aes.Create();

            cipher.Mode = CipherMode.CBC;  // Ensure the integrity of the ciphertext if using CBC
            cipher.Padding = PaddingMode.PKCS7;

            return cipher;
        }

        private Aes CreateCipher(string keyBase64, string iVBase64)
        {
            var cipher = CreateCipher();

            cipher.Key = Convert.FromBase64String(keyBase64);
            cipher.IV = Convert.FromBase64String(iVBase64);

            return cipher;
        }

        public string Encrypt(string text, string IV, string key)
        {
            byte[] plaintext = Encoding.UTF8.GetBytes(text);

            using (Aes cipher = CreateCipher(key, IV))
            {
                using (ICryptoTransform cryptTransform = cipher.CreateEncryptor())
                {
                    byte[] cipherText = cryptTransform.TransformFinalBlock(plaintext, 0, plaintext.Length);

                    return Convert.ToBase64String(cipherText);
                }
            }
        }

        public string Decrypt(string encryptedText, string IV, string key)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);

            using (Aes cipher = CreateCipher(key, IV))
            {
                using (ICryptoTransform cryptTransform = cipher.CreateDecryptor())
                {
                    byte[] plainBytes = cryptTransform.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

                    return Encoding.UTF8.GetString(plainBytes);
                }
            }
        }
    }
}
