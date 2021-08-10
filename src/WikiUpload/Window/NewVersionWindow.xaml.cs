using System.Windows;

namespace WikiUpload
{
    internal partial class NewVersionWindow : Window
    {
        public NewVersionWindow(UpdateCheckResponse e, bool showHint)
        {
            InitializeComponent();
            var newVersionViewModel = App.ServiceLocator.NewVersionViewModel(this);
            newVersionViewModel.Version = e.LatestVersion;
            newVersionViewModel.Url = e.Url;
            newVersionViewModel.IsHintVisible = showHint;
            DataContext = newVersionViewModel;
        }
    }
}
