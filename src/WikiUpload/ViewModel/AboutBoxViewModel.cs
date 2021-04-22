using System;
using System.Windows;
using System.Windows.Input;
using WikiUpload.Properties;

namespace WikiUpload
{
    public class AboutBoxViewModel : WindowViewModel
    {
        public AboutBoxViewModel(Window window, IHelpers helpers) : base(window)
        {
            var (copyright, version) = helpers.ApplicationInformation;
            CopyrightText = Resources.CopyrightText + copyright[copyright.IndexOf(' ')..];
            VersionText = $"{Resources.VersionText} {version}";
            LaunchWebSiteCommand = new RelayParameterizedCommand((uri) => helpers.LaunchProcess(((Uri)uri).AbsoluteUri));
        }

        public string CopyrightText { get; }

        public string VersionText { get; }

        public ICommand LaunchWebSiteCommand { get; set; }
    }

}
