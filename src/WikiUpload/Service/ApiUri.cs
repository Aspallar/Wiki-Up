using System;
using System.Text;

namespace WikiUpload
{
    internal class ApiUri : Uri
    {
        public ApiUri(string wikiSite) : base(wikiSite + "/api.php")  { }

        public Uri ApiQuery(string parameters)
        {
            var url = new StringBuilder("?action=query&format=xml&cb=");
            url.Append(DateTime.Now.Ticks.ToString());
            url.Append('&');
            url.Append(parameters);
            return new Uri(this, url.ToString());
        }
    }
}
