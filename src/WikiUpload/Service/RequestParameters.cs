using System.Collections.Generic;
using System.Text;
using System.Web;

namespace WikiUpload
{
    internal class RequestParameters : List<KeyValuePair<string, string>>
    {
        public RequestParameters() : base() { }

        public void Add(string name, string value)
        {
            base.Add(new KeyValuePair<string, string>(name, value));
        }

        public override string ToString()
        {
            var queryString = new StringBuilder();
            if (Count > 0)
            {
                queryString.Append('?');
                foreach (var param in this)
                {
                    queryString.Append(HttpUtility.UrlEncode(param.Key));
                    queryString.Append('=');
                    queryString.Append(HttpUtility.UrlEncode(param.Value));
                    queryString.Append('&');
                }
                queryString.Length -= 1;
            }
            return queryString.ToString();
        }
    }
}
