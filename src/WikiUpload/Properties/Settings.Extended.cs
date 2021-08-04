using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace WikiUpload.Properties
{
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase
    {
        private const string WindowPlacementEnabledSuffix = "WindowPlacementEnabled";
        private const string WindowPlacementSuffix = "WindowPlacement";

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

        public bool IsWindowPlacementEnabled(string prefix)
            => (bool)this[prefix + WindowPlacementEnabledSuffix];

        public WindowPlacement GetWindowPlacement(string prefix)
            => (WindowPlacement)this[prefix + WindowPlacementSuffix];

        public void SetWindowPlacement(string prefix, WindowPlacement wp)
        {
            this[prefix + WindowPlacementSuffix] = wp;
        }
    }
}
