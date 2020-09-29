using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace WikiUpload
{
    public interface IFileUploader
    {
        string PageContent { get; set; }
        IReadOnlyPermittedFiles PermittedFiles { get; }
        string Site { get; set; }
        string Summary { get; set; }

        void LogOff();
        void Dispose();
        Task<bool> LoginAsync(string site, string username, SecureString password, bool allFilesPermitted = false);
        Task RefreshTokenAsync();
        Task<IUploadResponse> UpLoadAsync(string fullPath,
                                          CancellationToken cancelToken,
                                          bool ignoreWarnings = false,
                                          bool includeInWatchlist = false);
    }
}