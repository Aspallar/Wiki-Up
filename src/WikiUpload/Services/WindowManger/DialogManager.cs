﻿using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WikiUpload
{
    internal class DialogManager : IDialogManager
    {
        public bool AddFilesDialog(string[] permittedExtensions, string imageExtensions, out IList<string> fileNames)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Select Files to Upload",
                Multiselect = true,
                CheckPathExists = true,
                CheckFileExists = true,
                Filter = AddFilesFilterBuilder.Build(permittedExtensions, imageExtensions)
            };
            var result = (bool)openFileDialog.ShowDialog();
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
