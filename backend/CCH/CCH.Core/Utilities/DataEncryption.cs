using CCH.Core.Encryption;

namespace CCH.Core.Utilities
{
    /// <summary>
    /// Ported from MyDimerco. Global encryption utility class.
    /// (繁體中文) 從 MyDimerco 移植。全域加密工具類別。
    /// </summary>
    public static class DataEncryption
    {
        private static readonly Symmetric symEncryption = new(Symmetric.Provider.Rijndael);
        private static readonly string ValueKey = "1qaz9ol.";

        public static string Encryption(string orgString)
        {
            if (string.IsNullOrEmpty(orgString)) return "";
            symEncryption.Key = new Data(ValueKey);
            try { return symEncryption.Encrypt(new Data(orgString)).Base64; }
            catch { return ""; }
        }

        public static string Decryption(string orgString)
        {
            if (string.IsNullOrEmpty(orgString)) return "";
            symEncryption.Key = new Data(ValueKey);
            try { return symEncryption.Decrypt(new Data() { Base64 = orgString }).Text; }
            catch { return ""; }
        }
    }
}
