using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WikiUpload
{
    public class DialogManager : IDialogManager
    {
        public bool AddFilesDialog(string[] permittedExtensions, string imageExtensions, out IList<string> fileNames)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Select Files to Upload",
                Multiselect = true,
                CheckPathExists = true,
                CheckFileExists = true,
                Filter = AddFilesFilter(permittedExtensions, imageExtensions),
            };
            var result = (bool)openFileDialog.ShowDialog();
            fileNames = openFileDialog.FileNames.ToList();
            return result;
        }


        private string AddFilesFilter(string[] permittedExtensions, string imageExtensionsString)
        {
            const string othersPrefix = "|Other Files|*";
            const string imagesPrefix = "|Image Files|*";

            var imageExtensions = imageExtensionsString.Split(';').Select(x => '.' + x).ToList();
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
            var result = (bool)openFileDialog.ShowDialog();
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
            var result = (bool)saveFileDialog.ShowDialog();
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
            var result = (bool)openFileDialog.ShowDialog();
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
            var result = (bool)saveFileDialog.ShowDialog();
            fileName = saveFileDialog.FileName;
            return result;
        }

        public bool ErrorMessage(string message, string subMessage, bool hasCancelButton = false)
        {
            var dlg = new ErrorMessageWindow(message, subMessage, hasCancelButton);
            return (bool)dlg.ShowDialog();
        }

        public void ErrorMessage(string message, Exception ex) => ErrorMessage(message, ex?.Message);
        
        public void ErrorMessage(string message) => ErrorMessage(message, string.Empty);

        public bool ConfirmInsecureLoginDialog()
        {
            var dlg = new InsecureWarningWindow();
            return (bool)dlg.ShowDialog();
        }

        public bool AddFolderDialog(out string folder)
        {
            var folderDialog = new VistaFolderBrowserDialog();
            var result = (bool)folderDialog.ShowDialog();
            folder = folderDialog.SelectedPath;
            return result;
        }

        public bool AddFolderOptionsDialog(
            string folderPath,
            out bool includeSubfolders,
            out IncludeFiles includeFiles,
            out string extension)
        {
            var dlg = new AddFolderWindow(folderPath);
            var result = (bool)dlg.ShowDialog();
            var data = (AddFolderOptionsViewModel)dlg.DataContext;
            includeSubfolders = data.IncludeSubfolders;
            includeFiles = data.GetIncludeFiles();
            extension = data.GetExtension();
            return result;
        }

    }
}
