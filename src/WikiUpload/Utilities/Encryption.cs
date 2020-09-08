using System;
using System.Text;
using System.Security.Cryptography;


namespace WikiUpload
{
    public static partial class Encryption
    {
        // If you get a build error here you need to create Utilities\Entropy.cs containing something
        // like the following, with your own set of entropy data because the actual data is a secret.
        //
        //namespace WikiUpload
        //{
        //    public partial class Encryption
        //    {
        //        private static readonly byte[] entropy = { 12, 222, 41, 108, 99, 63, 11, 12, 244, 201, 63 };
        //    }
        //}

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
