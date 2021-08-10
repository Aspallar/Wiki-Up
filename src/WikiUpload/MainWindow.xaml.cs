using System.Windows;

namespace WikiUpload
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    internal partial class MainWindow : WikiUpWindow
    {
        private UpdateCheck _updateCheck;

        public MainWindow()
        {
            InitializeComponent();
            CreateApplicationServices();
            DataContext = new MainWindowViewModel(this, App.Navigator, new WindowManager());
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.CheckForUpdates)
            {
                _updateCheck = new UpdateCheck(new Helpers(), new GithubProvider());
                var response = await _updateCheck.CheckForUpdates(App.UserAgent, 3000);
                if (response.IsNewerVersion)
                    new WindowManager().ShowNewVersionWindow(response, true);
                _updateCheck = null;
            }
        }

        private void CreateApplicationServices()
        {
            App.Navigator = new NavigationService(MainFrame.NavigationService);
            App.ServiceLocator = new ServiceLocator();
        }
    }
}
