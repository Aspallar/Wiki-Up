using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace WikiUpload.Properties
{
    public interface IAppSettings
    {
        string ImageExtensions { get; }
        StringCollection PreviousSites { get; }
        ObservableCollection<string> RecentlyUsedSites { get; }
        bool RememberPassword { get; set; }
        int UploadDelay { get; }
        string Username { get; set; }
        string WikiUrl { get; set; }

        void AddMostRecentlyUsedSite(string site);
        void Save();
    }
}