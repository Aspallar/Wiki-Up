using System.Collections.Generic;
using System.Security.Cryptography.Pkcs;
using System.Windows.Documents;
using System.Xml;

namespace WikiUpload
{
    public class SearchResponse
    {
        public string NextFrom { get; set; }

        public List<string> Categories { get; set; } = new List<string>();

        public static SearchResponse FromCategoryXml(XmlDocument doc)
        {
            var result = new SearchResponse();

            XmlNodeList categories = doc.SelectNodes("/api/query/allcategories/c");
            foreach (XmlNode node in categories)
                result.Categories.Add(node.InnerText);

            XmlNode continueNode = doc.SelectSingleNode("/api/query-continue/allcategories");
            if (continueNode != null)
                result.NextFrom = continueNode.Attributes["acfrom"].Value;

            return result;
        }

        public static SearchResponse FromTemplateXml(XmlDocument doc)
        {
            var result = new SearchResponse();

            XmlNodeList pages = doc.SelectNodes("/api/query/allpages/p");
            foreach (XmlNode node in pages)
            {
                var title = node.Attributes["title"].Value;
                if (!title.EndsWith("/doc"))
                    result.Categories.Add(title.Substring(9));
            }

            XmlNode continueNode = doc.SelectSingleNode("/api/query-continue/allpages");
            if (continueNode != null)
                result.NextFrom = continueNode.Attributes["apfrom"].Value;

            return result;
        }
    }
}