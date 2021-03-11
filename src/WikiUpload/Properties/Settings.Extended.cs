using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace WikiUpload.Properties
{
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase
    {
        public void AddMostRecentlyUsedSite(string site)
        {
            var sites = new StringCollection { site };
            if (PreviousSites != null)
                sites.AddRange(PreviousSites.OfType<string>().Where(x => x != site).ToArray());
            PreviousSites = sites;
        }

        public ObservableCollection<string> RecentlyUsedSites
        {
            get
            {
                var sites = new ObservableCollection<string>();
                if (PreviousSites != null)
                {
                    foreach (var site in PreviousSites)
                        sites.Add(site);
                }
                return sites;
            }
        }

    }
}
