using System.Windows;
using System.Windows.Input;

namespace WikiUpload
{
    public class AboutBoxViewModel : WindowViewModel
    {
        public AboutBoxViewModel(Window window, IHelpers helpers) : base(window)
        {
            var (copyright, version) = helpers.ApplicationInformation;
            CopyrightText = copyright;
            VersionText = $"Version {version}";
            LaunchWebSiteCommand = new RelayCommand(() => helpers.LaunchProcess("https://github.com/Aspallar/Wiki-Up"));
        }

        public string CopyrightText { get; }

        public string VersionText { get; }

        public ICommand LaunchWebSiteCommand { get; set; }
    }

}
