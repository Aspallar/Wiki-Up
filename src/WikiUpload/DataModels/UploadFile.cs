using PropertyChanged;
using System.ComponentModel;
using System.IO;

namespace WikiUpload
{
    [AddINotifyPropertyChangedInterface]
    public class UploadFile : INotifyPropertyChanged
    {

        [DoNotNotify]
        public string FullPath { get; set; }

        public UploadFileStatus Status { get; set; }

        public string Message { get; set; }

        [DoNotNotify]
        public string FileName => Path.GetFileName(FullPath);

        [DoNotNotify]
        public string Folder => Path.GetDirectoryName(FullPath);

        [DoNotNotify]
        public bool IsVideo => FullPath.StartsWith("https://");

        public UploadFile()
        {
            SetDefault();
        }

        public void SetDefault()
        {
            Status = UploadFileStatus.Waiting;
            Message = UploadMessages.AwaitingUpload;
        }

        public void SetError(string message)
        {
            Status = UploadFileStatus.Error;
            Message = message;
        }

        public void SetWarning(string message)
        {
            Status = UploadFileStatus.Warning;
            Message = message;
        }

        public void SetUploading()
        {
            Status = UploadFileStatus.Uploading;
            Message = UploadMessages.Uploading;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
