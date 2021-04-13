using System.Windows;
using System.Windows.Input;
using WikiUpload.Properties;

namespace WikiUpload
{
    public class NewVersionViewModel : WindowViewModel
    {
        private readonly IHelpers _helpers;

        public NewVersionViewModel(Window window, IHelpers helpers) : base(window)
        {
            TitleHeight = 26;
            _helpers = helpers;
            LauchWebsiteCommand = new RelayCommand(LaunchWebsite);
        }

        public string Message => string.Format(Resources.NewVersionMessage, Version);

        public string Version { get; set; } = "";

        public string Url { get; set; } = "";

        public ICommand LauchWebsiteCommand { get; }
        private void LaunchWebsite()
        {
            _helpers.LaunchProcess(Url);
        }

    }
}
