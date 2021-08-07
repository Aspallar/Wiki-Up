using System;
using System.Collections.Generic;

namespace WikiUpload
{
    internal interface IDialogManager
    {
        MultiplePathsDialogResponse AddFilesDialog(string[] permittedExtensions, string imageExtensions);
        bool ConfirmInsecureLoginDialog();
        void ErrorMessage(string message);
        void ErrorMessage(string message, Exception ex);
        bool ErrorMessage(string message, string subMessage, bool hasCancelButton = false);
        PathDialogResponse LoadContentDialog();
        PathDialogResponse LoadUploadListDialog();
        PathDialogResponse SaveContentDialog();
        PathDialogResponse SaveUploadListDialog();
        PathDialogResponse AddFolderDialog();
        AddFolderOptionsDialogResponse AddFolderOptionsDialog(string folderPath);
    }
}