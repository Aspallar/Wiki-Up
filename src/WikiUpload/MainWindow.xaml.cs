using System;
using System.Windows;
using System.Windows.Interop;

namespace WikiUpload
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CreateApplicationServices();
            DataContext = new MainWindowViewModel(this, App.Navigator);
        }

        private void CreateApplicationServices()
        {
            App.Navigator = new NavigationService(MainFrame.NavigationService);
            App.ServiceLocator = new ServiceLocator();
        }
    }
}
