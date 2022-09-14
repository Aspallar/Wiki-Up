using PropertyChanged;
using System.ComponentModel;
using System;
using System.Diagnostics;
using System.IO;

namespace WikiUpload
{

    [DebuggerDisplay("{FileName, nq}")]
    [AddINotifyPropertyChangedInterface]
    public class UploadFile : INotifyPropertyChanged
    {
        private string _fullPath;
        private string _fileName;
        private string _folder;
        private bool _isVideo;
        private string _uploadFileNane;

        public UploadFile()
        {
            SetDefault();
        }

        public UploadFile(string fullPath)
        {
            FullPath = fullPath;
            SetDefault();
        }

        [DoNotNotify]
        public string FullPath
        {
            get => _fullPath;
            set
            {
                _fullPath = value;
                _fileName = Path.GetFileName(FullPath);
                _folder = Path.GetDirectoryName(FullPath);
                _isVideo = FullPath.StartsWith("https://");
                UploadFileName = _fileName;
            }
        }

        public string DisplayName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_uploadFileNane) && !_fileName.Equals(UploadFileName, StringComparison.CurrentCultureIgnoreCase))
                {
                    return UploadFileName + " \u21D0 " + _fileName;
                }
                else
                {
                    return _fileName;
                }
            }
        }

        public string UploadFileName
        {
            get => string.IsNullOrWhiteSpace(_uploadFileNane) ? FileName : _uploadFileNane;
            set => _uploadFileNane = value;
        }

        public UploadFileStatus Status { get; set; }

        public string Message { get; set; }

        [DoNotNotify]
        public string FileName => _fileName;

        [DoNotNotify]
        public string Folder => _folder;

        [DoNotNotify]
        public bool IsVideo => _isVideo;

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

        internal void SetDelaying(string message)
        {
            Status = UploadFileStatus.Delaying;
            Message = message;
        }

#pragma warning disable CS0067 // The event is never used - Fody will use it
        public event PropertyChangedEventHandler PropertyChanged;

#pragma warning restore CS0067

    }
}
