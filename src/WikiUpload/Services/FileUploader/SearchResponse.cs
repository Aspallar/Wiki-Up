using System.Collections.Generic;
using System.Xml;

namespace WikiUpload
{
    internal class SearchResponse
    {
        public string NextFrom { get; set; }

        public List<string> Categories { get; set; } = new List<string>();

        public static SearchResponse FromCategoryXml(XmlDocument doc)
        {
            var result = new SearchResponse();

            var categories = doc.SelectNodes("/api/query/allcategories/c");
            foreach (XmlNode node in categories)
                result.Categories.Add(node.InnerText);

            result.NextFrom = GetContinueValue(doc, "allcategories", "ac");
            return result;
        }

        public static SearchResponse FromTemplateXml(XmlDocument doc)
        {
            var result = new SearchResponse();

            var pages = doc.SelectNodes("/api/query/allpages/p");
            foreach (XmlNode node in pages)
            {
                var title = node.Attributes["title"].Value;
                if (!(title.EndsWith("/doc") || title.Contains("/doc/")))
                    result.Categories.Add(title.Substring(9));
            }

            result.NextFrom = GetContinueValue(doc, "allpages", "ap");
            return result;
        }

        private static string GetContinueValue(XmlDocument doc, string path, string prefix)
        {
            var continueNode = doc.SelectSingleNode("api/query-continue/" + path);
            return continueNode == null ? null : GetContinueValue(continueNode, prefix);
        }

        private static string GetContinueValue(XmlNode node, string prefix)
            => node.Attributes[prefix + "from"]?.Value
               ?? node.Attributes[prefix + "continue"]?.Value;
    }
}