using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Newtonsoft.Json;
using System;

namespace WikiUpload
{
    internal class JsonHtmlStringConverter : JsonConverter<string>
    {
        private readonly HtmlParser _parser;
        private readonly IHtmlDocument _parseContext;

        public JsonHtmlStringConverter()
        {
            _parser = new HtmlParser();
            _parseContext = _parser.ParseDocument("<html><body></body></html>");
        }

        public override bool CanWrite => false;

        public override string ReadJson(JsonReader reader, Type objectType, string existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var text = (string)reader.Value;
            var nodes = _parser.ParseFragment(text, _parseContext.Body);

            if (nodes != null && nodes.Length > 0)
                text = nodes[0].TextContent.Trim();

            return text;
        }

        public override void WriteJson(JsonWriter writer, string value, JsonSerializer serializer)
            => throw new NotImplementedException();
    }
}
