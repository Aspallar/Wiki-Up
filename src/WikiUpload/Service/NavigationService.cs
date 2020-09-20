namespace WikiUpload.Service
{
    public class NavigationService : INavigatorService
    {
        private readonly System.Windows.Navigation.NavigationService _navigator;

        public NavigationService(System.Windows.Navigation.NavigationService navigator)
        {
            _navigator = navigator;
        }

        public void Navigate(object dest)
        {
            _navigator.Navigate(dest);
        }
    }
}
