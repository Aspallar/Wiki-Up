using System;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WikiUpload
{
    public sealed class UpdateCheck : IUpdateCheck
    {
        public async Task<UpdateCheckResponse> CheckForUpdates(string userAgent, int delay)
        {
            var response = new UpdateCheckResponse { IsNewerVersion = false };
            try
            {
                await Task.Delay(delay);
                string result;
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", userAgent);
                    client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
                    result = await client.GetStringAsync("https://api.github.com/repos/Aspallar/Wiki-Up/releases?per_page=1")
                        .ConfigureAwait(false);
                }
                var match = Regex.Match(result, @"""tag_name""\:\s*""v(\d+\.\d+\.\d+)""");
                if (match.Success)
                {
                    var latestVersion = new Version(match.Groups[1].Value + ".0");
                    var thisVersion = Assembly.GetExecutingAssembly().GetName().Version;
                    response.IsNewerVersion = latestVersion > thisVersion;
                    response.LatestVersion = match.Groups[1].Value;
                    var htmlUrlMatch = Regex.Match(result, @"""html_url""\:\s*""([^""]+)");
                    if (htmlUrlMatch.Success)
                        response.Url = htmlUrlMatch.Groups[1].Value;
                    else
                        response.Url = "https://github.com/Aspallar/Wiki-Up/releases";
                }
            }
            catch (Exception) { }
            return response;
        }
    }
}
