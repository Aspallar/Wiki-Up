namespace WikiUpload
{
    internal interface INavigatorService
    {
        void NewUploadPage();
        void NavigateToUploadPage();
        void NavigateToLoginPage();
        void NavigateToSearchPage();
        void NavigateToSettingsPage();
        void LeaveSettingsPage();
    }
}
