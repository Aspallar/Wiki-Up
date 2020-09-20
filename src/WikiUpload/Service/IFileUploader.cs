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

        void Dispose();
        Task<bool> LoginAsync(string site, string username, SecureString password, bool allFilesPermitted = false);
        Task RefreshTokenAsync();
        Task<UploadResponse> UpLoadAsync(string fullPath, CancellationToken cancelToken, bool ignoreWarnings = false);
    }
}