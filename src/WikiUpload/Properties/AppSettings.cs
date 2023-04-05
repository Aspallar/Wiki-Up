using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;

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

        public bool DontAddToSumarry
        {
            get => Settings.Default.DontAddToSummary;
            set => Settings.Default.DontAddToSummary = value;
        }


        public void AddMostRecentlyUsedSite(string site)
        {
            Settings.Default.AddMostRecentlyUsedSite(site);
        }

        public void RestoreConfigurationDefaults()
        {
            foreach (var property in GetConfigurationProperties())
                property.SetValue(Settings.Default, property.GetDefaultValue());
        }

        private static IEnumerable<PropertyInfo> GetConfigurationProperties()
        {
            string[] configPropertyNames =
            {
                nameof(Settings.Default.UploadDelay),
                nameof(Settings.Default.DontAddToSummary),
                nameof(Settings.Default.CheckForUpdates),
                nameof(Settings.Default.FollowUploadFile),
                nameof(Settings.Default.ImageExtensions),
                nameof(Settings.Default.ContentFileExtension),
                nameof(Settings.Default.MainWindowPlacementEnabled),
                nameof(Settings.Default.UploadedWindowPlacementEnabled),
                nameof(Settings.Default.InitialAddToWatchlist),
                nameof(Settings.Default.InitialIgnoreWarnings),
            };
            return typeof(Settings)
                .GetProperties()
                .Where(prop => configPropertyNames.Contains(prop.Name));
        }

        public void Save() => Settings.Default.Save();

        public void Reload() => Settings.Default.Reload();
    }
}
