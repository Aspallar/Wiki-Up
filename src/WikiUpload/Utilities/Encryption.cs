using System;
using System.Text;
using System.Security.Cryptography;


namespace WikiUpload
{
    public static class Encryption
    {
        private static readonly byte[] entropy = { 12, 222, 41, 108, 99, 63, 11, 12, 244, 201, 63 };

        public static string Decrypt(string text)
        {
            byte[] data = Convert.FromBase64String(text);
            byte[] unencrypted = ProtectedData.Unprotect(data, entropy, DataProtectionScope.CurrentUser);
            return Decode(unencrypted);
        }

        public static string Encrypt(string text)
        {
            byte[] data = Encode(text);
            byte[] encrypted = ProtectedData.Protect(data, entropy, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encrypted);
        }

        private static byte[] Encode(string data)
        {
            return Encoding.UTF8.GetBytes(data);
        }

        private static string Decode(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }


    }
}
