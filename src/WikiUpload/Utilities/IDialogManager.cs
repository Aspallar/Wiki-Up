using System;
using System.Collections.Generic;

namespace WikiUpload
{
    public interface IDialogManager
    {
        bool AddFilesDialog(string[] permittedExtensions, string imageExtensions, out IList<string> fileNames);
        bool ConfirmInsecureLoginDialog();
        void ErrorMessage(string message);
        void ErrorMessage(string message, Exception ex);
        void ErrorMessage(string message, string subMessage);
        bool LoadContentDialog(out string fileName);
        bool LoadUploadListDialog(out string fileName);
        bool SaveContentDialog(out string fileName);
        bool SaveUploadListDialog(out string fileName);
    }
}