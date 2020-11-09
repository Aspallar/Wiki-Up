using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using WikiUpload.Properties;

namespace WikiUpload
{
    public class SettingsViewModel : BaseViewModel
    {
        private INavigatorService _navigatorService;
        private IHelpers _helpers;
        private IAppSettings _appSettings;
        private IWindowManager _windowManager;
        private IUpdateCheck _updateCheck;

        public List<Language> Languages { get; } = new List<Language>
        {
            new Language("English", "en-US"),
            new Language("Deutsch (German)", "de-DE"),
        };

        public List<ColorTheme> ColorThemes { get; } = new List<ColorTheme>
        {
           new ColorTheme(Skin.PurpleOverload, Resources.PutpleOverloadText),
           new ColorTheme(Skin.PurpleHaze, Resources.PutpleHazeText),
           new ColorTheme(Skin.GreenForest, Resources.GreenForrestTest),
           new ColorTheme(Skin.BlueLight, Resources.BlueLightText),
           new ColorTheme(Skin.Solarized, Resources.SolarizedText),
           new ColorTheme(Skin.Rakdos, Resources.RakdosText),
        };

        public SettingsViewModel(
            IAppSettings appSettings,
            INavigatorService navigatorService,
            IUpdateCheck updateCheck,
            IWindowManager windowManager,
            IHelpers helpers) : base()
        {
            _navigatorService = navigatorService;
            _helpers = helpers;
            _appSettings = appSettings;
            _windowManager = windowManager;
            _updateCheck = updateCheck;
            _updateCheck.CheckForUpdateCompleted += updateCheck_CheckForUpdateCompleted;

            SetPropeertiesFromAppSettings();

            SelectedLanguage = Languages.Where(x => x.Code == _appSettings.Language).FirstOrDefault();
            SelectedColorTheme = ColorThemes.Where(x => x.Id == _appSettings.Theme).FirstOrDefault();

            CancelSettingsCommand = new RelayCommand(CancelSettings);
            SaveSettingsCommand = new RelayCommand(SaveSettings);
            CheckForUpdatesNowCommand = new RelayCommand(CheckForUpdatesNow);
            RestoreDefaultsCommand = new RelayCommand(RestoreDefaults);
        }

        private void SetPropeertiesFromAppSettings()
        {
            Delay = _appSettings.UploadDelay;
            CheckForUpdates = _appSettings.CheckForUpdates;
            ImageExtensions = _appSettings.ImageExtensions;
        }

        private void RestoreDefaults()
        {
            _appSettings.RestoreConfigurationDefaults();
            SetPropeertiesFromAppSettings();
        }

        private void SaveSettings()
        {
            _appSettings.UploadDelay = Delay;
            _appSettings.ImageExtensions = ImageExtensions;
            _appSettings.CheckForUpdates = CheckForUpdates;
            if (SelectedColorTheme != null)
                _appSettings.Theme = SelectedColorTheme.Id;
            if (SelectedLanguage != null)
                _appSettings.Language = SelectedLanguage.Code;
            _appSettings.Save();
            _navigatorService.LeaveSettingsPage();
        }

        private void CancelSettings()
        {
            _navigatorService.LeaveSettingsPage();
        }

        private void CheckForUpdatesNow()
        {
            UpdateCheckIsRunning = true;
            CheckUpdateMessage = "";
            _updateCheck.CheckForUpdates(_helpers.UserAgent, 200);
        }

        private void updateCheck_CheckForUpdateCompleted(object sender, CheckForUpdatesEventArgs e)
        {
            UpdateCheckIsRunning = false;
            if (e.IsNewerVersion)
                _windowManager.ShowNewVersionWindow(e);
            else
                CheckUpdateMessage = Resources.UpToDateText;
        }

        public ICommand CancelSettingsCommand { get; }
        public ICommand SaveSettingsCommand { get; }
        public ICommand CheckForUpdatesNowCommand { get; }
        public ICommand RestoreDefaultsCommand { get; }

        public int Delay { get; set; }

        public Language SelectedLanguage { get; set; }

        public ColorTheme SelectedColorTheme { get; set; }

        public string ImageExtensions { get; set; }

        public bool CheckForUpdates { get; set; }

        public string CheckUpdateMessage { get; set; }

        public bool IsCheckForUpdateMessage => !string.IsNullOrEmpty(CheckUpdateMessage);

        public bool UpdateCheckIsRunning { get; set; }
    }

}
