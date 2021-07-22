using System;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using WikiUpload.Utilities;

namespace WikiUpload
{
    internal class PasswordManager : IPasswordManager
    {
        private readonly PasswordDictionary _passwords;
        private readonly IPasswordStore _passwordStore;

        public PasswordManager(IPasswordStore passwordStore)
        {
            _passwordStore = passwordStore;
            _passwords = _passwordStore.Load();
        }

        private void Save()
        {
            _passwordStore.Save(_passwords);
        }

        public SecureCharArray GetPassword(string site, string username)
        {
            _ = MakeUri(site, out var uri);
            try
            {
                var password = GetDecryptedPassword(MakeKey(uri, username));
                if (password == null)
                    password = GetDecryptedPassword(MakeDomainKey(uri, username));
                return password;
            }
            catch (Exception ex) when (ex is CryptographicException || ex is FormatException)
            {
                _passwords.Clear();
                return null;
            }
        }

        public bool HasPassword(string site, string username)
        {
            if (!MakeUri(site, out var uri))
                return false;
            return _passwords.ContainsKey(MakeKey(uri, username))
                || _passwords.ContainsKey(MakeDomainKey(uri, username));
        }

        private SecureCharArray GetDecryptedPassword(string key)
        {
            SecureCharArray password = null;
            if (_passwords.TryGetValue(key, out var encryptedPassword))
                password = Encryption.Decrypt(encryptedPassword);
            return password;
        }

        public void SavePassword(string site, string username, SecureString password)
        {
            var key = MakeKey(site, username);
            SaveEncryptedPassword(key, password);
        }

        public void SaveDomainPassword(string site, string username, SecureString password)
        {
            var key = MakeDomainKey(site, username);
            SaveEncryptedPassword(key, password);
        }

        private void SaveEncryptedPassword(string key, SecureString passsword)
        {
            var encryptedPassword = EncryptPassword(passsword, key, out var passwordHasChanged);
            if (passwordHasChanged)
            {
                _passwords[key] = encryptedPassword;
                Save();
            }
        }

        private string EncryptPassword(SecureString passsword, string key, out bool passwordHasChanged)
        {
            string encryptedPassword;
            using (var currentPassword = GetDecryptedPassword(key))
            {
                encryptedPassword = passsword.UseUnsecuredString(unsecuredPassword =>
                {
                    return currentPassword == null || !unsecuredPassword.IsEquivalentTo(currentPassword.Data)
                        ? Encryption.Encrypt(unsecuredPassword)
                        : null;
                });
            }
            passwordHasChanged = encryptedPassword != null;
            return encryptedPassword;
        }

        public void RemovePassword(string site, string username)
        {
            _ = MakeUri(site, out var uri);

            var key = MakeKey(uri, username);
            if (!_passwords.ContainsKey(key))
                key = MakeDomainKey(uri, username);

            if (_passwords.ContainsKey(key))
            {
                _passwords.Remove(key);
                Save();
            }
        }

        private static string MakeKey(string site, string username)
        {
            _ = MakeUri(site, out var uri);
            return MakeKey(uri, username);
        }

        private static string MakeDomainKey(string site, string username)
        {
            _ = MakeUri(site, out var uri);
            return MakeDomainKey(uri, username);
        }

        private static string MakeKey(Uri site, string username)
            => FormatKey(username, site.Authority, site.PathAndQuery);

        private static string MakeDomainKey(Uri site, string username)
        {
            var authority = site.Authority.Count(x => x == '.') > 1
                ? site.Authority.Substring(site.Authority.IndexOf('.') + 1)
                : site.Authority;
            return FormatKey(username, authority, site.PathAndQuery);
        }

        private static bool MakeUri(string url, out Uri uri)
        {
            if (!(url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || url.StartsWith("https://", StringComparison.OrdinalIgnoreCase)))
                url = "https://" + url;
            return Uri.TryCreate(url, UriKind.Absolute, out uri);
        }

        private static string FormatKey(string username, string authority, string pathAndQuery)
            => username + "@" + authority + pathAndQuery;
    }
}
