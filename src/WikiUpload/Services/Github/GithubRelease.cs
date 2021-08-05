using Newtonsoft.Json;
using System.Diagnostics;

namespace WikiUpload
{
    [DebuggerDisplay("{TagName, nq}")]
    internal class GithubRelease
    {
        [JsonProperty("tag_name")]
        public string TagName { get; set; }

        [JsonProperty("prerelease")]
        public bool IsPrerelease { get; set; }

        [JsonProperty("draft")]
        public bool IsDraft { get; set; }

        [JsonProperty("html_url")]
        public string HtmlUrl { get; set; }

        public bool IsProductionRelease => !(IsPrerelease || IsDraft);
    }
}
