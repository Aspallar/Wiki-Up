using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace WikiUpload
{
    /// <summary>
    /// Interaction logic for UploadTabContent.xaml
    /// </summary>
    public partial class UploadTabContent : UserControl
    {
        public UploadTabContent()
        {
            InitializeComponent();
            Loaded += UploadTabContent_Loaded;
        }

        private void UploadTabContent_Loaded(object sender, RoutedEventArgs e)
        {
            SetInitialFocus();
            AdjustFoCurrentrCulture();
        }

        private void AdjustFoCurrentrCulture()
        {
            switch (CultureInfo.CurrentUICulture.Name)
            {
                case "de-DE":
                    AddToWatchlistLabel.FontSize = 15;
                    IgnoreWarningsLabel.FontSize = 15;
                    SaveListButton.FontSize = 22;
                    LoadListButton.FontSize = 22;
                    break;

                case "fr-FR":
                    AddToWatchlistLabel.FontSize = 17;
                    IgnoreWarningsLabel.FontSize = 17;
                    SaveListButton.FontSize = 22;
                    LoadListButton.FontSize = 22;
                    StopUpload.FontSize = 22;
                    break;

                case "et-EE":
                    StopUpload.FontSize = 24;
                    break;
            }
        }

        private void SetInitialFocus()
        {
            AddFiles.Focus();
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
