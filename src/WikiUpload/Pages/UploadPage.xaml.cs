using System.Threading;
using System.Windows;
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

        private void UploadPage_Loaded(object sender, RoutedEventArgs e)
        {
            string cultureName = Thread.CurrentThread.CurrentUICulture.Name;

            if (cultureName == "de-DE")
            {
                AddToWatchlistLabel.FontSize = 15;
                IgnoreWarningsLabel.FontSize = 15;
                SaveListButton.FontSize = 22;
                LoadListButton.FontSize = 22;
            }
            else if (cultureName == "et-EE")
            {
                StopUpload.FontSize = 24;
            }
        }

        private void ErrorContextMenu_CopyToClipboard_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuitem)
            {
                var data = (UploadFile)menuitem.DataContext;
                Clipboard.SetText(data.Message);
            }
        }
    }
}
