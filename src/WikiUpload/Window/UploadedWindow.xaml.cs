using System.Windows;

namespace WikiUpload
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    internal partial class UploadedWindow : Window
    {
        public UploadedWindow()
        {
            InitializeComponent();
            DataContext = new WindowViewModel(this);
        }
    }
}
