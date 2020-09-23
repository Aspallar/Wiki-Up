namespace WikiUpload
{
    public interface IUploadFile
    {
        string FileName { get; }
        string Folder { get; }
        string FullPath { get; set; }
        string Message { get; set; }
        UploadFileStatus Status { get; set; }

        void SetDefault();
        void SetError(string message);
        void SetUploading();
        void SetWarning(string message);
    }
}