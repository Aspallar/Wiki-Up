using System.Net.Http;
using System.Threading.Tasks;

namespace WikiUpload
{
    public class GithubProvider : IGithubProvider
    {
        public async Task<string> FetchLatestReleases(string userAgent)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", userAgent);
                client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
                var response = await client.GetStringAsync("https://api.github.com/repos/Aspallar/Wiki-Up/releases?per_page=10");
                return response;
            }
        }
    }
}
