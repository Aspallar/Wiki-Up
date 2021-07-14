using System.Windows;

namespace WikiUpload
{
    internal class WindowManager : IWindowManager
    {
        private Window _uploadedFilesWindow;

        public void ShowNewVersionWindow(UpdateCheckResponse checkUpdateResponse)
        {
            var newVersionWindow = new NewVersionWindow(checkUpdateResponse);
            newVersionWindow.Show();
        }

        public void ShowUploadedFilesWindow()
        {
            if (_uploadedFilesWindow == null)
            {
                _uploadedFilesWindow = new UploadedWindow() { Owner = App.Current.MainWindow };
                _uploadedFilesWindow.Closed += (s, e) => _uploadedFilesWindow = null;
                _uploadedFilesWindow.Show();
            }
            else
            {
                _uploadedFilesWindow.Activate();
            }
        }
    }
}
