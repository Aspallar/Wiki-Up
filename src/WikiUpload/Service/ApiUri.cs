using System;
using System.Text;

namespace WikiUpload
{
    internal class ApiUri : Uri
    {
        public ApiUri(string wikiSite)
            : base(wikiSite.EndsWith("/") ? wikiSite + "api.php" : wikiSite + "/api.php")  { }

        public Uri ApiQuery(RequestParameters parameters)
        {
            var url = new StringBuilder(parameters.ToString());
            url.Append("&action=query&format=xml&cb=");
            url.Append(DateTime.Now.Ticks.ToString());
            return new Uri(this, url.ToString());
        }
    }
}
