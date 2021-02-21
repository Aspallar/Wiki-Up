using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace WikiUpload
{
    public interface IFileUploader
    {
        string HomePage { get; set; }
        string PageContent { get; set; }
        IReadOnlyPermittedFiles PermittedFiles { get; }
        string Site { get; set; }
        string ScriptPath { get; set; }
        string Summary { get; set; }
        bool CanUploadVideos { get; }
        void Dispose();
        Task<bool> LoginAsync(string site, string username, SecureString password, bool allFilesPermitted = false);
        void LogOff();
        Task RefreshTokenAsync();
        Task<IUploadResponse> UpLoadAsync(string fullPath, CancellationToken cancelToken, bool ignoreWarnings = false, bool includeInWatchlist = false);
        Task<IngestionControllerResponse> UpLoadVideoAsync(string fullPath, CancellationToken cancelToken);
        Task<SearchResponse> FetchCategories(string from);
        Task<SearchResponse> FetchTemplates(string from);
    }
}