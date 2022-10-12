using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        #region Event Handlers
        private void UploadTabContent_Loaded(object sender, RoutedEventArgs e)
        {
            SetInitialFocus();
            AdjustFoCurrentrCulture();
        }

        private void ErrorContextMenu_CopyToClipboard_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuitem)
            {
                var data = (UploadFile)menuitem.DataContext;
                Clipboard.SetText(data.Message);
            }
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (ShouldExecuteSelectFilesListKey(e))
            {
                FilesListBox.FocusSelectedOrFirstVisibleItem();
            }
        }

        private void FilesListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (ShouldExecuteEdirUploadFileNameKey(FilesListBox.SelectedItem, e.Key))
            {
                EditSelectedUploadFileName();
            }

        }
        #endregion

        #region Helpers

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

        private bool ShouldExecuteSelectFilesListKey(KeyEventArgs e)
        {
            return e.Key == Key.L
                   && Keyboard.IsKeyDown(Key.LeftCtrl)
                   && !UploadIsRunning();
        }

        private void EditSelectedUploadFileName()
        {
            var itemContainer = FilesListBox.ScrollToSelectedItem();
            FileRenamePopup.PlacementTarget = itemContainer;
            FileRenamePopup.ExitFocus = itemContainer;
            FileRenamePopup.IsOpen = true;
        }

        private bool ShouldExecuteEdirUploadFileNameKey(object selectedItem, Key key)
        {
            return selectedItem != null
                && IsEditUploadFileNameKey(key)
                && !UploadIsRunning();
        }

        private static bool IsEditUploadFileNameKey(Key key) => key == Key.F2 || key == Key.Return;

        private bool UploadIsRunning() => ((UploadViewModel)DataContext).UploadIsRunning;

        #endregion

    }
}
