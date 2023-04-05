using System;
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
        private readonly IExtensionValidater _extensionValidater;

        private string _newExtensionText;

        public SettingsViewModel(
            IAppSettings appSettings,
            INavigatorService navigatorService,
            IUpdateCheck updateCheck,
            IWindowManager windowManager,
            IExtensionValidater extensionValidater,
            IHelpers helpers) : base()
        {
            _extensionValidater = extensionValidater;
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
            ToggleStartupOptionsPopupCommand = new RelayCommand(() => IsStartupOptionsPopupOpen = !IsStartupOptionsPopupOpen);
            TogglePromotionPopupCommand = new RelayCommand(() => IsPromotionPopupOpen = !IsPromotionPopupOpen);
        }

        public ICommand RemoveImageExtensionCommand { get; }
        public ICommand ToggleAddImageExtensionPopupCommand { get; }
        public ICommand ToggleStartupOptionsPopupCommand { get; }
        public ICommand TogglePromotionPopupCommand { get; }

        public ICommand AddImageEtensionCommand { get; }
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
            InitialIgnoreWarnings = _appSettings.InitialIgnoreWarnings;
            InitialAddToWatchList = _appSettings.InitialAddToWatchlist;
            AllowPromotion = !_appSettings.DontAddToSumarry;
        }

        public ICommand RestoreDefaultsCommand { get; }
        private void RestoreDefaults()
        {
            _appSettings.RestoreConfigurationDefaults();
            SetPropeertiesFromAppSettings();
        }

        public ICommand SaveSettingsCommand { get; }
        private void SaveSettings()
        {
            _appSettings.UploadDelay = Delay;
            _appSettings.CheckForUpdates = CheckForUpdates;
            _appSettings.ImageExtensions = ImageFileExtensions.ToString();
            _appSettings.FollowUploadFile = FollowUploadFile;
            _appSettings.MainWindowPlacementEnabled = MainWindowPlacementEnabled;
            _appSettings.UploadedWindowPlacementEnabled = UploadedWindowPlacementEnabled;
            _appSettings.InitialAddToWatchlist = InitialAddToWatchList;
            _appSettings.InitialIgnoreWarnings = InitialIgnoreWarnings;
            _appSettings.DontAddToSumarry = !AllowPromotion;
            if (SelectedColorTheme != null)
                _appSettings.Theme = SelectedColorTheme.Id;
            if (SelectedLanguage != null)
                _appSettings.Language = SelectedLanguage.Code;
            _appSettings.Save();
            _navigatorService.LeaveSettingsPage();
        }

        public ICommand CancelSettingsCommand { get; }
        private void CancelSettings()
        {
            if (IsStartupOptionsPopupOpen)
                IsStartupOptionsPopupOpen = false;
            else if (IsAddingImageExtension)
                IsAddingImageExtension = false;
            else
                _navigatorService.LeaveSettingsPage();
        }

        public ICommand CheckForUpdatesNowCommand { get; }
        private async void CheckForUpdatesNow()
        {
            UpdateCheckIsRunning = true;
            CheckUpdateMessage = "";
            var response = await _updateCheck.CheckForUpdates(_helpers.UserAgent, 200);
            UpdateCheckIsRunning = false;
            if (response.IsNewerVersion)
                _windowManager.ShowNewVersionWindow(response, false);
            else
                CheckUpdateMessage = Resources.UpToDateText;
        }


        public ApplicationLanguages Languages { get; } = new ApplicationLanguages();

        public ApplicationColorThemes ColorThemes { get; } = new ApplicationColorThemes();

        public int Delay { get; set; }

        public Language SelectedLanguage { get; set; }

        public ColorTheme SelectedColorTheme { get; set; }

        public bool CheckForUpdates { get; set; }

        public bool AllowPromotion { get; set; }

        public string CheckUpdateMessage { get; set; }

        public bool IsCheckForUpdateMessage => !string.IsNullOrEmpty(CheckUpdateMessage);

        public bool UpdateCheckIsRunning { get; set; }

        public bool UploadedWindowPlacementEnabled { get; set; }

        public bool InitialAddToWatchList { get; set; }

        public bool InitialIgnoreWarnings { get; set; }

        public bool MainWindowPlacementEnabled { get; set; }

        public FileExensionsCollection ImageFileExtensions { get; set; }

        public bool IsAddingImageExtension { get; set; }

        public bool FollowUploadFile { get; set; }

        public bool IsStartupOptionsPopupOpen { get; set; }

        public bool IsPromotionPopupOpen { get; set; }

        public string NewExtensionText
        {
            get => _newExtensionText;
            set
            {
                if (_newExtensionText != value)
                {
                    _newExtensionText = value;
                    if (!_extensionValidater.IsValid(value))
                        throw new ArgumentException(Resources.InvalidExtension);
                }
            }
        }

    }

}
