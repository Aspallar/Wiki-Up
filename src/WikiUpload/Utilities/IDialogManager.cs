using System.Collections.Generic;

namespace WikiUpload
{
    public interface IDialogManager
    {
        bool AddFilesDialog(string[] permittedExtensions, string imageExtensions, out IList<string> fileNames);
        bool ConfirmInsecureLoginDialog();
        void ErrorMessage(string message);
        bool LoadContentDialog(out string fileName);
        bool LoadUploadListDialog(out string fileName);
        bool SaveContentDialog(out string fileName);
        bool SaveUploadListDialog(out string fileName);
    }
}