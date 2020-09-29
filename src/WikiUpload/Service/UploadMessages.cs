namespace WikiUpload
{
    public class UploadMessages
    {
        public const string InvalidXml = "Server returned an invalid XML response.";
        public const string FileNotFound = "File not found.";
        public const string ReadFail = "Unable to read file.";
        public const string Cancelled = "Upload cancelled.";
        public const string ServerBusy = "Server is too busy. Uploads cancelled. Try again later.";
        public const string NoEditToken = "Unable to obtain valid edit token. Uploads cancelled. You may have to restart Wiki-Up to resolve this error.";
        public const string NetworkError = "Network error. Unable to upload.";
        public const string TimedOut = "The upload timed out.";
        public const string UnkownServerResponse = "Unexpected server response.";
    }
}
