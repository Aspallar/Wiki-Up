using System;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WikiUpload
{
    public sealed class UpdateCheck : IUpdateCheck
    {
        public event EventHandler<CheckForUpdatesEventArgs> CheckForUpdateCompleted;

        public async Task CheckForUpdates(string userAgent, int delay)
        {
            //var e = new CheckForUpdatesEventArgs
            //{
            //    IsNewerVersion = true,
            //    LatestVersion = "2.0.0",
            //    Url = "http://localhost:10202",
            //};
            //await Task.Delay(600);
            //OnCheckForUpdatesCompleted(e);

            var eventArgs = new CheckForUpdatesEventArgs { IsNewerVersion = false };
            try
            {
                await Task.Delay(delay);
                string result;
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", userAgent);
                    client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
                    result = await client.GetStringAsync("https://api.github.com/repos/Aspallar/Wiki-Up/releases?per_page=1");
                }
                var match = Regex.Match(result, @"""tag_name""\:\s*""v(\d+\.\d+\.\d+)""");
                if (match.Success)
                {
                    var latestVersion = new Version(match.Groups[1].Value + ".0");
                    var thisVersion = Assembly.GetExecutingAssembly().GetName().Version;
                    eventArgs.IsNewerVersion = latestVersion > thisVersion;
                    eventArgs.LatestVersion = match.Groups[1].Value;
                    var htmlUrlMatch = Regex.Match(result, @"""html_url""\:\s*""([^""]+)");
                    if (htmlUrlMatch.Success)
                        eventArgs.Url = htmlUrlMatch.Groups[1].Value;
                    else
                        eventArgs.Url = "https://github.com/Aspallar/Wiki-Up/releases";
                }
            }
            catch (Exception) { }
            OnCheckForUpdatesCompleted(eventArgs);
        }

        private void OnCheckForUpdatesCompleted(CheckForUpdatesEventArgs e)
        {
            CheckForUpdateCompleted?.Invoke(this, e);
        }
    }
}
