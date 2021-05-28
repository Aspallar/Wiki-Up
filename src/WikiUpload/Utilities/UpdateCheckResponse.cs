namespace WikiUpload
{
    internal class UpdateCheckResponse
    {
        public bool IsNewerVersion { get; set; }
        public string LatestVersion { get; set; }
        public string Url { get; set; }
    }
}