namespace WikiUpload
{
    internal interface IWindowManager
    {
        void ShowNewVersionWindow(UpdateCheckResponse checkUpdateResponse, bool showHint);
        void ShowUploadedFilesWindow();
    }
}