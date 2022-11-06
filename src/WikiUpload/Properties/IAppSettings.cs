using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace WikiUpload.Properties
{
    internal interface IAppSettings
    {
        string ImageExtensions { get; set; }
        StringCollection PreviousSites { get; }
        ObservableCollection<string> RecentlyUsedSites { get; }
        RememberPasswordOptions RememberPassword { get; set; }
        int UploadDelay { get; set; }
        string Username { get; set; }
        string WikiUrl { get; set; }
        string Language { get; set; }
        bool CheckForUpdates { get; set; }
        bool FollowUploadFile { get; set; }
        Skin Theme { get; set; }
        int RateLimitedBackoffPeriod { get; }
        string ContentFileExtension { get; }
        bool UploadedWindowPlacementEnabled { get; set; }
        bool MainWindowPlacementEnabled { get; set; }

        void AddMostRecentlyUsedSite(string site);
        void Reload();
        void RestoreConfigurationDefaults();
        void Save();
    }
}