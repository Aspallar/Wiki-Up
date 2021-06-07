using System.Threading.Tasks;

namespace WikiUpload
{
    internal interface IGithubProvider
    {
        Task<string> FetchLatestReleases(string userAgent);
    }
}