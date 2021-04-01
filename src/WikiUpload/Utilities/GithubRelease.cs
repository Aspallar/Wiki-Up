using Newtonsoft.Json;
using System.Diagnostics;

namespace WikiUpload
{
    [DebuggerDisplay("{TagName, nq}")]
    public class GithubRelease
    {
        [JsonProperty("tag_name")]
        public string TagName { get; set; }

        [JsonProperty("prerelease")]
        public bool Prerelease { get; set; }

        [JsonProperty("html_url")]
        public string HtmlUrl { get; set; }
    }
}
