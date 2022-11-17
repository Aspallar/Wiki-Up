using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace WikiUpload
{
    internal interface IFileUploader
    {
        IReadOnlyPermittedFiles PermittedFiles { get; }
        string Site { get; }
        bool CanUploadVideos { get; }
        bool IncludeInWatchList { get; set; }
        bool IgnoreWarnings { get; set; }
        ISiteInfo SiteInfo { get; }
        bool IsLoggedIn { get; }
        void Dispose();
        Task<bool> LoginAsync(string site, string username, SecureString password, bool allFilesPermitted = false);
        void LogOff();
        Task RefreshTokenAsync();
        Task<IngestionControllerResponse> UpLoadVideoAsync(string fullPath, CancellationToken cancelToken);
        Task<SearchResponse> FetchCategories(string from);
        Task<SearchResponse> FetchTemplates(string from);
        string FileUrl(string fileName);
        string ServerFilename(string fileName);
        Task<IUploadResponse> UpLoadAsync(
            string fullFilePath,
            string uploadFileName,
            CancellationToken cancelToken,
            string summary,
            string newPageContent);
        Task<IUploadResponse> UpLoadAsync(
            string fullPath,
            CancellationToken cancelToken,
            string summary,
            string newPageContent);
    }
}