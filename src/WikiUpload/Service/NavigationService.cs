using System.Windows.Controls;

namespace WikiUpload
{
    public class NavigationService : INavigatorService
    {
        private readonly System.Windows.Navigation.NavigationService _navigator;
        private UploadPage _uploadPage;

        public NavigationService(System.Windows.Navigation.NavigationService navigator)
        {
            _navigator = navigator;
            // stop the service maintining a history
            _navigator.Navigated += (sender, _)
                => ((Frame)sender).NavigationService.RemoveBackEntry();
        }

        public void NavigateToCategoryPage()
        {
            _navigator.Navigate(new SearchPage());
        }

        public void NavigateToLoginPage()
            => _navigator.Navigate(new LoginPage());

        public void NavigateToUploadPage()
        {
            if (_uploadPage == null)
                _uploadPage = new UploadPage();
            _navigator.Navigate(_uploadPage);
        }

        public void NewUploadPage()
        {
            _uploadPage = null;
        }
    }
}
