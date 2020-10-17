using WikiUpload.Properties;

namespace WikiUpload
{
    public class UploadMessages
    {
        public static string InvalidXml => Resources.UploadMessageInvalidXml;
        public static string FileNotFound => Resources.UploadMessageFileNotFound;
        public static string ReadFail => Resources.UploadMessageReadFail;
        public static string Cancelled => Resources.UploadMessageCancelled;
        public static string ServerBusy => Resources.UploadMessageServerBusy;
        public static string NoEditToken => Resources.UploadMessageNoEditToken;
        public static string NetworkError => Resources.UploadMessageNetworkError;
        public static string TimedOut => Resources.UploadMessageTimedOut;
        public static string UnkownServerResponse => Resources.UploadMessageUnkownServerResponse;

        public static string FileTypeNotPermitted(string extension)
            => string.Format(Resources.UploadMessageFileTypeNotPermitted, extension);
    }
}
