namespace WikiUpload
{
    public class CheckForUpdatesEventArgs
    {
        public bool IsNewerVersion { get; internal set; }
        public string LatestVersion { get; internal set; }
        public string Url { get; internal set; }
    }
}