using System.Windows;

namespace WikiUpload
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        UpdateCheck _updateCheck;

        public MainWindow()
        {
            InitializeComponent();
            CreateApplicationServices();
            DataContext = new MainWindowViewModel(this, App.Navigator);
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.CheckForUpdates)
            {
                _updateCheck = new UpdateCheck();
                _updateCheck.CheckForUpdateCompleted += updateCheck_CheckForUpdateCompleted;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                _updateCheck.CheckForUpdates(App.UserAgent, 3000);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed 
            }
        }

        private void updateCheck_CheckForUpdateCompleted(object sender, CheckForUpdatesEventArgs e)
        {
            if (e.IsNewerVersion)
                new WindowManager().ShowNewVersionWindow(e);
            _updateCheck = null;
        }

        private void CreateApplicationServices()
        {
            App.Navigator = new NavigationService(MainFrame.NavigationService);
            App.ServiceLocator = new ServiceLocator();
        }
    }
}
