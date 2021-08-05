using System.Linq;
using System.Windows.Input;
using WikiUpload.Properties;

namespace WikiUpload
{
    internal class SettingsViewModel : BaseViewModel
    {
        private readonly INavigatorService _navigatorService;
        private readonly IHelpers _helpers;
        private readonly IAppSettings _appSettings;
        private readonly IWindowManager _windowManager;
        private readonly IUpdateCheck _updateCheck;

        private string _newExtensionText;
        private readonly ExtensionValidater _extensionValidater;

        public SettingsViewModel(
            IAppSettings appSettings,
            INavigatorService navigatorService,
            IUpdateCheck updateCheck,
            IWindowManager windowManager,
            IHelpers helpers) : base()
        {
            _extensionValidater = new ExtensionValidater();
            _navigatorService = navigatorService;
            _helpers = helpers;
            _appSettings = appSettings;
            _windowManager = windowManager;
            _updateCheck = updateCheck;

            SetPropeertiesFromAppSettings();

            SelectedLanguage = Languages.FirstOrDefault(x => x.Code == _appSettings.Language);
            SelectedColorTheme = ColorThemes.FirstOrDefault(x => x.Id == _appSettings.Theme);

            CancelSettingsCommand = new RelayCommand(CancelSettings);
            SaveSettingsCommand = new RelayCommand(SaveSettings);
            CheckForUpdatesNowCommand = new RelayCommand(CheckForUpdatesNow);
            RestoreDefaultsCommand = new RelayCommand(RestoreDefaults);
            RemoveImageExtensionCommand = new RelayParameterizedCommand(item => ImageFileExtensions.Remove((string)item));
            ToggleAddImageExtensionPopupCommand = new RelayCommand(() => IsAddingImageExtension = !IsAddingImageExtension);
            AddImageEtensionCommand = new RelayCommand(AddImageExtension);
            ToggleWindowPlacementPopupCommand = new RelayCommand(() => IsWindowPlacementPopupOpen = !IsWindowPlacementPopupOpen);
        }

        private void AddImageExtension()
        {
            IsAddingImageExtension = false;
            if (!string.IsNullOrWhiteSpace(_newExtensionText)
                && _extensionValidater.IsValid(_newExtensionText)
                && !ImageFileExtensions.Contains(_newExtensionText))
            {
                ImageFileExtensions.Add(_newExtensionText);
            }
        }

        private void SetPropeertiesFromAppSettings()
        {
            Delay = _appSettings.UploadDelay;
            CheckForUpdates = _appSettings.CheckForUpdates;
            FollowUploadFile = _appSettings.FollowUploadFile;
            MainWindowPlacementEnabled = _appSettings.MainWindowPlacementEnabled;
            UploadedWindowPlacementEnabled = _appSettings.UploadedWindowPlacementEnabled;
            ImageFileExtensions = new FileExensionsCollection(_appSettings.ImageExtensions);
        }

        private void RestoreDefaults()
        {
            _appSettings.RestoreConfigurationDefaults();
            SetPropeertiesFromAppSettings();
        }

        private void SaveSettings()
        {
            _appSettings.UploadDelay = Delay;
            _appSettings.CheckForUpdates = CheckForUpdates;
            _appSettings.ImageExtensions = ImageFileExtensions.ToString();
            _appSettings.FollowUploadFile = FollowUploadFile;
            _appSettings.MainWindowPlacementEnabled = MainWindowPlacementEnabled;
            _appSettings.UploadedWindowPlacementEnabled = UploadedWindowPlacementEnabled;
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

        private async void CheckForUpdatesNow()
        {
            UpdateCheckIsRunning = true;
            CheckUpdateMessage = "";
            var response = await _updateCheck.CheckForUpdates(_helpers.UserAgent, 200);
            UpdateCheckIsRunning = false;
            if (response.IsNewerVersion)
                _windowManager.ShowNewVersionWindow(response);
            else
                CheckUpdateMessage = Resources.UpToDateText;
        }

        public ICommand CancelSettingsCommand { get; }
        public ICommand SaveSettingsCommand { get; }
        public ICommand CheckForUpdatesNowCommand { get; }
        public ICommand RestoreDefaultsCommand { get; }
        public ICommand RemoveImageExtensionCommand { get; }
        public ICommand ToggleAddImageExtensionPopupCommand { get; }
        public ICommand AddImageEtensionCommand { get; }
        public ICommand ToggleWindowPlacementPopupCommand { get; }

        public ApplicationLanguages Languages { get; } = new ApplicationLanguages();

        public ApplicationColorThemes ColorThemes { get; } = new ApplicationColorThemes();

        public int Delay { get; set; }

        public Language SelectedLanguage { get; set; }

        public ColorTheme SelectedColorTheme { get; set; }

        public bool CheckForUpdates { get; set; }

        public string CheckUpdateMessage { get; set; }

        public bool IsCheckForUpdateMessage => !string.IsNullOrEmpty(CheckUpdateMessage);

        public bool UpdateCheckIsRunning { get; set; }

        public bool UploadedWindowPlacementEnabled { get; set; }

        public bool MainWindowPlacementEnabled { get; set; }

        public FileExensionsCollection ImageFileExtensions { get; set; }

        public bool IsAddingImageExtension { get; set; }

        public bool FollowUploadFile { get; set; }

        public bool IsValidImageFileExtension { get; set; } = true;

        public bool IsWindowPlacementPopupOpen { get; set; }

        public string NewExtensionText
        {
            get => _newExtensionText;
            set
            {
                if (_newExtensionText != value)
                {
                    _newExtensionText = value;
                    IsValidImageFileExtension = _extensionValidater.IsValid(value);
                }
            }
        }

    }

}
