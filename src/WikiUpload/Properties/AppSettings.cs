using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;

namespace WikiUpload.Properties
{
    public class AppSettings : IAppSettings
    {
        public string ImageExtensions
        {
            get => Settings.Default.ImageExtensions;
            set => Settings.Default.ImageExtensions = value;
        }

        public StringCollection PreviousSites => Settings.Default.PreviousSites;

        public ObservableCollection<string> RecentlyUsedSites => Settings.Default.RecentlyUsedSites;

        public bool RememberPassword
        {
            get => Settings.Default.RememberPassword;
            set => Settings.Default.RememberPassword = value;
        }

        public int UploadDelay
        {
            get => Settings.Default.UploadDelay;
            set => Settings.Default.UploadDelay = value;
        }

        public string Username
        {
            get => Settings.Default.Username;
            set => Settings.Default.Username = value;
        }

        public string WikiUrl
        {
            get => Settings.Default.WikiUrl;
            set => Settings.Default.WikiUrl = value;
        }
        public string Language
        {
            get => Settings.Default.Language;
            set => Settings.Default.Language = value;
        }

        public bool CheckForUpdates
        {
            get => Settings.Default.CheckForUpdates;
            set => Settings.Default.CheckForUpdates = value;
        }

        public void AddMostRecentlyUsedSite(string site)
        {
            Settings.Default.AddMostRecentlyUsedSite(site);
        }

        public void RestoreConfigurationDefaults()
        {
            DefaultSettingValueAttribute attribute;
            var properties = typeof(Settings).GetProperties();
            foreach (var property in properties)
            {
                switch(property.Name)
                {
                    case nameof(Settings.Default.UploadDelay):
                        attribute = DefaultValueAttribute(property);
                        Settings.Default.UploadDelay = int.Parse(attribute.Value);
                        break;

                    case nameof(Settings.Default.CheckForUpdates):
                        attribute = DefaultValueAttribute(property);
                        Settings.Default.CheckForUpdates = bool.Parse(attribute.Value);
                        break;

                    case nameof(Settings.Default.ImageExtensions):
                        attribute = DefaultValueAttribute(property);
                        Settings.Default.ImageExtensions = attribute.Value;
                        break;
                }
            }
        }

        private static DefaultSettingValueAttribute DefaultValueAttribute(System.Reflection.PropertyInfo property)
            => (DefaultSettingValueAttribute)property.GetCustomAttributes(typeof(DefaultSettingValueAttribute), false)[0];

        public void Save()
        {
            Settings.Default.Save();
        }
    }
}
