using System.Collections.Generic;
using System.Windows.Documents;
using System.Xml;

namespace WikiUpload
{
    public class CategoryResponse
    {
        public string NextFrom { get; set; }

        public List<string> Categories { get; set; } = new List<string>();

        public CategoryResponse()
        {

        }

        public CategoryResponse(XmlDocument doc)
        {
            XmlNodeList categories = doc.SelectNodes("/api/query/allcategories/c");
            foreach (XmlNode node in categories)
                Categories.Add(node.InnerText);

            XmlNode cont = doc.SelectSingleNode("/api/query-continue/allcategories");
            if (cont != null)
                NextFrom = cont.Attributes["acfrom"].Value;
        }
    }
}