using System.Threading.Tasks;

namespace WikiUpload
{
    public interface IGithubProvider
    {
        Task<string> FetchLatestReleases(string userAgent);
    }
}