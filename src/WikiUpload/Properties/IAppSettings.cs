using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace WikiUpload.Properties
{
    public interface IAppSettings
    {
        string ImageExtensions { get; set; }
        StringCollection PreviousSites { get; }
        ObservableCollection<string> RecentlyUsedSites { get; }
        bool RememberPassword { get; set; }
        int UploadDelay { get; set; }
        string Username { get; set; }
        string WikiUrl { get; set; }
        string Language { get; set; }
        bool CheckForUpdates { get; set; }
        bool FollowUploadFile { get; set; }
        Skin Theme { get; set; }

        void AddMostRecentlyUsedSite(string site);
        void RestoreConfigurationDefaults();
        void Save();
    }
}