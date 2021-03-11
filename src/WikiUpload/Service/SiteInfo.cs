using System.Collections.Generic;
using System.Xml;

namespace WikiUpload
{
    public class SiteInfo
    {
        public string BaseUrl { get; set; }

        public string ScriptPath { get; set; }
        
        public List<string> Extensions { get; } 

        public SiteInfo(XmlDocument doc)
        {
            var general = doc.SelectSingleNode("/api/query/general");

            if (general != null)
            {
                BaseUrl = general.Attributes["base"].Value;
                ScriptPath = general.Attributes["scriptpath"].Value;
            }

            Extensions = new List<string>();
            var fileExtensions = doc.SelectNodes("/api/query/fileextensions/fe");
            if (fileExtensions != null)
            {
                foreach (XmlNode fe in fileExtensions)
                    Extensions.Add(fe.Attributes["ext"].Value);
            }
        }
    }
}
