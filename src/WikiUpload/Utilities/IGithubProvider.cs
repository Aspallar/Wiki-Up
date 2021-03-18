using System.Threading.Tasks;

namespace WikiUpload
{
    public interface IGithubProvider
    {
        Task<string> FetchLatestRelease(string userAgent);
    }
}