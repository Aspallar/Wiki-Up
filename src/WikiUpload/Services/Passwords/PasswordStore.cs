using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace WikiUpload
{
    internal class PasswordStore : IPasswordStore
    {
        private readonly IHelpers _helpers;
        private readonly string _fileName;

        public PasswordStore(IHelpers helpers)
        {
            _helpers = helpers;
            _fileName = DetermineFileName();
        }

        public void Save(PasswordDictionary passwords)
        {
            var serializer = new XmlSerializer(typeof(PasswordDictionary));
            using (var sw = new StreamWriter(_fileName))
                serializer.Serialize(sw, passwords);
        }

        public PasswordDictionary Load()
            => File.Exists(_fileName) ? LoadFromFile() : new PasswordDictionary();

        private PasswordDictionary LoadFromFile()
        {
            PasswordDictionary passwords;
            try
            {
                var serializer = new XmlSerializer(typeof(PasswordDictionary));
                using (var sr = new StreamReader(_fileName))
                    passwords = (PasswordDictionary)serializer.Deserialize(sr);
            }
            catch (InvalidOperationException ex) when (ex.InnerException is XmlException)
            {
                // Fail silently as the worst case is just some remembered passwords are lost
                // the xml file will be regenerated the next time a password is saved.
                passwords = new PasswordDictionary();
            }
            return passwords;
        }

        public const string StoreName = @"\0ED8B7F4-7A81-4DC1-812F-9F120F60E8E2.xml";
        private string DetermineFileName() => _helpers.GetUserSettingsFolderName() + StoreName;

    }
}
