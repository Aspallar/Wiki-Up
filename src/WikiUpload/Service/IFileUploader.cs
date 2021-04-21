using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace WikiUpload
{
    public interface IFileUploader
    {
        string HomePage { get; set; }
        IReadOnlyPermittedFiles PermittedFiles { get; }
        string Site { get; set; }
        string ScriptPath { get; set; }
        bool CanUploadVideos { get; }
        bool IncludeInWatchList { get; set; }
        bool IgnoreWarnings { get; set; }
        void Dispose();
        Task<bool> LoginAsync(string site, string username, SecureString password, bool allFilesPermitted = false);
        void LogOff();
        Task RefreshTokenAsync();
        Task<IUploadResponse> UpLoadAsync(
            string fullPath,
            string summary,
            string newPageContent,
            CancellationToken cancelToken);
        Task<IngestionControllerResponse> UpLoadVideoAsync(string fullPath, CancellationToken cancelToken);
        Task<SearchResponse> FetchCategories(string from);
        Task<SearchResponse> FetchTemplates(string from);
    }
}