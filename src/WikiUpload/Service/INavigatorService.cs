namespace WikiUpload
{
    public interface INavigatorService
    {
        void NewUploadPage();
        void NavigateToUploadPage();
        void NavigateToLoginPage();
        void NavigateToSearchPage();
        void NavigateToSettingsPage();
        void LeaveSettingsPage();
    }
}
