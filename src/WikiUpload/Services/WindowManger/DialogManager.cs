﻿using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WikiUpload
{
    internal class DialogManager : IDialogManager
    {
        public MultiplePathsDialogResponse AddFilesDialog(string[] permittedExtensions, string imageExtensions)
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
            return new MultiplePathsDialogResponse
            {
                Ok = result,
                Paths = openFileDialog.FileNames.ToList(),
            };
        }

        public PathDialogResponse LoadContentDialog()
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Open Content File",
                Filter = "Text Files|*.txt;|All Files|*.*",
                CheckPathExists = true,
                CheckFileExists = true,
            };
            var result = (bool)openFileDialog.ShowDialog();
            return new PathDialogResponse()
            {
                Ok = result,
                Path = openFileDialog.FileName,
            };
        }

        public PathDialogResponse SaveContentDialog()
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
            return new PathDialogResponse
            {
                Ok = result,
                Path = saveFileDialog.FileName,
            };
        }

        public PathDialogResponse LoadUploadListDialog()
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Open Upload List",
                Filter = "Upload List Files|*.wul;|All Files|*.*",
                CheckPathExists = true,
                CheckFileExists = true,
            };
            var result = (bool)openFileDialog.ShowDialog();
            return new PathDialogResponse
            {
                Ok = result,
                Path = openFileDialog.FileName,
            };
        }

        public PathDialogResponse SaveUploadListDialog()
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
            return new PathDialogResponse
            {
                Ok = result,
                Path = saveFileDialog.FileName
            };
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

        public PathDialogResponse AddFolderDialog()
        {
            var folderDialog = new VistaFolderBrowserDialog();
            var result = (bool)folderDialog.ShowDialog();
            return new PathDialogResponse()
            {
                Ok = result,
                Path = folderDialog.SelectedPath,
            };
        }

        public AddFolderOptionsDialogResponse AddFolderOptionsDialog(string folderPath)
        {
            var dlg = new AddFolderWindow(folderPath);
            var result = (bool)dlg.ShowDialog();
            var data = (AddFolderOptionsViewModel)dlg.DataContext;
            return new AddFolderOptionsDialogResponse
            {
                Ok = result,
                IncludeSubfolders = data.IncludeSubfolders,
                IncludeFiles = data.GetIncludeFiles(),
                Extension = data.GetExtension(),
            };
        }
    }

}
