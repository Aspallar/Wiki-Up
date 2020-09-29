using System.Security;
using WikiUpload.Extensions;
using WikiUpload.Utilities;

namespace WikiUpload
{
    public class PasswordManager : IPasswordManager
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
            string key = MakeKey(site, username);
            var password = GetPasswordFromKey(key);
            return password;
        }

        public bool HasPassword(string site, string username)
            => _passwords.ContainsKey(MakeKey(site, username));

        private SecureCharArray GetPasswordFromKey(string key)
        {
            SecureCharArray password = null;
            if (_passwords.TryGetValue(key, out string encryptedPassword))
                password = Encryption.Decrypt(encryptedPassword);
            return password;
        }

        public void SavePassword(string site, string username, SecureString passsword)
        {
            string encryptedPassword;
            string key = MakeKey(site, username);
            using (var currentPassword = GetPasswordFromKey(key))
            {
                encryptedPassword = passsword.UseUnsecuredString<string>(unsecuredPassword =>
                {
                    if (currentPassword == null || !unsecuredPassword.IsEquivalentTo(currentPassword.Data))
                        return Encryption.Encrypt(unsecuredPassword);
                    else
                        return null;
                });
            }

            if (encryptedPassword != null)
            {
                _passwords[key] = encryptedPassword;
                Save();
            }
        }

        public void RemovePassword(string site, string username)
        {
            var key = MakeKey(site, username);
            if (_passwords.ContainsKey(key))
            {
                _passwords.Remove(key);
                Save();
            }
        }

        private static string MakeKey(string site, string username) => site + username;
    }
}
