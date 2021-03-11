namespace WikiUpload
{
    public class WindowManager : IWindowManager
    {
        public void ShowNewVersionWindow(CheckForUpdatesEventArgs checkUpdateEventArrgs)
        {
            var newVersionWindow = new NewVersionWindow(checkUpdateEventArrgs);
            newVersionWindow.Show();
        }
    }
}
