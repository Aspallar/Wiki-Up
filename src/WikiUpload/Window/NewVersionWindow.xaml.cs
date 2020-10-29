using System.Windows;

namespace WikiUpload
{
    public partial class NewVersionWindow : Window
    {
        public NewVersionWindow(CheckForUpdatesEventArgs e)
        {
            InitializeComponent();
            var newVersionViewModel = App.ServiceLocator.NewVersionViewModel(this);
            newVersionViewModel.Version = e.LatestVersion;
            newVersionViewModel.Url = e.Url;
            DataContext = newVersionViewModel;
        }
    }
}
