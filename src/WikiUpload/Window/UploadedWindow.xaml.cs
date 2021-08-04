using System.Windows;

namespace WikiUpload
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    internal partial class UploadedWindow : WikiUpWindow
    {
        public UploadedWindow()
        {
            InitializeComponent();
            base.DataContext = new WindowViewModel(this);
        }
    }
}
