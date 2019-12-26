using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace WikiUpload
{
    public class DialogManager
    {
        public bool AddFilesDialog(string[] permittedExtensions, out IList<string> fileNames)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Select Files to Upload",
                Multiselect = true,
                CheckPathExists = true,
                CheckFileExists = true,
                Filter = AddFilesFilter(permittedExtensions),
            };
            bool result = (bool)openFileDialog.ShowDialog();
            fileNames = openFileDialog.FileNames.ToList();
            return result;
        }

        private string AddFilesFilter(string[] permittedExtensions)
        {
            const string othersPrefix = "|Other Files|*";
            const string imagesPrefix = "|Image Files|*";

            var imageExtensions = new List<string> { ".png", ".jpg", ".jpeg", ".gif", ".ico", ".svg", ".webp" };
            string images, others;

            if (permittedExtensions.Length == 0)
            {
                others = othersPrefix + ".odt;*.ods;*.odp;*.odg;*.odc;*.odf;*.odi;*.odm;*.ogg;*.ogv;*.oga";
                images = imagesPrefix + string.Join(";*", imageExtensions);
            }
            else
            {
                var imageFiles = permittedExtensions.Intersect(imageExtensions).ToList();
                others = string.Join(";*", permittedExtensions.Except(imageFiles));
                if (others.Length > 0)
                    others = othersPrefix + others;
                images = string.Join(";*", imageFiles);
                if (images.Length > 0)
                    images = imagesPrefix + images;
            }

            return $"{images}{others}|All Files|*.*".Substring(1);
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
