using System.Windows;

namespace WikiUpload
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class AboutBoxWindow : Window
    {
        public AboutBoxWindow()
        {
            InitializeComponent();
            DataContext = App.ServiceLocator.AboutBoxViewModel(this);
        }
    }
}
