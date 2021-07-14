using System.Windows;
using System.Windows.Input;

namespace WikiUpload
{
    internal class MainWindowViewModel : WindowViewModel
    {
        public MainWindowViewModel(Window window, INavigatorService navigatorService, IWindowManager windowManager) : base(window)
        {
            SettingsCommand = new RelayCommand(() =>
            {
                navigatorService.NavigateToSettingsPage();
            });

            ShowUploadedFilesCommand = new RelayCommand(() =>
            {
                windowManager.ShowUploadedFilesWindow();
            });

            AboutCommand = new RelayCommand(About);
        }

        public ICommand AboutCommand { get; }
        private void About()
        {
            var dlg = new AboutBoxWindow { Owner = _window };
            dlg.ShowDialog();
        }

        public ICommand ShowUploadedFilesCommand { get; }

        public ICommand SettingsCommand { get; }
    }
}
