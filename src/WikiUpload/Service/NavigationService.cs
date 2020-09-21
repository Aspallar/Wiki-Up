namespace WikiUpload
{
    public class NavigationService : INavigatorService
    {
        private readonly System.Windows.Navigation.NavigationService _navigator;

        public NavigationService(System.Windows.Navigation.NavigationService navigator)
        {
            _navigator = navigator;
        }

        public void NavigateToUploadPage()
        {
            _navigator.Navigate(new UploadPage());
        }
    }
}
