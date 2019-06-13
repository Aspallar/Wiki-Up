using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace WikiUpload
{
    public class DialogManager
    {
        public bool AddFilesDialog(out IList<string> fileNames)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Select Files to Upload",
                Multiselect = true,
                CheckPathExists = true,
                CheckFileExists = true,
                Filter = "Images|*.png;*.jpg;*.jpeg;*.gif;*.ico;*.svg" +
                    "|Other Files|*.odt;*.ods;*.odp;*.odg;*.odc;*.odf;*.odi;*.odm;*.ogg;*.ogv;*.oga" +
                    "|All Files|*.*",
            };
            bool result = (bool)openFileDialog.ShowDialog();
            fileNames = openFileDialog.FileNames.ToList();
            return result;
        }

        public bool LoadContentDialog(out string fileName)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Open Content File",
                Filter = "Text Files|*.txt;|All Files|*.*",
                CheckPathExists = true,
                CheckFileExists = true,
            };
            bool result = (bool)openFileDialog.ShowDialog();
            fileName = openFileDialog.FileName;
            return result;
        }

        public bool SaveContentDialog(out string fileName)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Title = "Save Content To",
                Filter = "Text Document|*.txt",
                AddExtension = true,
                DefaultExt = ".txt",
                CheckPathExists = true,
                OverwritePrompt = true,
            };
            bool result = (bool)saveFileDialog.ShowDialog();
            fileName = saveFileDialog.FileName;
            return result;
        }

        public bool LoadUploadListDialog(out string fileName)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Open Upload List",
                Filter = "Upload List Files|*.wul;|All Files|*.*",
                CheckPathExists = true,
                CheckFileExists = true,
            };
            bool result = (bool)openFileDialog.ShowDialog();
            fileName = openFileDialog.FileName;
            return result;
        }

        public bool SaveUploadListDialog(out string fileName)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Title = "Save Upload List To",
                Filter = "Upload List|*.wul",
                AddExtension = true,
                DefaultExt = ".wul",
                CheckPathExists = true,
                OverwritePrompt = true,
            };
            bool result = (bool)saveFileDialog.ShowDialog();
            fileName = saveFileDialog.FileName;
            return result;
        }

        public void ErrorMessage(string message)
        {
            Utils.ErrorMessage(message);
        }

        public bool ConfirmInsecureLoginDialog()
        {
            var result = MessageBox.Show(
                "Login via an insecure connection (http) will result in your username and password being sent in unencrypted plain text.\n\nAre you sure you wish to continue?",
                "Wiki-Up Warning",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            return result == MessageBoxResult.Yes;
        }
    }
}
