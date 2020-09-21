﻿using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace WikiUpload.Properties
{
    public class AppSettings : IAppSettings
    {
        public string ImageExtensions => Settings.Default.ImageExtensions;

        public StringCollection PreviousSites => Settings.Default.PreviousSites;

        public ObservableCollection<string> RecentlyUsedSites => Settings.Default.RecentlyUsedSites;

        public bool RememberPassword
        {
            get => Settings.Default.RememberPassword;
            set => Settings.Default.RememberPassword = value;
        }

        public int UploadDelay => Settings.Default.UploadDelay;

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

        public void AddMostRecentlyUsedSite(string site)
        {
            Settings.Default.AddMostRecentlyUsedSite(site);
        }

        public void Save()
        {
            Settings.Default.Save();
        }
    }
}