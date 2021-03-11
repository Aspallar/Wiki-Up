using System.Windows;
using System.Windows.Input;

namespace WikiUpload
{
    public class MainWindowViewModel : WindowViewModel
    {
        public MainWindowViewModel(Window window, INavigatorService navigatorService) : base(window)
        {
            SettingsCommand = new RelayCommand(() =>
            {
                navigatorService.NavigateToSettingsPage();
            });
            AboutCommand = new RelayCommand(About);
        }

        private void About()
        {
            var dlg = new AboutBoxWindow { Owner = _window };
            dlg.ShowDialog();
        }

        public ICommand SettingsCommand { get; }
        public ICommand AboutCommand { get; }
    }
}
