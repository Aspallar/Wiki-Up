using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WikiUpload.Properties;

namespace WikiUpload
{
    public class SettingsViewModel : BaseViewModel
    {
        private INavigatorService _navigatorService;
        private IAppSettings _appSettings;
        public List<Language> Languages { get; } = new List<Language>
        {
            new Language("English", "en-US"),
            new Language("Deutsche (German)", "de-DE"),
        };

        public SettingsViewModel(
            IAppSettings appSettings,
            INavigatorService navigatorService) : base()
        {
            _navigatorService = navigatorService;
            _appSettings = appSettings;

            Delay = appSettings.UploadDelay;
            ImageExtensions = _appSettings.ImageExtensions;
            SelectedLanguage = Languages.Where(x => x.Code == _appSettings.Language).FirstOrDefault();

            CancelSettingsCommand = new RelayCommand(CancelSettings);
            SaveSettingsCommand = new RelayCommand(SaveSettings);
        }

        private void SaveSettings()
        {
            _appSettings.UploadDelay = Delay;
            _appSettings.ImageExtensions = ImageExtensions;
            if (SelectedLanguage != null)
                _appSettings.Language = SelectedLanguage.Code;
            _appSettings.Save();
            _navigatorService.LeaveSettingsPage();
        }

        private void CancelSettings()
        {
            _navigatorService.LeaveSettingsPage();
        }

        public ICommand CancelSettingsCommand { get; }
        public ICommand SaveSettingsCommand { get; }

        public int Delay { get; set; }

        public Language SelectedLanguage { get; set; }

        public string ImageExtensions { get; set; }
    }

}
