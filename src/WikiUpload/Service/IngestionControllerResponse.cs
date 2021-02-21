using Newtonsoft.Json;
using System.Net;

namespace WikiUpload
{
    public class IngestionControllerResponse
    {
        [JsonProperty("status")]
        [JsonConverter(typeof(JsonHtmlStringConverter))]
        public string Status { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonIgnore]
        public HttpStatusCode HttpStatusCode { get; set; }
    }
}
