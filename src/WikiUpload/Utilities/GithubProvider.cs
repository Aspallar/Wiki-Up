using System.Net.Http;
using System.Threading.Tasks;

namespace WikiUpload
{
    public class GithubProvider : IGithubProvider
    {
        public async Task<string> FetchLatestRelease(string userAgent)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", userAgent);
                client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
                var result = await client.GetStringAsync("https://api.github.com/repos/Aspallar/Wiki-Up/releases?per_page=1")
                    .ConfigureAwait(false);
                return result;
            }
        }
    }
}
