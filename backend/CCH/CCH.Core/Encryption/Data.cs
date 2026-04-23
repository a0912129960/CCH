using System;
using System.Text;

namespace CCH.Core.Encryption
{
    /// <summary>
    /// Ported from MyDimerco. Represents byte-oriented data for encryption.
    /// (繁體中文) 從 MyDimerco 移植。代表用於加密的位元組導向資料。
    /// </summary>
    public class Data
    {
        private byte[] _b;
        private int _MaxBytes = 0;
        private int _MinBytes = 0;
        private int _StepBytes = 0;

        public static Encoding DefaultEncoding = System.Text.Encoding.UTF8;
        public Encoding Encoding = DefaultEncoding;

        public Data() { }
        public Data(byte[] b) { _b = b; }
        public Data(string s) { this.Text = s; }

        public bool IsEmpty => _b == null || _b.Length == 0;

        public int StepBytes { get => _StepBytes; set => _StepBytes = value; }
        public int MinBytes { get => _MinBytes; set => _MinBytes = value; }
        public int MaxBytes { get => _MaxBytes; set => _MaxBytes = value; }

        public byte[] Bytes
        {
            get
            {
                if (_MaxBytes > 0 && _b != null && _b.Length > _MaxBytes)
                {
                    byte[] b = new byte[_MaxBytes];
                    Array.Copy(_b, b, b.Length);
                    _b = b;
                }
                if (_MinBytes > 0 && _b != null && _b.Length < _MinBytes)
                {
                    byte[] b = new byte[_MinBytes];
                    Array.Copy(_b, b, _b.Length);
                    _b = b;
                }
                return _b;
            }
            set { _b = value; }
        }

        public string Text
        {
            get
            {
                if (_b == null) return "";
                int i = Array.IndexOf(_b, (byte)0);
                return i >= 0 ? this.Encoding.GetString(_b, 0, i) : this.Encoding.GetString(_b);
            }
            set { _b = this.Encoding.GetBytes(value); }
        }

        public string Base64
        {
            get => _b == null ? "" : Convert.ToBase64String(_b);
            set => _b = string.IsNullOrEmpty(value) ? null : Convert.FromBase64String(value);
        }
    }
}
