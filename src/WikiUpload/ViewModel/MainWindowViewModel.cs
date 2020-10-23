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
        }

        public ICommand SettingsCommand { get; }
    }
}
