namespace WikiUpload
{
    internal class WindowManager : IWindowManager
    {
        public void ShowNewVersionWindow(UpdateCheckResponse checkUpdateResponse)
        {
            var newVersionWindow = new NewVersionWindow(checkUpdateResponse);
            newVersionWindow.Show();
        }
    }
}
