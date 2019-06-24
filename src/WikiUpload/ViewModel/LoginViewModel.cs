using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WikiUpload
{
    public class LoginViewModel : BaseViewModel
    {
        private DialogManager _dialogs = new DialogManager();

        public string Username { get; set; } = Properties.Settings.Default.Username;

        public string WikiUrl { get; set; } = Properties.Settings.Default.WikiUrl;

        public ObservableCollection<string> PreviousSites { get; set; }

        public bool IsLoginError { get; set; }

        public string LoginErrorMessage { get; set; }

        public bool LoginIsRunning { get; set; }

        public ICommand LoginCommand { get; set; }


        public LoginViewModel()
        {
            LoginCommand = new RelayParameterizedCommand(async (securePassword) => await Login(securePassword));
            PreviousSites = Properties.Settings.Default.RecentlyUsedSites;
        }

        public async Task Login(object securePassword)
        {
            IsLoginError = false;
            await RunCommand(() => this.LoginIsRunning, async () =>
            {
                string url;
                if ((url = await Validate()) != null)
                    await DoLogin(((IHavePassword)securePassword).SecurePassword, url);
            });
        }

        private async Task DoLogin(SecureString password, string url)
        {
            try
            {
                bool loggedIn = await UploadService.Uploader.LoginAsync(url, Username, password);

                if (loggedIn)
                {
                    var settings = Properties.Settings.Default;
                    settings.Username = Username;
                    settings.WikiUrl = WikiUrl;
                    settings.AddMostRecentlyUsedSite(WikiUrl);
                    settings.Save();
                    Navigator.NavigationService.Navigate(new UploadPage());
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
