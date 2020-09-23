using PropertyChanged;
using System;
using System.IO;

namespace WikiUpload
{
    [AddINotifyPropertyChangedInterface]
    public class UploadFile : IUploadFile
    {
        public string FullPath { get; set; }

        public UploadFileStatus Status { get; set; }

        public string Message { get; set; }

        public string FileName => Path.GetFileName(FullPath);

        public string Folder => Path.GetDirectoryName(FullPath);

        public UploadFile()
        {
            SetDefault();
        }

        public void SetDefault()
        {
            Status = UploadFileStatus.Waiting;
            Message = "Awaiting upload.";
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
            Message = "Uploading to wiki...";
        }
    }
}
