using System.Configuration;
using System.IO;
using System.Security;
using System.Xml.Serialization;

namespace WikiUpload
{
    public class PasswordManager : IPasswordManager
    {
        private PasswordDictionary _passwords;
        private string _fileName;

        //MessageBox.Show(config.FilePath);
        public PasswordManager()
        {
            _fileName = DetermineFileName();
            if (File.Exists(_fileName))
            {
                Load();
            }
            else
            {
                _passwords = new PasswordDictionary();
            }
        }

        private void Load()
        {
            var serializer = new XmlSerializer(typeof(PasswordDictionary));
            using (var sr = new StreamReader(_fileName))
                _passwords = (PasswordDictionary)serializer.Deserialize(sr);
       }

        private void Save()
        {
            var serializer = new XmlSerializer(typeof(PasswordDictionary));
            using (var sw = new StreamWriter(_fileName))
                serializer.Serialize(sw, _passwords);
        }

        private static string DetermineFileName()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
            var folder = Path.GetDirectoryName(config.FilePath);
            return folder + @"\0ED8B7F4-7A81-4DC1-812F-9F120F60E8E2.xml";
        }

        public string GetPassword(string site, string username)
        {
            string key = MakeKey(site, username);
            var password = GetPasswordFromKey(key);
            return password;
        }

        private string GetPasswordFromKey(string key)
        {
            string password = null;
            if (_passwords.TryGetValue(key, out string encryptedPassword))
                password = Encryption.Decrypt(encryptedPassword);
            return password;
        }

        public void SavePassword(string site, string username, SecureString securePassword)
        {
            string key = MakeKey(site, username);
            var currentPassword = GetPasswordFromKey(key);
            var password = securePassword.Unsecure();
            if (password != currentPassword)
            {
                var encryptedPassword = Encryption.Encrypt(password);
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
