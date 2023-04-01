using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;

namespace WikiUpload.Properties
{
    internal class AppSettings : IAppSettings
    {
        private readonly object _lock = new object();

        public string ImageExtensions
        {
            get => Settings.Default.ImageExtensions;
            set => Settings.Default.ImageExtensions = value;
        }

        public StringCollection PreviousSites => Settings.Default.PreviousSites;

        public ObservableCollection<string> RecentlyUsedSites => Settings.Default.RecentlyUsedSites;

        public RememberPasswordOptions RememberPassword
        {
            get => (RememberPasswordOptions)Settings.Default.RememberPassword;
            set => Settings.Default.RememberPassword = (int)value;
        }

        public int UploadDelay
        {
            get
            {
                lock (_lock)
                    return Settings.Default.UploadDelay;
            }
            set
            {
                lock (_lock)
                    Settings.Default.UploadDelay = value;
            }
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

        public string ContentFileExtension
        {
            get => Settings.Default.ContentFileExtension;
            set => Settings.Default.ContentFileExtension = value;
        }

        public bool CheckForUpdates
        {
            get => Settings.Default.CheckForUpdates;
            set => Settings.Default.CheckForUpdates = value;
        }

        public Skin Theme
        {
            get => (Skin)Settings.Default.Theme;
            set => Settings.Default.Theme = (int)value;
        }

        public bool InitialAddToWatchlist
        {
            get => Settings.Default.InitialAddToWatchlist;
            set => Settings.Default.InitialAddToWatchlist = value;
        }

        public bool InitialIgnoreWarnings
        {
            get => Settings.Default.InitialIgnoreWarnings;
            set => Settings.Default.InitialIgnoreWarnings = value;
        }

        public bool MainWindowPlacementEnabled
        {
            get => Settings.Default.MainWindowPlacementEnabled;
            set => Settings.Default.MainWindowPlacementEnabled = value;
        }

        public bool UploadedWindowPlacementEnabled
        {
            get => Settings.Default.UploadedWindowPlacementEnabled;
            set => Settings.Default.UploadedWindowPlacementEnabled = value;
        }

        public bool FollowUploadFile
        {
            get
            {
                lock (_lock)
                    return Settings.Default.FollowUploadFile;
            }
            set
            {
                lock (_lock)
                    Settings.Default.FollowUploadFile = value;
            }
        }

        public int RateLimitedBackoffPeriod => Settings.Default.RateLimitedBackoffPeriod * 1000;

        public bool DontAddToSumarry => Settings.Default.DontAddToSummary;

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
                        UploadDelay = int.Parse(attribute.Value);
                        break;

                    case nameof(Settings.Default.CheckForUpdates):
                        attribute = DefaultValueAttribute(property);
                        CheckForUpdates = bool.Parse(attribute.Value);
                        break;

                    case nameof(Settings.Default.ImageExtensions):
                        attribute = DefaultValueAttribute(property);
                        ImageExtensions = attribute.Value;
                        break;

                    case nameof(Settings.Default.FollowUploadFile):
                        attribute = DefaultValueAttribute(property);
                        FollowUploadFile = bool.Parse(attribute.Value);
                        break;

                    case nameof(Settings.Default.ContentFileExtension):
                        attribute = DefaultValueAttribute(property);
                        ContentFileExtension = attribute.Value;
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

        public void Reload()
        {
            Settings.Default.Reload();
        }
    }
}
