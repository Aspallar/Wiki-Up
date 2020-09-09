using System;
using System.Configuration;
using System.IO;
using System.Security;
using System.Xml;
using System.Xml.Serialization;
using WikiUpload.Extensions;

namespace WikiUpload
{
    public class PasswordManager : IPasswordManager
    {
        private PasswordDictionary _passwords;
        private string _fileName;

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
            try
            {
                var serializer = new XmlSerializer(typeof(PasswordDictionary));
                using (var sr = new StreamReader(_fileName))
                    _passwords = (PasswordDictionary)serializer.Deserialize(sr);
            }
            catch (InvalidOperationException ex) when (ex.InnerException is XmlException)
            {
                // Fail silently as the worst case is just some remembered passwords are lost
                // the xml file will be regenerated the next time a password is saved.
            }
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

        public char[] GetPassword(string site, string username)
        {
            string key = MakeKey(site, username);
            var password = GetPasswordFromKey(key);
            return password;
        }

        private char[] GetPasswordFromKey(string key)
        {
            char[] password = null;
            if (_passwords.TryGetValue(key, out string encryptedPassword))
                password = Encryption.Decrypt(encryptedPassword);
            return password;
        }

        public void SavePassword(string site, string username, SecureString passsword)
        {
            string key = MakeKey(site, username);
            var currentPassword = GetPasswordFromKey(key);
  
            string encryptedPassword = passsword.UseUnsecuredString<string>(unsecuredPassword =>
            {
                if (currentPassword == null || !unsecuredPassword.IsEquivalentTo(currentPassword))
                    return Encryption.Encrypt(unsecuredPassword);
                else
                    return null;
            });

            if (currentPassword != null)
                Array.Clear(currentPassword, 0, currentPassword.Length);

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
