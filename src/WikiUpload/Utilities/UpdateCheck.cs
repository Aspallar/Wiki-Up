using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
                var json = await _githubProvider.FetchLatestReleases(userAgent).ConfigureAwait(false);
                var releases = JsonConvert.DeserializeObject<List<GithubRelease>>(json);
                var releastVersionMatch = new Regex(@"^v\d+\.\d+\.\d+$");
                var latest = releases.Where(x => !x.Prerelease && releastVersionMatch.IsMatch(x.TagName)).FirstOrDefault();
                if (latest != null)
                {
                    var versionString = latest.TagName.Substring(1);
                    var latestVersion = new Version( versionString + ".0");
                    response.IsNewerVersion = latestVersion > _helpers.ApplicationVersion;
                    response.LatestVersion = versionString;
                    response.Url = latest.HtmlUrl;
                }
            }
            catch (Exception ex)
            {
                DebugHandleException(ex);
            }
            return response;
        }

        [Conditional("DEBUG")]
        private void DebugHandleException(Exception ex)
        {
            Debugger.Break();
        }
    }

}
