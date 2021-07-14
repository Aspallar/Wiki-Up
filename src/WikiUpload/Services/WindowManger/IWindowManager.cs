namespace WikiUpload
{
    internal interface IWindowManager
    {
        void ShowNewVersionWindow(UpdateCheckResponse checkUpdateEventArrgs);
        void ShowUploadedFilesWindow();
    }
}