using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using WikiUpload.Service;

namespace WikiUpload
{
    public class LoginViewModel : BaseViewModel
    {
        private IPasswordManager _passwordManager;
        private IDialogManager _dialogs;
        private IFileUploader _fileUploader;
        private INavigatorService _navigator;
        private Properties.IAppSettings _appSettings;

        public LoginViewModel(IFileUploader fileUploader,
            INavigatorService navigator,
            IDialogManager dialogManager,
            IPasswordManager passwordManager,
            Properties.IAppSettings appSettings)
        {
            _fileUploader = fileUploader;
            _navigator = navigator;
            _appSettings = appSettings;
            _dialogs = dialogManager;
            _passwordManager = passwordManager;

            InitializeFromApplicationSettings();

            LoginCommand = new RelayParameterizedCommand(async (securePassword) => await Login(securePassword));
        }

        public string Username { get; set; }

        public string WikiUrl { get; set; }

        public bool RememberPassword { get; set; }

        public ObservableCollection<string> PreviousSites { get; set; }

        public IPasswordManager PasswordManager => _passwordManager;

        public bool IsLoginError { get; set; }

        public string LoginErrorMessage { get; set; }

        public bool LoginIsRunning { get; set; }

        public ICommand LoginCommand { get; set; }

        public SecureString SavedPassword { get; } = new SecureString();

        public async Task Login(object securePassword)
        {
            IsLoginError = false;
            await RunCommand(() => this.LoginIsRunning, async () =>
            {
                string url;
                if ((url = await Validate()) != null)
                {
                    SecureString password = SavedPassword.Length > 0 ?
                        SavedPassword : ((IHavePassword)securePassword).SecurePassword;
                    await DoLogin(password, url);
                }
            });
        }

        private async Task DoLogin(SecureString password, string url)
        {
            try
            {
                bool loggedIn = await _fileUploader.LoginAsync(url, Username, password);

                if (loggedIn)
                {
                    UpdateApplicationSettings();
                    UpdateSavedPassword(password);
                    //SavedPassword.Dispose();
                    _navigator.Navigate(new UploadPage());
                }
                else
                {
                    LoginError("Make sure the above details are correct.");
                }
            }
            catch (LoginException ex)
            {
                if (ex.InnerException is null)
                    LoginError(ex.Message);
                else
                    LoginError(ex.InnerException.Message);
            }
        }

        private void UpdateSavedPassword(SecureString password)
        {
            if (RememberPassword)
                _passwordManager.SavePassword(WikiUrl, Username, password);
            else
                _passwordManager.RemovePassword(WikiUrl, Username);
        }

        private void UpdateApplicationSettings()
        {
            _appSettings.Username = Username;
            _appSettings.WikiUrl = WikiUrl;
            _appSettings.RememberPassword = RememberPassword;
            _appSettings.AddMostRecentlyUsedSite(WikiUrl);
            _appSettings.Save();
        }

        private void InitializeFromApplicationSettings()
        {
            PreviousSites = _appSettings.RecentlyUsedSites;
            Username = _appSettings.Username;
            WikiUrl = _appSettings.WikiUrl;
            RememberPassword = _appSettings.RememberPassword;
        }


        private async Task<string> Validate()
        {
            if (string.IsNullOrWhiteSpace(WikiUrl) || string.IsNullOrWhiteSpace(Username))
            {
                await Task.Delay(500);
                LoginError("You must supply a wiki url and username.");
                return null;
            }

            string url = WikiUrl.ToLowerInvariant();
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                url = "https://" + url;

            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute) || url.IndexOf('?') != -1)
            {
                await Task.Delay(500);
                LoginError("Invalid wiki url");
                return null;
            }

            if (url.StartsWith("http://") && !_dialogs.ConfirmInsecureLoginDialog())
                return null;

            return url;
        }

        private void LoginError(string message)
        {
            IsLoginError = true;
            LoginErrorMessage = message;
        }
    }
}
