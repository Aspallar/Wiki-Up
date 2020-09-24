using System.Windows.Controls;

namespace WikiUpload
{
    public class NavigationService : INavigatorService
    {
        private readonly System.Windows.Navigation.NavigationService _navigator;

        public NavigationService(System.Windows.Navigation.NavigationService navigator)
        {
            _navigator = navigator;
            // stop the service maintining a history
            _navigator.Navigated += (sender, _)
                => ((Frame)sender).NavigationService.RemoveBackEntry();
        }

        public void NavigateToLoginPage()
            => _navigator.Navigate(new LoginPage());

        public void NavigateToUploadPage() 
            => _navigator.Navigate(new UploadPage());
    }
}
