using System;

namespace PDCore.Services.IServ
{
    public interface ISymmetricEncryptionService
    {
        Tuple<string, string> GetKeys();
        string Encrypt(string text);
        string Decrypt(string text);
        string Encrypt(string text, string IV, string key);
        string Decrypt(string encryptedText, string IV, string key);
    }
}
