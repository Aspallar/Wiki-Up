using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WikiUpload
{
    internal sealed class UpdateCheck : IUpdateCheck
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
                var latest = await FetchLatestRelease(userAgent, delay).ConfigureAwait(false);
                if (latest != null)
                    SetResponse(response, latest);
            }
            catch (Exception ex)
            {
                DebugHandleException(ex);
            }
            return response;
        }

        private void SetResponse(UpdateCheckResponse response, GithubRelease latestRelease)
        {
            var versionString = latestRelease.TagName.Substring(1);
            var latestVersion = new Version(versionString + ".0");
            response.IsNewerVersion = latestVersion > _helpers.ApplicationVersion;
            response.LatestVersion = versionString;
            response.Url = latestRelease.HtmlUrl;
        }

        private async Task<GithubRelease> FetchLatestRelease(string userAgent, int delay)
        {
            await _helpers.Wait(delay);
            var json = await _githubProvider.FetchLatestReleases(userAgent).ConfigureAwait(false);
            var releases = JsonConvert.DeserializeObject<List<GithubRelease>>(json);
            var releastVersionMatch = new Regex(@"^v\d+\.\d+\.\d+$");
            var latest = releases
                .FirstOrDefault(x => x.IsProductionRelease && releastVersionMatch.IsMatch(x.TagName));
            return latest;
        }

        [Conditional("DEBUG")]
        private void DebugHandleException(Exception ex)
        {
            Debugger.Break();
        }
    }

}
