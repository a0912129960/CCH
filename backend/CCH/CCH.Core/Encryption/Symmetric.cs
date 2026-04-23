using System;
using System.Security.Cryptography;
using System.IO;

namespace CCH.Core.Encryption
{
    /// <summary>
    /// Ported from MyDimerco. Implements Rijndael-based symmetric encryption using Aes.
    /// (繁體中文) 從 MyDimerco 移植。使用 Aes 實作基於 Rijndael 的對稱加密。
    /// </summary>
    public class Symmetric
    {
        private const string _DefaultIntializationVector = "%1Az=-@qT";
        public enum Provider { Rijndael }
        private Data _key;
        private Data _iv;
        private Aes _crypto;

        public Symmetric(Provider provider)
        {
            _crypto = Aes.Create();
            _crypto.Mode = CipherMode.CBC;
            _crypto.Padding = PaddingMode.PKCS7;
            this.IntializationVector = new Data(_DefaultIntializationVector);
        }

        public Data Key 
        { 
            get => _key; 
            set 
            { 
                _key = value; 
                _key.MaxBytes = _crypto.LegalKeySizes[0].MaxSize / 8;
                _key.MinBytes = _crypto.LegalKeySizes[0].MinSize / 8;
            } 
        }
        public Data IntializationVector 
        { 
            get => _iv; 
            set 
            { 
                _iv = value; 
                _iv.MaxBytes = _crypto.BlockSize / 8;
                _iv.MinBytes = _crypto.BlockSize / 8;
            } 
        }

        private void ValidateKeyAndIv()
        {
            _crypto.Key = _key.Bytes;
            _crypto.IV = _iv.Bytes;
        }

        public Data Encrypt(Data d)
        {
            ValidateKeyAndIv();
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, _crypto.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cs.Write(d.Bytes, 0, d.Bytes.Length);
            }
            return new Data(ms.ToArray());
        }

        public Data Decrypt(Data encryptedData)
        {
            ValidateKeyAndIv();
            using var ms = new MemoryStream(encryptedData.Bytes);
            using var cs = new CryptoStream(ms, _crypto.CreateDecryptor(), CryptoStreamMode.Read);
            using var output = new MemoryStream();
            cs.CopyTo(output);
            return new Data(output.ToArray());
        }
    }
}
