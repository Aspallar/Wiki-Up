using System.Threading;
using System.Windows.Controls;

namespace WikiUpload
{
    /// <summary>
    /// Interaction logic for UploadPage.xaml
    /// </summary>
    public partial class UploadPage : Page
    {
        public UploadPage()
        {
            InitializeComponent();
            Loaded += UploadPage_Loaded;
        }

        private void UploadPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Thread.CurrentThread.CurrentUICulture.Name == "de-DE")
            {
                AddToWatchlistLabel.FontSize = 15;
                IgnoreWarningsLabel.FontSize = 15;
            }
        }
    }
}
