using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WikiUpload
{
    public sealed class UpdateCheck : IUpdateCheck
   {
        private readonly IHelpers _helpers;
        private readonly IGithubProvider _githubProvider;

        public UpdateCheck(IHelpers helpers, IGithubProvider githubProvider)
        {
            _helpers = helpers;
            _githubProvider = githubProvider;
        }

        public async Task<UpdateCheckResponse> CheckForUpdates(string userAgent, int delay)
        {
            var response = new UpdateCheckResponse { IsNewerVersion = false };
            try
            {
                await _helpers.Wait(delay);
                var result = await _githubProvider.FetchLatestRelease(userAgent).ConfigureAwait(false);

                var match = Regex.Match(result, @"""tag_name""\:\s*""v(\d+\.\d+\.\d+)""");
                if (match.Success)
                {
                    var latestVersion = new Version(match.Groups[1].Value + ".0");
                    var thisVersion = _helpers.ApplicationVersion;
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
